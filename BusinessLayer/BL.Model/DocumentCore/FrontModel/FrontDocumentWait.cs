using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentWait : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public string ResultTypeName { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public string TargetDescription { get; set; }
        //public DateTime? TargetAttentionDate { get; set; }
        public FrontDocumentEvent OnEvent { get; set; }
        public FrontDocumentEvent OffEvent { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOverDue { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public int? RecordCount { get; set; }



    }
}
