using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentEmployees
    {
        public int Id { get; set; }
        [Index("IX_PersonnelNumber", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_PersonnelNumber", 1, IsUnique = true)]
        public string PersonnelNumber { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }

        //[ForeignKey("AgentPersonId")]
        //public virtual DictionaryAgentPersons AgentPerson { get; set; }


    }
}
