using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Ограничительный список рассылки для шаблона документа
    /// </summary>
    public class AddTemplateDocumentRestrictedSendLists
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Ссылка на должность
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Уровень доступа
        /// </summary>
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
