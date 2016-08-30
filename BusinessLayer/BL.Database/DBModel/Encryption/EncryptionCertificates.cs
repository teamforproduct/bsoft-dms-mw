using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Encryption
{
    public class EncryptionCertificates
    {
        public int Id { get; set; }
        [MaxLength(400)]
        public string Name { get; set; }
        public byte[] Certificate { get; set; }
        [MaxLength(400)]
        public string Extension { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }
        public int AgentId { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual EncryptionCertificateTypes Type { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
