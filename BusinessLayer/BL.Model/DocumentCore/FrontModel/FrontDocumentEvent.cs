﻿using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentEvent
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumEventTypes EventType { get; set; }
        public EnumImportanceEventTypes ImportanceEventType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string EventTypeName { get; set; }
        public string EventImportanceTypeName { get; set; }
        public string SourcePositionName { get; set; }
        public string SourceAgenName { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetAgenName { get; set; }

        public string GeneralInfo { get; set; }

        public FrontDocumentWaits OnWaits { get; set; }
        public FrontDocumentWaits OffWaits { get; set; }

    }
}