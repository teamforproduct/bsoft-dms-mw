using System;

namespace BL.Model.Common
{
    /// <summary>
    /// Информция о последнем измееннии записи
    /// </summary>
    public class LastChangeInfo
    {
        /// <summary>
        /// ИД пользователя, последним изменившего запись
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}
