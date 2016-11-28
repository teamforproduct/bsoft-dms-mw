using System;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для посылки сообщений через события
    /// </summary>
    public class SendEventMessage
    {
        /// <summary>
        /// ИД события
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Примечание
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
        /// <summary>
        /// Событие доступно в рамках задачи
        /// </summary>
        ///public bool IsAvailableWithinTask { get; set; }

    }
}
