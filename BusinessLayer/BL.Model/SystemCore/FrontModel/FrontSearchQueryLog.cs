using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore.FrontModel
{
    /// <summary>
    /// Модель для возврата лога предыдущих поисковых запросов
    /// </summary>
    public class FrontSearchQueryLog
    {
        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchQueryText { get; set; }
        /// <summary>
        /// Признак, собственной строки поиска
        /// </summary>
        public bool IsOwn { get; set; }
    }
}
