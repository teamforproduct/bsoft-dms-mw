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
using BL.CrossCutting.Interfaces;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _fileDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;
        private readonly ITempStorageService _tempStorageService;
        private InternalDocumentFile _file;
        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService, ITempStorageService tempStorageService)
        {
            _fileDb = operationDb;
            _fStore = fStore;
            _queueWorkerService = queueWorkerService;
            _tempStorageService = tempStorageService;
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

        private void FillModel(IContext ctx, List<AddDocumentFile> files)
        {
            files.ForEach(x =>
            {
                if (x.TmpFileId.HasValue)
                    x.File = new InternalDocumentFile { File = _tempStorageService.GetStoreObject(x.TmpFileId.Value) as BaseFile };
                else if ((x.CopyingFileId ?? x.MovingFileId).HasValue)
                {
                    x.File = _fileDb.GetDocumentFileInternal(_context, (x.CopyingFileId ?? x.MovingFileId).Value, x.IsAllVersionsProcessing ?? false);
                    _fStore.GetFile(_context, x.File);
                }
            });
        }

        public override bool CanExecute()
        {
            FillModel(_context, Model);
            var addModel = Model.Where(x => (x.CopyingFileId ?? x.MovingFileId).HasValue && (x.IsAllVersionsProcessing ?? false))
                .Select(x => x.File.OtherFileVersions.Select(y => new { Model = x, File = y }))
                .SelectMany(x => x.ToList()).Select(x => new AddDocumentFile
                {
                    TmpFileId = null,
                    LinkingFileId = null,
                    CopyingFileId = x.Model.CopyingFileId.HasValue ? x.File.Id : (int?)null,
                    MovingFileId = x.Model.MovingFileId.HasValue ? x.File.Id : (int?)null,
                    Description = x.Model.Description,
                    CurrentPositionId = x.Model.CurrentPositionId,
                    DocumentId = x.Model.DocumentId,
                    EventId = x.Model.EventId,
                    ExecutorPositionId = x.Model.ExecutorPositionId,
                    File = x.File,
                    IsAllVersionsProcessing = false,
                    Type = x.Model.Type,
                    OrderInDocument = -x.Model.File.Id, //для добавленных файлов оставляем ссылку на файл радотель, чтобы узнать номер файла, если его нет, а потом сгенерировать версию 
                    IsMainVersion = false,
                }).ToList();
            addModel.ForEach(x => _fStore.GetFile(_context, x.File));
            Model.AddRange(addModel);
            if (Model.Where(x => (x.TmpFileId ?? x.CopyingFileId ?? x.MovingFileId).HasValue).Any(x => x.File.File == null))
                throw new CannotAccessToFile();
            _adminProc.VerifyAccess(_context, CommandType);
            _document = _fileDb.AddDocumentFilePrepare(_context, Model.Select(x => x.DocumentId).FirstOrDefault());
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            Model.Where(x => x.TmpFileId.HasValue || x.CopyingFileId.HasValue || x.MovingFileId.HasValue).ToList().ForEach(m =>
             {
                 if (m.OrderInDocument.HasValue && m.OrderInDocument.Value > 0)
                 {
                     var mainFile = _document.DocumentFiles.FirstOrDefault(x => x.OrderInDocument == m.OrderInDocument);
                     if (mainFile == null
                         || (mainFile.Type == EnumFileTypes.Main && (m.IsMainVersion ?? false) && _document.ExecutorPositionId != _context.CurrentPositionId)
                         || (mainFile.Type == EnumFileTypes.Additional && (m.IsMainVersion ?? false) && mainFile.ExecutorPositionId != _context.CurrentPositionId)
                         || (m.MovingFileId.HasValue && m.File.ExecutorPositionId != _context.CurrentPositionId)
                         )
                     {
                         throw new CouldNotPerformOperation();
                     }
                     m.Type = mainFile.Type;
                     m.ExecutorPositionId = mainFile.ExecutorPositionId;
                 }
                 else if (!m.OrderInDocument.HasValue)
                 {
                     if (m.MovingFileId.HasValue)
                         throw new CouldNotPerformOperation();
                     m.IsMainVersion = true;
                     if (!m.Type.HasValue || (m.Type == EnumFileTypes.Main && _document.ExecutorPositionId != _context.CurrentPositionId))
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
                                    _file = new InternalDocumentFile { Id = x.LinkingFileId.Value, EventId = x.EventId.Value, IsLinkOnly = true, };
                                }
                                else
                                {
                                    if ((x.OrderInDocument ?? 0) < 0)
                                    {
                                        var parModel = Model.Where(y => y.File.Id == -x.OrderInDocument.Value).FirstOrDefault();
                                        x.OrderInDocument = parModel.OrderInDocument;
                                        x.ExecutorPositionId = parModel.ExecutorPositionId;
                                    }
                                    x.ExecutorPositionId = x.Type == EnumFileTypes.Main ? _document.ExecutorPositionId : (x.OrderInDocument.HasValue ? x.ExecutorPositionId : _context.CurrentPositionId);
                                    var executorPositionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, x.ExecutorPositionId);
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
                                        x.OrderInDocument = _file.OrderInDocument = _fileDb.GetNextFileOrderNumber(_context, x.DocumentId);
                                    }
                                    if (x.MovingFileId.HasValue)
                                        _file.Id = x.MovingFileId.Value;
                                    _fStore.SaveFile(_context, _file);
                                    if (_document.IsRegistered.HasValue && !_file.EventId.HasValue && _file.File != null)
                                    {
                                        _file.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId,
                                            EnumEventTypes.AddDocumentFile, null, null, $"{_file.File.FileName} v.{_file.Version}");
                                    }
                                }
                                res.Add(_fileDb.AddDocumentFile(_context, _file));
                                if (x.MovingFileId.HasValue)
                                    _fStore.DeleteFileVersion(_context, x.File);
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