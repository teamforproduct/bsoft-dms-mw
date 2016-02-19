namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Модель для получения вложения
    /// </summary>
    public class FilterDocumentFileIdentity
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