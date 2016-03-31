using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSubscription : LastChangeInfo
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
    }
}
