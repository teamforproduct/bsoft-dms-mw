using BL.Model.Database;
using BL.Model.SystemCore;

namespace BL.Model.AspNet.FrontModel
{
    public class FrontAspNetLicence
    {
        public int Id { get; set; }
        public bool IsTrial { get; set; }

        public bool IsDateLimit { get; set; }

        public bool IsConcurenteLicence { get; set; }

        public bool IsNamedLicence { get; set; }

        public bool IsFunctionals { get; set; }


        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }

        public int? DurationDay { get; set; }

        public string Functionals { get; set; }
    }
}