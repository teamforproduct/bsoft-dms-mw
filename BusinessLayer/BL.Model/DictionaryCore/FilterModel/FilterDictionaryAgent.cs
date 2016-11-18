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
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }

    }
}
