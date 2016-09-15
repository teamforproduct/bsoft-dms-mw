using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель подписания
    /// </summary>
    public class AffixSigning: SendEventMessage
    {
        /// <summary>
        /// Текст визы
        /// </summary>
        public string VisaText { get; set; }

        public int? CertificateId { get; set; }
        public string CertificatePassword { get; set; }

    }
}
