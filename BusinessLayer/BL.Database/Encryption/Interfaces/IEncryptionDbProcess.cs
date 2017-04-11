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

        void DeleteCertificate(IContext ctx, FilterEncryptionCertificate filter);

        #endregion

        #region CertificateSign
        string GetCertificateSign(IContext ctx, int certificateId, string certificatePassword, string dataToSign, string serverMapPath);

        bool VerifyCertificateSign(IContext ctx, string dataToSign, string sign, string serverMapPath);
        #endregion

        #region CertificateSignPdf
        byte[] GetCertificateSignPdf(IContext ctx, int certificateId, string certificatePassword, byte[] pdf, string serverMapPath); //TODO NOT USED
        bool VerifyCertificateSignPdf(byte[] pdf, string serverMapPath);
        #endregion

        #region InternalSign
        string GetInternalSign(string dataToSign);

        bool VerifyInternalSign(string dataToVerify, string signedData);
        #endregion
    }
}