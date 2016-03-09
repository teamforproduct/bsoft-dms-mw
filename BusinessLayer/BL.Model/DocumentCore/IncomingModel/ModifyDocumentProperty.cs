using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyDocumentProperty
    {
        public int PropertyLinkId { get; set; }
        public string Value { get; set; }
    }
}
