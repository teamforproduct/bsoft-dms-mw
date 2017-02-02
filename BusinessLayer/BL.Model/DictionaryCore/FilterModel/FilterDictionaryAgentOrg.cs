using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры
    /// </summary>
    public class FilterDictionaryAgentOrg : DictionaryBaseFilterParameters
    {
        public string FullName { get; set; }

        /// <summary>
        /// Сужение компаний по отделам
        /// </summary>
        public List<int> DepartmentIDs { get; set; }
    }
}
