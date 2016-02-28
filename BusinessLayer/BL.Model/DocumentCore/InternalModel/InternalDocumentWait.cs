using System;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentWait: LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public InternalDocumentEvent OnEvent { get; set; }
        public InternalDocumentEvent OffEvent { get; set; }
    }
}
