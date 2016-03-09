using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentAddresses
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AdressTypeId { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("AdressTypeId")]
        public virtual DictionaryAddressTypes AddressType { get; set; }
    }
}
