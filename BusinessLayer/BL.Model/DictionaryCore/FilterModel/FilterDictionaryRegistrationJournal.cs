using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryRegistrationJournal
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryRegistrationJournal : DictionaryBaseFilterParameters
    {
       
        /// <summary>
        /// Сужение по индексу журнала
        /// </summary>
        public string Index { get; set; }
        public string IndexExact { get; set; }
        public string NameExact { get; set; }
        /// <summary>
        /// Список подразделений
        /// </summary>
        public List<int> DepartmentIDs { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> DepartmentByPositionIDs { get; set; }

        /// <summary>
        /// Входящий от внешнего агента
        /// </summary>
        public bool? IsIncoming { get; set; }

        /// <summary>
        /// Исходящий - направлен внешнему агенту
        /// </summary>
        public bool? IsOutcoming { get; set; }

        /// <summary>
        ///  Внутренний - внутренняя переписка
        /// </summary>
        public bool? IsInternal { get; set; }

    }
}
