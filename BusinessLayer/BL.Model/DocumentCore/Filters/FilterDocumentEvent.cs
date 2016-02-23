using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр событий документов
    /// </summary>
    public class FilterDocumentEvent
    {
        /// <summary>
        /// Массив ИД событий документов
        /// </summary>
        public List<int> EventId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}
