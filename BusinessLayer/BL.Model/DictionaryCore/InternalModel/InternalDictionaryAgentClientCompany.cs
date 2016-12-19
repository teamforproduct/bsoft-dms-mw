using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Компании"
    /// </summary>
    public class InternalDictionaryAgentClientCompany : LastChangeInfo
    {

        public InternalDictionaryAgentClientCompany()
        { }

        public InternalDictionaryAgentClientCompany(AddAgentClientCompany model)
        {
            SetInternalDictionaryAgentClientCompany(model);
        }

        public InternalDictionaryAgentClientCompany(ModifyAgentClientCompany model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentClientCompany(model);
        }

        private void SetInternalDictionaryAgentClientCompany(AddAgentClientCompany model)
        {
            IsActive = model.IsActive;
            Name = model.Name;
            FullName = model.FullName;
            Description = model.Description;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        public string Name { get; set; }

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