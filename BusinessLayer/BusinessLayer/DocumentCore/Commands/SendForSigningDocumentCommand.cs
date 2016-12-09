﻿using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForSigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
                if (model.SendType != EnumSendTypes.SendForSigning
                    && model.SendType != EnumSendTypes.SendForVisaing
                    && model.SendType != EnumSendTypes.SendForАgreement
                    && model.SendType != EnumSendTypes.SendForАpproval
                    )
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
            _document = _operationDb.SendForInformationDocumentPrepare(_context, Model);
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
                ex = new TaskIsNotDefined();
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
                    SubordinationType = EnumSubordinationTypes.Informing,
                    TargetPosition = Model.TargetPositionId.Value,
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, Model, _document),
                }))
            {
                ex = new SubordinationHasBeenViolated();
            }
            if (Model.TargetPositionId.HasValue
                && (Model.DueDate.HasValue || Model.DueDay.HasValue)
                && !_admin.VerifySubordination(_context, new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Execution,
                    TargetPosition = Model.TargetPositionId.Value,
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, Model, _document),
                }))
            {
                ex = new SubordinationForDueDateHasBeenViolated();
            }

            if (ex != null)
            {
                Model.AddDescription = ex.Message;
                _operationDb.ModifyDocumentSendListAddDescription(_context, Model);
                throw ex;
            }
            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }
        public override object Execute()
        {
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);

            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, _eventType, EnumEventCorrespondentType.FromSourceToTarget);
            _document.Waits = new List<InternalDocumentWait> { waitTarget };

            var subscription = CommonDocumentUtilities.GetNewDocumentSubscription(_context, Model);
            subscription.SendEvent = waitTarget.OnEvent;

            _document.Subscriptions = new List<InternalDocumentSubscription> { subscription };

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