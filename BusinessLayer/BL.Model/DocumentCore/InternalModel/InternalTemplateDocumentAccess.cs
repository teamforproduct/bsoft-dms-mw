using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentAccess : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }

        public InternalTemplateDocumentAccess()
        {
        }

        public InternalTemplateDocumentAccess(AddTemplateDocumentAccess model)
        {
            SetInternalTemplateDocumentAccess(model);
        }

        public InternalTemplateDocumentAccess(ModifyTemplateDocumentAccess model)
        {
            Id = model.Id;
            SetInternalTemplateDocumentAccess(model);
        }

        private void SetInternalTemplateDocumentAccess(AddTemplateDocumentAccess model)
        {
            DocumentId = model.DocumentId;
            PositionId = model.PositionId;
        }


    }


    
}
