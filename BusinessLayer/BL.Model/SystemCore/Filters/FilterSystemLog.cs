using System;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра лога
    /// </summary>
    public class FilterSystemLog
    {
        /// <summary>
        /// Массив ИД лога
        /// </summary>
        public List<int> IDs { get; set; }
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
        public DateTime? LogDateFrom { get; set; }
        /// <summary>
        /// Дата записи лог по
        /// </summary>
        public DateTime? LogDateTo { get; set; }
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
    }
}
