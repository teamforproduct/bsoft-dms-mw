using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? TaskId { get; set; }
        public bool IsWorkGroup { get; set; }
        public bool IsAddControl { get; set; }
        public string SelfDescription { get; set; }
        public int? SelfDueDay { get; set; }
        public int? SelfAttentionDay { get; set; }

        public bool IsAvailableWithinTask { get; set; }
        public string Description { get; set; }
        public string TaskName { get; set; }
        
        public int Stage { get; set; }
        public EnumStageTypes? StageType { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }

        public InternalTemplateDocumentSendList()
        {
        }

        public InternalTemplateDocumentSendList(ModifyTemplateDocumentSendLists list)
        {
            this.Id =list.Id ?? -1;
            this.DocumentId=list.DocumentId;
            this.SendType = list.SendType;
            this.StageType = list.StageType;
            this.TargetPositionId = list.TargetPositionId;
            this.TargetAgentId = list.TargetAgentId;
            this.TaskId = list.TaskId;
            this.IsWorkGroup = list.IsWorkGroup;
            this.IsAddControl = list.IsAddControl;
            this.SelfDescription = list.SelfDescription;
            this.SelfDueDay = list.SelfDueDay;
            this.SelfAttentionDay = list.SelfAttentionDay;

            this.IsAvailableWithinTask = list.IsAvailableWithinTask;
            this.Description= list.Description;
            this.Stage = list.Stage;
            this.DueDay=list.DueDay;
            this.AccessLevel = list.AccessLevel;
        }


    }
}
