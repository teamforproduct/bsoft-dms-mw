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

        /// <summary>
        /// Контакт агента
        /// Тип контакта не учитываем, ищем только по значению
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// Коллекция доступных типов агентов
        /// </summary>
        public IEnumerable<EnumDictionaryAgentTypes> ActualTypes { get; set; }
        
        
    }
}
