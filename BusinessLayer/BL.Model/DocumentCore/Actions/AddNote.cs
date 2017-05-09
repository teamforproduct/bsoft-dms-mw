using System;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;
using System.Collections.Generic;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для добавления примечания
    /// </summary>
    public class AddNote : CurrentPosition
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get { return _eventDate; } set { _eventDate = value.ToUTC(); } }
        private DateTime? _eventDate;
        /// <summary>
        /// ИД родительского события, если нужен ответ
        /// </summary>
        public int? ParentEventId { get; set; }
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
