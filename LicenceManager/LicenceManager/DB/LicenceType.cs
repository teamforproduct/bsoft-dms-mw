using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LicenceManager.DB
{
    public class LicenceType
    {
        public LicenceType()
        {
            this.Clients = new HashSet<ClientsLicence>();
        }

        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }

        public bool Activ { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        [MaxLength(2000)]
        public string Functionals { get; set; }

        public virtual ICollection<ClientsLicence> Clients { get; set; }
    }
}