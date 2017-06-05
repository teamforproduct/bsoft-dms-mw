using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocuments
    {
        public TemplateDocuments()
        {
            this.Documents = new HashSet<Document.Documents>();
            this.SendLists = new HashSet<TemplateDocumentSendLists>();
            this.RestrictedSendLists = new HashSet<TemplateDocumentRestrictedSendLists>();
            this.Accesses = new HashSet<TemplateAccesses>();
            this.DocumentFiles = new HashSet<TemplateDocumentFiles>();
            this.Tasks = new HashSet<TemplateDocumentTasks>();
            this.Papers = new HashSet<TemplateDocumentPapers>();
        }

        public int Id { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        [MaxLength(400)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public bool IsHard { get; set; }
        public bool IsForProject { get; set; }
        public bool IsForDocument { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
//        public Nullable<int> DocumentSubjectId { get; set; }
        [MaxLength(2000)]
        public string DocumentSubject { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> SenderAgentId { get; set; }
        public Nullable<int> SenderAgentPersonId { get; set; }
        [MaxLength(2000)]
        public string Addressee { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("TemplateDocumentId")]
        public virtual ICollection<Document.Documents> Documents { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentRestrictedSendLists> RestrictedSendLists { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateAccesses> Accesses { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentFiles> DocumentFiles { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentTasks> Tasks { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentPapers> Papers { get; set; }

        [ForeignKey("DocumentDirectionId")]
        public virtual DictionaryDocumentDirections DocumentDirection { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DictionaryDocumentTypes DocumentType { get; set; }
        [ForeignKey("RegistrationJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }
        [ForeignKey("SenderAgentId")]
        public virtual DictionaryAgents SenderAgent { get; set; }
        [ForeignKey("SenderAgentPersonId")]
        public virtual DictionaryAgentPersons SenderAgentPerson { get; set; }

    }
}
