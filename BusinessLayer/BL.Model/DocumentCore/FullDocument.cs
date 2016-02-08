using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore
{
    public class FullDocument: ModifyDocument
    {
        public FullDocument()
        {
        }

        public FullDocument(ModifyDocument document) :this()
        {
            Id = document.Id;
            TemplateDocumentId = document.TemplateDocumentId;
            DocumentSubjectId = document.DocumentSubjectId;
            Description = document.Description;
            ExecutorPositionId = document.ExecutorPositionId;
            SenderAgentId = document.SenderAgentId;
            SenderAgentPersonId = document.SenderAgentPersonId;
            SenderNumber = document.SenderNumber;
            SenderDate = document.SenderDate;
            Addressee = document.Addressee;
            AccessLevel = document.AccessLevel;
        }

        public DateTime? CreateDate { get; set; }
        public bool IsRegistered { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? LastChangeUserId { get; set; }
        public DateTime? LastChangeDate { get; set; }

        public string TemplateDocumentName { get; set; }
        public int IsHard { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public string DocumentDirectionName { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionAgentName { get; set; }
        public string ExecutorAgentName { get; set; }

        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }
        public string AccessLevelName { get; set; }

        public bool IsFavourite { get; set; }
        public bool IsInWork { get; set; }

        public DateTime DocumentDate { get; set; }
        public DateTime DateOfControl { get; set; }
        public string RegistrationFullNumber { get; set; }

        public string GeneralInfo { get; set; }

        public int EventsCount { get; set; }
        public int NewEventCount { get; set; }
        public int AttachedFilesCount { get; set; }
        public int LinkedDocumentsCount { get; set; }

        public IEnumerable<BaseDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public IEnumerable<BaseDocumentSendList> SendLists { get; set; }
        public int SendListStagesCount { get; set; }
        public IEnumerable<BaseDocumentEvent> Events { get; set; }
        public IEnumerable<BaseDocumentAccess> Accesses { get; set; }

        public int? TemporaryRegistrationJournalId { get; set; }
        public int? TemporaryRegistrationNumber { get; set; }
        public string TemporaryRegistrationNumberSuffix { get; set; }
        public string TemporaryRegistrationNumberPrefix { get; set; }
        public DateTime? TemporaryRegistrationDate { get; set; }
    }
}