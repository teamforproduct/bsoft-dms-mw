using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Encryption
{
    public class EncryptionCertificateTypes
    {
        public EncryptionCertificateTypes()
        {
            this.Certificates = new HashSet<EncryptionCertificates>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public virtual ICollection<EncryptionCertificates> Certificates { get; set; }
    }
}
