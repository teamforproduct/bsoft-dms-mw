using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

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
        public DateTime LogDate { get; set; }

        [ForeignKey("ExecutorAgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }
    }
}