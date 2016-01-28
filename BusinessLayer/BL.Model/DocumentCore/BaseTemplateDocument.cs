using System;

namespace BL.Model.DocumentCore
{
    public class BaseTemplateDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public string DocumentDirectionName { get; set; }

        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public Nullable<int> DocumentSubjectId { get; set; }
        public string DocumentSubjectName { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public string RegistrationJournalName { get; set; }

        public Nullable<int> SenderAgentId { get; set; }
        public string SenderAgentName { get; set; }
        public string SenderPerson { get; set; }
        public string Addressee { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
