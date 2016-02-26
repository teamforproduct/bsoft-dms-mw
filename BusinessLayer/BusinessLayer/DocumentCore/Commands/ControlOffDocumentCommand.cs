﻿using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOffDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        private InternalDocumentWait _docWait;

        public ControlOffDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _operationDb = operationDb;
            _adminDb = adminDb;
        }

        private ControlOff Model
        {
            get
            {
                if (!(_param is ControlOff))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOff) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ControlOffDocumentPrepare(_context, Model.EventId);
            _docWait = _document.Waits.FirstOrDefault();
            if (_docWait?.OnEvent?.SourcePositionId == null)
            {
                throw new EventNotFoundOrUserHasNoAccess();
            }
            if (_docWait.OffEventId != null)
            {
                throw new WaitHasAlreadyClosed(); 
            }
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _adminDb.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {

            _docWait.ResultTypeId = Model.ResultTypeId;

            _docWait.OffEvent =
                CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.ControlOff,
                    $"{_docWait.Task} / {Model.Description}", _docWait.OnEvent.TargetPositionId)
                    .FirstOrDefault();

            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            _operationDb.CloseDocumentWait(_context, _docWait);
            return _docWait.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ControlOff;
    }
}