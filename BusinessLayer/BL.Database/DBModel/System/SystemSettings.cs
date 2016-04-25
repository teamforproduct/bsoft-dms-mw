using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.System
{
    public class SystemSettings
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        [Index("IX_KeyExecutorAgent", 1, IsUnique = true)]
        public string Key { get; set; }
        [MaxLength(2000)]
        public string Value { get; set; }
        [Index("IX_KeyExecutorAgent", 2, IsUnique = true)]
        [Index("IX_ExecutorAgentId", 1)]
        public Nullable<int> ExecutorAgentId { get; set; }

        [ForeignKey("ExecutorAgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }
    }
}