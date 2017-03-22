using System.Collections.Generic;
using BL.Model.Enums;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Полнотекстовый поиск
    /// </summary>
    public class FullTextSearch 
    {
        /// <summary>
        /// Поисковая фраза для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }
        /// <summary>
        /// Признак, не сохранять лог полнотекстовых запросов 
        /// </summary>
        public bool IsDontSaveSearchQueryLog { get; set; }
        /// <summary>
        /// Массив ИД документов полученного из полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public List<int> FullTextSearchId { get; set; }
    }
}
