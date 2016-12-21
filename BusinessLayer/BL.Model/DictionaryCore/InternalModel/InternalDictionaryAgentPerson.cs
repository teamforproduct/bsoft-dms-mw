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

        public InternalDictionaryAgentPerson(AddAgentPerson model)
        {
            SetInternalDictionaryAgentPerson(model);
        }

        public InternalDictionaryAgentPerson(AddAgentContactPerson model)
        {
            SetInternalDictionaryAgentContactPerson(model);
        }

        public InternalDictionaryAgentPerson(ModifyAgentContactPerson model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentContactPerson(model);
        }

        public InternalDictionaryAgentPerson(ModifyAgentPerson model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentPerson(model);
        }

        public void SetInternalDictionaryAgentPerson(AddAgentPerson model)
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
            IsActive = model.IsActive;
            BirthDate = model.BirthDate;
            AgentCompanyId = model.AgentCompanyId;
            Description = model.Description;
        }

        public void SetInternalDictionaryAgentContactPerson(AddAgentContactPerson model)
        {
            AgentCompanyId = model.CompanyId;
            Position = model.Position;
            Name = model.Name;
            FirstName = model.FirstName;
            LastName = model.LastName;
            MiddleName = model.MiddleName;
            IsMale = model.IsMale;
            IsActive = model.IsActive;
            Description = model.Description;
        }

        public InternalDictionaryAgentPerson(InternalDictionaryAgentEmployee model)
        {
            Id = model.Id;
            Name = model.Name;
            LastChangeDate = model.LastChangeDate;
            LastChangeUserId = model.LastChangeUserId;
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
            //AgentCompanyId = model.AgentCompanyId;
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

        public string Name { get; set; }// { get { return LastName + " " + FirstName.Trim().Substring(1, 1) + "." + " " + MiddleName.Trim().Substring(1, 1) + "."; } }
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
        /// Id компании, контактным лицом которой является физическое лицо
        /// </summary>
        public int? AgentCompanyId { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Position { get; set; }

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
