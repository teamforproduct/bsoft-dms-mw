using BL.Model.SystemCore.InternalModel;

namespace BL.Model.Constants
{
    public static class SettingConstants
    {
        public static string MAIL_SERVER_TYPE = "MAILSERVER_TYPE";
        public static string MAIL_SERVER_NAME = "MAILSERVER_NAME";
        public static string MAIL_SERVER_PORT = "MAILSERVER_PORT";
        public static string MAIL_SERVER_LOGIN = "MAILSERVER_LOGIN";
        public static string MAIL_SERVER_PASS = "MAILSERVER_PASSWORD";
        public static string MAIL_SERVER_SYSTEMMAIL = "MAILSERVER_SYSTEMMAIL";
        public static string MAIL_TIMEOUT_MIN = "MAILSERVER_TIMEOUT_MINUTE";

        public static string FULLTEXT_INDEX_PATH = "FULLTEXTSEARCH_DATASTORE_PATH";
        public static string FULLTEXT_TIMEOUT_MIN = "FULLTEXTSEARCH_REFRESH_TIMEOUT";
        public static string FULLTEXT_WAS_INITIALIZED = "FULLTEXTSEARCH_WAS_INITIALIZED";
        public static InternalSystemSetting DefaultFulltextWasInitialized()
        {
            return new InternalSystemSetting
            {
                Key = FULLTEXT_WAS_INITIALIZED,
                Value = "true",
                ValueType = Enums.EnumValueTypes.Bool,
            };
        }

        public static string FILE_STORE_PATH = "IRF_DMS_FILESTORE_PATH";
        public static InternalSystemSetting DefaultFileStorePath()
        {
            return new InternalSystemSetting
            {
                Key = FILE_STORE_PATH,
                Value = @"d:\IRF_DMS_FILESTORE",
                ValueType = Enums.EnumValueTypes.Text,
            };
        }
        public static string FILE_STORE_PATH_TMP = "Temp";
        public static string FILE_STORE_DOCUMENT_FOLDER = @"DOCUMENT";
        public static string FILE_STORE_TEMPLATE_FOLDER = @"TEMPLATE";

        public static string FILE_STORE_TEMPLATE_REPORTS_FOLDER = @"TEMPLATE_REPORTS";

        public static string FILE_STORE_TEMPLATE_REPORT_FILE = "FILE_STORE_TEMPLATE_REPORT_FILE_";
        public static InternalSystemSetting DefaultFileStoreTemplateReportFile(string KeySuffix)
        {
            return new InternalSystemSetting
            {
                Key = FILE_STORE_TEMPLATE_REPORT_FILE + KeySuffix,
                Value = @"Report.rpt",
                ValueType = Enums.EnumValueTypes.Text,
            };
        }

        public static string AUTOPLAN_TIOMEOUT_MIN = @"RUN_AUTOPLAN_TIMEOUT_MINUTE";

        public static string CLEARTRASHDOCUMENTS_TIOMEOUT_MIN = @"RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE";
        public static string CLEARTRASHDOCUMENTS_TIOMEOUT_MIN_FOR_CLEAR = @"CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR";

        public static string CRYPTO_LOCAL_SIGNDATA_KEY_NAME = @"DMS_CRYPTO_LOCAL_SIGNDATA_KEY_NAME";
        public static string CRYPTO_OBTAINED_PUBLIC_KEY_NAME_FOR_VERIFY_SIGNED_HASH = @"DMS_CRYPTO_OBTAINED_PUBLIC_KEY_NAME_FOR_VERIFY_SIGNED_HASH";

        public static int LICENCE_TRIAL_DAY_LIMIT = 30;
        public static int LICENCE_TRIAL_NUMBER_OF_CONNECTIONS = 2;

        public static string DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN = "DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN";
        public static InternalSystemSetting DefaultDigitalSignatureIsUseInternalSign()
        {
            return new InternalSystemSetting
            {
                Key = DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN,
                Value = "false",
                ValueType = Enums.EnumValueTypes.Bool,
            };
        }

        public static string DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN = "DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN";
        public static InternalSystemSetting DefaultDigitalSignatureIsUseCertificateSign()
        {
            return new InternalSystemSetting
            {
                Key = DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN,
                Value = "false",
                ValueType = Enums.EnumValueTypes.Bool,
            };
        }

        public static string SUBORDINATIONS_SEND_ALL_FOR_EXECUTION = "SUBORDINATIONS_SEND_ALL_FOR_EXECUTION";
        public static InternalSystemSetting DefaultSubordinationsSendAllForExecution()
        {
            return new InternalSystemSetting
            {
                Key = SUBORDINATIONS_SEND_ALL_FOR_EXECUTION,
                Value = "true",
                ValueType = Enums.EnumValueTypes.Bool,
            };
        }


        public static string SUBORDINATIONS_SEND_ALL_FOR_INFORMING = "SUBORDINATIONS_SEND_ALL_FOR_INFORMING";
        public static InternalSystemSetting DefaultSubordinationsSendAllForInforming()
        {
            return new InternalSystemSetting
            {
                Key = SUBORDINATIONS_SEND_ALL_FOR_INFORMING,
                Value = "true",
                ValueType = Enums.EnumValueTypes.Bool,
            };
        }

    }
}
