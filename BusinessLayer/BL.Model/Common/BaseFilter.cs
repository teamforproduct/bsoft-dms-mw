﻿using System.Collections.Generic;

namespace BL.Model.Common
{
    public class BaseFilter : IBaseFilter
    {
        /// <summary>
        /// Сужение по Id
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Исключение по Id (кроме)
        /// </summary>
        public List<int> NotContainsIDs { get; set; }
    }

    public interface IBaseFilter
    {
        /// <summary>
        /// Сужение по Id
        /// </summary>
        List<int> IDs { get; set; }

        /// <summary>
        /// Исключение по Id (кроме)
        /// </summary>
        List<int> NotContainsIDs { get; set; }
    }
}
