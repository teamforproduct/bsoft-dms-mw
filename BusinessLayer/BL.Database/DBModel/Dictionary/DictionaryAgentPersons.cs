using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentPersons
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public Nullable<int> PersonAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("PersonAgentId")]
        public virtual DictionaryAgents PersonAgent { get; set; }
    }
}
