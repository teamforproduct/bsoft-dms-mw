using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentCompanies
    {
        public DictionaryAgentCompanies()
        {
            this.AgentPersons = new HashSet<DictionaryAgentPersons>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string TaxCode { get; set; }
        public string OKPOCode { get; set; }
        public string VATCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("AgentCompanyId")]
        public virtual ICollection<DictionaryAgentPersons> AgentPersons { get; set; }
    }
}
