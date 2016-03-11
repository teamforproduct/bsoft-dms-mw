using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentWaits
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public string ResultTypeName { get; set; }
        
        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public FrontDocumentEvent OnEvent { get; set; }
        public FrontDocumentEvent OffEvent { get; set; }
    }
}
