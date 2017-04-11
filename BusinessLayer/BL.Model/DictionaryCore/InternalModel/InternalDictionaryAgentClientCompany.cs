using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Компании"
    /// </summary>
    public class InternalDictionaryAgentOrg : InternalDictionaryAgent
    {

        public InternalDictionaryAgentOrg()
        { }

        public InternalDictionaryAgentOrg(AddOrg model)
        {
            SetInternalDictionaryAgentClientCompany(model);
        }

        public InternalDictionaryAgentOrg(ModifyOrg model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentClientCompany(model);
        }

        private void SetInternalDictionaryAgentClientCompany(AddOrg model)
        {
            IsActive = model.IsActive;
            Name = model.Name;
            FullName = model.FullName;
            Description = model.Description;
        }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }
    }
}