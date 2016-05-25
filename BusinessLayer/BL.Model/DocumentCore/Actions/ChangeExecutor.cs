using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.Users;
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
        public string Description { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumDocumentAccesses AccessLevel { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get; set; }
        /// <summary>
        /// Массив событий по перемещению бумажных носителей
        /// </summary>
        public List<PaperEvent> PaperEvents { get; set; }
    }
}
