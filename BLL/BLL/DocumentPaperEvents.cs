//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BLL.BLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentPaperEvents
    {
        public int Id { get; set; }
        public int PaperId { get; set; }
        public Nullable<int> SendListId { get; set; }
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
        public Nullable<int> PlanAgentId { get; set; }
        public Nullable<System.DateTime> PlanDate { get; set; }
        public Nullable<int> SendAgentId { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<int> RecieveAgentId { get; set; }
        public Nullable<System.DateTime> RecieveDate { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    
        public virtual DocumentPapers Paper { get; set; }
        public virtual DocumentPaperLists PaperList { get; set; }
        public virtual DictionaryEventTypes EventType { get; set; }
        public virtual DocumentEvents Event { get; set; }
        public virtual DictionaryPositions SourcePosition { get; set; }
        public virtual DictionaryAgents SourcePositionExecutorAgent { get; set; }
        public virtual DictionaryAgents SourceAgent { get; set; }
        public virtual DictionaryPositions TargetPosition { get; set; }
        public virtual DictionaryAgents TargetPositionExecutorAgent { get; set; }
        public virtual DictionaryAgents TargetAgent { get; set; }
        public virtual DictionaryAgents PlanAgent { get; set; }
        public virtual DictionaryAgents SendAgent { get; set; }
        public virtual DictionaryAgents RecieveAgent { get; set; }
        public virtual DocumentSendLists SendList { get; set; }
    }
}