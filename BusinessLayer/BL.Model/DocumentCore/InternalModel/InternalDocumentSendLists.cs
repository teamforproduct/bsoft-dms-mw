using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int Stage { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
        public bool IsInitial { get; set; }
        public int? StartEventId { get; set; }
        public int? CloseEventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
