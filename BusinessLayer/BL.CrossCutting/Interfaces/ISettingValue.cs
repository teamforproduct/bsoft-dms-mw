using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettingValues
    {
        string GetCurrentServerName();

        string GetMainHost();
        string GetVirtualHost();

        string GetAuthAddress();

        bool GetSubordinationsSendAllForExecution(IContext ctx);
        bool GetSubordinationsSendAllForInforming(IContext ctx);

        #region MailDocum
        int GetMailDocumSenderTimeoutMin();
        MailServerType GetMailDocumServerType();
        string GetMailDocumServerName();
        int GetMailDocumServerPort();

        string GetMailDocumEmail();
        string GetMailDocumLogin();
        string GetMailDocumPassword();
        #endregion

        #region MailNoreply
        int GetMailNoreplySenderTimeoutMin();
        MailServerType GetMailNoreplyServerType();
        string GetMailNoreplyServerName();
        int GetMailNoreplyServerPort();

        string GetMailNoreplyEmail();
        string GetMailNoreplyLogin();
        string GetMailNoreplyPassword();
        #endregion

        #region MailSMS
        int GetMailSMSSenderTimeoutMin();
        MailServerType GetMailSMSServerType();
        string GetMailSMSServerName();
        int GetMailSMSServerPort();

        string GetMailSMSEmail();
        string GetMailSMSLogin();
        string GetMailSMSPassword();
        #endregion

        bool GetDigitalSignatureIsUseCertificateSign(IContext ctx);

        bool GetDigitalSignatureIsUseInternalSign(IContext ctx);

        #region [+] Fulltext ...
        string GetFulltextStorePath();

        int GetFulltextRefreshTimeout();
        int GetFulltextRowLimit();

        bool GetFulltextWasInitialized(IContext ctx);

        #endregion

        #region [+] FileStore ...
        string GetFileStorePath();
        #endregion

        string GetReportDocumentForDigitalSignature(IContext ctx);

        string GetReportRegisterTransmissionDocuments(IContext ctx);

        string GetReportRegistrationCardIncomingDocument(IContext ctx);

        string GetReportRegistrationCardInternalDocument(IContext ctx);

        string GetReportRegistrationCardOutcomingDocument(IContext ctx);

        int GetClearOldPdfCopiesInDay(IContext ctx);

        int GetAutoplanTimeoutMinute(IContext ctx);

        int GetClearTrashDocumentsTimeoutMinute(IContext ctx);

        int GetClearTrashDocumentsTimeoutMinuteForClear(IContext ctx);

        

    }
}