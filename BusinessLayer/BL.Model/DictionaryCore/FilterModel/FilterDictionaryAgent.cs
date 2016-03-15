using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря агентов
    /// </summary>
    public class FilterDictionaryAgent
    {
        /// <summary>
        /// Массив ИД агентов
        /// </summary>
        public List<int> AgentId { get; set; }
        /// <summary>
        /// Отрывок наименования агента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак активности агента
        /// </summary>
        public bool? IsActive { get; set; }

        public bool? IsIndividual { get; set; }
        public bool? IsCompany { get; set; }
        public bool? IsEmployee { get; set; }
        public bool? IsBank { get; set; }
        
        
    }
}
