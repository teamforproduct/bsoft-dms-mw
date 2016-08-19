using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр типов адресов
    /// </summary>
    public class FilterDictionaryAddressType : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Сужение по краткому наименованию
        /// </summary>
        public string Code { get; set; }
    }
}
