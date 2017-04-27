using BL.Database.Common;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentEvents
    {
        public DocumentEvents()
        {
            this.OnWait = new HashSet<DocumentWaits>();
            this.OffWait = new HashSet<DocumentWaits>();
            this.SendSubscription = new HashSet<DocumentSubscriptions>();
            this.DoneSubscription = new HashSet<DocumentSubscriptions>();
            this.StartSendList = new HashSet<DocumentSendLists>();
            this.CloseSendList = new HashSet<DocumentSendLists>();
            this.Accesses = new HashSet<DocumentEventAccesses>();
            this.AccessGroups = new HashSet<DocumentEventAccessGroups>();
            this.Files = new HashSet<DocumentFiles>();
            this.ChildEvents = new HashSet<DocumentEvents>();
        }

        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        [Index("IX_ReadDate", 3)]
        [Index("IX_DocumentId",1)]
        public int DocumentId { get; set; }
        public int EventTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        [Index("IX_Date", 1)]
        public DateTime Date { get; set ; }
        [Index("IX_TaskId",1)]
        public Nullable<int> TaskId { get; set; }
        //public string TaskName { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string AddDescription { get; set; }


        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> PaperId { get; set; }
        public Nullable<int> SendListId { get; set; }
        public Nullable<int> ParentEventId { get; set; }
        public Nullable<int> PaperListId { get; set; }
        public Nullable<int> PaperPlanAgentId { get; set; }
        public Nullable<DateTime> PaperPlanDate { get; set; }
        public Nullable<int> PaperSendAgentId { get; set; }
        public Nullable<DateTime> PaperSendDate { get; set; }
        public Nullable<int> PaperRecieveAgentId { get; set; }
        public Nullable<DateTime> PaperRecieveDate { get; set; }


        public int LastChangeUserId { get; set; }
        [Index("IX_LastChangeDate",1)]
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("EventTypeId")]
        public virtual DictionaryEventTypes EventType { get; set; }
        [ForeignKey("TaskId")]
        public virtual DocumentTasks Task { get; set; }


        [ForeignKey("OnEventId")]
        public virtual ICollection<DocumentWaits> OnWait { get; set; }
        [ForeignKey("OffEventId")]
        public virtual ICollection<DocumentWaits> OffWait { get; set; }

        [ForeignKey("SendEventId")]
        public virtual ICollection<DocumentSubscriptions> SendSubscription { get; set; }
        [ForeignKey("DoneEventId")]
        public virtual ICollection<DocumentSubscriptions> DoneSubscription { get; set; }
        [ForeignKey("StartEventId")]
        public virtual ICollection<DocumentSendLists> StartSendList { get; set; }
        [ForeignKey("CloseEventId")]
        public virtual ICollection<DocumentSendLists> CloseSendList { get; set; }
        [ForeignKey("EventId")]
        public virtual ICollection<DocumentEventAccesses> Accesses { get; set; }
        [ForeignKey("EventId")]
        public virtual ICollection<DocumentEventAccessGroups> AccessGroups { get; set; }
        [ForeignKey("EventId")]
        public virtual ICollection<DocumentFiles> Files { get; set; }

        [ForeignKey("PaperId")]
        public virtual DocumentPapers Paper { get; set; }
        [ForeignKey("SendListId")]
        public virtual DocumentSendLists SendList { get; set; }
        [ForeignKey("ParentEventId")]
        public virtual DocumentEvents ParentEvent { get; set; }
        [ForeignKey("ParentEventId")]
        public virtual ICollection<DocumentEvents> ChildEvents { get; set; }
        [ForeignKey("PaperListId")]
        public virtual DocumentPaperLists PaperList { get; set; }
        [ForeignKey("PaperPlanAgentId")]
        public virtual DictionaryAgents PaperPlanAgent { get; set; }
        [ForeignKey("PaperSendAgentId")]
        public virtual DictionaryAgents PaperSendAgent { get; set; }
        [ForeignKey("PaperRecieveAgentId")]
        public virtual DictionaryAgents PaperRecieveAgent { get; set; }







        //TODO DELETE
        [Index("IX_ReadDate", 4)]
        [Index("IX_SourcePositionId", 1)]
        public int? SourcePositionId { get; set; }
        public Nullable<int> SourcePositionExecutorAgentId { get; set; }
        public int? SourcePositionExecutorTypeId { get; set; }
        public int? SourceAgentId { get; set; }
        [Index("IX_ReadDate", 2)]
        [Index("IX_TargetPositionId", 1)]
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetPositionExecutorAgentId { get; set; }
        public int? TargetPositionExecutorTypeId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        [Index("IX_ReadDate", 1)]
        public Nullable<DateTime> ReadDate { get; set; }
        public Nullable<int> ReadAgentId { get; set; }
        [ForeignKey("SourcePositionId")]
        public virtual DictionaryPositions SourcePosition { get; set; }
        [ForeignKey("SourcePositionExecutorAgentId")]
        public virtual DictionaryAgents SourcePositionExecutorAgent { get; set; }
        [ForeignKey("SourcePositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes SourcePositionExecutorType { get; set; }
        [ForeignKey("SourceAgentId")]
        public virtual DictionaryAgents SourceAgent { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetPositionExecutorAgentId")]
        public virtual DictionaryAgents TargetPositionExecutorAgent { get; set; }
        [ForeignKey("TargetPositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes TargetPositionExecutorType { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        [ForeignKey("ReadAgentId")]
        public virtual DictionaryAgents ReadAgent { get; set; }

    }
}
