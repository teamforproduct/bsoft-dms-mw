﻿using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Logic.Common;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.FileWorker;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    internal class DeleteDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminService _admin;
        private readonly IFileStore _fStore;

        public DeleteDocumentCommand(IDocumentsDbProcess documentDb, IAdminService admin, IFileStore fStore)
        {
            _documentDb = documentDb;
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            return action.DocumentAction == CommandType && !_document.IsRegistered;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.DeleteDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

           if (_document.IsRegistered)
            {
                throw new DocumentCannotBeModifiedOrDeleted();
            }
            return true;
        }

        public override object Execute()
        {
            if (_document.DocumentFiles != null && _document.DocumentFiles.Any())
            {
                _fStore.DeleteAllFileInDocument(_context, _document.Id);
            }

            _documentDb.DeleteDocument(_context, _document.Id);
            return null;
        }

    }
}