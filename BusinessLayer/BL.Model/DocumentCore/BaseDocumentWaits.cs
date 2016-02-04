using System;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentWaits
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public BaseDocumentEvent OnEvent { get; set; }
        public BaseDocumentEvent OffEvent { get; set; }
    }
}
