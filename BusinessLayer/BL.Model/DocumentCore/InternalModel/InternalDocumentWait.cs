using System;
using BL.Model.Common;
using BL.Model.Reports.Interfaces;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentWait : LastChangeInfo, IReports
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? ParentId { get; set; }
        public int? ParentOnEventId { get; set; }
        public int? ParentOffEventId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PlanDueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
        public string TargetDescription { get; set; }
        public bool IsHasMarkExecution { get; set; }
        public bool IsHasAskPostponeDueDate { get; set; }
        public InternalDocumentEvent OnEvent { get; set; }
        public InternalDocumentEvent OffEvent { get; set; }
        public InternalDocumentWait ParentWait { get; set; }
        public InternalDocumentWait AskPostponeDueDateWait { get; set; }

        #region For Report
        public DateTime CreateDate { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public bool IsClosed { get; set; }
        public string ResultTypeName { get; set; }
        public string OnEventTypeName { get; set; }
        public DateTime? OffEventDate { get; set; }
        #endregion

    }
}
