using System.Runtime.Serialization;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyDocumentSavedFilter : CurrentPosition
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Icon { get; set; }
        public object Filter { get; set; }
        public bool IsCommon { get; set; }
    }
}
