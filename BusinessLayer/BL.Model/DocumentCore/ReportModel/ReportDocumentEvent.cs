using System;
using BL.Model.Enums;
using BL.Model.Reports.Interfaces;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocumentEvent : IReports
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }

        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }

        public int? PaperId { get; set; }
        public ReportDocumentPaper Paper { get; set; }
    }
}
