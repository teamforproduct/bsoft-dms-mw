using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр документа
    /// </summary>
    public class FilterDocumentById
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [IgnoreDataMember]
        public int DocumentId { get; set; }
        /// <summary>
        /// Возвращать ли связанные документы
        /// </summary>
        public bool IsShowInfoByLinkDocument { get; set; }
       
    }
}
