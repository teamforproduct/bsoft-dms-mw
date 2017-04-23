using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettingValues
    {
        bool GetSubordinationsSendAllForExecution(IContext ctx);
        bool GetSubordinationsSendAllForInforming(IContext ctx);
        int GetMailTimeoutMin(IContext ctx);
        MailServerType GetMailInfoServerType(IContext ctx);


        string GetMailInfoSystemMail(IContext ctx);

        string GetMailInfoName(IContext ctx);

        string GetMailInfoLogin(IContext ctx);

        string GetMailInfoPassword(IContext ctx);

        int GetMailInfoPort(IContext ctx);

        bool GetDigitalSignatureIsUseCertificateSign(IContext ctx);

        bool GetDigitalSignatureIsUseInternalSign(IContext ctx);

        string GetFulltextDatastorePath(IContext ctx);

        int GetFulltextRefreshTimeout(IContext ctx);
        int GetFulltextRowLimit(IContext ctx);

        bool GetFulltextWasInitialized(IContext ctx);

        string GetFileStorePath(IContext ctx);

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