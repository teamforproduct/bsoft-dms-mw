using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.System
{
    public class SystemSettings
    {
        public int Id { get; set; }
        [Index("IX_KeyExecutorAgent", 3, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_KeyExecutorAgent", 1, IsUnique = true)]
        public string Key { get; set; }

        [MaxLength(400)]
        public string Name { get; set; }    //TODO DEL!!!
        [MaxLength(2000)]
        public string Description { get; set; } //TODO DEL!!!

        [MaxLength(2000)]
        public string Value { get; set; }
        public int ValueTypeId { get; set; }
        public int SettingTypeId { get; set; }
        public int Order { get; set; }

        [Index("IX_KeyExecutorAgent", 2, IsUnique = true)]
        [Index("IX_ExecutorAgentId", 1)]
        public Nullable<int> ExecutorAgentId { get; set; }
        public int AccessType { get; set; }

        [ForeignKey("ExecutorAgentId")]
        public virtual DictionaryAgents ExecutorAgent { get; set; }

        [ForeignKey("ValueTypeId")]
        public virtual SystemValueTypes ValueTypes { get; set; }
        [ForeignKey("SettingTypeId")]
        public virtual DictionarySettingTypes SettingType { get; set; }
    }
}