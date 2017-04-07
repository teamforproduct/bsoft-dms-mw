using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.Clients
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClientSaaS
    {
        /// <summary>
        /// Id клиента сlient156454.ostrean.com
        /// </summary>
        [IgnoreDataMember]
        public int ClientId { get; set; }

        [IgnoreDataMember]
        public string Password { get; set; }

        /// <summary>
        /// Доменное имя клиента сlient156454.ostrean.com
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "{0} должно быть по крайней мере {2} символов.", MinimumLength = 3)]
        public string ClientCode { get { return _ClientCode; } set { _ClientCode = value.Trim().ToLower(); } }
        private string _ClientCode;
        
        /// <summary>
        /// Язык интерфейса
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Адрес первого пользователя - директора, админа
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get { return _Email; } set { _Email = value.Trim(); } }
        private string _Email;

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