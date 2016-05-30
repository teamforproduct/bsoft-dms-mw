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

        private ModifyDocumentFile Model
        {
            get
            {
                if (!(_param is ModifyDocumentFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
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

            if (!Model.IsAdditional && _document.ExecutorPositionId != _context.CurrentPositionId)
            {
                //TODO Саше проверить.
                Model.IsAdditional = true;
                //throw new CouldNotPerformOperation();
            }

            if (_document.DocumentFiles.Any(x => (x.Name + "." + x.Extension).Equals(Model.FileName) && x.ExecutorPositionId != _context.CurrentPositionId))
            {
                throw new CannotAccessToFile();
            }

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
                Date = DateTime.Now,
                PostedFileData = Model.PostedFileData,
                IsAdditional = Model.IsAdditional,
                FileType = Model.FileType,
                Name = Path.GetFileNameWithoutExtension(Model.FileName),
                Extension = Path.GetExtension(Model.FileName).Replace(".", ""),
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
                att.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, att.DocumentId, EnumEventTypes.AddDocumentFile, null, att.Name + "." + att.Extension);
            }
            res.Add(_operationDb.AddNewFileOrVersion(_context, att));
            // Модель фронта содержит дополнительно только одно поле - пользователя, который последний модифицировал файл. 
            // это поле не заполняется, иначе придется после каждого добавления файла делать запрос на выборку этого файла из таблицы
            // как вариант можно потому будет добавить получение имени текущего пользователя вначале и дописывать его к модели
            //res.Add(new FrontDocumentAttachedFile(att));


            return res;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentFile;
    }
}