using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель замены позиции в документе
    /// </summary>
    public class ChangePosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД старой должности
        /// </summary>
        [Required]
        public int OldPositionId { get; set; }
        /// <summary>
        /// ИД новой должности
        /// </summary>
        [Required]
        public int NewPositionId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
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
