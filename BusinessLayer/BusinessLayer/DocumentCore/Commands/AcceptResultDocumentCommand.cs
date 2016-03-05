﻿using System.Collections.Generic;
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
    public class AcceptResultDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentWait _docWait;

        public AcceptResultDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _operationDb = operationDb;
            _admin = admin;
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
            if (_docWait?.OnEvent?.SourcePositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformThisOperation();
            }
            _operationDb.ControlOffSendListPrepare(_context, _document);
            _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = Model.ResultTypeId;
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.AcceptResult, Model.Description, _docWait.OnEvent.Task, _docWait.OnEvent.TargetPositionId, null, _docWait.OnEvent.SourcePositionId);
            CommonDocumentUtilities.SetLastChange(_context, _document.Waits);
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);
            _operationDb.CloseDocumentWait(_context, _document);
            return _document.Id;
        }


        public override EnumDocumentActions CommandType => EnumDocumentActions.MarkExecution;
    }
}