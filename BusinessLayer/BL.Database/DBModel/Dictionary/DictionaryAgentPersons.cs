using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentPersons
    {
        public DictionaryAgentPersons(){
            //this.AgentEmployees = new HashSet<DictionaryAgentEmployees>();
        }

        public int Id { get; set; }
        //[Index("IX_FullName", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        public Nullable<int> AgentCompanyId { get; set; }

        /// <summary>
        /// Должность контактного лица (текстовое поле)
        /// </summary>
        [MaxLength(2000)]
        public string Position { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentPeople People { get; set; }

        [ForeignKey("AgentCompanyId")]
        public virtual DictionaryAgentCompanies AgentCompany { get; set; }

        //[ForeignKey("AgentPersonId")]
        //public virtual ICollection<DictionaryAgentEmployees> AgentEmployees { get; set; }

        //     [ForeignKey("AgentId")]
        //     public virtual DictionaryAgents Agent { get; set; }
        // [ForeignKey("PersonAgentId")]
        // public virtual DictionaryAgents PersonAgent { get; set; }
    }
}
