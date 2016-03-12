using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Модель внутренняя. Расширенная.
    /// </summary>
    public class InternalDictionaryDocumentSubject :  LastChangeInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название тематики документа. Отображается в документе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Ссылка на родителя.
        /// </summary>
        public int? ParentId { get; set; }
    }
}