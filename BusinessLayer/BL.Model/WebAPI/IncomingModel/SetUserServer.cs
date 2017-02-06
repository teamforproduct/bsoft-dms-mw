using BL.Model.Database;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class SetUserServer
    {
        public int ServerId { get; set; }
        public int ClientId { get; set; }
        public string ClientCode { get; set; }
    }
}