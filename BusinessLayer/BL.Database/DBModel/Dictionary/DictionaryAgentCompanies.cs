using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [MaxLength(2000)]
        [Index("IX_FullName", 1, IsUnique = true)]
        public string FullName { get; set; }
        [MaxLength(2000)]
        [Index("IX_TaxCode", 1, IsUnique = true)]
        public string TaxCode { get; set; }
        [MaxLength(2000)]
        public string OKPOCode { get; set; }
        [MaxLength(2000)]
        public string VATCode { get; set; }
        [MaxLength(2000)]
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
