using System;
using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    public class LicenceInfo
    {
        public int ClientId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Только для информации
        /// </summary>
        public bool IsTrial { get; set; }

        public bool IsDateLimit { get; set; }

        public bool IsConcurenteLicence { get; set; }

        public bool IsNamedLicence { get; set; }

        public bool IsFunctionals { get; set; }

        public DateTime FirstStart { get; set; }

        public int? NamedNumberOfConnections { get; set; }
        public int NamedNumberOfConnectionsNow { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int ConcurenteNumberOfConnectionsNow { get; set; }

        public int? DateLimit { get; set; }

        public string Functionals { get; set; }

        public string LicenceKey { get; set; }
    }
}