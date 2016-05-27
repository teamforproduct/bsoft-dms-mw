using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Компании"
    /// </summary>
    public class InternalDictionaryCompany : LastChangeInfo
    {

        public InternalDictionaryCompany()
        { }

        public InternalDictionaryCompany(ModifyDictionaryCompany model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            Name = model.Name;
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
    }
}