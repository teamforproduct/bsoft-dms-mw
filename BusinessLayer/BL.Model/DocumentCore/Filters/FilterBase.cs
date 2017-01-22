namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Базовый фильтр
    /// </summary>
    public class FilterBase
    {
        /// <summary>
        /// Фильтра по документам
        /// </summary>
        public FilterDocument Document { get; set; }
        /// <summary>
        /// Фильтра по событиям
        /// </summary>
        public FilterDocumentEvent Event { get; set; }
        /// <summary>
        /// Фильтра по ожиданиям
        /// </summary>
        public FilterDocumentWait Wait { get; set; }
        /// <summary>
        /// Фильтра по файлам
        /// </summary>
        public FilterDocumentFile File { get; set; }
    }
}
