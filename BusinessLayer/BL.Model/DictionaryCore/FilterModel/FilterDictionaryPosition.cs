using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPosition
    /// </summary>
    public class FilterDictionaryPosition
    {
        /// <summary>
        /// Массив ИД должностей
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
        /// Массив ИД документов для поиска корреспондентов в событиях
        /// </summary>
        public List<int> DocumentId { get; set; }
        /// <summary>
        /// Массив ИД должностей для проверки субординации
        /// </summary>
        public List<int> SubordinatedPositions { get; set; }

    }
}
