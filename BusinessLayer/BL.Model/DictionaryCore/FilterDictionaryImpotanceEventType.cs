﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    ///  Фильтр словаря типов важности событий
    /// </summary>
    public class FilterDictionaryImpotanceEventType
    {
        /// <summary>
        /// Массив ИД типов важности событий
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
    }
}
