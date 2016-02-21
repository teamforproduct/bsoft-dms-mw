using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.Database
{
    public class EventAccessModel
    {
        public InternalDocumentAccesses DocumentAccess { get; set; }
        public InternalDocumentEvents DocumentEvent { get; set; }
    }
}
