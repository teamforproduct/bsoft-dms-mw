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
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class MarkExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentWait _docWait;

        public MarkExecutionDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
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

        public override bool CanBeDisplayed(int positionId)
        {
            var markExecWaitId =
                _document.Waits.Where(x => x.OnEvent.EventType == EnumEventTypes.MarkExecution).Select(x => x.ParentId).ToList();
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
                        x.OnEvent.TargetPositionId != x.OnEvent.SourcePositionId &&
                        x.OffEventId == null &&
                        !markExecWaitId.Contains(x.Id) &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType].Contains(x.OnEvent.EventType))
                        .Select(x => new InternalActionRecord
                        {
                            EventId = x.OnEvent.Id,
                            WaitId = x.Id
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ControlOffDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits.FirstOrDefault();
            _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);
            if (_docWait?.OnEvent?.TargetPositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformThisOperation();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.MarkExecution, Model.Description, _docWait.OnEvent.Task, _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId);

            var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, _document.Id, newEvent);

            newWait.ParentId = _docWait.Id;

            _document.Waits = new List<InternalDocumentWait> { newWait };

            _operationDb.AddDocumentWaits(_context, _document);
            return _document.Id;
        }


    }
}