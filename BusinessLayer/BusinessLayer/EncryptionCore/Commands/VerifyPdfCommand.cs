using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.Encryption.Interfaces;
using BL.Model.EncryptionCore.InternalModel;

namespace BL.Logic.EncryptionCore.Commands
{
    public class VerifyPdfCommand : BaseEncryptionCommand
    {

        private readonly IEncryptionDbProcess _encryptiontDb;

        public VerifyPdfCommand(IEncryptionDbProcess encryptiontDb)
        {
            _encryptiontDb = encryptiontDb;
        }

        private VerifyPdfCertificate Model
        {
            get
            {
                if (!(_param is VerifyPdfCertificate))
                {
                    throw new WrongParameterTypeError();
                }
                return (VerifyPdfCertificate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override object Execute()
        {
            return _encryptiontDb.VerifyCertificateSignPdf(Model.FileData, Model.ServerPath);
        }
    }
}