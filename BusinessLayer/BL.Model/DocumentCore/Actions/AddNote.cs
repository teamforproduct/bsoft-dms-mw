using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для добавления примечания
    /// </summary>
    public class AddNote : CurrentPosition
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        [Required]
        public string Description { get; set; }
     
    }
}
