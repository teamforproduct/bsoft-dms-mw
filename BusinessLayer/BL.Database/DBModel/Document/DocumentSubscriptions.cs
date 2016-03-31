using System;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Document
{
    public class DocumentSubscriptions
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendEventId { get; set; }
        public Nullable<int> DoneEventId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public Nullable<int> SubscriptionStateId { get; set; }
        [MaxLength(2000)]
        public string Hash { get; set; }
        [MaxLength(2000)]
        public string FullHash { get; set; }
        [MaxLength(2000)]
        public string ChangedHash { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("SendEventId")]
        public virtual DocumentEvents SendEvent { get; set; }
        [ForeignKey("DoneEventId")]
        public virtual DocumentEvents DoneEvent { get; set; }
        [ForeignKey("SubscriptionStateId")]
        public virtual DictionarySubscriptionStates SubscriptionState { get; set; }
    }
}
