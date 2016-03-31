using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - Банк
    /// </summary>
    public class InternalDictionaryAgentBank : LastChangeInfo
    {
        public InternalDictionaryAgentBank()
        { }

        public InternalDictionaryAgentBank(ModifyDictionaryAgentBank model)
        {
            Id = model.Id;
            MFOCode = model.MFOCode;
            Swift = model.Swift;
            Name = model.Name;
            IsActive = model.IsActive;
            Description = model.Description;
        }

        /// <summary>
        /// Ид
        /// </summary>
        public int Id { get; set; }
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
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
    }
}
