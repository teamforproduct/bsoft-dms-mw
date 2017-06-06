using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? TaskId { get; set; }
        public bool IsAddControl { get; set; }
        public string SelfDescription { get; set; }
        public int? SelfDueDay { get; set; }
        public int? SelfAttentionDay { get; set; }
        public string Description { get; set; }
        public string TaskName { get; set; }
        
        public int Stage { get; set; }
        public EnumStageTypes? StageType { get; set; }
        public int? DueDay { get; set; }
        public EnumAccessLevels AccessLevel { get; set; }
        public IEnumerable<InternalTemplateSendListAccessGroup> AccessGroups { get; set; }

        public InternalTemplateSendList()
        {
        }

    }
}
