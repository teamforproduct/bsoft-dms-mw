using BL.Model.Enums;
using BL.Model.Extensions;
using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа
    /// </summary>
    public class RenameDocumentFile
    {
        /// <summary>
        /// ИД файла
        /// </summary>
        [Required]
        public int FileId { get; set; }
         /// <summary>
        /// Название файла
        /// Только для изменения имени файла
        /// </summary>
        public string FileName { get; set; }
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
