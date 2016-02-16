using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryAgent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual IEnumerable<BaseDictionaryAgentPerson> AgentPersonsAgents { get; set; }
        public virtual IEnumerable<BaseDictionaryAgentPerson> AgentPersonsPersonAgents { get; set; }
    }
}