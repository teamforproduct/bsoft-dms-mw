using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class FrontDictionaryAgentPerson : FrontDictionaryAgent
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
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
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Сокращенное имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport
        {
            get { string pass = PassportSerial?.Trim() + " " + PassportNumber + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); return pass.Trim(); }
        }

        /// <summary>
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryContact> Contacts { get; set; }
        /// <summary>
        /// Список адресов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryAgentAddress> Addresses { get; set; }

    }
}
