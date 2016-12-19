using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// ОСНОВНОЙ. Справочник контрагентов
    /// </summary>
    public class FrontDictionaryAgentUser : ModifyDictionaryAgentUser
    {
        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryAgentContact> Contacts { get; set; }
        /// <summary>
         /// Список адресов контрагента
         /// </summary>
        public IEnumerable<FrontDictionaryAgentAddress> Addresses { get; set; }
        
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ИД языка пользователя
        /// </summary>
        public int? LanguageId { get; set; }
        /// <summary>
        /// Название языка пользователя
        /// </summary>
        public string LanguageName { get; set; }
        /// <summary>
        /// Признак отсылать ли уведомления на почту
        /// </summary>
        public bool? IsSendEMail { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
