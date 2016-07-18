using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.EncryptionCore.InternalModel;

namespace BL.Logic.EncryptionCore.CertificateType
{
    public class DeleteEncryptionCertificateTypeCommand : BaseEncryptionCommand
    {

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var item = _encryptionDb.ModifyCertificateTypePrepare(_context, Model);
            if (item == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                _encryptionDb.DeleteCertificateType(_context, Model);

                return Model;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

