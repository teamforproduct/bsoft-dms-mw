namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для получения вложения
    /// </summary>
    public class DocumentFileIdentity
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер вложения в документе
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Версия вложения
        /// </summary>
        public int Version { get; set; }
    }
}