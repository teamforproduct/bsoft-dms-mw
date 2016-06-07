using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LicenceManager.DB
{
    public class ClientsInfo
    {
        public ClientsInfo()
        {
            this.Licences = new HashSet<ClientsLicence>();
        }

        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }

        public virtual ICollection<ClientsLicence> Licences { get; set; }
    }
}