using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentEvent : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumEventTypes EventType { get; set; }
        //public EnumImportanceEventTypes ImportanceEventType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public int? SourcePositionId { get; set; }
        public int? SourcePositionExecutorAgentId { get; set; }
        public int SourceAgentId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetPositionExecutorAgentId { get; set; }
        public int? TargetAgentId { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? ReadAgentId { get; set; }
        public DateTime? SendDate { get; set; }
        public string GeneralInfo { get; set; }
        public string SourcePositionName { get; set; }
        public string TargetPositionName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
    }
}
