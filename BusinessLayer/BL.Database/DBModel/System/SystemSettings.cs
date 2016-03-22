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
        public string Key { get; set; }
        [MaxLength(2000)]
        public string Value { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }

        [ForeignKey("ExecutorAgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }
    }
}