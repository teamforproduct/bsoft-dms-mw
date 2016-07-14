using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.Filters;
using BL.Model.EncryptionCore.InternalModel;

namespace BL.Database.Encryption.Interfaces
{
    public interface IEncryptionDbProcess
    {
        IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging);

        void AddCertificate(IContext ctx, InternalEncryptionCertificate item);

        InternalEncryptionCertificate ModifyCertificatePrepare(IContext context, int itemId);

        void ModifyCertificate(IContext ctx, InternalEncryptionCertificate item);

        void DeleteCertificate(IContext ctx, int itemId);

        InternalEncryptionCertificate ExportEncryptionCertificatePrepare(IContext ctx, int itemId);
        FrontEncryptionCertificate ExportEncryptionCertificate(IContext ctx, int itemId);
    }
}