using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.AspNet.IncomingModel
{
    public class ModifyAspNetUserServers
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}