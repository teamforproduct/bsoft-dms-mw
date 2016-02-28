﻿using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ChangeExecutorDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public ChangeExecutorDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private ChangeExecutor Model
        {
            get
            {
                if (!(_param is ChangeExecutor))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeExecutor)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeExecutorDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.IsRegistered)
            {
                throw new DocumentHasAlredyBeenRegistered();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);

            _document.ExecutorPositionId = Model.PositionId;
            _document.AccessLevel = Model.AccessLevel;

            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model.DocumentId, EnumEventTypes.ChangeExecutor, Model.Description, null, Model.PositionId);

            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.PositionId);

            _documentDb.ChangeExecutorDocument(_context, _document);

            return Model.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ChangeExecutor;
    }
}