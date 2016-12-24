using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр типов адресов
    /// </summary>
    public class FilterDictionaryAddressType : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        /// <summary>
        /// Сужение по краткому наименованию
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по краткому наименованию
        /// </summary>
        public string CodeExact { get; set; }
        
    }
}
