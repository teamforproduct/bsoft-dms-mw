using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.EncryptionCore.IncomingModel;
using BL.Model.EncryptionCore.InternalModel;
using System.IO;

namespace BL.Logic.EncryptionCore.Certificate
{
    public class AddEncryptionCertificateCommand : BaseEncryptionCommand
    {
        private AddEncryptionCertificate Model
        {
            get
            {
                if (!(_param is AddEncryptionCertificate))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddEncryptionCertificate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            if (!Path.GetExtension(Model.PostedFileData.FileName).Replace(".", "").Equals("pfx", StringComparison.OrdinalIgnoreCase))
                throw new WrongParameterTypeError();

            return true;
        }

        public override object Execute()
        {

            var item = new InternalEncryptionCertificate
            {
                Name = Model.Name,
                CreateDate = DateTime.Now,
                NotBefore = Model.NotBefore,
                NotAfter = Model.NotAfter,
                AgentId = _context.CurrentAgentId,
                Password = Model.Password,
            };

            using (var memoryStream = new MemoryStream())
            {
                Model.PostedFileData.InputStream.CopyTo(memoryStream);
                item.Certificate = memoryStream.ToArray();
            }

            CommonDocumentUtilities.SetLastChange(_context, item);
            _encryptionDb.AddCertificate(_context, item);

            return item.Id;
        }
    }
}