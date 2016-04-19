using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public class SystemLogs
    {
        public int Id { get; set; }
        public int LogLevel { get; set; }
        [MaxLength(2000)]
        public string Message { get; set; }
        [MaxLength(2000)]
        public string LogTrace { get; set; }
        [MaxLength(2000)]
        public string LogException { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }
        [Index("IX_LogDate", 1)]
        public DateTime LogDate { get; set; }
    }
}