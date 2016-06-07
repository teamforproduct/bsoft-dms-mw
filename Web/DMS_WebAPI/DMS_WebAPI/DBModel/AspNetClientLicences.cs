using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientLicences
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime FirstStart { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        [MaxLength(2000)]
        public string Functionals { get; set; }

        [MaxLength(2000)]
        public string LicenceKey { get; set; }

        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }
    }
}
