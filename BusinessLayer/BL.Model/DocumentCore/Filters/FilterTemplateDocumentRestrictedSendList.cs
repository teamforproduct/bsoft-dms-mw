using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр ограничительных списков рассылки в шаблонах
    /// </summary>
    public class FilterTemplateDocumentRestrictedSendList
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> IDs { get; set; }
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
        public EnumDocumentAccesses? AccessLevel { get; set; }
    }
}
