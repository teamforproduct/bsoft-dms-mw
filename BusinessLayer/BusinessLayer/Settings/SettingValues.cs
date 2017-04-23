using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.Settings
{
    public class SettingValues : ISettingValues
    {
        private ISettings sett => DmsResolver.Current.Get<ISettings>();

        #region [+] Частные настройки ...
        public bool GetSubordinationsSendAllForExecution(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION);

        public bool GetSubordinationsSendAllForInforming(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING);


        public int GetMailTimeoutMin(IContext ctx) =>
            sett.GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_TIMEOUT_MINUTE);

        public MailServerType GetMailInfoServerType(IContext ctx) =>
             (MailServerType)sett.GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_TYPE);

        public string GetMailInfoSystemMail(IContext ctx) =>
             sett.GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_SYSTEMMAIL);

        public string GetMailInfoName(IContext ctx) =>
             sett.GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_NAME);

        public string GetMailInfoLogin(IContext ctx) =>
             sett.GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_LOGIN);

        public string GetMailInfoPassword(IContext ctx) =>
             sett.GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_PASSWORD);

        public int GetMailInfoPort(IContext ctx) =>
             sett.GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_PORT);

        public bool GetDigitalSignatureIsUseCertificateSign(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN);

        public bool GetDigitalSignatureIsUseInternalSign(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN);

        public string GetFulltextDatastorePath(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FULLTEXTSEARCH_DATASTORE_PATH);

        public int GetFulltextRefreshTimeout(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.FULLTEXTSEARCH_REFRESH_TIMEOUT);

        public bool GetFulltextWasInitialized(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED);

        public int GetFulltextRowLimit(IContext ctx) =>
            sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.FULLTEXTSEARCH_ROWLIMIT);

        public string GetFileStorePath(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.IRF_DMS_FILESTORE_PATH);

        public string GetReportDocumentForDigitalSignature(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature);

        public string GetReportRegisterTransmissionDocuments(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments);

        public string GetReportRegistrationCardIncomingDocument(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument);

        public string GetReportRegistrationCardInternalDocument(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument);

        public string GetReportRegistrationCardOutcomingDocument(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument);


        public int GetAutoplanTimeoutMinute(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE);

        public int GetClearTrashDocumentsTimeoutMinute(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE);

        public int GetClearOldPdfCopiesInDay(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.OLDPDFDELETEPERIOD);

        public int GetClearTrashDocumentsTimeoutMinuteForClear(IContext ctx) =>
             sett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR);

        #endregion
    }
}