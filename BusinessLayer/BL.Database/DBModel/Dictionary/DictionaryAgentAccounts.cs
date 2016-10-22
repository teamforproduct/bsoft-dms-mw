using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentAccounts
    {
        public int Id { get; set; }
        [Index("IX_AgentName", 1, IsUnique = true)]
        public int AgentId { get; set; }
        [MaxLength(400)]
        [Index("IX_AgentName", 2, IsUnique = true)]
        public string Name { get; set; }
        //public int AgentBankId { get; set; }
        [MaxLength(2000)]
        public string AccountNumber { get; set; }
        public bool IsMain { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        //[ForeignKey("AgentId")]
        public virtual DictionaryAgentBanks AgentBank { get; set; }
    }
}
