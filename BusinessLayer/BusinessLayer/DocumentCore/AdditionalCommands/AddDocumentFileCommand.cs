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
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _fileDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;
        private InternalDocumentFile _file;
        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService)
        {
            _fileDb = operationDb;
            _fStore = fStore;
            _queueWorkerService = queueWorkerService;
        }

        private List<AddDocumentFile> Model
        {
            get
            {
                if (!(_param is List<AddDocumentFile>))
                {
                    throw new WrongParameterTypeError();
                }
                return (List<AddDocumentFile>)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            var tempStorageService = DmsResolver.Current.Get<ITempStorageService>();
            Model.ForEach(x =>
            {
                if (x.TmpFileId.HasValue)
                    x.File = tempStorageService.GetStoreObject(x.TmpFileId.Value) as BaseFile;
                else if (x.CopyingFileId.HasValue)
                {
                    var file = _fileDb.GetDocumentFileInternal(_context, x.CopyingFileId.Value);
                    _fStore.GetFile(_context, file);
                    x.File = file.File;
                }
            });
            if (Model.Any(x => x.File == null))
                throw new CannotAccessToFile();
            _adminProc.VerifyAccess(_context, CommandType);
            _document = _fileDb.AddDocumentFilePrepare(_context, Model.Select(x => x.DocumentId).FirstOrDefault());
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            Model.Where(x => x.TmpFileId.HasValue || x.CopyingFileId.HasValue).ToList().ForEach(m =>
             {
                 if (m.OrderInDocument.HasValue)
                 {
                     var mainFile = _document.DocumentFiles.FirstOrDefault(x => x.OrderInDocument == m.OrderInDocument);
                     if (mainFile == null || m.Type != mainFile.Type
                         || (m.Type == EnumFileTypes.Main && (m.IsMainVersion ?? false) && _document.ExecutorPositionId != _context.CurrentPositionId)
                         || (m.Type == EnumFileTypes.Additional && (m.IsMainVersion ?? false) && mainFile.ExecutorPositionId != _context.CurrentPositionId)
                         )
                     {
                         throw new CouldNotPerformOperation();
                     }
                     m.ExecutorPositionId = mainFile.ExecutorPositionId;
                 }
                 else
                 {
                     m.IsMainVersion = true;
                     if (m.Type == EnumFileTypes.Main && _document.ExecutorPositionId != _context.CurrentPositionId)
                     {
                         throw new CouldNotPerformOperation();
                     }
                 }
             });
            return true;
        }

        public override object Execute()
        {
            var admContext = new AdminContext(_context);
            var res = new List<int>();
            using (var transaction = Transactions.GetTransaction())
            {
                Model.ForEach(x =>
                            {
                                if (x.LinkingFileId.HasValue && x.EventId.HasValue)
                                {
                                    _file = new InternalDocumentFile { Id = x.LinkingFileId.Value, EventId = x.EventId.Value, };
                                }
                                else
                                {
                                    var executorPositionId = x.Type == EnumFileTypes.Main ? _document.ExecutorPositionId : (x.OrderInDocument.HasValue ? x.ExecutorPositionId : _context.CurrentPositionId);
                                    var executorPositionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, executorPositionId);
                                    if (!executorPositionExecutor?.ExecutorAgentId.HasValue ?? true)
                                    {
                                        throw new ExecutorAgentForPositionIsNotDefined();
                                    }
                                    _file = CommonDocumentUtilities.GetNewDocumentFile(_context, (int)EnumEntytiTypes.Document, x, executorPositionExecutor);
                                    if (x.OrderInDocument.HasValue)
                                    {
                                        _file.Version = _fileDb.GetFileNextVersion(_context, _file.DocumentId, x.OrderInDocument.Value);
                                    }
                                    else
                                    {
                                        _file.OrderInDocument = _fileDb.GetNextFileOrderNumber(_context, x.DocumentId);
                                    }
                                    _fStore.SaveFile(_context, _file);
                                    if (_document.IsRegistered.HasValue && !_file.EventId.HasValue && _file.File != null)
                                    {
                                        _file.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId,
                                            EnumEventTypes.AddDocumentFile, null, null, $"{_file.File.FileName} v.{_file.Version}");
                                    }
                                }
                                res.Add(_fileDb.AddNewFileOrVersion(_context, _file));

                                _queueWorkerService.AddNewTask(admContext, () =>
                                {
                                    if (_fStore.CreatePdfFile(admContext, _file))
                                    {
                                        _fileDb.UpdateFilePdfView(admContext, _file);
                                    }
                                });
                            });
                transaction.Complete();
            }
            return res;
        }
    }
}