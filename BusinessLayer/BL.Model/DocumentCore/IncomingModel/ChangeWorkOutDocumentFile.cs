using BL.Model.Extensions;
using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для изменения признака принятия файла
    /// </summary>
    public class ChangeAttributesDocumentFile
    {
        /// <summary>
        /// ИД файла
        /// </summary>
        [Required]
        public int FileId { get; set; }
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
        /// группы получателей
        /// </summary>
        public List<AccessGroup> TargetAccessGroups { get; set; }

    }
}
