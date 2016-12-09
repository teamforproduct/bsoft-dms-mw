using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlTargetChangeDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public ControlTargetChangeDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlTargetChange Model
        {
            get
            {
                if (!(_param is ControlTargetChange))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlTargetChange)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.Accesses != null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
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
            _document = _operationDb.ControlTargetChangeDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits?.FirstOrDefault();
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
            var addDescripton = (Model.TargetDescription != _docWait.TargetDescription ? "формулировка задачи" + "," : "")
                    + ((Model.TargetAttentionDate != _docWait.AttentionDate ? "дата постоянного внимания" + "," : ""));
            if (!string.IsNullOrEmpty(addDescripton))
            {
                addDescripton = addDescripton.Remove(addDescripton.Length - 1);
                var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.ControlTargetChange, Model.EventDate, Model.TargetDescription, addDescripton, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask);
                _docWait.TargetDescription = Model.TargetDescription;
                _docWait.AttentionDate = Model.TargetAttentionDate;
                CommonDocumentUtilities.SetLastChange(_context, _docWait);

                _operationDb.ChangeTargetDocumentWait(_context, _docWait, newEvent);
            }
            else
                throw new ContriolHasNotBeenChanged();
            return _docWait.DocumentId;
        }

    }
}