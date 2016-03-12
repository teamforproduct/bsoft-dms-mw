using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgents
    {
        public DictionaryAgents()
        {
       //     this.Positions = new HashSet<DictionaryPositions>();
            this.AgentAddresses = new HashSet<DictionaryAgentAddresses>();
            this.AgentContacts = new HashSet<DictionaryAgentContacts>();
            this.AgentAccounts = new HashSet<DictionaryAgentAccounts>();

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ResidentTypeId { get; set; }
        public bool IsCompany { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsBank { get; set; }
        public bool IsUser { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

     //   public virtual ICollection<DictionaryPositions> Positions { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgentCompanies AgentCompany { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentPersons AgentPerson { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentBanks AgentBank { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentEmployees AgentEmployee { get; set; }

        public virtual DictionaryResidentTypes ResidentType { get; set; }

        public virtual ICollection<DictionaryAgentAddresses> AgentAddresses { get; set; }

        public virtual ICollection<DictionaryAgentContacts> AgentContacts { get; set; }

        public virtual ICollection<DictionaryAgentAccounts> AgentAccounts { get; set; }

        //[ForeignKey("AgentId")]
        //public virtual ICollection<DictionaryAgentPersons> AgentPersonsAgents { get; set; }

        //[ForeignKey("PersonAgentId")]
        //public virtual ICollection<DictionaryAgentPersons> AgentPersonsPersonAgents { get; set; }
    }
}
