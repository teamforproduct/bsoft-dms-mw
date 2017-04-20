using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Ограничительный список рассылки для шаблона документа
    /// </summary>
    public class AddTemplateDocumentRestrictedSendList
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Ссылка на должность
        /// </summary>
        [Required]
        public int PositionId { get; set; }
        /// <summary>
        /// Уровень доступа
        /// </summary>
        [Required]
        public EnumAccessLevels AccessLevel { get; set; }
    }
}
