using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateRestrictedSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumAccessLevels AccessLevel { get; set; }

        public InternalTemplateRestrictedSendList()
        {
        }

        public InternalTemplateRestrictedSendList(AddTemplateRestrictedSendList model)
        {
            SetInternalTemplateRestrictedSendList(model);
        }

        public InternalTemplateRestrictedSendList(ModifyTemplateRestrictedSendList model)
        {
            Id = model.Id;
            SetInternalTemplateRestrictedSendList(model);
        }

        private void SetInternalTemplateRestrictedSendList(AddTemplateRestrictedSendList model)
        {
            DocumentId = model.DocumentId;
            PositionId = model.PositionId;
            AccessLevel = model.AccessLevel;
        }


    }


    
}
