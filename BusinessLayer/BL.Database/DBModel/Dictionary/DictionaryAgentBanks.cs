using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentBanks
    {
        public int Id { get; set; }
        [Index("IX_FullName", 2, IsUnique = true)]
        [Index("IX_MFOCode", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_MFOCode", 1, IsUnique = true)]
        public string MFOCode { get; set; }
        [MaxLength(400)]
        [Index("IX_FullName", 1, IsUnique = true)]
        public string FullName { get; set; }
        [MaxLength(2000)]
        public string Swift { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
