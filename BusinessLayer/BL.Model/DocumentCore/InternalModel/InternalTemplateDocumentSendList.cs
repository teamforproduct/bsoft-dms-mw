﻿using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

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
        public string Description { get; set; }
        public string TaskName { get; set; }
        
        public int Stage { get; set; }
        public EnumStageTypes? StageType { get; set; }
        public int? DueDay { get; set; }
        public EnumAccessLevels AccessLevel { get; set; }
        public IEnumerable<InternalTemplateDocumentSendListAccessGroup> AccessGroups { get; set; }

        public InternalTemplateDocumentSendList()
        {
        }

        public InternalTemplateDocumentSendList(ModifyTemplateDocumentSendList model)
        {
            Id = model.Id;
            SetInternalTemplateDocumentSendList(model);
        }

        private void SetInternalTemplateDocumentSendList(AddTemplateDocumentSendList model)
        {
            DocumentId = model.DocumentId;
            SendType = model.SendType;
            StageType = model.StageType;
            TargetPositionId = model.TargetPositionId;
            TargetAgentId = model.TargetAgentId;
            TaskId = model.TaskId;
            IsWorkGroup = model.IsWorkGroup;
            IsAddControl = model.IsAddControl;
            SelfDescription = model.SelfDescription;
            SelfDueDay = model.SelfDueDay;
            SelfAttentionDay = model.SelfAttentionDay;
            Description = model.Description;
            Stage = model.Stage;
            DueDay = model.DueDay;
            AccessLevel = model.AccessLevel;
        }

    }
}
