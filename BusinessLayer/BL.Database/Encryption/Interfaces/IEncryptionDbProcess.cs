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
        #region Certificates
        IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging);

        void AddCertificate(IContext ctx, InternalEncryptionCertificate item);

        InternalEncryptionCertificate ModifyCertificatePrepare(IContext ctx, int itemId);

        void ModifyCertificate(IContext ctx, InternalEncryptionCertificate item);

        void DeleteCertificate(IContext ctx, int itemId);

        InternalEncryptionCertificate ExportEncryptionCertificatePrepare(IContext ctx, int itemId);

        FrontEncryptionCertificate ExportEncryptionCertificate(IContext ctx, int itemId);

        #endregion

        #region CertificateTypes

        IEnumerable<FrontEncryptionCertificateType> GetCertificateTypes(IContext ctx, FilterEncryptionCertificateType filter, UIPaging paging);

        void AddCertificateType(IContext ctx, InternalEncryptionCertificateType item);

        InternalEncryptionCertificateType ModifyCertificateTypePrepare(IContext ctx, int itemId);

        void ModifyCertificateType(IContext ctx, InternalEncryptionCertificateType item);

        void DeleteCertificateType(IContext ctx, int itemId);

        #endregion
    }
}