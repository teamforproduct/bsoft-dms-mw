﻿using System.Collections.Generic;

namespace BL.Model.AdminCore
{
    /// <summary>
    ///Фильтр ожиданий по документу
    /// </summary>
    public class FilterDocumentWaits
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
        /// <summary>
        /// ИД. инициирующего события
        /// </summary>
        public int? OnEventId { get; set; }
        /// <summary>
        /// ИД. закрывающего события. 
        /// </summary>
        public int? OffEventId { get; set; }

        /// <summary>
        ///Если истина, то фильтруются ожидания, где OffEvent = null
        /// </summary>
        public bool Opened { get; set; }
    }
}