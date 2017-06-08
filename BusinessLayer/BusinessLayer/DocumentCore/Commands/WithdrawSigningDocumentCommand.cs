using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using BL.Logic.DocumentCore.Interfaces;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Model.DocumentCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class WithdrawSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public WithdrawSigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
                        x.OnEvent.ControllerPositionId == positionId &&
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
            if (_docWait?.OnEvent?.ControllerPositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.ControllerPositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.SetSendListForControlOffPrepare(_context, _document);
            _operationDb.SetSubscriptionForControlOffPrepare(_context, _document);
            _context.SetCurrentPosition(_docWait.OnEvent.ControllerPositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = (int)EnumResultTypes.CloseByWithdrawing;
            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = _docWait.OnEvent.TargetPositionId } })
                .Concat(CommonDocumentUtilities.GetAccessGroupsFileExecutors(_context, _document.Id, Model.AddDocumentFiles))
                .ToList();
            var newEvent = _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, _eventType, Model.EventDate, 
                                                                                            Model.Description, null, Model.EventId, _docWait.OnEvent.TaskId, evAcceesses);
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent);
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
            CommonDocumentUtilities.SetLastChange(Context, _document.Subscriptions);

            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign(), Model.ServerPath);
                if (Model.AddDocumentFiles?.Any() ?? false)
                {
                    Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Waits.Select(y => y.OffEventId).First(); });
                    _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                }
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
            return _document.Id;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());

    }
}