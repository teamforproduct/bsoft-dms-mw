using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Доступ к шаблону документа
    /// </summary>
    public class AddTemplateDocumentAccess
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Ссылка на должность
        /// </summary>
        [Required]
        public int PositionId { get; set; }
    }
}
