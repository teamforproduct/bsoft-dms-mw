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
        [Required]
        public string VisaText { get; set; }
     
    }
}
