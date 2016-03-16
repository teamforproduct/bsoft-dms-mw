using System.Collections.Generic;
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
        /// Массив ИД документов для отображения ДИПов
        /// </summary>
        public List<int> DocumentsId { get; set; }
       
    }
}
