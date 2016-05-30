using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClients
    {
        public AspNetClients()
        {
            this.ClientServers = new HashSet<AspNetClientServers>();
            this.Licences = new HashSet<AspNetClientLicences>();
            this.UserServers = new HashSet<AspNetUserServers>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string VerificationCode { get; set; }

        public virtual ICollection<AspNetClientServers> ClientServers { get; set; }
        public virtual ICollection<AspNetClientLicences> Licences { get; set; }
        public virtual ICollection<AspNetUserServers> UserServers { get; set; }
    }
}
