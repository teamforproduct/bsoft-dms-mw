using System;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для окончания/возобновления работы с документом
    /// </summary>
    public class ChangeWorkStatus : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Комментарий к действию
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
