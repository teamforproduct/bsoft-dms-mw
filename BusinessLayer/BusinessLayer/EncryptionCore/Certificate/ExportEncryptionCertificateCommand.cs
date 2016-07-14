using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class ExportEncryptionCertificateCommand : BaseEncryptionCommand
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

            _encryptionDb.ExportEncryptionCertificatePrepare(_context, Model);

            return true;
        }

        public override object Execute()
        {
            var item = _encryptionDb.ExportEncryptionCertificate(_context, Model);

            return item;
        }
    }
}