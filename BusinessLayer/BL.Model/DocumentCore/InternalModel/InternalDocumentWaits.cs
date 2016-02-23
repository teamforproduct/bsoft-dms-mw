using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentWaits
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public string Task { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public DateTime LastChangeDate { get; set; }
        public InternalDocumentEvents OnEvent { get; set; }
        public InternalDocumentEvents OffEvent { get; set; }
    }
}
