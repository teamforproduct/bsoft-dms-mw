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
        /// Сгенерировать сертификат
        /// </summary>
        GenerateKeyEncryptionCertificate = 401005,
        #endregion EncryptionCertificates

        #region EncryptionCertificateTypes
        /// <summary>
        /// Добавить тип сертификат
        /// </summary>
        AddEncryptionCertificateType = 402001,
        /// <summary>
        /// Изменить тип сертификат
        /// </summary>
        ModifyEncryptionCertificateType = 402002,
        /// <summary>
        /// Удалить тип сертификат
        /// </summary>
        DeleteEncryptionCertificateType = 402003,
        #endregion EncryptionCertificateTypes
    }
}