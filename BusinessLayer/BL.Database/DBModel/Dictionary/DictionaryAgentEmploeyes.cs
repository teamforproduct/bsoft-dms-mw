using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentEmployees
    {

        public DictionaryAgentEmployees()
        {
            this.PositionExecutors = new HashSet<DictionaryPositionExecutors>();
        }

        public int Id { get; set; }

        [Index("IX_PersonnelNumber", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
       
        [Index("IX_PersonnelNumber", 1, IsUnique = true)]
        public int PersonnelNumber { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }

        //[ForeignKey("AgentPersonId")]
        //public virtual DictionaryAgentPersons AgentPerson { get; set; }

        [ForeignKey("AgentId")]
        public virtual ICollection<DictionaryPositionExecutors> PositionExecutors { get; set; }

    }
}
