using System;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocument : LastChangeInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHard { get; set; }
        public bool IsForProject { get; set; }
        public bool IsForDocument { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public int DocumentTypeId { get; set; }
        public int? DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? SenderAgentId { get; set; }
        public int? SenderAgentPersonId { get; set; }
        public string Addressee { get; set; }
        public bool IsActive { get; set; }
        public virtual IEnumerable<InternalTemplateDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public virtual IEnumerable<InternalTemplateDocumentSendList> SendLists { get; set; }
        public virtual IEnumerable<InternalTemplateDocumentTask> Tasks { get; set; }
        public IEnumerable<ModifyPropertyValue> Properties { get; set; }

        public InternalTemplateDocument()
        {
        }

        public InternalTemplateDocument(ModifyTemplateDocument model)
        {
            this.Id = model.Id ?? -1;
            this.Name = model.Name;
            this.IsHard = model.IsHard;
            this.IsForProject = model.IsForProject;
            this.IsForDocument = model.IsForDocument;
            this.DocumentDirection = model.DocumentDirection;
            this.DocumentTypeId = model.DocumentTypeId;
            this.DocumentSubjectId = model.DocumentSubjectId;
            this.Description = model.Description;
            this.RegistrationJournalId = model.RegistrationJournalId;
            this.SenderAgentId = model.SenderAgentId;
            this.SenderAgentPersonId = model.SenderAgentPersonId;
            this.Addressee = model.Addressee;
            this.IsActive = model.IsActive;
            this.Properties = model.Properties;
            this.LastChangeDate = model.LastChangeDate;
            this.LastChangeUserId = model.LastChangeUserId;
        }


    }
}
