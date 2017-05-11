using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForControlDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentService _documentServ;
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForControlDocumentCommand(IDocumentService documentServ, IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentServ = documentServ;
            _documentDb = documentDb;
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
                if (model.SendType != EnumSendTypes.SendForControl)
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
            else if (_document.Events != null && _document.Events.Any())
            {
                ex = new ControlerHasAlreadyBeenDefined();
            }

            if (Model.TargetPositionId.HasValue
                && (_document.RestrictedSendLists?.Any() ?? false)
                && !_document.RestrictedSendLists.Select(x => x.PositionId).Contains(Model.TargetPositionId.Value)
                )
            {
                ex = new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            if (Model.TargetPositionId.HasValue
                && !_adminProc.VerifySubordination(_context, new VerifySubordination
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
            _document.Subscriptions = null;

            var newEvent = Model.CloseEvent = Model.StartEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model);
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, (int)EnumEntytiTypes.Document, Model.AccessLevel, newEvent.Accesses);
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromTargetToTarget, true, true); //TODO ? Can present copy
            waitTarget.OnEvent.AddDescription = $"##l@TaskExecutor:Initiator@l## - {Model.InitiatorPositionExecutorAgentName}({Model.InitiatorPositionName})";
            _document.Waits = new List<InternalDocumentWait> { waitTarget };
            if (Model.IsAddControl)
            {
                _document.Waits = _document.Waits.Concat(CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource, false));
            }
            _operationDb.SendBySendList(_context, _document);

            _documentServ.CheckIsInWorkForControls(_context, new FilterDocumentAccess { DocumentId = new List<int> { _document.Id } });

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}