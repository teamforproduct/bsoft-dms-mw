using BL.Model.DictionaryCore.IncomingModel;

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
            UserName = model.UserName;
            //Login = model.Login;
            //PasswordHash = model.PasswordHash;
            UserEmail = model.Login;
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
            IsMale = model.IsMale;
            BirthDate = model.BirthDate;

            LanguageId = model.LanguageId;

            TaxCode = model.TaxCode;
            PersonnelNumber = model.PersonnelNumber;

            IsActive = model.IsActive;
            Description = model.Description;
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
        /// WEB - USER  Login
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Имейл, на который высылается письмо с приглашением
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        public string Phone { get; set; }


        #endregion

    }
}
