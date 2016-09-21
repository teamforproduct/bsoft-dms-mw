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

        InternalEncryptionCertificate ModifyCertificatePrepare(IContext ctx, int itemId, int? agentId);

        void ModifyCertificate(IContext ctx, InternalEncryptionCertificate item);

        void DeleteCertificate(IContext ctx, int itemId);

        #endregion

        #region CertificateSign
        string GetCertificateSign(IContext ctx, int certificateId, string certificatePassword, string dataToSign);

        bool VerifyCertificateSign(IContext ctx, string dataToSign, string sign);
        #endregion

        #region CertificateSignPdf
        byte[] GetCertificateSignPdf(IContext ctx, int certificateId, string certificatePassword, byte[] pdf);
        bool VerifyCertificateSignPdf(IContext ctx, byte[] pdf);
        #endregion

        #region InternalSign
        string GetInternalSign(string dataToSign);

        bool VerifyInternalSign(string dataToVerify, string signedData);
        #endregion
    }
}