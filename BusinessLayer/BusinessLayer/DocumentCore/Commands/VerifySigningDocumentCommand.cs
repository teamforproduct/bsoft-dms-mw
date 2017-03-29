using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.EncryptionCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class VerifySigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public VerifySigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private VerifySignCertificate Model
        {
            get
            {
                if (!(_param is VerifySignCertificate))
                {
                    throw new WrongParameterTypeError();
                }
                return (VerifySignCertificate)_param;
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
            _document = _operationDb.SelfAffixSigningDocumentPrepare(_context, Model.Id);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            return true;
        }

        public override object Execute()
        {
            var isUseCertificateSign = GetIsUseCertificateSign();
            
            _operationDb.VerifySigningDocument(_context, Model.Id, GetIsUseInternalSign(), isUseCertificateSign, Model.ServerPath);
            return _document.Id;
        }

    }
}