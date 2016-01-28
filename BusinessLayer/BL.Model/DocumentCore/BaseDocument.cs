using System;

namespace BL.Model.DocumentCore
{
    public class BaseDocument
    {
        public int Id { get; set; }
        public int TemplateDocumentId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int ExecutorPositionId { get; set; }
        public int ExecutorAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public int? SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public DateTime? SenderDate { get; set; }
        public string Addressee { get; set; }

        public int AccessLevelId { get; set; }

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

    }
}