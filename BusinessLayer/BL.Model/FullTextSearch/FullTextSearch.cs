using System.Collections.Generic;
using BL.Model.Enums;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BL.Model.Extensions;

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
        public string FullTextSearchString { get { return _FullTextSearchString; } set { _FullTextSearchString = value.RemoveSpecialCharactersFullText(); } }
        private string _FullTextSearchString;
        /// <summary>
        /// Признак, не сохранять лог полнотекстовых запросов 
        /// </summary>
        public bool IsDontSaveSearchQueryLog { get; set; }
        /// <summary>
        /// Массив ИД документов полученного из полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public List<int> FullTextSearchId { get; set; }
        /// <summary>
        /// Массив документов полученного из полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public List<FullTextSearchResult> FullTextSearchResult { get; set; }
        /// <summary>
        /// Признак того, что полнотекст вернул не все результаты запроса из-за ограничений
        /// </summary>
        [IgnoreDataMember]
        public bool IsNotAll { get; set; }        
    }
}
