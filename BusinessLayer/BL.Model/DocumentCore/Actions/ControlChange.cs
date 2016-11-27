using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель изменения параметров контроля
    /// </summary>
    public class ControlChange 
    {
        /// <summary>
        /// ИД события, породившего контроль
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Контрольный срок
        /// </summary>
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set { _dueDate = value.HasValue ? value.Value.ToUniversalTime() : value; }
        }
        private DateTime? _dueDate { get; set; }
        /// <summary>
        /// Дата включения режима постоянное внимание
        /// </summary>
        public DateTime? AttentionDate { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate
        {
            get { return _eventDate; }
            set { _eventDate = value.HasValue ? value.Value.ToUniversalTime() : value; }
        }
        private DateTime? _eventDate { get; set; }
    }
}
