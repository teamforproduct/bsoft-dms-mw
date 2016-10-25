using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Модель фильтра действий системы
    /// </summary>
    public class FilterSystemAction
    {
        /// <summary>
        /// Массив ИД действий системы
        /// </summary>
        public List<int> ActionId { get; set; }
        /// <summary>
        /// Отрывок описания действий системы
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> ObjectId { get; set; }
    }
}
