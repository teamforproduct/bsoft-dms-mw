using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    ///  Фильтр словаря типов важности событий
    /// </summary>
    public class FilterDictionaryImportanceEventType
    {
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
