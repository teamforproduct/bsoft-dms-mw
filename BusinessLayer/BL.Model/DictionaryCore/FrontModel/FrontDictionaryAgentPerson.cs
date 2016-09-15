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
        public override string Description { get; set; }

        //pss убрать вычисляемые свойства
        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName
        {
            get { return LastName?.Trim() + " " + FirstName?.Trim() + " " + MiddleName?.Trim(); }
        }
        /// <summary>
        /// Сокращенное имя
        /// </summary>
        public string ShortName
        {
            get { return LastName?.Trim() + " " + (string.IsNullOrEmpty(FirstName) ? "" : FirstName.Trim().Substring(0, 1) + ".") 
                         + (string.IsNullOrEmpty(MiddleName) ? "": MiddleName.Trim().Substring(0, 1) + "."); }
        }
        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport
        {
            get { return PassportSerial?.Trim() + " " + PassportNumber + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); }
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
