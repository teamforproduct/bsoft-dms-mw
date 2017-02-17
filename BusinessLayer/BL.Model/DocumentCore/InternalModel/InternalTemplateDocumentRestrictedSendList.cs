using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentRestrictedSendList : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }

        public InternalTemplateDocumentRestrictedSendList()
        {
        }

        public InternalTemplateDocumentRestrictedSendList(AddTemplateDocumentRestrictedSendList model)
        {
            SetInternalTemplateDocumentRestrictedSendList(model);
        }

        public InternalTemplateDocumentRestrictedSendList(ModifyTemplateDocumentRestrictedSendList model)
        {
            Id = model.Id;
            SetInternalTemplateDocumentRestrictedSendList(model);
        }

        private void SetInternalTemplateDocumentRestrictedSendList(AddTemplateDocumentRestrictedSendList model)
        {
            DocumentId = model.DocumentId;
            PositionId = model.PositionId;
            AccessLevel = model.AccessLevel;
        }


    }


    
}
