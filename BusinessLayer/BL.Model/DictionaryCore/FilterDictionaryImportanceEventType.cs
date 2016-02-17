using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    ///  Фильтр словаря типов важности событий
    /// </summary>
    public class FilterDictionaryImportanceEventType
    {
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}
