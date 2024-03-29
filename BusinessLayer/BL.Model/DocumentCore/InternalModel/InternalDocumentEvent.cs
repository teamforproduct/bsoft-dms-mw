﻿using System;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Reports.Interfaces;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentEvent : LastChangeInfo, IReports//, IDocumentEventAccessSet
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public EnumEventTypes EventType { get; set; }
        //public EnumImportanceEventTypes ImportanceEventType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public int? TaskId { get; set; }
        public string Description { get; set; }
        public string AddDescription { get; set; }

        public int? SourcePositionId { get; set; }
        public int? ControllerPositionId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? PaperId { get; set; }
        public Nullable<int> ParentEventId { get; set; }
        public Nullable<int> SendListId { get; set; }
        public Nullable<int> PaperListId { get; set; }
        public int? PaperPlanAgentId { get; set; }
        public DateTime? PaperPlanDate { get; set; }
        public Nullable<int> PaperSendAgentId { get; set; }
        public Nullable<DateTime> PaperSendDate { get; set; }
        public Nullable<int> PaperRecieveAgentId { get; set; }
        public Nullable<DateTime> PaperRecieveDate { get; set; }
        public InternalDocumentPaper Paper { get; set; }
        public IEnumerable<InternalDocumentEventAccess> Accesses { get; set; }
        public IEnumerable<InternalDocumentEventAccessGroup> AccessGroups { get; set; }
    }
}
