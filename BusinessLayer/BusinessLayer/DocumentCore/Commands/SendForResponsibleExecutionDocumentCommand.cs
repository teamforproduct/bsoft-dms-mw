using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.AdminCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForResponsibleExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForResponsibleExecutionDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
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
            _admin.VerifyAccess(_context, CommandType);   //TODO без позиций
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
            else if (!Model.TargetPositionId.HasValue)
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

            if (Model.TargetPositionId.HasValue
                && (_document.RestrictedSendLists?.Any() ?? false)
                && !_document.RestrictedSendLists.Select(x => x.PositionId).Contains(Model.TargetPositionId.Value)
                )
            {
                ex = new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            if (Model.TargetPositionId.HasValue
                && !_admin.VerifySubordination(_context, new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Execution,
                    TargetPosition = Model.TargetPositionId.Value,
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
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);

            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, _eventType, EnumEventCorrespondentType.FromSourceToTarget);

            if (_document.Events?.Any() ?? false)
            {
                var eventControler = _document.Events.First();
                waitTarget.OnEvent.SourcePositionId = eventControler.TargetPositionId;
                waitTarget.OnEvent.SourcePositionExecutorAgentId = eventControler.TargetPositionExecutorAgentId;
                waitTarget.OnEvent.SourcePositionExecutorTypeId = eventControler.TargetPositionExecutorTypeId;
                if (Model.SourcePositionId != waitTarget.OnEvent.SourcePositionId)
                {
                    _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model, EnumEventTypes.InfoSendForResponsibleExecutionReportingControler);
                    waitTarget.OnEvent.AddDescription = $"##l@TaskExecutor:Initiator@l## - {Model.InitiatorPositionExecutorAgentName}({Model.InitiatorPositionName}), ##l@TaskExecutor:Controler@l## - {eventControler.TargetPositionExecutorAgentName}({eventControler.TargetPositionName})";
                    CommonDocumentUtilities.SetLastChange(_context, waitTarget.OnEvent);
                    if (waitTarget.OnEvent.Date == waitTarget.OnEvent.CreateDate)
                        waitTarget.OnEvent.Date = waitTarget.OnEvent.CreateDate = waitTarget.OnEvent.LastChangeDate;
                }
            }
            _document.Waits = new List<InternalDocumentWait> { waitTarget };

            _document.Subscriptions = null;

            if (Model.IsAddControl)
            {
                ((List<InternalDocumentWait>)_document.Waits).AddRange(CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource));
            }

            Model.StartEvent = waitTarget.OnEvent;
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };


            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}