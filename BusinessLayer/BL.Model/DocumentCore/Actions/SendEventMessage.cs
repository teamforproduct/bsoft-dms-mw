using System;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;

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
        public DateTime? EventDate { get { return _eventDate; } set { _eventDate = value.ToUTC(); } }
        private DateTime? _eventDate;

        /// <summary>
        /// Событие доступно в рамках задачи
        /// </summary>
        ///public bool IsAvailableWithinTask { get; set; }

    }
}
