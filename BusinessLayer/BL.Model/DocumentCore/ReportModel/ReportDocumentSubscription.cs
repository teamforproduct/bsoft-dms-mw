using BL.Model.Reports.Interfaces;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocumentSubscription : IReports
    {
        public ReportDocumentSubscription()
        {
        }
        public int Id { get; set; }
        public int DocumentId { get; set; }

        public int? SubscriptionStatesId { get; set; }
        public string SubscriptionStatesName { get; set; }

        public string DoneEventSourcePositionName { get; set; }
        public string DoneEventSourcePositionExecutorAgentName { get; set; }

    }
}