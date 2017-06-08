using System;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplate : LastChangeInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHard { get; set; }
        public bool IsForProject { get; set; }
        public bool IsForDocument { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentSubject { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? SenderAgentId { get; set; }
        public int? SenderAgentPersonId { get; set; }
        public string Addressee { get; set; }
        public bool IsActive { get; set; }
        public int? MaxPaperOrderNumber { get; set; }
        public int FileCount { get; set; }
        public IEnumerable<InternalTemplateRestrictedSendList> RestrictedSendLists { get; set; }
        public IEnumerable<InternalTemplateSendList> SendLists { get; set; }
        public IEnumerable<InternalTemplateTask> Tasks { get; set; }
        public IEnumerable<InternalTemplatePaper> Papers { get; set; }
        public IEnumerable<InternalPropertyValue> Properties { get; set; }
        public IEnumerable<InternalTemplateFile> Files { get; set; }

        public InternalTemplate()
        {
        }

        public InternalTemplate(AddTemplate model)
        {
            SetInternalTemplate(model);
        }

        public InternalTemplate(ModifyTemplate model)
        {
            Id = model.Id;
            SetInternalTemplate(model);
        }

        private void SetInternalTemplate(AddTemplate model)
        {
            Name = model.Name;
            IsHard = model.IsHard;
            IsForProject = model.IsForProject;
            IsForDocument = model.IsForDocument;
            DocumentDirection = model.DocumentDirection;
            DocumentTypeId = model.DocumentTypeId;
            DocumentSubject = model.DocumentSubject;
            Description = model.Description;
            RegistrationJournalId = model.RegistrationJournalId;
            SenderAgentId = model.SenderAgentId;
            SenderAgentPersonId = model.SenderAgentPersonId;
            Addressee = model.Addressee;
            IsActive = model.IsActive;
        }
    }
}
