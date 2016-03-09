using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentAccounts
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public int AgentBankId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsMain { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("AgentBankId")]
        public virtual DictionaryAgentBanks AgentBank { get; set; }
    }
}
