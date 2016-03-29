using System;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class InternalDictionaryAgentPerson : LastChangeInfo
    {

        public InternalDictionaryAgentPerson()
        { }

        public InternalDictionaryAgentPerson(ModifyDictionaryAgentPerson model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            MiddleName = model.MiddleName;
            TaxCode = model.TaxCode;
            IsMale = model.IsMale;
            PassportSerial = model.PassportSerial;
            PassportNumber = model.PassportNumber;
            PassportText = model.PassportText;
            PassportDate = model.PassportDate;
            IsActive = model.IsActive;
            BirthDate = model.BirthDate;
            Description = model.Description;
        }      
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
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        
    }
}
