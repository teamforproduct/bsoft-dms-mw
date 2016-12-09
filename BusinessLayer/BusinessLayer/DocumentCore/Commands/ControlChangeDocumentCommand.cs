using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using System;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlChangeDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public ControlChangeDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
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
            if (_document.Accesses?.Count() != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
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
            if (_docWait?.OnEvent?.SourcePositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var addDescripton = (Model.DueDate != _docWait.DueDate ? "контрольный срок на ," : "")
                                + (Model.Description != _docWait.OnEvent.Description ? "формулировка задачи," : "")
                                + ((_eventType == EnumEventTypes.ControlChange && Model.AttentionDate != _docWait.AttentionDate ? ("дата постоянного внимания"+ (Model.AttentionDate.HasValue? ($" на {Model.AttentionDate.Value.ToString(@"dd.MM.yyyy")}") : "" ) + ",") : ""));
            if (!string.IsNullOrEmpty(addDescripton))
            {
                addDescripton = addDescripton.Remove(addDescripton.Length - 1);
                var controlOn = new ControlOn(Model, _docWait.DocumentId);
                var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, controlOn);
                newWait.Id = _docWait.Id;
                newWait.TargetDescription = _docWait.TargetDescription;
                newWait.AttentionDate = _eventType == EnumEventTypes.ControlChange ? Model.AttentionDate : _docWait.AttentionDate;

                var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, addDescripton, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask, _docWait.OnEvent.TargetPositionId);
                var oldEvent = _docWait.OnEvent;

                newEvent.Id = newWait.OnEventId = oldEvent.Id;

                newWait.OnEvent = newEvent;
                newWait.ParentWait = _docWait;

                _docWait.Id = 0;
                _docWait.ResultTypeId = (int)EnumResultTypes.CloseByChanging;
                oldEvent.Id = _docWait.OnEventId = 0;
                _docWait.OffEventId = newEvent.Id;

                CommonDocumentUtilities.SetLastChange(_context, _docWait);

                //var waits = new List<InternalDocumentWait> { newWait };

                _operationDb.ChangeDocumentWait(_context, newWait);
            }
            else
                throw new ContriolHasNotBeenChanged();
            return _docWait.DocumentId;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());

    }
}