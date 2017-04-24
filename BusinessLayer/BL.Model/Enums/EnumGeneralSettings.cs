namespace BL.Model.Enums
{
    /// <summary>
    /// Общие (не клиентозависимые настройки)
    /// </summary>
    public enum EnumGeneralSettings
    {
        ServerForNewClient,

        #region MailDocum
        MailDocumSenderTimeoutMin,

        MailDocumServerType,
        MailDocumServerName,
        MailDocumServerPort,

        MailDocumLogin,
        MailDocumEmail,

        MailDocumPassword,

        #endregion

        #region MailNoreplay
        MailNoreplaySenderTimeoutMin,

        MailNoreplayServerType,
        MailNoreplayServerName,
        MailNoreplayServerPort,

        MailNoreplayLogin,
        MailNoreplayEmail,

        MailNoreplayPassword,

        #endregion

        #region MailSMS
        MailSMSSenderTimeoutMin,

        MailSMSServerType,
        MailSMSServerName,
        MailSMSServerPort,

        MailSMSLogin,
        MailSMSEmail,

        MailSMSPassword,

        #endregion

        FileStorePath,

        FulltextStorePath,

    }
}