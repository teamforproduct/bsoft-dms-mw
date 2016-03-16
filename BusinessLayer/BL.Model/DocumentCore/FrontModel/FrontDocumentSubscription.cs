using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSubscription
    {
        public int Id { get; set; }
        public int? DocumentId { get; set; }
        public int? SendEventId { get; set; }
        public int? DoneEventId { get; set; }
        public int? SubscriptionStatesId { get; set; }
        public string SubscriptionStatesName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string ChangedHash { get; set; }
        public FrontDocumentEvent SendEvent { get; set; }
        public FrontDocumentEvent DoneEvent { get; set; }

        public string RegistrationFullNumber { get; set; }
        public DateTime? DocumentDate { get; set; }

        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentDescription { get; set; }
    }
}
