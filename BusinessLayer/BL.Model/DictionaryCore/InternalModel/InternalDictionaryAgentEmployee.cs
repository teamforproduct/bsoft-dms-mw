﻿using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class InternalDictionaryAgentEmployee : InternalDictionaryAgentPeople
    {

        public InternalDictionaryAgentEmployee()
        { }

        public InternalDictionaryAgentEmployee(AddAgentEmployeeUser model)
        {
            SetInternalDictionaryAgentEmployee(model);

            #region [+] AgentUser ...
            UserId = model.UserId;
            //Login = model.Login;
            //PasswordHash = model.PasswordHash;
            Login = model.Login;
            Phone = model.Phone;
            #endregion

        }

        public InternalDictionaryAgentEmployee(AddAgentEmployee model)
        {
            SetInternalDictionaryAgentEmployee(model);
        }

        public InternalDictionaryAgentEmployee(ModifyAgentEmployee model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentEmployee(model);
        }

        public InternalDictionaryAgentEmployee(ModifyAgentPeoplePassport model)
        {
            Id = model.Id;
            PassportDate = model.PassportDate;
            PassportNumber = model.PassportNumber;
            PassportSerial = model.PassportSerial;
            PassportText = model.PassportText;
        }

        private void SetInternalDictionaryAgentEmployee(AddAgentEmployee model)
        {
            Name = model.Name;
            FirstName = model.FirstName;
            LastName = model.LastName;
            MiddleName = model.MiddleName;
            TaxCode = model.TaxCode;
            IsMale = model.IsMale;
            Description = model.Description;
            BirthDate = model.BirthDate;
            IsActive = model.IsActive;
            LanguageId = model.LanguageId;


            #region [+] Employee ...
            PersonnelNumber = model.PersonnelNumber;
            #endregion

        }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// табельный номер
        /// </summary>
        public int PersonnelNumber { get; set; }

        #region [+] AgentUser ...
        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Связь с WEB - USER
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Основной имейл, на который высылается письмо с приглашением
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Основной номер мобильного телефона
        /// </summary>
        public string Phone { get; set; }


        #endregion

    }
}
