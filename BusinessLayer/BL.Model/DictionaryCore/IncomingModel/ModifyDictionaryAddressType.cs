using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class ModifyDictionaryAddressType
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Название типа документа. Отображается в документе
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public int IsActive { get; set; }
    }
}
