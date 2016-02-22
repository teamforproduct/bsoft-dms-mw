﻿using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class ChangeExecutorCommand : BaseCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public ChangeExecutorCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
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
                return _param as ChangeExecutor;
            }
        }

        public override bool CanBeDisplayed()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { ActionCode = EnumActions.ChangeExecutor, PositionId = _context.CurrentPositionId });
                if (_document == null || _document.ExecutorPositionId != _context.CurrentPositionId)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { ActionCode = EnumActions.ChangeExecutor, PositionId = _context.CurrentPositionId });
                _document = _documentDb.ChangeExecutorDocumentPrepare(_context, Model);
                if (_document == null || _document.ExecutorPositionId != _context.CurrentPositionId)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChangeForDocument(_context, _document);

            _document.ExecutorPositionId = Model.PositionId;
            _document.AccessLevel = Model.AccessLevel;

            _document.Events = CommonDocumentUtilities.GetEventForChangeExecutorDocument(_context, Model);

            _document.Accesses = CommonDocumentUtilities.GetAccessesForChangeExecutorDocument(_context, Model);

            _documentDb.ChangeExecutorDocument(_context, _document);

            return Model.DocumentId;
        }
    }
}