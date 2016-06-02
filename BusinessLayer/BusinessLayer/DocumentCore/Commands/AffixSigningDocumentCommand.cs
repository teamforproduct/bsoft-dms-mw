using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;

namespace BL.Logic.DocumentCore.Commands
{
    public class AffixSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public AffixSigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AffixSigning Model
        {
            get
            {
                if (!(_param is AffixSigning))
                {
                    throw new WrongParameterTypeError();
                }
                return (AffixSigning)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
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
            if (_docWait?.OnEvent?.TargetPositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.ControlOffSendListPrepare(_context, _document);
            _operationDb.ControlOffSubscriptionPrepare(_context, _document);
            _context.SetCurrentPosition(_docWait.OnEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = (int)EnumResultTypes.CloseByAffixing;
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask,
                _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId);
            CommonDocumentUtilities.SetLastChange(_context, _document.Waits);
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);
            var subscription = _document.Subscriptions.First();
            subscription.Description = Model.VisaText;
            subscription.DoneEvent = _docWait.OffEvent;
            subscription.SubscriptionStates = CommonDocumentUtilities.SubscriptionStatesForAction[CommandType];
            //TODO HASH!!!!
            CommonDocumentUtilities.SetLastChange(Context, _document.Subscriptions);
            _operationDb.CloseDocumentWait(_context, _document);
            return _document.Id;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());

    }
}