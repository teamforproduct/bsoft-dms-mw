using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPosition
    /// </summary>
    public class FilterDictionaryPosition : DictionaryBaseFilterParameters
    {
 
        /// <summary>
        /// Сужение по полному наименованию подразделений
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Массив ИД документов для поиска корреспондентов в событиях
        /// </summary>
        public List<int> DocumentIDs { get; set; }
        /// <summary>
        /// Массив ИД должностей для проверки субординации
        /// </summary>
        public List<int> SubordinatedPositions { get; set; }

        /// <summary>
        /// Тип субординации
        /// </summary>
        public int? SubordinatedTypeId { get; set; }

    }
}
