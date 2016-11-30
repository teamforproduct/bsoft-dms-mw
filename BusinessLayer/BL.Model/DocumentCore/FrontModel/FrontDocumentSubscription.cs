using BL.Model.Enums;
using BL.Model.Extensions;
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSubscription : FrontDocumentInfo
    {
        public int Id { get; set; }
        public int? SendEventId { get; set; }
        public int? DoneEventId { get; set; }
        public int? SubscriptionStatesId { get; set; }
        public string SubscriptionStatesName { get; set; }
        public bool? IsSuccess { get; set; }
		
        private DateTime?  _DueDate; 
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate=value.ToUTC(); } }
		
        public string Description { get; set; }
        public string Hash { get; set; }
        public string ChangedHash { get; set; }
        public FrontDocumentEvent SendEvent { get; set; }
        public FrontDocumentEvent DoneEvent { get; set; }

        public EnumSigningTypes SigningType { get; set; }

        public int? CertificateId { get; set; }
        public string CertificateName { get; set; }

        public int? CertificatePositionId { get; set; }
        public int? CertificatePositionExecutorAgentId { get; set; }
        public string CertificatePositionName { get; set; }
        public string CertificatePositionExecutorAgentName { get; set; }

        private DateTime?  _CertificateSignCreateDate; 
        public DateTime? CertificateSignCreateDate { get { return _CertificateSignCreateDate; } set { _CertificateSignCreateDate=value.ToUTC(); } }

    }
}
