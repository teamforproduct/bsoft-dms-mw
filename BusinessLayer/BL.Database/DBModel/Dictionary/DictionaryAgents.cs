using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgents
    {
        public DictionaryAgents()
        {
            this.Positions = new HashSet<DictionaryPositions>();
            this.AgentPersonsAgents = new HashSet<DictionaryAgentPersons>();
            this.AgentPersonsPersonAgents = new HashSet<DictionaryAgentPersons>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryPositions> Positions { get; set; }

        [ForeignKey("AgentId")]
        public virtual ICollection<DictionaryAgentPersons> AgentPersonsAgents { get; set; }
        
        [ForeignKey("PersonAgentId")]
        public virtual ICollection<DictionaryAgentPersons> AgentPersonsPersonAgents { get; set; }
    }
}
