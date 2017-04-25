using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель изменения параметров контроля
    /// </summary>
    public class ControlChange : SendEventMessage
    {
        /// <summary>
        /// Контрольный срок
        /// </summary>
        public DateTime? DueDate { get { return _dueDate; } set { _dueDate = value.ToUTC(); } }
        private DateTime? _dueDate;
        /// <summary>
        /// Дата включения режима постоянное внимание
        /// </summary>
        public DateTime? AttentionDate { get; set; }

    }
}
