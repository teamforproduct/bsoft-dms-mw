using System;
using System.Collections.Generic;

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
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// Дата создания сессии по
        /// </summary>
        public DateTime? CreateDateTo { get; set; }
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

    }
}
