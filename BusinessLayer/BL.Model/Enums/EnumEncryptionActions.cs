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
        #endregion EncryptionCertificates

        /// <summary>
        /// VerifyPdf
        /// </summary>
        VerifyPdf = 401006
       
        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!

    }
}