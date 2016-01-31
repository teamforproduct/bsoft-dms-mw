using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore
{
    public class BaseDocument : ModifyDocument
    {
        public DateTime CreateDate { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string TemplateDocumentName { get; set; }
        public int IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public string DocumentDirectionName { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorAgentName { get; set; }

        public string SenderAgentName { get; set; }
        public string AccessLevelName { get; set; }
        public string GeneralInfo { get; set; }

        public virtual ICollection<BaseDocumentEvent> Events { get; set; }

    }
}