﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentPaperEvent
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Description { get; set; }
        //TODO Пересмотреть
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
        public int? LastPaperEventId { get; set; }
    }
}