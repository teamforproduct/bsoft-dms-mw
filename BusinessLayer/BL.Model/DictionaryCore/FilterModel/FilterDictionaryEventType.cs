using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря типов событий
    /// </summary>
    public class FilterDictionaryEventType : DictionaryBaseFilterParameters
    {
   
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> ImportanceEventTypeIDs { get; set; }
        
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentIDs { get; set; }
    
    }
}
