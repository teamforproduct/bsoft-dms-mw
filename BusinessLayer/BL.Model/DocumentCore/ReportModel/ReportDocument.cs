using BL.Model.DocumentCore.FrontModel;
using BL.Model.Reports.Interfaces;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocument : FrontRegistrationFullNumber, IReports
    {
        public ReportDocument()
        {
        }
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int Id { get; set; }

        public string DocumentTypeName { get; set; }

        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
        /// <summary>
        /// Кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Краткое содержание
        /// </summary>
        public string Description { get; set; }
        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }

        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public string RegistrationFullNumber { get; set; }

        public IEnumerable<ReportDocumentWait> DocumentWaits { get; set; }
        public IEnumerable<ReportDocumentSubscription> DocumentSubscriptions { get; set; }

        public IEnumerable<ReportDocumentEvent> DocumentEvents { get; set; }

    }
}