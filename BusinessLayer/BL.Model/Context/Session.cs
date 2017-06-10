using System;

namespace BL.Model.Context
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class Session
    {
        
        /// <summary>
        /// ИД лога сесии
        /// </summary>
        public int Id { get; set; }
        public DateTime CreateDate { get; } = DateTime.UtcNow;
        public DateTime LastUsage { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    }
}