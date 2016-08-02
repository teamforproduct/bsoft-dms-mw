using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientLicences
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int LicenceId { get; set; }
        public DateTime FirstStart { get; set; }
        public bool IsActive { get; set; }
        public string LicenceKey { get; set; }

        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }
        [ForeignKey("LicenceId")]
        public virtual AspNetLicences Licence { get; set; }
    }
}
