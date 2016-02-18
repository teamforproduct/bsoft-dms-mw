﻿using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentEvents
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumEventTypes EventType { get; set; }
        //public EnumImportanceEventTypes ImportanceEventType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? ReadAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}