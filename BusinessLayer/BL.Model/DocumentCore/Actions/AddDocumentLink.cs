﻿using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для связывания документов
    /// </summary>
    public class AddDocumentLink : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД родительского документа
        /// </summary>
        public int ParentDocumentId { get; set; }
        /// <summary>
        /// ИД типа связи
        /// </summary>
        public int LinkTypeId { get; set; }       

    }
}
