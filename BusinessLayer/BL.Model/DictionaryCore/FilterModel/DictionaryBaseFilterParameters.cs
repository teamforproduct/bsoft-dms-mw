using BL.Model.Common;
using System.Collections.Generic;
using System.Dynamic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Базовые фильтры для справочников
    /// </summary>
    public class DictionaryBaseFilterParameters : BaseFilter
    {
        

        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Сужение по наименованию (вхождение)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию (равенство)
        /// </summary>
        public string NameExact { get; set; }
    }
}
