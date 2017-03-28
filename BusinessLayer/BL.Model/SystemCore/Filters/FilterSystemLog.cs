using BL.Model.Common;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра лога
    /// </summary>
    public class FilterSystemLog: BaseFilter
    {
        /// <summary>
        /// Массив ИД пользователей
        /// </summary>
        public List<int> ExecutorAgentIDs { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string ExecutorAgentName { get; set; }
        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> ObjectIDs { get; set; }
        /// <summary>
        /// Массив ИД экшинов системы
        /// </summary>
        public List<int> ActionIDs { get; set; }
        /// <summary>
        /// Массив ИД записей
        /// </summary>
        public List<int> RecordIDs { get; set; }
        /// <summary>
        /// Массив уровней лога
        /// </summary>
        public List<int> LogLevels { get; set; }
        /// <summary>
        /// Дата записи лог с
        /// </summary>
        public DateTime? LogDateFrom { get { return _LogDateFrom; } set { _LogDateFrom = value.ToUTC(); } }
        private DateTime? _LogDateFrom;
        /// <summary>
        /// Дата записи лог по
        /// </summary>
        public DateTime? LogDateTo { get { return _LogDateTo; } set { _LogDateTo = value.ToUTC(); } }
        private DateTime? _LogDateTo;
        /// <summary>
        /// Отрывок сообщения
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Отрывок трассы
        /// </summary>
        public string LogTrace { get; set; }
        /// <summary>
        /// Отрывок описания исключения
        /// </summary>
        public string LogException { get; set; }
        /// <summary>
        /// Поисковая фраза для полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public string FullTextSearchString { get; set; }
    }
}
