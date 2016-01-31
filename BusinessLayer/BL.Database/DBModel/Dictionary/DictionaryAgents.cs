using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgents
    {
        public DictionaryAgents()
        {
            this.Positions = new HashSet<DictionaryPositions>();
            this.AgentPersons = new HashSet<DictionaryAgentPersons>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryPositions> Positions { get; set; }
        public virtual ICollection<DictionaryAgentPersons> AgentPersons { get; set; }
    }
}
