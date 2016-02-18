using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Model.Database
{
    public class EventAccessModel
    {
        public BaseDocumentAccess DocumentAccess { get; set; }
        public BaseDocumentEvent DocumentEvent { get; set; }
    }
}
