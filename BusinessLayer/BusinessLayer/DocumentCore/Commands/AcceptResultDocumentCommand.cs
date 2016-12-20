﻿using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;

namespace BL.Logic.DocumentCore.Commands
{
    public class AcceptResultDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public AcceptResultDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlOff Model
        {
            get
            {
                if (!(_param is ControlOff))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOff)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.Accesses!=null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.SourcePositionId == positionId &&
                        x.OnEvent.TargetPositionId != x.OnEvent.SourcePositionId &&
                        x.OffEventId == null &&
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
            if (_docWait == null)
            {
                throw new CouldNotPerformOperation();
            }
            //if (_docWait.OnEvent.)

            _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);

            if (_docWait?.OnEvent?.SourcePositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.ControlOffSendListPrepare(_context, _document);
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = Model.ResultTypeId;
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, null, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask, _docWait.OnEvent.TargetPositionId, null, _docWait.OnEvent.SourcePositionId);
            CommonDocumentUtilities.SetLastChange(_context, _document.Waits);
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);
            _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign());
            return _document.Id;
        }
        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());
    }
}