namespace BL.Model.Enums
{
    /// <summary>
    /// Типы документов
    /// </summary>
    // На предприятиях принято делить весь поток документов на три потока: входящие документы, исходящие и внутренние.
    // Если внутренний документ нужно отправить внешнему агенту, нужно создать исходящий на основании внутреннего.
    public enum EnumDocumentTypes
    {

        /// <summary>
        /// Письмо
        /// </summary>
        Letter,

        /// <summary>
        /// Приказ
        /// </summary>
        Order,

        
        /// <summary>
        /// Распоряжение
        /// </summary>
        Decree,

        /// <summary>
        /// Служебная записка
        /// </summary>
        Memo,

        /// <summary>
        /// Поручение
        /// </summary>
        Commission,

        /// <summary>
        /// Протокол
        /// </summary>
        Protocol,

        /// <summary>
        /// Договор
        /// </summary>
        Agreement,

    }
}