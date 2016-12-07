using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontDictionaryAgentEmployee: FrontDictionaryAgent
    {

        #region [+] Person ...
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }
        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string PassportSerial { get; set; }
        /// <summary>
        /// Номер паспорта
        /// </summary>
        public int? PassportNumber { get; set; }
        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        public DateTime? PassportDate { get { return _PassportDate; } set { _PassportDate=value.ToUTC(); } }
        private DateTime?  _PassportDate; 
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string PassportText { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate=value.ToUTC(); } }
        private DateTime?  _BirthDate; 

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public new string Description { get; set; }
        
        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }
        
     
        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport
        {
            // pss  проверить сериализуется ли поведение, либо результат
            get { string pass =  PassportSerial?.Trim() + " " + PassportNumber?.ToString() + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); return pass.Trim(); }
        }

#endregion

        #region [+] Employee ...
        /// <summary>
        /// табельный номер сотрудника
        /// </summary>
        public int PersonnelNumber { get; set; }
        #endregion

        #region [+] User ...
        /// <summary>
        /// Наименование компании
        /// </summary>
        [IgnoreDataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int? LanguageId { get; set; }
        #endregion

    }
}
