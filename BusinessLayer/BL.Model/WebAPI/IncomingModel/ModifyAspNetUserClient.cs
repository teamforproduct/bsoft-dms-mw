using BL.Model.Database;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetUserClient
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ClientId { get; set; }
    }
}