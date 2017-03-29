using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class DigitalSignatureDocumentPdf : CurrentPosition
    {
        public int DocumentId { get; set; }
        public bool IsAddSubscription { get; set; }
        public string ServerPath { get; set; }
    }
}
