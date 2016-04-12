using BL.Model.Reports.Interfaces;
using System;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocumentWait: IReports
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public DateTime CreateDate { get; set; }
        public string TargetPositionName { get; set; }
        public string SourcePositionName { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsClosed { get; set; }
        public string ResultTypeName { get; set; }
        public ReportDocument Document { get; set; }
    }
}
