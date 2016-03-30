using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря агентов
    /// </summary>
    public class FilterDictionaryAgent : DictionaryBaseFilterParameters
    {
    
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
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }

    }
}
