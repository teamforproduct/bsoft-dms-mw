using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClients
    {
        public AspNetClients()
        {
            this.Servers = new HashSet<AdminServers>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }

        public DateTime? FirstStart { get; set; }

        public string LicenceType { get; set; }

        public int NumberOfConnections { get; set; }

        public int DurationDay { get; set; }

        public string LicenceKey { get; set; }

        public virtual ICollection<AdminServers> Servers { get; set; }
    }
}
