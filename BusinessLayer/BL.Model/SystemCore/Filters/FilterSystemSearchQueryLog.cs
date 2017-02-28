using System;
using BL.Model.Enums;
using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра для истории поисковых запросов 
    /// </summary>
    public class FilterSystemSearchQueryLog : BaseFilter
    {
        /// <summary>
        /// Массив модулей
        /// </summary>
        public List<string> Module { get; set; }
        /// <summary>
        /// Массив фич
        /// </summary>
        public List<string> Feature { get; set; }
        /// <summary>
        /// Текст запроса для поиска, должны присутствовать все части, разделенные пробелом 
        /// </summary>
        public string AllSearchQueryTextParts { get; set; }
        /// <summary>
        /// Текст запроса для поиска, должна присутствовать хотя бы одна часть из частей, разделенных пробелом 
        /// </summary>
        public string OneSearchQueryTextParts { get; set; }
        /// <summary>
        /// Текст запроса для поиска, точное совпадение
        /// </summary>
        public string SearchQueryTextExact { get; set; }
    }
}