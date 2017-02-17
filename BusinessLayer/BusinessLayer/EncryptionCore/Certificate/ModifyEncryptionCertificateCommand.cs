using BL.Logic.Common;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class ModifyEncryptionCertificateCommand : BaseEncryptionCommand
    {
        private ModifyEncryptionCertificate Model { get { return GetModel<ModifyEncryptionCertificate>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var item = _encryptionDb.ModifyCertificatePrepare(_context, Model.Id, Model.AgentId);
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