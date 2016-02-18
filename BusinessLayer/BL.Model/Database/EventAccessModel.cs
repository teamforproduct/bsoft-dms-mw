using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Model.Database
{
    public class EventAccessModel
    {
        public FrontDocumentAccess DocumentAccess { get; set; }
        public FrontDocumentEvent DocumentEvent { get; set; }
    }
}
