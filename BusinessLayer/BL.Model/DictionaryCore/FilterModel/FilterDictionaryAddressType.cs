using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр типов адресов
    /// </summary>
    public class FilterDictionaryAddressType
    {
        /// <summary>
        /// Список ИД
        /// </summary>
        public List<int> AddressTypeId { get; set; }
        /// <summary>
        /// Фрагмент наименования
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
    }
}
