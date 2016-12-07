using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Reports.Interfaces;
using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSubscription : LastChangeInfo, IReports
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendEventId { get; set; }
        public int? DoneEventId { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string FullHash { get; set; }
        public string ChangedHash { get; set; }
        public InternalDocumentEvent SendEvent { get; set; }
        public InternalDocumentEvent DoneEvent { get; set; }
        public EnumSubscriptionStates SubscriptionStates { get; set; }

        public EnumSigningTypes SigningType { get; set; }

        public string InternalSign { get; set; }
        public string CertificateSign { get; set; }
        public int? CertificateId { get; set; }
        public string CertificatePassword { get; set; }

        public int? CertificatePositionId { get; set; }
        public int? CertificatePositionExecutorAgentId { get; set; }
        public int? CertificatePositionExecutorTypeId { get; set; }

        #region For Report
        public string SubscriptionStatesName { get; set; }

        public string DoneEventSourcePositionName { get; set; }

        public string DoneEventSourcePositionExecutorAgentName { get; set; }
        #endregion
    }
}
