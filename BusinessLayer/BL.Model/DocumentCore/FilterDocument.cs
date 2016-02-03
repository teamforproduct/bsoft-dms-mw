using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore
{
    public class FilterDocument
    {
        public List<int> Id { get; set; }
        public List<int> TemplateDocumentId { get; set; }
        public List<int> DocumentDirectionId { get; set; }
        public List<int> DocumentTypeId { get; set; }
        public DateTime? CreateFromDate { get; set; }
        public DateTime? CreateToDate { get; set; }
        public List<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public List<int> RegistrationJournalId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationFromDate { get; set; }
        public DateTime? RegistrationToDate { get; set; }
        public List<int> ExecutorPositionId { get; set; }
        public List<int> SenderAgentId { get; set; }
        public string SenderPerson { get; set; }
        public string SenderNumber { get; set; }
        public DateTime? SenderFromDate { get; set; }
        public DateTime? SenderToDate { get; set; }
        public string Addressee { get; set; }
        public bool IsInWork { get; set; } // should be true by default
    }
}
