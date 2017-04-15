using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetLicences
    {
        public AspNetLicences()
        {
            this.ClientLicences = new HashSet<AspNetClientLicences>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(2000)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }

        [MaxLength(2000)]
        public string Functionals { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("LicenceId")]
        public virtual ICollection<AspNetClientLicences> ClientLicences { get; set; }
    }
}
