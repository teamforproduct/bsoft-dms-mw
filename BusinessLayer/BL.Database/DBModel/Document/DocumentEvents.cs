using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentEvents
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentEvents()
        {
            //this.OnWaits = new HashSet<DocumentWaits>();
            //this.OffWaits = new HashSet<DocumentWaits>();
            //this.SendSubscriptions = new HashSet<DocumentSubscriptions>();
            //this.DoneSubscriptions = new HashSet<DocumentSubscriptions>();
            this.SendLists = new HashSet<DocumentSendLists>();
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int EventTypeId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime Date { get; set; }
        public string Description { get; set; }
        public int SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
        public int DocumentSubscriptionsId { get; set; }

        public virtual Documents Document { get; set; }
        public virtual DictionaryEventTypes EventType { get; set; }
        public virtual DictionaryPositions SourcePosition { get; set; }
        public virtual DictionaryAgents SourceAgent { get; set; }
        public virtual DictionaryPositions TargetPosition { get; set; }
        public virtual DictionaryAgents TargetAgent { get; set; }
        //public virtual ICollection<DocumentWaits> OnWaits { get; set; }
        //public virtual ICollection<DocumentWaits> OffWaits { get; set; }
        //public virtual ICollection<DocumentSubscriptions> SendSubscriptions { get; set; }
        //public virtual ICollection<DocumentSubscriptions> DoneSubscriptions { get; set; }
        public virtual ICollection<DocumentSendLists> SendLists { get; set; }
    }
}
