using System;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для модификации плана работы над документом
    /// </summary>
    public class ModifyDocumentSendListStage
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Этап
        /// </summary>
        [Required]
        public int Stage { get; set; }
    }
}
