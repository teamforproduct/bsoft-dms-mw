using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class SelfAffixSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        //private InternalDocumentWait _docWait;

        public SelfAffixSigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private SelfAffixSigning Model
        {
            get
            {
                if (!(_param is SelfAffixSigning))
                {
                    throw new WrongParameterTypeError();
                }
                return (SelfAffixSigning)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.SelfAffixSigningDocumentPrepare(_context, Model.DocumentId);
            //_docWait = _document?.Waits.FirstOrDefault();
            //throw new CouldNotPerformOperation();
            //_operationDb.ControlOffSendListPrepare(_context, _document);
            //_operationDb.ControlOffSubscriptionPrepare(_context, _document);

            if (Model.CurrentPositionId.HasValue)
            {
                _context.SetCurrentPosition(Model.CurrentPositionId.Value);
            }
            else
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }

            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _document.Events = new List<InternalDocumentEvent> { CommonDocumentUtilities.GetNewDocumentEvent(_context, _document.Id, EnumEventTypes.AffixSigning, Model.EventDate, Model.Description, null, null, false,
              _context.CurrentPositionId, null, _context.CurrentPositionId) };

            var subscription = new InternalDocumentSubscription
            {
                DocumentId = _document.Id,
                SubscriptionStates = CommonDocumentUtilities.SubscriptionStatesForAction[CommandType],
                Description = Model.VisaText,
                SigningType = Model.SigningType,
            };

            var isUseCertificateSign = GetIsUseCertificateSign();
            if (isUseCertificateSign && Model.SigningType == EnumSigningTypes.CertificateSign)
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

            CommonDocumentUtilities.SetLastChange(Context, subscription);

            _document.Subscriptions = new List<InternalDocumentSubscription> { subscription };

            _operationDb.SelfAffixSigningDocument(_context, _document, GetIsUseInternalSign(), isUseCertificateSign);
            return _document.Id;
        }

    }
}