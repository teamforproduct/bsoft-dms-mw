using System.Collections.Generic;

namespace BL.Model.Common
{
    public class BaseFilter
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
}
