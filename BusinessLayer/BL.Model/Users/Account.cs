using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class Account
    {
        /// <summary>
        /// Email
        /// </summary>       
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>       
        [Required]
        public string PhoneNumber { get; set; }


        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Имя
        /// </summary>       
        [Required]
        public string FullName { get; set; }


        /// <summary>
        /// Код языка (ru_RU)
        /// </summary>    
        [Required]
        public string LanguageCode { get; set; }



        /// <summary>
        /// Google Recaptcha
        /// </summary>
        [Required]
        public string Recaptcha { get; set; }

    }
}
