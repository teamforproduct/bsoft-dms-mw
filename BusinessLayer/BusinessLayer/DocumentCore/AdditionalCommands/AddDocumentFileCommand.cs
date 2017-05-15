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
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;
        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService)
        {
            _operationDb = operationDb;
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
            var tempStorageService = DmsResolver.Current.Get<ITempStorageService>();
            Model.ForEach(x => x.File = tempStorageService.GetStoreObject(x.TmpFileId) as BaseFile);
            if (Model.Any(x => x.File == null))
                throw new CannotAccessToFile();
            _adminProc.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddDocumentFilePrepare(_context, Model.Select(x => x.DocumentId).FirstOrDefault());
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            Model.ForEach(m =>
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
                    m.File.Name = mainFile.File.Name;
                    m.File.Extension = mainFile.File.Extension;
                    m.File.FileType = mainFile.File.FileType;
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
            var res = new List<int>();

            using (var transaction = Transactions.GetTransaction())
            {
                Model.ForEach(m =>
                            {
                                var executorPositionId = m.Type == EnumFileTypes.Main ? _document.ExecutorPositionId : (m.OrderInDocument.HasValue ? m.ExecutorPositionId : _context.CurrentPositionId);
                                var executorPositionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, executorPositionId);
                                if (!executorPositionExecutor?.ExecutorAgentId.HasValue ?? true)
                                {
                                    throw new ExecutorAgentForPositionIsNotDefined();
                                }
                                var att = CommonDocumentUtilities.GetNewDocumentFile(_context, (int)EnumEntytiTypes.Document, m, executorPositionExecutor);
                                if (m.OrderInDocument.HasValue)
                                {
                                    att.Version = _operationDb.GetFileNextVersion(_context, att.DocumentId, m.OrderInDocument.Value);
                                }
                                else
                                {
                                    m.File.Name = CommonDocumentUtilities.GetNextDocumentFileName(_context, att.DocumentId, m.File.Name, m.File.Extension);
                                    att.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, m.DocumentId);
                                }
                                _fStore.SaveFile(_context, att);
                                if (_document.IsRegistered.HasValue && !att.EventId.HasValue)
                                {
                                    att.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, att.DocumentId,
                                        EnumEventTypes.AddDocumentFile, null, null, $"{att.File.FileName} v.{att.Version}");
                                }
                                res.Add(_operationDb.AddNewFileOrVersion(_context, att));
                                var admContext = new AdminContext(_context);
                                _queueWorkerService.AddNewTask(admContext, () =>
                                {
                                    if (_fStore.CreatePdfFile(admContext, att))
                                    {
                                        _operationDb.UpdateFilePdfView(admContext, att);
                                    }
                                });
                            });
                transaction.Complete();
            }
            return res;
        }
    }
}