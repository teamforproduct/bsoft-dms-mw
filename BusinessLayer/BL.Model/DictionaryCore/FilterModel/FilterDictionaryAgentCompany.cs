using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр юридических лиц
    /// </summary>
    public class FilterDictionaryAgentCompany
    {
        /// <summary>
        /// Массив ИД компаний
        /// </summary>
        public List<int> CompanyId { get; set; }
        /// <summary>
        /// наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }

    }
}
