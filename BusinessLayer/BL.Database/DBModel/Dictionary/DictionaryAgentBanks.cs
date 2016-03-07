using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentBanks
    {
        public int Id { get; set; }
        public string MFOCode { get; set; }
        public string Swift { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
