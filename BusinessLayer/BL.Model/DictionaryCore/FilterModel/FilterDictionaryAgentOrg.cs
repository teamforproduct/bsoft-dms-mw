using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры
    /// </summary>
    public class FilterDictionaryAgentOrg : BaseFilterNameIsActive
    {
        /// <summary>
        /// Полное наименование
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Сужение компаний по отделам
        /// </summary>
        public List<int> DepartmentIDs { get; set; }
    }
}
