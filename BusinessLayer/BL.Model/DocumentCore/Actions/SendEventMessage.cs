using System;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;
using BL.Model.DocumentCore.IncomingModel;
using System.Collections.Generic;

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
        public string ServerPath { get; set; }
        /// <summary>
        /// группы получателей копии
        /// </summary>
        public List<AccessGroup> TargetCopyAccessGroups { get; set; }
        /// <summary>
        /// Массив файлов
        /// </summary>
        public List<AddDocumentFile> AddDocumentFiles { get; set; }

    }
}
