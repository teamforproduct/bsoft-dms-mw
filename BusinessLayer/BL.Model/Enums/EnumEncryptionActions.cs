namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по сертификатам
    /// </summary>
    public enum EnumEncryptionActions
    {
        #region EncryptionCertificates
        /// <summary>
        /// Добавить сертификат
        /// </summary>
        AddEncryptionCertificate = 401001,
        /// Изменить сертификат
        /// </summary>
        ModifyEncryptionCertificate = 401002,
        /// <summary>
        /// Удалить сертификат
        /// </summary>
        DeleteEncryptionCertificate = 401004,
        /// <summary>
        /// Экспорт сертификата
        /// </summary>
        ExportEncryptionCertificate = 401003,
        #endregion EncryptionCertificates
    }
}