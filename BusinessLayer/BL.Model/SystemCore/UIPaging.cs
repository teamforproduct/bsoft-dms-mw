﻿using System.Xml.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        [XmlIgnore]
        public int TotalItemsCount { get; set; }
        /// <summary>
        /// Вернет только количество записей если = true
        /// </summary>
        public bool IsOnlyCounter { get; set; }
        public UICounters Counters { get; set; }
    }
}