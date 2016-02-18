using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/изменения записи тегов документа
    /// </summary>
    public class ModifyDocumentTag
    {
        /// <summary>
        /// ИД записи тегов
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД тега
        /// </summary>
        [Required]
        public int TagId { get; set; }
    }
}
