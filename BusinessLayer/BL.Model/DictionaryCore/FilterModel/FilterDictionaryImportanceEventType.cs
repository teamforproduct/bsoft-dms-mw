using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    ///  Фильтр словаря типов важности событий
    /// </summary>
    public class FilterDictionaryImportanceEventType : BaseFilterNameIsActive
    {
    
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentIDs { get; set; }
        
    }
}
