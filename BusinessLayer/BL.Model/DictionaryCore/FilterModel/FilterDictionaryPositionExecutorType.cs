using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPositionExecutorType
    /// </summary>
    public class FilterDictionaryPositionExecutorType : BaseFilterNameIsActive
    {

        /// <summary>
        /// Сужение по коду
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Оставляет только те типы, которые можно применить к текущей должности за указанный период
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Оставляет только те типы, которые можно применить к текущей должности за указанный период
        /// </summary>
        public Period Period { get; set; }

    }
}
