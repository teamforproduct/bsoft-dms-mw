using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentEmployees
    {
        public int Id { get; set; }
        public string PersonnelNumber { get; set; }
        public int AgentPersonId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentPersons AgentPerson { get; set; }
    }
}
