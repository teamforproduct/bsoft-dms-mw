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

        public InternalTemplateDocumentRestrictedSendList(ModifyTemplateDocumentRestrictedSendLists model)
        {
            this.Id = model.Id ?? -1;
            this.DocumentId = model.DocumentId;
            this.PositionId = model.PositionId;
            this.AccessLevel = model.AccessLevel;
            this.LastChangeDate = model.LastChangeDate;
            this.LastChangeUserId = model.LastChangeUserId;

        }

       
    }


    
}
