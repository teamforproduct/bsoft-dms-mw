using System;

namespace BL.Model.Context
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class Session
    {
        public string Key { get; set; }
        /// <summary>
        /// ИД лога сесии
        /// </summary>
        public int SignInId { get; set; }
        public DateTime CreateDate { get; } = DateTime.UtcNow;
        public DateTime LastUsage { get; set; } = DateTime.UtcNow;

    }
}