using BL.Model.Users;
using System.Runtime.Serialization;

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
