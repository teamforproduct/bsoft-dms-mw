using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для запроса на досылку связанных документов
    /// </summary>
    public class AdditinalLinkedDocumentSendList : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Массив должностей, для которых нужно рассчитать досылку
        /// </summary>
        public List<int> Positions { get; set; }
    }
}
