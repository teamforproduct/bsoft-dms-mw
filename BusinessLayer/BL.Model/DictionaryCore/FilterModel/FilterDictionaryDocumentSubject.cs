using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDocumentSubject
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryDocumentSubject : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Наименование (равенство)
        /// </summary>
        public string NameExact { set; get; }

        /// <summary>
        /// Сужение по списку вышестоящих элементов
        /// </summary>
        public List<int> ParentIDs { get; set; }

    }
}
