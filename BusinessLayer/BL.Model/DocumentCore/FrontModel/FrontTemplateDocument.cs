using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHard { get; set; }
        public bool IsForProject { get; set; }
        public bool IsForDocument { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public string DocumentDirectionName { get; set; }

        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public int? DocumentSubjectId { get; set; }
        public string DocumentSubjectName { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public string RegistrationJournalName { get; set; }

        public int? SenderAgentId { get; set; }
        public string SenderAgentName { get; set; }
        public int? SenderAgentPersonId { get; set; }
        public string SenderAgentPersonName { get; set; }

        public string Addressee { get; set; }

        public virtual IEnumerable<FrontTemplateDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public virtual IEnumerable<FrontTemplateDocumentSendList> SendLists { get; set; }

        public IEnumerable<FrontPropertyValue> Properties { get; set; }

    }
}
