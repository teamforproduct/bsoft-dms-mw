using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentAddresses
    {
        public int Id { get; set; }

        [Index("IX_AdressType", 1, IsUnique = true)]
        public int AgentId { get; set; }

        [Index("IX_AdressType", 2, IsUnique = true)]
        [Index("IX_AdressTypeId", 1)]
        public int AdressTypeId { get; set; }

        [MaxLength(2000)]
        public string PostCode { get; set; }
        [MaxLength(2000)]
        public string Address { get; set; }
        [MaxLength(2000)]
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
