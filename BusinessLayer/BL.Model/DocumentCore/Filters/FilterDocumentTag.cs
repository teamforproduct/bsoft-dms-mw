using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр ожиданий по документу
    /// </summary>
    public class FilterDocumentTag
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

        /// <summary>
        /// Список ИД. Позиций
        /// </summary>
        public List<int> CurrentPositionsId { get; set; }
    }
}