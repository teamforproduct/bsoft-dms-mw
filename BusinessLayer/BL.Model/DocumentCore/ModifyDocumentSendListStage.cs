using System;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.Users;

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
        public int DocumentId { get; set; }
        /// <summary>
        /// Этап
        /// </summary>
        public int Stage { get; set; }
    }
}
