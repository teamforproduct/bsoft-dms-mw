using System;
using System.Collections.Generic;
using BL.Model.DocumentAdditional;
using BL.Model.Enums;
using System.Linq;

namespace BL.Model.DocumentCore
{
    public class FullDocument : ModifyDocument
    {
        public FullDocument()
        {
        }

        public FullDocument(ModifyDocument model, FullDocument doc) : this()
        {
            if (model != null)
            {
                Id = model.Id;
                DocumentSubjectId = model.DocumentSubjectId;
                Description = model.Description;
                SenderAgentId = model.SenderAgentId;
                SenderAgentPersonId = model.SenderAgentPersonId;
                SenderNumber = model.SenderNumber;
                SenderDate = model.SenderDate;
                Addressee = model.Addressee;
                AccessLevel = model.AccessLevel;
            }
            if (doc != null)
            {
                TemplateDocumentId = doc.TemplateDocumentId;
                IsHard = doc.IsHard;
                ExecutorPositionId = doc.ExecutorPositionId;
                DocumentDirection = doc.DocumentDirection;
                DocumentTypeId = doc.DocumentTypeId;
            }
        }
        public int TemplateDocumentId { get; set; }
        public int ExecutorPositionId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsRegistered { get; set; }
        public int? RegistrationJournalId { get; set; }
        public string NumerationPrefixFormula { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? LastChangeUserId { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public int? LinkId { get; set; }

        public string TemplateDocumentName { get; set; }
        public bool IsHard { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public string DocumentDirectionName { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionAgentName { get; set; }

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
        public IEnumerable<BaseDocumentSendListStage> SendListStages { get; set; }
        public int SendListStageMax { get; set; }
        public IEnumerable<BaseDocumentEvent> Events { get; set; }
        public IEnumerable<BaseDocumentAccess> Accesses { get; set; }
        public IEnumerable<DocumentAttachedFile> DocumentFiles { get; set; }
        public IEnumerable<FullDocument> LinkedDocuments { get; set; }
        public IEnumerable<ВaseDocumentLink> Links { get; set; }
        public IEnumerable<BaseDocumentWaits> DocumentWaits { get; set; }

    }
}