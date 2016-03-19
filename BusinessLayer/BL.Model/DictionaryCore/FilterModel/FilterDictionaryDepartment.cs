using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDepartment
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryDepartment
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
        /// Сужение по списку вышестоящих подразделений
        /// </summary>
        public List<int> ParentIDs { get; set; }

        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Сужение по наименованию подразделений
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по полному наименованию подразделений
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Индекс подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по компании, которая представляет этот отдел
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public int? ChiefPositionId { get; set; }
        
    }
}
