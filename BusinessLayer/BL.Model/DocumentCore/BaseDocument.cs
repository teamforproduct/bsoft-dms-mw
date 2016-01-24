using System;

namespace BL.Model.DocumentCore
{
    public class BaseDocument
    {
        public int Id { get; set; }
        public int? TemplateDocumentId { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? DocumentSubjectId { get; set; }
        public string DocumentSubject { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int ExecutorPositionId { get; set; }
        public string ExecutorName { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}