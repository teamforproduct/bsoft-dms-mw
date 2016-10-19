using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public class SystemLogs
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        public int LogLevel { get; set; }
        [MaxLength(2000)]
        public string Message { get; set; }
        [MaxLength(2000)]
        public string LogTrace { get; set; }
        [MaxLength(2000)]
        public string LogException { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public Nullable<int> ActionId { get; set; }
        public Nullable<int> RecordId { get; set; }

        [Index("IX_LogDate", 1)]
        public DateTime LogDate { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }
    }
}