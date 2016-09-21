using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class DigitalSignatureDocumentPdf : CurrentPosition
    {
        public int DocumentId { get; set; }
        public bool IsAddSubscription { get; set; }
    }
}
