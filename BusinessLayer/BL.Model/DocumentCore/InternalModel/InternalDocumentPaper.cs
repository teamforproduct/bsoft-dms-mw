﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentPaper : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
        public int? LastPaperEventId { get; set; }
        public int? NextPaperEventId { get; set; }
        public int? PreLastPaperEventId { get; set; }
        public bool IsInWork { get; set; }
        public InternalDocumentPaperEvent LastPaperEvent { get; set; }
        //public InternalDocumentPaperEvent NextPaperEvent { get; set; }
        public IEnumerable<InternalDocumentPaperEvent> Events { get; set; }
    }
}
