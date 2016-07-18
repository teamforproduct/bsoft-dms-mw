using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;
using System.IO;

namespace BL.Logic.EncryptionCore.CertificateType
{
    public class AddEncryptionCertificateTypeCommand : BaseEncryptionCommand
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

            return true;
        }

        public override object Execute()
        {
            var item = new InternalEncryptionCertificateType
            {
                Name = Model.Name,
                Code = Model.Code,
            };

            CommonDocumentUtilities.SetLastChange(_context, item);
            _encryptionDb.AddCertificateType(_context, item);

            return item.Id;
        }
    }
}