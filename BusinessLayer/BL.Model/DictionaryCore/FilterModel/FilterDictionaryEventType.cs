using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря типов событий
    /// </summary>
    public class FilterDictionaryEventType
    {
        /// <summary>
        /// Массив ИД типов событий
        /// </summary>
        public List<int> EventTypeId { get; set; }
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
    }
}
