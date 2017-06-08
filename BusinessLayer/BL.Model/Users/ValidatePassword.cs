using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class ValidatePassword
    {

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}
