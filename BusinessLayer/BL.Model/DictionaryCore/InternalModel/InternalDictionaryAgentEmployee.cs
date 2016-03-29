using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class InternalDictionaryAgentEmployee : InternalDictionaryAgentPerson
    {

        public InternalDictionaryAgentEmployee()
        { }

        public InternalDictionaryAgentEmployee(ModifyDictionaryAgentEmployee model)
        {
            Id = model.Id;
            PersonnelNumber = model.PersonnelNumber;
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
            BirthDate = model.BirthDate;
            IsActive = model.IsActive;
        }

        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
    }
}
