using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;

namespace BL.Logic.DocumentCore.Commands
{
    public class RejectResultDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentWait _docWait;

        public RejectResultDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _operationDb = operationDb;
            _admin = admin;
        }

        private SendEventMessage Model
        {
            get
            {
                if (!(_param is SendEventMessage))
                {
                    throw new WrongParameterTypeError();
                }
                return (SendEventMessage)_param;
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
            if (_docWait?.OnEvent?.SourcePositionId == null || _docWait?.OnEvent?.TargetPositionId == null)
            {
                throw new EventNotFoundOrUserHasNoAccess();
            }
            if (_docWait.OffEventId != null)
            {
                throw new WaitHasAlreadyClosed();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.RejectResult, Model.Description, _docWait.OnEvent.Task, _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId);
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            _operationDb.CloseDocumentWait(_context, _docWait);
            return _document.Id;
        }


        public override EnumDocumentActions CommandType => EnumDocumentActions.MarkExecution;
    }
}