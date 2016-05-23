using BL.Model.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetClientLicence
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime FirstStart { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        [MaxLength(2000)]
        public string Functionals { get; set; }
    }
}