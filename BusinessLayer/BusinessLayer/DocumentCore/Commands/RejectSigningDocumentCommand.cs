using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.DocumentCore.Interfaces;
using System.Transactions;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class RejectSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public RejectSigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
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
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
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
            _docWait.ResultTypeId = (int)EnumResultTypes.CloseByRejecting;
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, null, _docWait.OnEvent.TaskId, _docWait.OnEvent.IsAvailableWithinTask, _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId);
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            var sendList = _document.SendLists.FirstOrDefault(x => x.IsInitial);
            if (sendList != null)
            {
                sendList.StartEventId = null;
            }
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);

            var subscription = _document.Subscriptions.First();
            subscription.Description = CommandType.ToString();
            subscription.DoneEvent = _docWait.OffEvent;
            subscription.SubscriptionStates = EnumSubscriptionStates.No;
            //TODO HASH!!!!
            CommonDocumentUtilities.SetLastChange(Context, _document.Subscriptions);

            //TODO null
            //var subscription = _document.Subscriptions.First();
            //subscription.Description = CommandType.ToString();
            //subscription.DoneEvent = null;
            //subscription.SubscriptionStates = EnumSubscriptionStates.No;
            //CommonDocumentUtilities.SetLastChange(Context, _document.Subscriptions);
            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign());
                if (sendList != null)
                {
                    var docProc = DmsResolver.Current.Get<IDocumentService>();
                    if (_document.IsLaunchPlan)
                    {
                        var adminCtx = new CrossCutting.Context.AdminContext(_context);
                        docProc.ExecuteAction(EnumDocumentActions.StopPlan, adminCtx, _document.Id);
                    }
                }
                transaction.Complete();
            }
            //if (sendList != null)
            //{
            //    var aplan = DmsResolver.Current.Get<IAutoPlanService>();
            //    aplan.ManualRunAutoPlan(_context);
            //}
            return _document.Id;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());

    }
}