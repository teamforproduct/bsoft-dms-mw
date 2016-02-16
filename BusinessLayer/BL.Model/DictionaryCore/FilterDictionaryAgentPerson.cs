using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    /// Фильтр словаря контактов посторонних организаций
    /// </summary>
    public class FilterDictionaryAgentPerson
    {
        /// <summary>
        /// Массив ИД контактов
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД посторонних организаций
        /// </summary>
        public List<int> AgentId { get; set; }
        /// <summary>
        /// Отрывок наименования контакта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отрывок наименования посторонней организации
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// Отрывок наименования агента, связанного с контактом
        /// </summary>
        public string PersonAgentName { get; set; }
    }
}
