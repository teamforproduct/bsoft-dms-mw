using System;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocument : ModifyDocument
    {
        public FrontDocument()
        {
        }

        public FrontDocument(InternalDocument model)
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

                TemplateDocumentId = model.TemplateDocumentId;
                ExecutorPositionId = model.ExecutorPositionId;
                CreateDate = model.CreateDate;
                IsRegistered = model.IsRegistered;
                RegistrationJournalId = model.RegistrationJournalId;
                NumerationPrefixFormula = model.NumerationPrefixFormula;
                RegistrationNumber = model.RegistrationNumber;
                RegistrationNumberSuffix = model.RegistrationNumberSuffix;
                RegistrationNumberPrefix = model.RegistrationNumberPrefix;
                RegistrationDate = model.RegistrationDate;
                LastChangeUserId = model.LastChangeUserId;
                LastChangeDate = model.LastChangeDate;
                LinkId = model.LinkId;

                IsHard = model.IsHard;
                DocumentDirection = model.DocumentDirection;
                DocumentTypeId = model.DocumentTypeId;

                IsFavourite = model.IsFavourite;
                IsInWork = model.IsInWork;

                DocumentDate = model.DocumentDate;
                DateOfControl = model.DateOfControl;
                RegistrationFullNumber = model.RegistrationFullNumber;

                EventsCount = model.EventsCount;
                NewEventCount = model.NewEventCount;
                AttachedFilesCount = model.AttachedFilesCount;
                LinkedDocumentsCount = model.LinkedDocumentsCount;

            }
        }

        public FrontDocument(ModifyDocument model)
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
        }

        public string TemplateDocumentName { get; set; }

        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionAgentName { get; set; }

        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }


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

        public IEnumerable<FrontDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public IEnumerable<FrontDocumentSendList> SendLists { get; set; }
        public IEnumerable<FrontDocumentSendListStage> SendListStages { get; set; }
        public int SendListStageMax { get; set; }
        public IEnumerable<FrontDocumentEvent> Events { get; set; }
        public IEnumerable<FrontDocumentAccess> Accesses { get; set; }
        public IEnumerable<FrontDocumentAttachedFile> DocumentFiles { get; set; }
        public IEnumerable<FrontDocument> LinkedDocuments { get; set; }
        public IEnumerable<FrontDocumentLink> Links { get; set; }
        public IEnumerable<FrontDocumentWaits> DocumentWaits { get; set; }

    }
}