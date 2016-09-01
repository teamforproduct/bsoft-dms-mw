using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsMale { get; set; }
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
        public DateTime? PassportDate { get; set; }
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string PassportText { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }

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
            get { return PassportSerial?.Trim() + " " + PassportNumber?.ToString() + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); }
        }

#endregion

        #region [+] Employee ...
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
        #endregion

        #region [+] User ...
        /// <summary>
        /// Наименование компании
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int? LanguageId { get; set; }
        #endregion

    }
}
