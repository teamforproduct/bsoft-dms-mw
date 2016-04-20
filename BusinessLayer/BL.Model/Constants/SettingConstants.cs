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

        public static string FILE_STORE_PATH = "IRF_DMS_FILESTORE_PATH";
        public static string FILE_STORE_DEFAULT_PATH = @"c:\IRF_DMS_FILESTORE";
        public static string FILE_STORE_DOCUMENT_FOLDER = @"DOCUMENT";
        public static string FILE_STORE_TEMPLATE_FOLDER = @"TEMPLATE";

        public static string FILE_STORE_TEMPLATE_REPORTS_FOLDER = @"TEMPLATE_REPORTS";
        public static string FILE_STORE_TEMPLATE_REPORT_FILE = "FILE_STORE_TEMPLATE_REPORT_FILE_";
        public static string FILE_STORE_DEFAULT_TEMPLATE_REPORT_FILE = @"Report.rpt";

        public static string AUTOPLAN_TIOMEOUT_MIN = @"RUN_AUTOPLAN_TIMEOUT_MINUTE";
    }
}