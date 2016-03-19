using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр типов документов
    /// </summary>
    public class FilterDictionaryDocumentType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> DocumentTypeId { get; set; }
        /// <summary>
        /// наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
    }
}
