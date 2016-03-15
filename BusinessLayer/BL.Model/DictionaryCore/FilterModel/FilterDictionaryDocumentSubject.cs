using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDocumentSubject
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryDocumentSubject
    {
        public List<int> DocumentSubjectId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Сужение по активности элементов. Если IsActive == Null, то в запросе не учавствует
        /// </summary>
        public bool? IsActive { get; set; }

        public int? ParentId { get; set; }
    }
}
