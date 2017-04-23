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
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x => (x.OnEvent.EventType == EnumEventTypes.AskPostponeDueDate && x.OnEvent.TargetPositionId == positionId ||
                        x.OnEvent.EventType != EnumEventTypes.AskPostponeDueDate && x.OnEvent.SourcePositionId == positionId) &&
                        x.OffEventId == null &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType]
                            .Contains(x.OnEvent.EventType != EnumEventTypes.AskPostponeDueDate ? x.OnEvent.EventType : _document.Waits.Where(y=>y.Id == x.ParentId).Select(y=>y.OnEvent.EventType).FirstOrDefault()))
                        .Select(x => new InternalActionRecord
                        {
                            EventId = x.OnEvent.Id,
                            WaitId = x.Id,
                            IsHideInMainMenu = x.OnEvent.EventType != EnumEventTypes.AskPostponeDueDate && x.IsHasAskPostponeDueDate

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
            if (_docWait.IsHasAskPostponeDueDate)
                _operationDb.ControlOffAskPostponeDueDateWaitPrepare(_context, _document);
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
                var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, (int)EnumEntytiTypes.Document, controlOn);
                newWait.Id = _docWait.Id;
                newWait.TargetDescription = _docWait.TargetDescription;
                newWait.AttentionDate = _eventType == EnumEventTypes.ControlChange ? Model.AttentionDate : _docWait.AttentionDate;

                var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, addDescripton, 
                    null, //TODO SET PARENT ????!!!!
                    _docWait.OnEvent.TaskId, _docWait.OnEvent.TargetPositionId);
                var oldEvent = _docWait.OnEvent;

                newEvent.Id = newWait.OnEventId = oldEvent.Id;
                newEvent.Accesses?.ToList().ForEach(x => x.EventId = newEvent.Id);
                newEvent.AccessGroups?.ToList().ForEach(x => x.EventId = newEvent.Id);


                newWait.OnEvent = newEvent;
                newWait.ParentWait = _docWait;

                _docWait.Id = 0;
                _docWait.ResultTypeId = (int)EnumResultTypes.CloseByChanging;
                oldEvent.Id = _docWait.OnEventId = 0;
                _docWait.OffEventId = newEvent.Id;

                CommonDocumentUtilities.SetLastChange(_context, _docWait);

                if (_document.Waits.Any(x=>x.OnEvent.EventType == EnumEventTypes.AskPostponeDueDate))
                {
                    newWait.AskPostponeDueDateWait = _document.Waits.First(x => x.OnEvent.EventType == EnumEventTypes.AskPostponeDueDate);
                    newWait.AskPostponeDueDateWait.ResultTypeId = (int)EnumResultTypes.CloseByChanging;
                    newWait.AskPostponeDueDateWait.OffEventId = newEvent.Id;
                    newWait.AskPostponeDueDateWait.ParentId = 0;
                    CommonDocumentUtilities.SetLastChange(_context, newWait.AskPostponeDueDateWait);
                }
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