﻿using BL.Model.Enums;
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
        public List<int> IDs { get; set; }

        /// <summary>
        /// Массив ИД действий системы
        /// </summary>
        public List<int> NotContainsIDs { get; set; }
        /// <summary>
        /// Отрывок описания действий системы
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Массив ИД объектов системы
        /// </summary>
        public List<int> ObjectIDs { get; set; }

        /// <summary>
        /// Отмечнено
        /// </summary>
        //public bool? IsChecked { get; set; }

    }
}
