using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра объектов системы
    /// </summary>
    public class FilterSystemObject
    {
        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> NotContainsIDs { get; set; }

        /// <summary>
        /// Отрывок описания объектов системы
        /// </summary>
        public string Description { get; set; }

    }
}
