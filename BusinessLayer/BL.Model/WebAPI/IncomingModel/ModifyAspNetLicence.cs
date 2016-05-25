using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetLicence
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        public string Functionals { get; set; }
    }
}