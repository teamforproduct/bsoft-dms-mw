﻿using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentTasks : CurrentPosition
    {
        /// <summary>
        /// ИД Task
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}
