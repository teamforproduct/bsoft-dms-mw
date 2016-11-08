using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class AddDictionaryAgentEmployee : ModifyDictionaryAgentEmployee
    {
   
        #region [+] AgentUser ...
        /// <summary>
        /// Логин
        /// </summary>
        //[Required]
        //public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Связь с WEB - USER
        /// </summary>
        //[IgnoreDataMember]
        //public string UserId { get; set; }

        /// <summary>
        /// Основной имейл, на который высылается письмо с приглашением
        /// </summary>
        [Required]
        //[EmailAddress]
        public string Login { get; set; }

        /// <summary>
        /// Основной номер мобильного телефона
        /// </summary>
        public string Phone { get; set; }

        #endregion
    }
}
