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
    public class ModifyDocumentFile
    {
        /// <summary>
        /// ИД файла
        /// </summary>
        [Required]
        public int FileId { get; set; }
        /// <summary>
        /// <summary>
        /// Тип файла дополнительный или основной (менять можно только для основной версии!).
        /// </summary>
        public EnumFileTypes? Type { get; set; }
        /// <summary>
        /// Описание файла
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
