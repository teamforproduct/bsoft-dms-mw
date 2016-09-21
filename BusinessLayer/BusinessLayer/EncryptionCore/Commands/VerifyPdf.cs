using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.Encryption.Interfaces;

namespace BL.Logic.EncryptionCore.Commands
{
    public class VerifyPdf : BaseEncryptionCommand
    {

        private readonly IEncryptionDbProcess _encryptiontDb;

        public VerifyPdf(IEncryptionDbProcess encryptiontDb)
        {
            _encryptiontDb = encryptiontDb;
        }

        private byte[] Model
        {
            get
            {
                if (!(_param is byte[]))
                {
                    throw new WrongParameterTypeError();
                }
                return (byte[])_param;
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
            return _encryptiontDb.VerifyCertificateSignPdf(Model);
        }
    }
}