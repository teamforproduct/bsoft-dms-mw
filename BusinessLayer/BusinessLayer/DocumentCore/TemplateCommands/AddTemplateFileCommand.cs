﻿using System;
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

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public AddTemplateFileCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyTemplateAttachedFile Model
        {
            get
            {
                if (!(_param is ModifyTemplateAttachedFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateAttachedFile)_param;
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
                IsAdditional = Model.IsAdditional,
                FileType = Model.FileType,
                //FileSize = Model.FileSize,
                Name = Path.GetFileNameWithoutExtension(Model.FileName),
                Extension = Path.GetExtension(Model.FileName ?? "").Replace(".", ""),
                PostedFileData = Model.PostedFileData,

            };
            _fStore.SaveFile(_context, att);
            CommonDocumentUtilities.SetLastChange(_context, att);
            _operationDb.AddNewFile(_context, att);

            return att.Id;
        }


    }
}