using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class SelfAffixSigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

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
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.SelfAffixSigningDocumentPrepare(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _document.Events = new List<InternalDocumentEvent> { CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _document.Id, EnumEventTypes.AffixSigning, Model.EventDate, Model.Description) };

            var subscription = new InternalDocumentSubscription
            {
                DocumentId = _document.Id,
                ClientId = _document.ClientId,
                EntityTypeId = _document.EntityTypeId,
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

            _operationDb.SelfAffixSigningDocument(_context, _document, GetIsUseInternalSign(), isUseCertificateSign, Model.ServerPath);
            return _document.Id;
        }

    }
}