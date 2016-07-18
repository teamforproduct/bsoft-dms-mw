using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class ModifyEncryptionCertificateCommand : BaseEncryptionCommand
    {
        private ModifyEncryptionCertificate Model
        {
            get
            {
                if (!(_param is ModifyEncryptionCertificate))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyEncryptionCertificate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var item = _encryptionDb.ModifyCertificatePrepare(_context, Model.Id);
            if (item == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            return true;
        }

        public override object Execute()
        {
            var item = new InternalEncryptionCertificate {
                Id = Model.Id,
                Name = Model.Name
            };
            CommonDocumentUtilities.SetLastChange(_context, item);
            _encryptionDb.ModifyCertificate(_context, item);

            return Model.Id;
        }
    }
}