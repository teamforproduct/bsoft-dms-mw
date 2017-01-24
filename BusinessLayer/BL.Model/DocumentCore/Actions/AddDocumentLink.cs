using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для связывания документов
    /// </summary>
    public class AddDocumentLink : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД родительского документа
        /// </summary>
        [Required]
        public int ParentDocumentId { get; set; }
        /// <summary>
        /// ИД типа связи
        /// </summary>
        [Required]
        public int LinkTypeId { get; set; }       

    }
}
