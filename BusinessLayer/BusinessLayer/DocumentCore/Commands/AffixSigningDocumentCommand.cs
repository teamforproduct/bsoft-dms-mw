﻿using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.DocumentCore.IncomingModel;
using System.Collections.Generic;

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
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = (int)EnumResultTypes.CloseByAffixing;
            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = _docWait.OnEvent.SourcePositionId } })
                .ToList();
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, null, Model.EventId, _docWait.OnEvent.TaskId,
                _docWait.OnEvent.SourcePositionId, null, _docWait.OnEvent.TargetPositionId, null, evAcceesses); 
            CommonDocumentUtilities.SetLastChange(_context, _document.Waits);
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);
            var subscription = _document.Subscriptions.First();
            subscription.Description = Model.VisaText;
            subscription.DoneEvent = _docWait.OffEvent;
            subscription.SubscriptionStates = CommonDocumentUtilities.SubscriptionStatesForAction[CommandType];
            subscription.SigningType = Model.SigningType;


            var isUseCertificateSign = GetIsUseCertificateSign();
            if (isUseCertificateSign && subscription.SigningType == EnumSigningTypes.CertificateSign)
            {
                subscription.CertificateId = Model.CertificateId;
                subscription.CertificatePassword = Model.CertificatePassword;
                subscription.CertificatePositionId = _context.CurrentPositionId;
                var positionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
                if (!positionExecutor?.ExecutorAgentId.HasValue ?? true)
                {
                    throw new ExecutorAgentForPositionIsNotDefined();
                }
                subscription.CertificatePositionExecutorAgentId = positionExecutor.ExecutorAgentId;
                subscription.CertificatePositionExecutorTypeId = positionExecutor.ExecutorTypeId;
            }

            //TODO HASH!!!!
            CommonDocumentUtilities.SetLastChange(Context, _document.Subscriptions);
            _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), isUseCertificateSign, Model.ServerPath);
            if (_document.IsLaunchPlan)
                DmsResolver.Current.Get<IAutoPlanService>().ManualRunAutoPlan(_context, null, _document.Id);
            return _document.Id;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());

    }
}