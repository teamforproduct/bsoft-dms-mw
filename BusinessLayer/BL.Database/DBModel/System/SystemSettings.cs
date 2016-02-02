using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.System
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }

        [ForeignKey("AgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }
    }
}