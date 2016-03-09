using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAddressType : LastChangeInfo
    {
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