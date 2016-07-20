using BL.CrossCutting.Interfaces;
using BL.Model.EncryptionCore.InternalModel;
using BL.Model.EncryptionCore.IncomingModel;

namespace BL.Database.EncryptionWorker
{
    public interface IEncryptionGeneratorKey
    {
        InternalEncryptionCertificate GenerateKey(IContext ctx, GenerateKeyEncryptionCertificate model);
        byte[] GetPublicKey(IContext ctx, InternalEncryptionCertificate model);
    }
}
