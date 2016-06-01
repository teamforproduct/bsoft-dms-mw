using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocumentSendLists : LastChangeInfo
    {
        public int? Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? SourcePositionId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? TaskId { get; set; }
        public bool IsWorkGroup { get; set; }
        public bool IsAddControl { get; set; }
        public Nullable<DateTime> SelfDueDate { get; set; }
        public int? SelfDueDay { get; set; }
        public Nullable<DateTime> SelfAttentionDate { get; set; }
        public bool IsAvailableWithinTask { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
