﻿using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class DeleteTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;
        private readonly IFileStore _fStore;
        InternalTemplateFile _docFile;

        public DeleteTemplateFileCommand(ITemplateDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            _docFile = _operationDb.DeleteTemplateFilePrepare(_context, Model);
            if (_docFile == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            _fStore.DeleteFile(_context, _docFile);
            _operationDb.DeleteTemplateFile(_context, Model);
            return null;
        }

    }
}