using System.Collections.Generic;
using System.Dynamic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Базовые фильтры для админки
    /// </summary>
    public class AdminBaseFilterParameters
    {
        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Исключение записей по ID
        /// </summary>
        public List<int> NotContainsIDs { get; set; }
   
    }
}
