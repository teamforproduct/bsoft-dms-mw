namespace BL.Model.Enums
{
    /// <summary>
    /// Общие (не клиентозависимые настройки)
    /// </summary>
    public enum EnumGeneralSettings
    {
        ServerForNewClient,
        MainHost,
        VirtualHost,
        AuthDomain,


        #region MailDocum
        MailDocumSenderTimeoutMin,

        MailDocumServerType,
        MailDocumServerName,
        MailDocumServerPort,

        MailDocumLogin,
        MailDocumEmail,

        MailDocumPassword,

        #endregion

        #region MailNoreply
        MailNoreplySenderTimeoutMin,

        MailNoreplyServerType,
        MailNoreplyServerName,
        MailNoreplyServerPort,

        MailNoreplyLogin,
        MailNoreplyEmail,

        MailNoreplyPassword,

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

        FulltextRefreshTimeout,
        FulltextRowLimit,

    }
}