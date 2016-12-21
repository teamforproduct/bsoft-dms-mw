using System;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Человек
    /// </summary>
    public class InternalDictionaryAgentPeople : InternalDictionaryAgent
    {

        public InternalDictionaryAgentPeople()
        { }

        public InternalDictionaryAgentPeople(AddAgentPerson model)
        {
            SetInternalDictionaryAgentPerson(model);
        }

        public InternalDictionaryAgentPeople(ModifyAgentPerson model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentPerson(model);
        }

        public InternalDictionaryAgentPeople(AddAgentEmployee model)
        {
            SetInternalDictionaryAgentPerson(model);
        }

        public InternalDictionaryAgentPeople(ModifyAgentEmployee model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentPerson(model);
        }

        private void SetInternalDictionaryAgentPerson(AddAgentPerson model)
        {
            Name = model.Name;
            FirstName = model.FirstName;
            LastName = model.LastName;
            MiddleName = model.MiddleName;
            TaxCode = model.TaxCode;
            IsMale = model.IsMale;
            PassportSerial = model.PassportSerial;
            PassportNumber = model.PassportNumber;
            PassportText = model.PassportText;
            PassportDate = model.PassportDate;
            BirthDate = model.BirthDate;
        }


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
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }
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
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }

    }
}
