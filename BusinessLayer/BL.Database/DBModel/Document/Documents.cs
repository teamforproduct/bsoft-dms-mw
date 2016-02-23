using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class Documents
    {
        public Documents()
        {
            this.SendLists = new HashSet<DocumentSendLists>();
            this.Files = new HashSet<DocumentFiles>();
            this.LinksDocuments = new HashSet<DocumentLinks>();
            this.LinksParentDocuments = new HashSet<DocumentLinks>();
            this.Accesses = new HashSet<DocumentAccesses>();
            this.Subscriptions = new HashSet<DocumentSubscriptions>();
            this.Waits = new HashSet<DocumentWaits>();
            this.Events = new HashSet<DocumentEvents>();
            this.RestrictedSendLists = new HashSet<DocumentRestrictedSendLists>();
            this.Tags = new HashSet<DocumentTags>();
        }

        public int Id { get; set; }
        public int TemplateDocumentId { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public bool IsRegistered { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        public string NumerationPrefixFormula { get; set; }
        public Nullable<int> RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public Nullable<DateTime> RegistrationDate { get; set; }
        public int ExecutorPositionId { get; set; }
        //public int ExecutorAgentId { get; set; }
        public Nullable<int> SenderAgentId { get; set; }
        public Nullable<int> SenderAgentPersonId { get; set; }
        public string SenderNumber { get; set; }
        public Nullable<DateTime> SenderDate { get; set; }
        public string Addressee { get; set; }
        public int? LinkId { get; set; }
        public bool IsLaunchPlan { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("TemplateDocumentId")]
        public virtual TemplateDocuments TemplateDocument { get; set; }
        [ForeignKey("DocumentSubjectId")]
        public virtual DictionaryDocumentSubjects DocumentSubject { get; set; }
        [ForeignKey("RegistrationJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }
        [ForeignKey("ExecutorPositionId")]
        public virtual DictionaryPositions ExecutorPosition { get; set; }
        //[ForeignKey("ExecutorAgentId")]
        //public virtual DictionaryAgents ExecutorAgent { get; set; }
        [ForeignKey("SenderAgentId")]
        public virtual DictionaryAgents SenderAgent { get; set; }
        [ForeignKey("SenderAgentPersonId")]
        public virtual DictionaryAgentPersons SenderAgentPerson { get; set; }


        public virtual ICollection<DocumentSendLists> SendLists { get; set; }
        public virtual ICollection<DocumentFiles> Files { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<DocumentLinks> LinksDocuments { get; set; }
        [ForeignKey("ParentDocumentId")]
        public virtual ICollection<DocumentLinks> LinksParentDocuments { get; set; }
        public virtual ICollection<DocumentAccesses> Accesses { get; set; }
        public virtual ICollection<DocumentSubscriptions> Subscriptions { get; set; }
        public virtual ICollection<DocumentWaits> Waits { get; set; }
        public virtual ICollection<DocumentEvents> Events { get; set; }

        public virtual ICollection<DocumentRestrictedSendLists> RestrictedSendLists { get; set; }
        public virtual ICollection<DocumentTags> Tags { get; set; }

    }

}
