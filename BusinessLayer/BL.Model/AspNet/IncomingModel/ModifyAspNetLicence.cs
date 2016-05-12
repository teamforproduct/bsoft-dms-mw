using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.AspNet.IncomingModel
{
    public class ModifyAspNetLicence
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Только для информации
        /// </summary>
        public bool IsTrial { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        public string Functionals { get; set; }
    }
}