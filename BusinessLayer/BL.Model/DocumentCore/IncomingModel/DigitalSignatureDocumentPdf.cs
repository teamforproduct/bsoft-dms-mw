using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class DigitalSignatureDocumentPdf
    {
        public int DocumentId { get; set; }

        public int? CertificateId { get; set; }
        public string CertificatePassword { get; set; }

    }
}
