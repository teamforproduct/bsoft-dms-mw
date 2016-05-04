using System;
using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    public class LicenceInfo
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public DateTime FirstStart { get; set; }
        public EnumLicenceTypes LicType { get; set; }
        public int NumberOfConnections { get; set; }
        public int DateLimit { get; set; }
    }
}