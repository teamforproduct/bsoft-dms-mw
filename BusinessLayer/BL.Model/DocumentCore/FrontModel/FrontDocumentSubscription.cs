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
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string ChangedHash { get; set; }
        public FrontDocumentEvent SendEvent { get; set; }
        public FrontDocumentEvent DoneEvent { get; set; }

    }
}
