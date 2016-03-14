using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentContacts
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int ContactTypeId { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("ContactTypeId")]
        public virtual DictionaryContactTypes ContactType { get; set; }
    }
}
