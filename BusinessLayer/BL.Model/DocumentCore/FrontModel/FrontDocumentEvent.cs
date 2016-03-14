﻿using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Describe the display model for the Document events.
    /// </summary>
    public class FrontDocumentEvent
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumEventTypes EventType { get; set; }
        public EnumImportanceEventTypes ImportanceEventType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }

        public int? SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string EventTypeName { get; set; }
        public string EventImportanceTypeName { get; set; }

        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourceAgentName { get; set; }

        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }

        public string GeneralInfo { get; set; }

    }
}
