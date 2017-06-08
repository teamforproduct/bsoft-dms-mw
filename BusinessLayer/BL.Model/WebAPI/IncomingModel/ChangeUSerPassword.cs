using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ChangeUserPassword
    {
        /// <summary>
        /// Текущий пароль
        /// </summary>
        [Required]
        public string OldPassword { get; set; }

        /// <summary>
        /// Новый пароль
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

    }

}