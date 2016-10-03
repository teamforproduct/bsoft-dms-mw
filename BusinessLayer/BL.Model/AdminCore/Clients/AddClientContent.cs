using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

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
        [IgnoreDataMember]
        public int ClientId { get; set; }


        /// <summary>
        /// Доменное имя клиента сlient156454.ostrean.com
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "{0} должно быть по крайней мере {2} символов.", MinimumLength = 3)]
        public string Domain { get; set; }

        /// <summary>
        /// Язык интерфейса
        /// </summary>
        [Required]
        [Range(1, 999)]
        public int LanguageId { get; set; }

        /// <summary>
        /// Адрес первого пользователя - директора, админа
        /// </summary>
        [Required]
        [EmailAddress]
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
        [Phone]
        public string PhoneNumber { get; set; }

    }
}