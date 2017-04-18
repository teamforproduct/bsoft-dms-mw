using System;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public int? Stage { get; set; }
        public EnumStageTypes? StageType { get; set; }
        public EnumSendTypes SendType { get; set; }

        public int? SourcePositionId { get; set; }
        public int? SourcePositionExecutorAgentId { get; set; }
        public int? SourcePositionExecutorTypeId { get; set; }
        public int SourceAgentId { get; set; }

        public int? TargetPositionId { get; set; }
        public int? TargetPositionExecutorAgentId { get; set; }
        public int? TargetPositionExecutorTypeId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? TaskId { get; set; }
        public string TaskName { get; set; }
        public bool IsWorkGroup { get; set; }
        public bool IsAddControl { get; set; }
        public string SelfDescription { get; set; }
        public Nullable<DateTime> SelfDueDate { get; set; }
        public int? SelfDueDay { get; set; }
        public Nullable<DateTime> SelfAttentionDate { get; set; }
        public int? SelfAttentionDay { get; set; }
        public string Description { get; set; }
        public string AddDescription { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public bool IsInitial { get; set; }
        public int? StartEventId { get; set; }
        public int? CloseEventId { get; set; }
        public InternalDocumentEvent StartEvent { get; set; }
        public InternalDocumentEvent CloseEvent { get; set; }
        public InternalDocumentTask Task { get; set; }
        public List<InternalDocumentEvent> PaperEvents { get; set; }
        public InternalDictionaryAgent TargetAgent { get; set; }

        public string InitiatorPositionName { get; set; }
        public string InitiatorPositionExecutorAgentName { get; set; }
        public IEnumerable<InternalDocumentSendListAccessGroup> AccessGroups { get; set; }

    }
}
