using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOff
    {
        /// <summary>
        /// ИД события, породившего контроль
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// ИД типа результата
        /// </summary>
        public int? ResultTypeId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Каскадное закрытие контроля
        /// </summary>
        public bool IsCascade { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get { return _eventDate; } set { _eventDate = value.ToUTC(); } }
        private DateTime? _eventDate;
        public string ServerPath { get; set; }
    }
}
