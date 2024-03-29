﻿using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;
using BL.Model.DocumentCore.IncomingModel;

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
        public EnumAccessLevels AccessLevel { get; set; }

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get { return _eventDate; } set { _eventDate = value.ToUTC(); } }
        private DateTime? _eventDate;

        /// <summary>
        /// Массив событий по перемещению бумажных носителей
        /// </summary>
        public List<PaperEvent> PaperEvents { get; set; }
        /// <summary>
        /// группы получателей копии
        /// </summary>
        public List<AccessGroup> TargetCopyAccessGroups { get; set; }
    }
}
