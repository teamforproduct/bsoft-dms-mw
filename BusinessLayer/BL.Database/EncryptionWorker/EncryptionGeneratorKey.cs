using BL.CrossCutting.Interfaces;
using BL.Model.EncryptionCore.InternalModel;
using BL.Model.EncryptionCore.IncomingModel;
using System.Security.Cryptography;
using System;
using BL.Model.Enums;

namespace BL.Database.EncryptionWorker
{
    public class EncryptionGeneratorKey : IEncryptionGeneratorKey
    {
        public InternalEncryptionCertificate GenerateKey(IContext ctx, GenerateKeyEncryptionCertificate model)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    var res = new InternalEncryptionCertificate {
                        AgentId = ctx.CurrentAgentId,
                        Certificate = rsa.ExportCspBlob(true),
                        CreateDate = DateTime.Now,
                        Extension = "cer",
                        IsPrivate = true,
                        IsPublic = true,
                        Name = model.Name,
                        ValidFromDate = null,
                        ValidToDate = null,     
                        Type = EnumEncryptionCertificateTypes.RSA,               
                    };
                    return res;
                }
            }
            catch (CryptographicException e)
            {
                throw e;
            }
        }
        public byte[] GetPublicKey(IContext ctx, InternalEncryptionCertificate model)
        {
            if (model.Type == EnumEncryptionCertificateTypes.RSA)
            {
                try
                {
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                    {
                        rsa.ImportCspBlob(model.Certificate);

                        return rsa.ExportCspBlob(false);
                    }
                }
                catch (CryptographicException e)
                {
                    throw e;
                }
            }
            return null;
        }
    }
}
