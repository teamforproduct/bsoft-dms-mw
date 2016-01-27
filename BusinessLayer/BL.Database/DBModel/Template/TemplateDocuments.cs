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
            this.IncomingDetail = new HashSet<TemplateDocumentIncomingDetails>();
            this.SendLists = new HashSet<TemplateDocumentSendLists>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IsHard { get; set; }
        public int DocumentDirectionId { get; set; }
        public int DocumentTypeId { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public Nullable<int> RestrictedSendListId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<TemplateDocumentIncomingDetails> IncomingDetail { get; set; }
        public virtual ICollection<TemplateDocumentSendLists> SendLists { get; set; }
        [ForeignKey("DocumentDirectionId")]
        public virtual DictionaryDocumentDirections DocumentDirection { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DictionaryDocumentTypes DocumentType { get; set; }
        [ForeignKey("DocumentSubjectId")]
        public virtual DictionaryDocumentSubjects DocumentSubject { get; set; }
        [ForeignKey("RegistrationJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }
        [ForeignKey("RestrictedSendListId")]
        public virtual DictionaryStandartSendLists RestrictedSendList { get; set; }
    }
}
