using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClients
    {
        public AspNetClients()
        {
            this.ClientLicences = new HashSet<AspNetClientLicences>();
            this.ClientServers = new HashSet<AspNetClientServers>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string VerificationCode { get; set; }


        [ForeignKey("ClientId")]
        public virtual ICollection<AspNetClientLicences> ClientLicences { get; set; }

        [ForeignKey("ClientId")]
        public virtual ICollection<AspNetClientServers> ClientServers { get; set; }
    }
}
