using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;

namespace BL.Logic.EncryptionCore.CertificateType
{
    public class ModifyEncryptionCertificateTypeCommand : BaseEncryptionCommand
    {
        private ModifyEncryptionCertificateType Model
        {
            get
            {
                if (!(_param is ModifyEncryptionCertificateType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyEncryptionCertificateType)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var item = _encryptionDb.ModifyCertificateTypePrepare(_context, Model.Id);
            if (item == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            return true;
        }

        public override object Execute()
        {
            var item = new InternalEncryptionCertificateType {
                Id = Model.Id,
                Name = Model.Name,
                Code = Model.Code,
            };

            CommonDocumentUtilities.SetLastChange(_context, item);

            _encryptionDb.ModifyCertificateType(_context, item);

            return Model.Id;
        }
    }
}