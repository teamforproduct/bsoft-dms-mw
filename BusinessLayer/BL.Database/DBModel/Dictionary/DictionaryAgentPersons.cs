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
        [MaxLength(400)]
        //[Index("IX_FullName", 1, IsUnique = true)]
        [Obsolete("UseAgentPeople",true)]
        public string FullName { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string LastName { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string FirstName { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string MiddleName { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string TaxCode { get; set; }
        [Obsolete("UseAgentPeople", true)]
        public bool IsMale { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string PassportSerial { get; set; }
        [Obsolete("UseAgentPeople", true)]
        public Nullable<int> PassportNumber { get; set; }
        [MaxLength(2000)]
        [Obsolete("UseAgentPeople", true)]
        public string PassportText { get; set; }
        [Obsolete("UseAgentPeople", true)]
        public Nullable<DateTime> PassportDate { get; set; }
        [Obsolete("UseAgentPeople", true)]
        public Nullable<DateTime> BirthDate { get; set; }
        
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
