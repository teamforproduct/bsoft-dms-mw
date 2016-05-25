using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentUsers
    {
        public int Id { get; set; }
        [MaxLength(128)]
        [Index("IX_UserId", 1, IsUnique = true)]
        public string UserId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
