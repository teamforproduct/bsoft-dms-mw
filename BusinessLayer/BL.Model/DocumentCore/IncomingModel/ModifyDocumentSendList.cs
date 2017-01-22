using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для изменения записи плана работы над документом
    /// </summary>
    public class ModifyDocumentSendList : BaseModifyDocumentSendList
    {
        public ModifyDocumentSendList(AddDocumentSendList model)
        {
            DocumentId = model.DocumentId;
            Stage = model.Stage;
            StageType = model.StageType;
            SendType = model.SendType;
            SourcePositionId = model.SourcePositionId;
            TargetPositionId = model.TargetPositionId;
            TargetAgentId = model.TargetAgentId;
            Task = model.Task;
            IsWorkGroup = model.IsWorkGroup;
            IsAddControl = model.IsAddControl;
            SelfDescription = model.SelfDescription;
            SelfDueDate = model.SelfDueDate;
            SelfDueDay = model.SelfDueDay;
            SelfAttentionDate = model.SelfAttentionDate;
            SelfAttentionDay = model.SelfAttentionDay;
            IsAvailableWithinTask = model.IsAvailableWithinTask;
            Description = model.Description;
            DueDate = model.DueDate;
            DueDay = model.DueDay;
            AccessLevel = model.AccessLevel;
            IsInitial = model.IsInitial;
            PaperEvents = model.PaperEvents;
            IsLaunchItem = model.IsLaunchItem;
        }
        /// <summary>
        /// ИЗ записи плана
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
    }
}
