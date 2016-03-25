using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentPaperEvents
    {
        public int Id { get; set; }
        public int PaperId { get; set; }
        public int EventTypeId { get; set; }
        public Nullable<int> EventId { get; set; }
        public string Description { get; set; }
        public Nullable<int> SourcePositionId { get; set; }
        public Nullable<int> SourcePositionExecutorAgentId { get; set; }
        public Nullable<int> SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetPositionExecutorAgentId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<int> PaperListId { get; set; }
        public int PlanAgentId { get; set; }
        public DateTime PlanDate { get; set; }
        public Nullable<int> SendAgentId { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<int> RecieveAgentId { get; set; }
        public Nullable<DateTime> RecieveDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PaperId")]
        public virtual DocumentPapers Paper { get; set; }
        [ForeignKey("PaperListId")]
        public virtual DocumentPaperLists PaperList { get; set; }
        [ForeignKey("EventTypeId")]
        public virtual DictionaryEventTypes EventType { get; set; }
        [ForeignKey("EventId")]
        public virtual DocumentEvents Event { get; set; }
        [ForeignKey("SourcePositionId")]
        public virtual DictionaryPositions SourcePosition { get; set; }
        [ForeignKey("SourcePositionExecutorAgentId")]
        public virtual DictionaryAgents SourcePositionExecutorAgent { get; set; }
        [ForeignKey("SourceAgentId")]
        public virtual DictionaryAgents SourceAgent { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetPositionExecutorAgentId")]
        public virtual DictionaryAgents TargetPositionExecutorAgent { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        [ForeignKey("PlanAgentId")]
        public virtual DictionaryAgents PlanAgent { get; set; }
        [ForeignKey("SendAgentId")]
        public virtual DictionaryAgents SendAgent { get; set; }
        [ForeignKey("RecieveAgentId")]
        public virtual DictionaryAgents RecieveAgent { get; set; }
    }
}
