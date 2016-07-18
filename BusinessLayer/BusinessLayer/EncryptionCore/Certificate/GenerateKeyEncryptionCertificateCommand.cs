using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;
using BL.Database.EncryptionWorker;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class GenerateKeyEncryptionCertificateCommand : BaseEncryptionCommand
    {
        private readonly IEncryptionGeneratorKey _eGeneratorKey;

        private InternalEncryptionCertificate _certificate;

        public GenerateKeyEncryptionCertificateCommand(IEncryptionGeneratorKey eGeneratorKey)
        {
            _eGeneratorKey = eGeneratorKey;
        }
        private GenerateKeyEncryptionCertificate Model
        {
            get
            {
                if (!(_param is GenerateKeyEncryptionCertificate))
                {
                    throw new WrongParameterTypeError();
                }
                return (GenerateKeyEncryptionCertificate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            _certificate = _eGeneratorKey.GenerateKey(_context, Model);

            return true;
        }

        public override object Execute()
        {
            var item = _certificate;

            CommonDocumentUtilities.SetLastChange(_context, item);
            _encryptionDb.AddCertificate(_context, item);

            return item.Id;
        }
    }
}