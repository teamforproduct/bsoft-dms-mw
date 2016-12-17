using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class AddDictionaryAgentEmployee
    {
        #region [+] Person ...

        /// <summary>
        /// Имя (кратко)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        //[Required]
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
        public DateTime? PassportDate { get { return _PassportDate; } set { _PassportDate=value.ToUTC(); } }
        private DateTime? _PassportDate;
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string PassportText { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate=value.ToUTC(); } }
        private DateTime? _BirthDate;

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }


        /// <summary>
        /// ИД аватарки, если она была загружена
        /// </summary>
        public int? ImageId { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public string PostedFileData { get; set; }
        #endregion

        #region [+] Employee ...
        /// <summary>
        /// табельный номер
        /// </summary>
        public int PersonnelNumber { get; set; }
        #endregion

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int LanguageId { get; set; }

    }
}
