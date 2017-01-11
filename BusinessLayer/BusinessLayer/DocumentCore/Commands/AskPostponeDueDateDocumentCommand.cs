using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class AskPostponeDueDateDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public AskPostponeDueDateDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AskPostponeDueDate Model
        {
            get
            {
                if (!(_param is AskPostponeDueDate))
                {
                    throw new WrongParameterTypeError();
                }
                return (AskPostponeDueDate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            //var askPostponeDueDateWaitId = _document.Waits.Where(x => x.OnEvent.EventType == EnumEventTypes.AskPostponeDueDate && !x.OffEventId.HasValue).Select(x => x.ParentId).ToList();
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
                        x.OnEvent.TargetPositionId != x.OnEvent.SourcePositionId &&
                        x.OffEventId == null &&
                        !x.IsHasMarkExecution &&
                        //!askPostponeDueDateWaitId.Contains(x.Id) &&
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
            _operationDb.ControlOffAskPostponeDueDateWaitPrepare(_context, _document);
            if (_docWait?.OnEvent?.TargetPositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.AskPostponeDueDate, Model.EventDate, Model.Description, null, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask, _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId);

            var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, _document.Id, newEvent);

            newWait.PlanDueDate = Model.PlanDueDate;

            newWait.ParentId = _docWait.Id;

            _document.Waits = new List<InternalDocumentWait> { newWait };

            _operationDb.AddDocumentWaits(_context, _document);
            return _document.Id;
        }


    }
}