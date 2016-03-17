using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int Stage { get; set; }
        public int SendTypeId { get; set; }
        public int SourcePositionId { get; set; }
        public int SourcePositionExecutorAgentId { get; set; }
        public int SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetPositionExecutorAgentId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int? DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInitial { get; set; }
        public bool IsAddControl { get; set; }
        public bool IsAvailableWithinTask { get; set; }
        public Nullable<int> StartEventId { get; set; }
        public Nullable<int> CloseEventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("SendTypeId")]
        public virtual DictionarySendTypes SendType { get; set; }
        [ForeignKey("TaskId")]
        public virtual DocumentTasks TaskD { get; set; }
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
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
        [ForeignKey("StartEventId")]
        public virtual DocumentEvents StartEvent { get; set; }
        [ForeignKey("CloseEventId")]
        public virtual DocumentEvents CloseEvent { get; set; }
    }
}
