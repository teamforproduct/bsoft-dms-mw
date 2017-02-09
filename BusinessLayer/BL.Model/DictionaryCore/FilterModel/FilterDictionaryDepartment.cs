using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDepartment
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryDepartment : DictionaryBaseFilterParameters
    {

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
        public List<int> CompanyIDs { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public int? ChiefPositionId { get; set; }

        /// <summary>
        /// Сужение по списку вышестоящих элементов
        /// </summary>
        public List<int> ParentIDs { get; set; }

        /// <summary>
        /// исключить отделы без журналов
        /// </summary>
        public bool? ExcludeDepartmentsWithoutJournals { get; set; }

        /// <summary>
        /// исключить отделы без должностей
        /// </summary>
        public bool? ExcludeDepartmentsWithoutPositions { get; set; }

    }
}
