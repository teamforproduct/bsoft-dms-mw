using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            this.Papers = new HashSet<DocumentPapers>();
        }

        [Index("IX_IsRegistered", 2)]
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        public int? DocumentDirectionId { get; set; }
        public int? DocumentTypeId { get; set; }
        [Index("IX_IsRegistered", 3)]
        [Index("IX_TemplateDocumentId", 1)]
        public int TemplateDocumentId { get; set; }

        [Index("IX_CreateDate",1)]
        public DateTime CreateDate { get; set; }
        public Nullable<int> DocumentSubjectId { get; set; }
        [MaxLength(2000)]
        public string DocumentSubject { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string AddDescription { get; set; }

        [Index("IX_IsRegistered",1)]
        public bool? IsRegistered { get; set; }
        public Nullable<int> RegistrationJournalId { get; set; }
        [MaxLength(2000)]
        public string NumerationPrefixFormula { get; set; }
        public Nullable<int> RegistrationNumber { get; set; }
        [MaxLength(100)]
        public string RegistrationNumberSuffix { get; set; }
        [MaxLength(100)]
        public string RegistrationNumberPrefix { get; set; }
        public Nullable<DateTime> RegistrationDate { get; set; }
        public int? ExecutorPositionId { get; set; }
        [Column("ExecutorPositionExeAgentId")]
        public int ExecutorPositionExecutorAgentId { get; set; }
        [Column("ExecutorPositionExeTypeId")]
        public int? ExecutorPositionExecutorTypeId { get; set; }
        //public int ExecutorAgentId { get; set; }
        public Nullable<int> SenderAgentId { get; set; }
        public Nullable<int> SenderAgentPersonId { get; set; }
        [MaxLength(2000)]
        public string SenderNumber { get; set; }
        public Nullable<DateTime> SenderDate { get; set; }
        [MaxLength(2000)]
        public string Addressee { get; set; }
        public int? LinkId { get; set; }
        public bool IsLaunchPlan { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }


        [ForeignKey("DocumentDirectionId")]
        public virtual DictionaryDocumentDirections DocumentDirection { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DictionaryDocumentTypes DocumentType { get; set; }
        [ForeignKey("TemplateDocumentId")]
        public virtual TemplateDocuments TemplateDocument { get; set; }
        [ForeignKey("RegistrationJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }
        [ForeignKey("ExecutorPositionId")]
        public virtual DictionaryPositions ExecutorPosition { get; set; }
        [ForeignKey("ExecutorPositionExecutorAgentId")]
        public virtual DictionaryAgents ExecutorPositionExecutorAgent { get; set; }
        [ForeignKey("ExecutorPositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes ExecutorPositionExecutorType { get; set; }
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
        public virtual ICollection<DocumentTasks> Tasks { get; set; }

        public virtual ICollection<DocumentRestrictedSendLists> RestrictedSendLists { get; set; }
        public virtual ICollection<DocumentTags> Tags { get; set; }
        [ForeignKey("DocumentId")]
        public virtual ICollection<DocumentPapers> Papers { get; set; }

    }

}
