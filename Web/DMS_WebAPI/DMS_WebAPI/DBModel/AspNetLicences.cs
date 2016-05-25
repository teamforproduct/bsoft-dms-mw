using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AspNetLicences
    {
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
    }
}
