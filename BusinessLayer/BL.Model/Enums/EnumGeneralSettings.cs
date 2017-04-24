namespace BL.Model.Enums
{
    /// <summary>
    /// Общие (не клиентозависимые настройки)
    /// </summary>
    public enum EnumGeneralSettings
    {
        ServerForNewClient,

        MailSenderTimeoutMin,

        // При добавлении новых настроек не забудь добавить значения по умолчанию в SettingsFactory
    }
}