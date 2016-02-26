using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для изменения признака фаворита
    /// </summary>
    public class ChangeFavourites : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
    }
}
