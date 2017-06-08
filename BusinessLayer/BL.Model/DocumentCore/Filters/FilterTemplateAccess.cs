using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using BL.Model.Common;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр доступов в шаблонах
    /// </summary>
    public class FilterTemplateAccess: BaseFilter
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        /// <summary>
        /// Ссылка на должность
        /// </summary>
        public int? PositionId { get; set; }
    }
}
