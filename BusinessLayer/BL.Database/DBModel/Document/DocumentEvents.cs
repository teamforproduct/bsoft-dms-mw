using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentEvents
    {
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
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("EventTypeId")]
        public virtual DictionaryEventTypes EventType { get; set; }
        [ForeignKey("SourcePositionId")]
        public virtual DictionaryPositions SourcePosition { get; set; }
        [ForeignKey("SourceAgentId")]
        public virtual DictionaryAgents SourceAgent { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        //public virtual ICollection<DocumentWaits> OnWaits { get; set; }
        //public virtual ICollection<DocumentWaits> OffWaits { get; set; }
        //public virtual ICollection<DocumentSubscriptions> SendSubscriptions { get; set; }
        //public virtual ICollection<DocumentSubscriptions> DoneSubscriptions { get; set; }
        public virtual ICollection<DocumentSendLists> SendLists { get; set; }
    }
}
