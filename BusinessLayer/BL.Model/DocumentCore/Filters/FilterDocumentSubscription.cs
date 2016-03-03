using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр ожиданий по документу
    /// </summary>
    public class FilterDocumentSubscription
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}