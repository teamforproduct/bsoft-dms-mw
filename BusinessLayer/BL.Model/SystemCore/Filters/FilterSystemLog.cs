using System;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра лога
    /// 
    /// </summary>
    public class FilterSystemLog
    {
        /// <summary>
        /// Массив ИД пользователей
        /// </summary>
        public List<int> ExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> ObjectId { get; set; }
        /// <summary>
        /// Массив ИД экшинов системы
        /// </summary>
        public List<int> ActionId { get; set; }
        /// <summary>
        /// Массив ИД записей
        /// </summary>
        public List<int> RecordId { get; set; }
        /// <summary>
        /// Массив уровней лога
        /// </summary>
        public List<int> LogLevel { get; set; }
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
