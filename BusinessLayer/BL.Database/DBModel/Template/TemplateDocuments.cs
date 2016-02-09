using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocuments
    {
        public TemplateDocuments()
        {
            //this.IncomingDetail = new HashSet<TemplateDocumentIncomingDetails>();
            this.SendLists = new HashSet<TemplateDocumentSendLists>();
            this.RestrictedSendLists = new HashSet<TemplateDocumentRestrictedSendLists>();
            this.DocumentFiles = new HashSet<TemplateDocumentFiles>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> SenderAgentId { get; set; }
        public Nullable<int> SenderAgentPersonId { get; set; }
        public string SenderNumber { get; set; }
        public Nullable<DateTime> SenderDate { get; set; }
        public string Addressee { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentRestrictedSendLists> RestrictedSendLists { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<TemplateDocumentFiles> DocumentFiles { get; set; }
        [ForeignKey("DocumentDirectionId")]
        public virtual DictionaryDocumentDirections DocumentDirection { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DictionaryDocumentTypes DocumentType { get; set; }
        [ForeignKey("DocumentSubjectId")]
        public virtual DictionaryDocumentSubjects DocumentSubject { get; set; }
        [ForeignKey("RegistrationJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }
        [ForeignKey("SenderAgentId")]
        public virtual DictionaryAgents SenderAgent { get; set; }
        [ForeignKey("SenderAgentPersonId")]
        public virtual DictionaryAgentPersons SenderAgentPerson { get; set; }
    }
}
