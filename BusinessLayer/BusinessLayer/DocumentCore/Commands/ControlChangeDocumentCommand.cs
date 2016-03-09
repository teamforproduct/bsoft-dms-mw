﻿using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlChangeDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentWait _docWait;

        public ControlChangeDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _operationDb = operationDb;
            _admin = admin;
        }

        private ControlChange Model
        {
            get
            {
                if (!(_param is ControlChange))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlChange)_param;
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
                        .Select( x=>new InternalActionRecord
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
            if (_docWait?.OnEvent?.SourcePositionId == null 
                || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformThisOperation();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var controlOn = new ControlOn(Model, _docWait.DocumentId, _docWait.OnEvent.Task);
            var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, controlOn);
            newWait.Id = _docWait.Id;

            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, _docWait.OnEvent.EventType, Model.Description, _docWait.OnEvent.Task, _docWait.OnEvent.TargetPositionId);
            var oldEvent = _docWait.OnEvent;

            newEvent.Id = newWait.OnEventId = oldEvent.Id;

            newWait.OnEvent = newEvent;


            newWait.ParentWait = _docWait;

            _docWait.Id = 0;
            oldEvent.Id = _docWait.OnEventId = 0;
            _docWait.OffEventId = newEvent.Id;
           // _docWait.OffEvent = newEvent;
            CommonDocumentUtilities.SetLastChange(_context, _docWait);

            //var waits = new List<InternalDocumentWait> { newWait };

            _operationDb.ChangeDocumentWait(_context, newWait);
            return _docWait.DocumentId;
        }

    }
}