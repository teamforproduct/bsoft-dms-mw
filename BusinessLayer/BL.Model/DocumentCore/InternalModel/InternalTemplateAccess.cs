using System;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateAccess : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }

        public InternalTemplateAccess()
        {
        }

        public InternalTemplateAccess(AddTemplateAccess model)
        {
            SetInternalTemplateAccess(model);
        }

        public InternalTemplateAccess(ModifyTemplateAccess model)
        {
            Id = model.Id;
            SetInternalTemplateAccess(model);
        }

        private void SetInternalTemplateAccess(AddTemplateAccess model)
        {
            DocumentId = model.DocumentId;
            PositionId = model.PositionId;
        }


    }


    
}
