using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

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
        /// <summary>
        /// Признак фаворита
        /// </summary>
        [Required]
        public bool IsFavourite { get; set; }
    }
}
