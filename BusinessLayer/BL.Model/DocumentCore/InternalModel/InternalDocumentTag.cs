﻿using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentTag : LastChangeInfo
    {
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Список ИД тегов
        /// </summary>
        public List<int> Tags { get; set; }
        public int TagId { get; set; }
    }
}
