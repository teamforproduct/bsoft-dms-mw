using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class ResetPassword
    {

        /// <summary>
        /// Пользователь
        /// </summary>       
        [Required]
        public string UserId { get; set; }


        /// <summary>
        /// Код
        /// </summary>       
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

    }
}
