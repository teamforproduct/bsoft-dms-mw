﻿using System.Linq;
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

        private InternalDocumentWait _docWait;

        public ControlOffDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
                return (ControlOff) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.SourcePositionId == positionId &&
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
            _document = _operationDb.ControlChangeDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits?.FirstOrDefault();
            if (_docWait?.OnEvent?.SourcePositionId == null || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value))
            {
                throw new CouldNotPerformThisOperation();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {

            _docWait.ResultTypeId = Model.ResultTypeId;

            _docWait.OffEvent =
                CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.ControlOff,Model.Description, _docWait.OnEvent.Task, _docWait.OnEvent.TargetPositionId);
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            _operationDb.CloseDocumentWait(_context, _document);
            return _docWait.DocumentId;
        }

    }
}