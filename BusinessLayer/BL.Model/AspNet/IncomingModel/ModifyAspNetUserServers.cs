using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.AspNet.IncomingModel
{
    public class ModifyAspNetUserServers
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ServerId { get; set; }
    }
}