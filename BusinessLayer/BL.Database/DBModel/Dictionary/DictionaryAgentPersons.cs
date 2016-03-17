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
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string TaxCode { get; set; }
        public bool IsMale { get; set; }
        public string PassportSerial { get; set; }
        public Nullable<int> PassportNumber { get; set; }
        public string PassportText { get; set; }
        public Nullable<DateTime> PassportDate { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<int> AgentCompanyId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("AgentCompanyId")]
        public virtual DictionaryAgentCompanies AgentCompany { get; set; }


      

        //     [ForeignKey("AgentId")]
        //     public virtual DictionaryAgents Agent { get; set; }
        // [ForeignKey("PersonAgentId")]
        // public virtual DictionaryAgents PersonAgent { get; set; }
    }
}
