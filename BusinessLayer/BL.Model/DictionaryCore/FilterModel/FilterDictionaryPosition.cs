using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря должностей
    /// </summary>
    public class FilterDictionaryPosition
    {
        /// <summary>
        /// Массив ИД должностей
        /// </summary>
        public List<int> PositionId { get; set; }
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
