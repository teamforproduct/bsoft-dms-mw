using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.Filters;
using System.Collections.Generic;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class DeleteEncryptionCertificateCommand : BaseEncryptionCommand
    {

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var item = _encryptionDb.ModifyCertificatePrepare(_context, Model, null);
            if (item == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            return true;
        }

        public override object Execute()
        {
            _encryptionDb.DeleteCertificate(_context, new FilterEncryptionCertificate { CertificateId = new List<int> { Model } } );

            return Model;
        }
    }

}

