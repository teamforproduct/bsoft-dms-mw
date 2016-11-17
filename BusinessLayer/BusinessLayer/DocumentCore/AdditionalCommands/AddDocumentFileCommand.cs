using System;
using System.Collections.Generic;
using System.IO;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private AddDocumentFile Model
        {
            get
            {
                if (!(_param is AddDocumentFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (CommandType == EnumDocumentActions.AddDocumentFileUseMainNameFile)
            {
                _actionRecords =
                  _document.DocumentFiles.Where(x => x.IsMainVersion && !x.IsDeleted)
                            .Where(
                      x =>
                          x.Type == EnumFileTypes.Additional && x.ExecutorPositionId == positionId
                          || x.Type == EnumFileTypes.Main)
                                                  .Select(x => new InternalActionRecord
                                                  {
                                                      FileId = x.Id,
                                                  });
                if (!_actionRecords.Any())
                {
                    return false;
                }
                return true;
            }
            else
                return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddDocumentFilePrepare(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }

            if (Model.IsUseMainNameFile)
            {
                var mainFile = _document.DocumentFiles.FirstOrDefault(x => x.OrderInDocument == Model.OrderInDocument);
                if (mainFile == null)
                {
                    throw new CannotAccessToFile();
                }

                Model.FileName = mainFile.Name + "." + mainFile.Extension;
                Model.FileType = mainFile.FileType;
            }

            var file = _document.DocumentFiles.FirstOrDefault(x => (x.Name + "." + x.Extension).Equals(Model.FileName));

            if (file != null)
            {
                Model.Type = file.Type;
                if ((file.Type == EnumFileTypes.Additional && file.ExecutorPositionId != _context.CurrentPositionId)
                    || (!_context.IsAdmin && !new List<EnumFileTypes> { EnumFileTypes.Main, EnumFileTypes.Additional }.Contains(file.Type))
                    )
                {
                    throw new CannotAccessToFile();
                }
            }
            //else
            //{
            //    if (!Model.IsAdditional && _document.ExecutorPositionId != _context.CurrentPositionId)
            //    {
            //        //TODO Саше проверить.
            //        Model.IsAdditional = true;
            //        //throw new CouldNotPerformOperation();
            //    }
            //}

            return true;
        }

        public override object Execute()
        {
            var executorPositionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
            if (!executorPositionExecutorAgentId.HasValue)
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }
            var res = new List<int>();
            var att = new InternalDocumentAttachedFile
            {
                DocumentId = Model.DocumentId,
                Date = DateTime.UtcNow,
                PostedFileData = Model.PostedFileData,
                Type = Model.Type,
                IsMainVersion = Model.Type == EnumFileTypes.Additional || (Model.Type == EnumFileTypes.Main && _document.ExecutorPositionId == _context.CurrentPositionId) || _context.IsAdmin,
                FileType = Model.FileType,
                Name = Path.GetFileNameWithoutExtension(Model.FileName),
                Extension = Path.GetExtension(Model.FileName).Replace(".", ""),
                Description = Model.Description,
                IsWorkedOut = (Model.Type == EnumFileTypes.Main && _document.ExecutorPositionId != _context.CurrentPositionId) ? false : (bool?)null,

                WasChangedExternal = false,
                ExecutorPositionId = _context.CurrentPositionId,
                ExecutorPositionExecutorAgentId = executorPositionExecutorAgentId.Value
            };
            var ordInDoc = _operationDb.CheckFileForDocument(_context, Model.DocumentId, att.Name, att.Extension);
            if (ordInDoc == -1)
            {
                att.Version = 1;
                att.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId);
            }
            else
            {
                att.Version = _operationDb.GetFileNextVersion(_context, att.DocumentId, ordInDoc);
                att.OrderInDocument = ordInDoc;
            }


            _fStore.SaveFile(_context, att);
            CommonDocumentUtilities.SetLastChange(_context, att);
            if (_document.IsRegistered.HasValue)
            {
                att.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, att.DocumentId, EnumEventTypes.AddDocumentFile, null, null, att.Name + "." + att.Extension, null, false, att.Type != EnumFileTypes.Additional ? (int?)null : _document.ExecutorPositionId);
            }
            res.Add(_operationDb.AddNewFileOrVersion(_context, att));
            // Модель фронта содержит дополнительно только одно поле - пользователя, который последний модифицировал файл. 
            // это поле не заполняется, иначе придется после каждого добавления файла делать запрос на выборку этого файла из таблицы
            // как вариант можно потому будет добавить получение имени текущего пользователя вначале и дописывать его к модели
            //res.Add(new FrontDocumentAttachedFile(att));


            return res;
        }
    }
}