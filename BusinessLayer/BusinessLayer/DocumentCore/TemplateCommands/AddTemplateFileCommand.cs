﻿using System.IO;
using BL.CrossCutting.Context;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;

        public AddTemplateFileCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService)
        {
            _operationDb = operationDb;
            _fStore = fStore;
            _queueWorkerService = queueWorkerService;
        }

        private AddTemplateAttachedFile Model
        {
            get
            {
                if (!(_param is AddTemplateAttachedFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateAttachedFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            if (!_operationDb.CanAddTemplateAttachedFile(_context, Model))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            return true;

           
        }

        public override object Execute()
        {
            var att = new InternalTemplateAttachedFile
            {
                DocumentId = Model.DocumentId,
                OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId),
                //FileContent = Convert.FromBase64String(Model.FileData),
                Type = Model.Type,
                FileType = Model.FileType,
                //FileSize = Model.FileSize,
                Name = Path.GetFileNameWithoutExtension(Model.FileName),
                Extension = Path.GetExtension(Model.FileName ?? "").Replace(".", ""),
                PostedFileData = Model.PostedFileData,
                Description = Model.Description
            };
            _fStore.SaveFile(_context, att);
            CommonDocumentUtilities.SetLastChange(_context, att);
            _operationDb.AddNewFile(_context, att);

            var admContext = new AdminContext(_context);
            _queueWorkerService.AddNewTask(admContext, () =>
            {
                _fStore.CreatePdfFile(admContext, att);
                _operationDb.UpdateFilePdfView(admContext, att);
            });

            return att.Id;
        }


    }
}