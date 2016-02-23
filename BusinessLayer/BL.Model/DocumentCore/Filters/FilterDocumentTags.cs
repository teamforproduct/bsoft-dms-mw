using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр ожиданий по документу
    /// </summary>
    public class FilterDocumentTags
    {
        /// <summary>
        /// ИД. Документа
        /// </summary>
        public int? DocumentId { get; set; }

        /// <summary>
        /// Список ИД. Позиций
        /// </summary>
        public List<int> CurrentPositionsId { get; set; }
    }
}