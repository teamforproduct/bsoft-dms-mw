using BL.Model.Enums;
using BL.Model.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для передачи управления над документом
    /// </summary>
    public class ChangeExecutor
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности кому передается управление
        /// </summary>
        [Required]
        public int PositionId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumDocumentAccess AccessLevel { get; set; }
    }
}
