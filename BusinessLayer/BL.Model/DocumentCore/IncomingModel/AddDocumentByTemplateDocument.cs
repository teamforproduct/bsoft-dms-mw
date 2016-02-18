using System.ComponentModel.DataAnnotations;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавление документа по шаблону
    /// </summary>
    public class AddDocumentByTemplateDocument : CurrentPosition
    {
        /// <summary>
        /// ИД шаблона документа
        /// </summary>
        [Required]
        public int TemplateDocumentId { get; set; }
        /// <summary>
        /// ИД документа, с которым нужно установить связь (если необходимо)
        /// </summary>
        public int? ParentDocumentId { get; set; }
    }
}
