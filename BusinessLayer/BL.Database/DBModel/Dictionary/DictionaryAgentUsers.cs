using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Admin;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentUsers
    {
        public int Id { get; set; }
        [MaxLength(128)]
        [Index("IX_UserId", 1, IsUnique = true)]
        public string UserId { get; set; }
        public Nullable<int> LanguageId { get; set; }
        [MaxLength(256)]
        public string Login { get; set; }
        [MaxLength(2000)]
        public string PasswordHash { get; set; }
        public byte[] Picture { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }
//        [ForeignKey("LanguageId")]
//        public virtual AdminLanguages Language { get; set; }
    }
}
