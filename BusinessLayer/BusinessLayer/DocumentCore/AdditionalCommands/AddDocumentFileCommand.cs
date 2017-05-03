using System;
using System.Collections.Generic;
using System.IO;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.CrossCutting.Context;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Common;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.TempStorage;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;
        private BaseFile _file;

        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService)
        {
            _operationDb = operationDb;
            _fStore = fStore;
            _queueWorkerService = queueWorkerService;
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
            return true;
        }

        public override bool CanExecute()
        {
            _file = DmsResolver.Current.Get<ITempStorageService>().GetStoreObject(Model.TmpFileId) as BaseFile;
            if (_file == null)
                throw new CannotAccessToFile();
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
                    throw new CannotAccessToFile();
                _file.Name = mainFile.File.Name + "." + mainFile.File.Extension;
                _file.FileType = mainFile.File.FileType;
            }

            var file = _document.DocumentFiles.FirstOrDefault(x => (x.File.Name + "." + x.File.Extension).Equals(_file.FileName));

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
            var executorPositionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
            if (!executorPositionExecutor?.ExecutorAgentId.HasValue ?? true)
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }
            var res = new List<int>();
            try
            {

                var att = CommonDocumentUtilities.GetNewDocumentFile(_context, (int)EnumEntytiTypes.Document, _document.ExecutorPositionId.Value, Model, _file, executorPositionExecutor);
                var ordInDoc = _operationDb.CheckFileForDocument(_context, Model.DocumentId, att.File.Name, att.File.Extension);
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
                if (_document.IsRegistered.HasValue)
                {
                    att.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, att.DocumentId,
                        EnumEventTypes.AddDocumentFile, null, null, att.File.FileName, null, null,
                        att.Type != EnumFileTypes.Additional ? (int?) null : _document.ExecutorPositionId);
                }
                res.Add(_operationDb.AddNewFileOrVersion(_context, att));
                // Модель фронта содержит дополнительно только одно поле - пользователя, который последний модифицировал файл. 
                // это поле не заполняется, иначе придется после каждого добавления файла делать запрос на выборку этого файла из таблицы
                // как вариант можно потому будет добавить получение имени текущего пользователя вначале и дописывать его к модели
                //res.Add(new FrontDocumentAttachedFile(att));
                var admContext = new AdminContext(_context);
                _queueWorkerService.AddNewTask(admContext, () =>
                {
                    if (_fStore.CreatePdfFile(admContext, att))
                    {
                        _operationDb.UpdateFilePdfView(admContext, att);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(_context, ex, "Error on adding document file");
                throw ex;
            }
            return res;
        }
    }
}