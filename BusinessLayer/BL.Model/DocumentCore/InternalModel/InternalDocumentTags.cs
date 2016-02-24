using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentTags : LastChangeInfo
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Список ИД тегов
        /// </summary>
        public List<int> Tags { get; set; }
    }
}
