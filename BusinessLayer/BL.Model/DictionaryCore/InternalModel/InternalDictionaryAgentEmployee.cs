﻿using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class InternalDictionaryAgentEmployee : LastChangeInfo
    {

        public InternalDictionaryAgentEmployee()
        { }

        public InternalDictionaryAgentEmployee(ModifyDictionaryAgentEmployee model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            MiddleName = model.MiddleName;
            TaxCode = model.TaxCode;
            IsMale = model.IsMale;
            PassportDate = model.PassportDate;
            PassportNumber = model.PassportNumber;
            PassportSerial = model.PassportSerial;
            PassportText = model.PassportText;
            Description = model.Description;
            if (model.BirthDate != null)
            { BirthDate = new DateTime(model.BirthDate?.Year ?? 0, model.BirthDate?.Month ?? 0, model.BirthDate?.Day ?? 0); ; }
            //BirthDate = model.BirthDate;
            IsActive = model.IsActive;


            #region [+] Employee ...
            PersonnelNumber = model.PersonnelNumber;
            #endregion

            #region [+] AgentUser ...
            LanguageId = model.LanguageId;
            UserId = model.UserId;
            #endregion

        }

        #region [+] Person ...
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

        public string Name { get { return LastName + " " + FirstName.Trim().Substring(1, 1) + "." + " " + MiddleName.Trim().Substring(1, 1) + "."; } }

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
        #endregion

        #region [+] Employee ...
        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
        #endregion

        #region [+] AgentUser ...
        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int? LanguageId { get; set; }
        
        /// <summary>
        /// Связь с WEB - USER
        /// </summary>
        public string UserId { get; set; }

        #endregion

    }
}
