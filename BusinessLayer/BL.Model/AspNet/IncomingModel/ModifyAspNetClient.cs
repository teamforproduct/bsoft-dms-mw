using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.AspNet.IncomingModel
{
    public class ModifyAspNetClient
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}