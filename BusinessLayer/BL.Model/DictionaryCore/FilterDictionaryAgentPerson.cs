using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    public class FilterDictionaryAgentPerson
    {
        public List<int> Id { get; set; }
        public List<int> AgentId { get; set; }
        public string Name { get; set; }
        public string AgentName { get; set; }
        public string PersonAgentName { get; set; }
    }
}
