using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.AdminCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForResponsibleExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentService _documentServ;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForResponsibleExecutionDocumentCommand(IDocumentService documentServ, IDocumentOperationsDbProcess operationDb)
        {
            _documentServ = documentServ;
            _operationDb = operationDb;
        }

        private InternalDocumentSendList Model
        {
            get
            {
                if (!(_param is InternalDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                var model = (InternalDocumentSendList)_param;
                if (model.SendType != EnumSendTypes.SendForResponsibleExecution)
                {
                    throw new WrongParameterTypeError();
                }
                return (InternalDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(Model.SourcePositionId);
            _adminProc.VerifyAccess(_context, CommandType);   //TODO без позиций
            _document = _operationDb.SendForExecutionDocumentPrepare(_context, Model);
            DmsExceptions ex = null;
            if (_document == null)
            {
                ex = new DocumentNotFoundOrUserHasNoAccess();
            }
            else if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                ex = new PlanPointHasAlredyBeenLaunched();
            }
            else if (!Model.TargetPositionId.HasValue || Model.AccessGroups.Count(x => x.AccessType == EnumEventAccessTypes.Target) > 1)
            {
                ex = new TargetIsNotDefined();
            }
            else if (!Model.TaskId.HasValue)
            {
                ex = new TaskIsNotDefined();
            }
            else if (_document.Waits != null && _document.Waits.Any())
            {
                ex = new ResponsibleExecutorHasAlreadyBeenDefined();
            }
            _operationDb.SetRestrictedSendListsPrepare(_context, _document);
            if (Model.TargetPositionId.HasValue
                && !_adminProc.VerifySubordination(_context, new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Execution,
                    TargetPosition = new List<int> { Model.TargetPositionId.Value },
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, Model, _document),
                }))
            {
                ex = new SubordinationHasBeenViolated();
            }

            if (ex != null) CommonDocumentUtilities.ThrowError(_context, ex, Model);

            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }
        public override object Execute()
        {
            _document.Subscriptions = null;
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);

            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, _eventType, EnumEventCorrespondentType.FromSourceToTarget);
            var newEvent = Model.StartEvent = waitTarget.OnEvent;
            var ex = CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses,
                new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Informing,
                    TargetPosition = newEvent.Accesses.Where(x => x.AccessType != EnumEventAccessTypes.Source && x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList(),
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, Model, _document),
                },
                true, Model.AccessLevel);
            if (ex != null) CommonDocumentUtilities.ThrowError(_context, ex, Model);
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            if (_document.Events?.Any() ?? false)
            {
                var eventControler = _document.Events.First();
                var accessSource = waitTarget.OnEvent.Accesses.First(x => x.AccessType == EnumEventAccessTypes.Source);
                accessSource.PositionId = eventControler.TargetPositionId;
                accessSource.AgentId = eventControler.TargetPositionExecutorAgentId;
                accessSource.PositionExecutorTypeId = eventControler.TargetPositionExecutorTypeId;
                if (Model.SourcePositionId != eventControler.TargetPositionId)
                {
                    _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model, EnumEventTypes.InfoSendForResponsibleExecutionReportingControler);
                    waitTarget.OnEvent.AddDescription = $"##l@TaskExecutor:Initiator@l## - {Model.InitiatorPositionExecutorAgentName}({Model.InitiatorPositionName}), ##l@TaskExecutor:Controler@l## - {eventControler.TargetPositionExecutorAgentName}({eventControler.TargetPositionName})";
                    CommonDocumentUtilities.SetLastChange(_context, waitTarget.OnEvent);
                    if (waitTarget.OnEvent.Date == waitTarget.OnEvent.CreateDate)
                        waitTarget.OnEvent.Date = waitTarget.OnEvent.CreateDate = waitTarget.OnEvent.LastChangeDate;
                }
            }
            _document.Waits = new List<InternalDocumentWait> { waitTarget };
            if (Model.IsAddControl)
            {
                _document.Waits = _document.Waits.Concat(CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource,false));
            }

            _operationDb.SendBySendList(_context, _document);

            _documentServ.CheckIsInWorkForControls(_context, new FilterDocumentAccess { DocumentId = new List<int> { _document.Id } });

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}