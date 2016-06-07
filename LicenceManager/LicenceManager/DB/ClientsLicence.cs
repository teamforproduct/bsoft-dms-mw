using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenceManager.DB
{
    public class ClientsLicence
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? LicenceId { get; set; }
        public bool IsTrial { get; set; }
        public DateTime StartDate { get; set; }
        [MaxLength(2000)]
        public string VerificationCode { get; set; }
        [MaxLength(2000)]
        public string LicenceKey { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientsInfo Clients { get; set; }

        [ForeignKey("LicenceId")]
        public virtual LicenceType Licences { get; set; }
    }
}