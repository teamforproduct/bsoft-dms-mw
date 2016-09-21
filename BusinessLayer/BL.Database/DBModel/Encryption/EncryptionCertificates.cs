using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Encryption
{
    public class EncryptionCertificates
    {
        public EncryptionCertificates()
        {
            this.DocumentSubscriptions = new HashSet<DocumentSubscriptions>();
        }
        public int Id { get; set; }
        [MaxLength(400)]
        public string Name { get; set; }

        public string Thumbprint { get; set; }

        public byte[] Certificate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? NotBefore { get; set; }
        public DateTime? NotAfter { get; set; }

        public bool IsRememberPassword { get; set; }

        public int AgentId { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DocumentSubscriptions> DocumentSubscriptions { get; set; }
    }
}
