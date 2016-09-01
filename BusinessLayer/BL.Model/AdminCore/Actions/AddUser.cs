using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.Actions
{
    /// <summary>
    /// Модель для добавления агента, соответствующего с WEB-пользователем
    /// </summary>
    public class AddUser
    {
        /// <summary>
        /// ИД WEB-пользователя
        /// </summary>
        [Required]
        public string UserId { get; set; }

    }
}
