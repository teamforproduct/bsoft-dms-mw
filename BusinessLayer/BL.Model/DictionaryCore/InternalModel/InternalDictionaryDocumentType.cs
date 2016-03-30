using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// типы документов
    /// </summary>
    public class InternalDictionaryDocumentType : LastChangeInfo
    {

        public InternalDictionaryDocumentType()
        { }

        public InternalDictionaryDocumentType(ModifyDictionaryDocumentType model)
        {
            Id = model.Id;
            Name = model.Name;
            IsActive = model.IsActive;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название типа документа. Отображается в документе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

    }
}