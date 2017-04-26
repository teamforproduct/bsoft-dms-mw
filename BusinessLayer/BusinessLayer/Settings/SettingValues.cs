using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using System;

namespace BL.Logic.Settings
{
    public class SettingValues : ISettingValues
    {
        private ISettings cliSett => DmsResolver.Current.Get<ISettings>();

        private IGeneralSettings genSett => DmsResolver.Current.Get<IGeneralSettings>();

        public bool GetSubordinationsSendAllForExecution(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION);

        public bool GetSubordinationsSendAllForInforming(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING);


        public bool GetDigitalSignatureIsUseCertificateSign(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN);

        public bool GetDigitalSignatureIsUseInternalSign(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN);

        public string GetFulltextStorePath()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.FulltextStorePath);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.FulltextStorePath.ToString());
            return val;
        }

        public int GetFulltextRefreshTimeout() =>
             genSett.GetSetting<int>(EnumGeneralSettings.FulltextRefreshTimeout);

        public bool GetFulltextWasInitialized(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED);

        public int GetFulltextRowLimit() =>
            genSett.GetSetting<int>(EnumGeneralSettings.FulltextRowLimit);


        public string GetFileStorePath()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.FileStorePath);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.FileStorePath.ToString());
            return val;
        }
        public string GetReportDocumentForDigitalSignature(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature);

        public string GetReportRegisterTransmissionDocuments(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments);

        public string GetReportRegistrationCardIncomingDocument(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument);

        public string GetReportRegistrationCardInternalDocument(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument);

        public string GetReportRegistrationCardOutcomingDocument(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument);


        public int GetAutoplanTimeoutMinute(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE);

        public int GetClearTrashDocumentsTimeoutMinute(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE);

        public int GetClearOldPdfCopiesInDay(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.OLDPDFDELETEPERIOD);

        public int GetClearTrashDocumentsTimeoutMinuteForClear(IContext ctx) =>
             cliSett.GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR);

        public string GetCurrentServerName() =>
            genSett.GetSetting<string>(EnumGeneralSettings.ServerForNewClient);

        #region MailDocum

        public int GetMailDocumSenderTimeoutMin() =>
            genSett.GetSetting<int>(EnumGeneralSettings.MailDocumSenderTimeoutMin);

        public MailServerType GetMailDocumServerType()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailDocumServerType);

            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumServerType.ToString());

            try { return (MailServerType)Enum.Parse(typeof(MailServerType), val); }
            catch { throw new SettingValueIsInvalid(EnumGeneralSettings.MailDocumServerType.ToString()); }
        }

        public string GetMailDocumServerName()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailDocumServerName);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumServerName.ToString());
            return val;
        }

        public int GetMailDocumServerPort()
        {
            var val = genSett.GetSetting<int>(EnumGeneralSettings.MailDocumServerPort);
            if (val == 0) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumServerPort.ToString());
            return val;
        }


        public string GetMailDocumEmail()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailDocumEmail);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumEmail.ToString());
            return val;
        }

        public string GetMailDocumLogin()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailDocumLogin);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumLogin.ToString());
            return val;
        }

        public string GetMailDocumPassword()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailDocumPassword);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailDocumPassword.ToString());
            return val;
        }

        #endregion

        #region MailNoreply

        public int GetMailNoreplySenderTimeoutMin() =>
            genSett.GetSetting<int>(EnumGeneralSettings.MailNoreplySenderTimeoutMin);

        public MailServerType GetMailNoreplyServerType()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailNoreplyServerType);

            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyServerType.ToString());

            try { return (MailServerType)Enum.Parse(typeof(MailServerType), val); }
            catch { throw new SettingValueIsInvalid(EnumGeneralSettings.MailNoreplyServerType.ToString()); }
        }

        public string GetMailNoreplyServerName()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailNoreplyServerName);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyServerName.ToString());
            return val;
        }

        public int GetMailNoreplyServerPort()
        {
            var val = genSett.GetSetting<int>(EnumGeneralSettings.MailNoreplyServerPort);
            if (val == 0) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyServerPort.ToString());
            return val;
        }


        public string GetMailNoreplyEmail()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailNoreplyEmail);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyEmail.ToString());
            return val;
        }

        public string GetMailNoreplyLogin()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailNoreplyLogin);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyLogin.ToString());
            return val;
        }

        public string GetMailNoreplyPassword()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailNoreplyPassword);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailNoreplyPassword.ToString());
            return val;
        }

        #endregion

        #region MailSMS

        public int GetMailSMSSenderTimeoutMin() =>
            genSett.GetSetting<int>(EnumGeneralSettings.MailSMSSenderTimeoutMin);

        public MailServerType GetMailSMSServerType()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailSMSServerType);

            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSServerType.ToString());

            try { return (MailServerType)Enum.Parse(typeof(MailServerType), val); }
            catch { throw new SettingValueIsInvalid(EnumGeneralSettings.MailSMSServerType.ToString()); }
        }

        public string GetMailSMSServerName()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailSMSServerName);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSServerName.ToString());
            return val;
        }

        public int GetMailSMSServerPort()
        {
            var val = genSett.GetSetting<int>(EnumGeneralSettings.MailSMSServerPort);
            if (val == 0) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSServerPort.ToString());
            return val;
        }


        public string GetMailSMSEmail()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailSMSEmail);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSEmail.ToString());
            return val;
        }

        public string GetMailSMSLogin()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailSMSLogin);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSLogin.ToString());
            return val;
        }

        public string GetMailSMSPassword()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MailSMSPassword);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MailSMSPassword.ToString());
            return val;
        }

        #endregion


        public string GetMainHost()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MainHost);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MainHost.ToString());
            return val;
        }

        public string GetMainHostProtocol()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.MainHostProtocol);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.MainHostProtocol.ToString());
            return val;
        }

        public string GetVirtualHost()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.VirtualHost);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.VirtualHost.ToString());
            return val;
        }

        public string GetAuthDomain()
        {
            var val = genSett.GetSetting<string>(EnumGeneralSettings.AuthDomain);
            if (string.IsNullOrEmpty(val)) throw new SettingValueIsNotSet(EnumGeneralSettings.AuthDomain.ToString());
            return val;
        }

        public string GetAuthAddress() => GetMainHostProtocol() + "://" + GetAuthDomain() + "." + GetMainHost();
    }
}