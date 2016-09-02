using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.Actions
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClient
    {
        /// <summary>
        /// Доменное имя клиента сlient156454.ostrean.com
        /// </summary>
        [Required]
        public string Domain { get; set; }

        /// <summary>
        /// Язык интерфейса
        /// </summary>
        [Required]
        public int LanguageId { get; set; }

        /// <summary>
        /// Адрес первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string Email { get; set; }

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
        /// Номер телефона первого пользователя - директора, админа
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

    }
}
