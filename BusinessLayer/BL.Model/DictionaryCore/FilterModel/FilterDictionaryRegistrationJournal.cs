using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryRegistrationJournal
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryRegistrationJournal
    {
        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Исключение записей по ID
        /// </summary>
        public List<int> NotContainsIDs { get; set; }

        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Сужение по названию журнала
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по индексу журнала
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Список подразделений
        /// </summary>
        public List<int> DepartmentIDs { get; set; }

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
