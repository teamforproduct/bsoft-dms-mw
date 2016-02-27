using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? SourcePositionId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public string Task { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int Stage { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }

    }
}
