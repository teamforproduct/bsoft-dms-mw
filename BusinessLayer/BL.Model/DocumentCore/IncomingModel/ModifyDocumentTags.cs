using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentTags
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Список ИД тегов
        /// </summary>
        [Required]
        public List<int> Tags { get; set; }
    }
}
