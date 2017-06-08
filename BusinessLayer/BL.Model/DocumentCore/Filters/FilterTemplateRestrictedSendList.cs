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
    /// Фильтр ограничительных списков рассылки в шаблонах
    /// </summary>
    public class FilterTemplateRestrictedSendList: BaseFilter
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        /// <summary>
        /// Ссылка на должность
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Уровень доступа
        /// </summary>
        public EnumAccessLevels? AccessLevel { get; set; }
    }
}
