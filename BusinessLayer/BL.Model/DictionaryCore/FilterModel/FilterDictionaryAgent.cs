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
        /// является физлицом
        /// </summary>
        public bool? IsIndividual { get; set; }
        /// <summary>
        /// ъявляется юрлицом
        /// </summary>
        public bool? IsCompany { get; set; }
        /// <summary>
        /// является сотрудником
        /// </summary>
        public bool? IsEmployee { get; set; }
        /// <summary>
        /// является банком
        /// </summary>
        public bool? IsBank { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }

    }
}
