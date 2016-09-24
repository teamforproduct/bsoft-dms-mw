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

        /// <summary>
        /// Сужение по краткому наименованию
        /// </summary>
        public string CodeExact { get; set; }
        
        /// <summary>
        /// по наименованию (равенство)
        /// </summary>
        public string NameExact { get; set; }

    }
}
