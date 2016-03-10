using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Тематики документов"
    /// </summary>
    public class ModifyDictionaryDocumentSubject
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        
        /// <summary>
        /// Признак активности.
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// Ссылка на родителя (Id).
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Название тематики документа.
        /// </summary>
        public string Name { get; set; }
        
       
    }
}
