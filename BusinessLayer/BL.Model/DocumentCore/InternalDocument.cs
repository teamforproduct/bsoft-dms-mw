using System;
using System.Collections.Generic;
using BL.Model.DocumentAdditional;
using BL.Model.Enums;

namespace BL.Model.DocumentCore
{
    public class InternalDocument: ModifyDocument
    {
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

        public bool IsHard { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public int DocumentTypeId { get; set; }

        public bool IsFavourite { get; set; }
        public bool IsInWork { get; set; }

        public DateTime DocumentDate { get; set; }
        public DateTime DateOfControl { get; set; }
        public string RegistrationFullNumber { get; set; }

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
        public IEnumerable<FrontDocument> LinkedDocuments { get; set; }
        public IEnumerable<FrontDocumentLink> Links { get; set; }
        public IEnumerable<BaseDocumentWaits> DocumentWaits { get; set; }
    }
}