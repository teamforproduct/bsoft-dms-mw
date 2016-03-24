using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для списка ограничений
    /// </summary>
    public class FilterDocumentRestrictedSendList
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

    }
}