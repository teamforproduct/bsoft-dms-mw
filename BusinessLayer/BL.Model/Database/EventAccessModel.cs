using BL.Model.DocumentCore;

namespace BL.Model.Database
{
    public class EventAccessModel
    {
        public BaseDocumentAccess DocumentAccess { get; set; }
        public BaseDocumentEvent DocumentEvent { get; set; }
    }
}
