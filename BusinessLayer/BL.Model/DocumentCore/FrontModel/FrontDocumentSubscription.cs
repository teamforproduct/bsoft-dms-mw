﻿namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSubscription
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendEventId { get; set; }
        public int? DoneEventId { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string ChangedHash { get; set; }
        public FrontDocumentEvent SendEvent { get; set; }
        public FrontDocumentEvent DoneEvent { get; set; }
    }
}
