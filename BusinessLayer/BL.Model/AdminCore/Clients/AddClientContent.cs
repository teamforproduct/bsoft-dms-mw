using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.Clients
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClientContent
    {
        /// <summary>
        /// Id клиента сlient156454.ostrean.com
        /// </summary>
        [Required]
        public int ClientId { get; set; }
        
        /// <summary>
        /// Язык интерфейса
        /// </summary>
        [Required]
        public int LanguageId { get; set; }

        /// <summary>
        /// Имя первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Фамилия первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string LastName { get; set; }


        /// <summary>
        /// Пароль первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Адрес первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

    }
}
