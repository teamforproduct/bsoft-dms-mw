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
        /// Экспорт сертификата
        /// </summary>
        ExportEncryptionCertificate = 401003,
        /// <summary>
        /// Удалить сертификат
        /// </summary>
        DeleteEncryptionCertificate = 401004,
        /// <summary>
        /// Сгенерировать сертификат
        /// </summary>
        GenerateKeyEncryptionCertificate = 401005,
        #endregion EncryptionCertificates
    }
}