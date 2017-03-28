using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра сессий
    /// </summary>
    public class FilterSystemSession
    {
        /// <summary>
        /// Массив ИД пользователей
        /// </summary>
        public List<int> ExecutorAgentIDs { get; set; }
        /// <summary>
        /// Дата создания сессии с
        /// </summary>
        public DateTime? CreateDateFrom { get { return _CreateDateFrom; } set { _CreateDateFrom = value.ToUTC(); } }
        private DateTime? _CreateDateFrom;
        /// <summary>
        /// Дата создания сессии по
        /// </summary>
        public DateTime? CreateDateTo { get { return _CreateDateTo; } set { _CreateDateTo = value.ToUTC(); } }
        private DateTime? _CreateDateTo;
        /// <summary>
        /// Признак показывать только активные сессии
        /// </summary>
        public bool IsOnlyActive { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string ExecutorAgentName { get; set; }
        /// <summary>
        /// Дополнительная информация лога
        /// </summary>
        public string LoginLogInfo { get; set; }
        /// <summary>
        /// Поисковая фраза для полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public string FullTextSearchString { get; set; }

    }
}
