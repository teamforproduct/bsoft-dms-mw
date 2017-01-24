using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - Банк
    /// </summary>
    public class InternalDictionaryAgentBank : InternalDictionaryAgent
    {
        public InternalDictionaryAgentBank()
        { }

        public InternalDictionaryAgentBank(ModifyAgentBank model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentBank(model);
        }

        public InternalDictionaryAgentBank(AddAgentBank model)
        {
            SetInternalDictionaryAgentBank(model);
        }

        public void SetInternalDictionaryAgentBank(AddAgentBank model)
        {
            Name = model.Name;
            FullName = model.FullName;
            MFOCode = model.MFOCode;
            Swift = model.Swift;
            IsActive = model.IsActive;
            Description = model.Description;
        }

        /// <summary>
        /// Полное название
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// МФО
        /// </summary>
        public string MFOCode { get; set; }
        /// <summary>
        /// Код Свифт
        /// </summary>
        public string Swift { get; set; }
        /// <summary>
        /// Комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        
    }
}
