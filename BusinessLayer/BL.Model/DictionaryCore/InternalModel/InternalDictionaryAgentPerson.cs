using System;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class InternalDictionaryAgentPerson : InternalDictionaryAgentPeople
    {
        /// <summary>
        /// 
        /// </summary>
        public InternalDictionaryAgentPerson()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public InternalDictionaryAgentPerson(AddAgentPerson model)
        {
            SetInternalDictionaryAgentPerson(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public InternalDictionaryAgentPerson(ModifyAgentPerson model)
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
            
            BirthDate = model.BirthDate;
            IsMale = model.IsMale;
            PassportSerial = model.PassportSerial;
            PassportNumber = model.PassportNumber;
            PassportText = model.PassportText;
            PassportDate = model.PassportDate;
            TaxCode = model.TaxCode;

            Position = model.Position;
            AgentCompanyId = model.AgentCompanyId;
            IsActive = model.IsActive;
            Description = model.Description;
        }


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
