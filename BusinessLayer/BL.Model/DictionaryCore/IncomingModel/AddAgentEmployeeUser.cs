using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class AddAgentEmployeeUser : AddAgentEmployee
    {

        /// <summary>
        /// Связь с WEB - USER
        /// </summary>
        [IgnoreDataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Связь с WEB - USER - Login
        /// </summary>
        [IgnoreDataMember]
        public string UserName { get; set; }

        [IgnoreDataMember]
        public string Password { get; set; }

        [IgnoreDataMember]
        public bool IsChangePasswordRequired { get; set; } = true;
        [IgnoreDataMember]
        public bool IsEmailConfirmRequired { get; set; } = true;
        [IgnoreDataMember]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Имейл, на который высылается письмо с приглашением
        /// </summary>
        [Required]
        //[EmailAddress]
        public string Login { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        public string Phone { get; set; }

    }
}
