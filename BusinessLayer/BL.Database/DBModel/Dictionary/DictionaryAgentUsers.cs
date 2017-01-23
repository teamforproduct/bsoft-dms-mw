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
        [Index("IX_UserId", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(128)]
        [Index("IX_UserId", 1, IsUnique = true)]
        public string UserId { get; set; }
        public int LanguageId { get; set; }
        [MaxLength(2000)]
        public string LastPositionChose { get; set; }
        public bool IsSendEMail { get; set; }
        /// <summary>
        /// Определяет может ли пользователь войти в систему
        /// </summary>
        [Obsolete("Этот флаг лишний. В WEB-базе есть дубль", true)]
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgents Agent { get; set; }

        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }
    }
}
