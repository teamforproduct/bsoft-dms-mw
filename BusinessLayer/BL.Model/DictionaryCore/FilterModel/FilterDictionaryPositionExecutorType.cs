using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPositionExecutorType
    /// </summary>
    public class FilterDictionaryPositionExecutorType : DictionaryBaseFilterParameters
    {

        /// <summary>
        /// Сужение по коду
        /// </summary>
        public string Code { get; set; }

    }
}
