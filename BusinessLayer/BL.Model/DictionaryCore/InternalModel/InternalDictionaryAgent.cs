using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент
    /// </summary>
    public class InternalDictionaryAgent : LastChangeInfo
    {
        
        public InternalDictionaryAgent()
        { }

        public InternalDictionaryAgent(ModifyDictionaryAgent model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = string.Empty;// model.Description;
            IsActive = true; //model.IsActive;
            ResidentTypeId = model.ResidentTypeId ?? null;
        }

        public InternalDictionaryAgent(InternalDictionaryAgentPerson model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            IsActive = model.IsActive;
            LastChangeUserId = model.LastChangeUserId;
            LastChangeDate = model.LastChangeDate;
        }

        public InternalDictionaryAgent(InternalDictionaryAgentClientCompany model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            IsActive = model.IsActive;
            LastChangeUserId = model.LastChangeUserId;
            LastChangeDate = model.LastChangeDate;
        }

        public InternalDictionaryAgent(InternalDictionaryAgentCompany model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            IsActive = model.IsActive;
            LastChangeUserId = model.LastChangeUserId;
            LastChangeDate = model.LastChangeDate;
        }
        public InternalDictionaryAgent(InternalDictionaryAgentBank model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            IsActive = model.IsActive;
            LastChangeUserId = model.LastChangeUserId;
            LastChangeDate = model.LastChangeDate;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя/наименование
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// резидентность
        /// </summary>
        public int? ResidentTypeId { get; set; }
        //pss Уточниить ResidentTypeId это точно реквизит базовой сущности или выносок
    }
}
