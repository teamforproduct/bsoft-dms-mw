using System.Collections.Generic;
using System.Dynamic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Базовые фильтры для справочников
    /// </summary>
    public class DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Исключение записей по ID
        /// </summary>
        public List<int> NotContainsIDs { get; set; }

        /// <summary>
        /// Сужение по списку вышестоящих элементов
        /// </summary>
        public List<int> ParentIDs { get; set; }

        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Сужение по наименованию элементов
        /// </summary>
        public string Name { get; set; }
    
    }
}
