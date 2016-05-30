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
        public int? SourcePositionId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public int? TaskId { get; set; }
        public bool IsAddControl { get; set; }

        public Nullable<DateTime> SelfDueDate { get; set; }
        public int? SelfDueDay { get; set; }
        public Nullable<DateTime> SelfAttentionDate { get; set; }

        public bool IsAvailableWithinTask { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
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
            this.SourcePositionId = list.SourcePositionId;
            this.TargetPositionId = list.TargetPositionId;
            this.TargetAgentId = list.TargetAgentId;
            this.TaskId = list.TaskId;
            this.IsAddControl = list.IsAddControl;

            this.SelfDueDate = list.SelfDueDate;
            this.SelfDueDay = list.SelfDueDay;
            this.SelfAttentionDate = list.SelfAttentionDate;

            this.IsAvailableWithinTask = list.IsAvailableWithinTask;
            this.Description= list.Description;
            this.Stage = list.Stage;
            this.DueDay=list.DueDay;
            this.AccessLevel = list.AccessLevel;
            this.LastChangeDate = list.LastChangeDate;
            this.LastChangeUserId = list.LastChangeUserId;
        }


    }
}
