using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контакт контрагента
    /// </summary>
    public class InternalDictionaryContact : LastChangeInfo
    {

        public InternalDictionaryContact()
        { }

        public InternalDictionaryContact(ModifyDictionaryContact model)
        {
            Id = model.Id;
            AgentId = model.AgentId;
            ContactTypeId = model.ContactTypeId;
            Value = model.Value;
            IsActive = model.IsActive;
            IsConfirmed = model.IsConfirmed;
            Description = model.Description;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID агента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// Тип контакта
        /// </summary>
        public int ContactTypeId { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Признак подтверждения
        /// </summary>
        public bool IsConfirmed { get; set; }
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
    }
}
