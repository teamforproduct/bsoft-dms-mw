using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр задач по документу
    /// </summary>
    public class FilterDocumentSavedFilter
    {
        /// <summary>
        /// Признак показывать ли фильтры, доступные только текущему пользователю
        /// </summary>
        public bool IsOnlyCurrentUser { get; set; }
    }
}
