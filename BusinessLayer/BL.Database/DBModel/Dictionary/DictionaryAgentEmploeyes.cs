using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentEmployees
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        [Index("IX_PersonnelNumber", 1, IsUnique = true)]
        public string PersonnelNumber { get; set; }
        [Index("IX_AgentPersonId", 1, IsUnique = true)]
        public int AgentPersonId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
