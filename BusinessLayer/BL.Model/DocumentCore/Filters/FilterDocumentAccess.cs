using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр доступов документа
    /// </summary>
    public class FilterDocumentAccess
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

        /// <summary>
        /// Массив ИД уровней доступа по документу
        /// </summary>
        public List<int> AccessLevelId { get; set; }

        /// <summary>
        /// Признак в работе
        /// </summary>
        public bool? IsInWork { get; set; } // should be true by default
    }
}
