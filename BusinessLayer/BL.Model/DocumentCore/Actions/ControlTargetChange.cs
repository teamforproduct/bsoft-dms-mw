using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель изменения параметров контроля для исполнителя
    /// </summary>
    public class ControlTargetChange
    {
        /// <summary>
        /// ИД события, породившего контроль
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Комментарий исполнителя
        /// </summary>
        public string TargetDescription { get; set; }
        /// <summary>
        /// Дата включения режима постоянное внимание исполнителя
        /// </summary>
        public DateTime? TargetAttentionDate { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get { return _eventDate; } set { _eventDate = value.ToUTC(); } }
        private DateTime? _eventDate;
        /// <summary>
        /// группы получателей копии
        /// </summary>
        public List<AccessGroup> TargetCopyAccessGroups { get; set; }
    }
}
