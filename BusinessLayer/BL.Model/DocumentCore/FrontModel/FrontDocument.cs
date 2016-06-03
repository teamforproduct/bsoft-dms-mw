using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.FrontModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocument 
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
                if (model.AccessLevel != null) AccessLevelId = (int)model.AccessLevel;

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
                //    RegistrationFullNumber = model.RegistrationFullNumber;

                //    EventsCount = model.EventsCount;
                //    NewEventCount = model.NewEventCount;
                //    AttachedFilesCount = model.AttachedFilesCount;
                //    LinkedDocumentsCount = model.LinkedDocumentsCount;


                if (model.Properties?.Count() > 0)
                {
                    Properties = model.Properties.Select(x => new FrontPropertyValue { PropertyLinkId = x.PropertyLinkId, Value = x.ValueString != null ? x.ValueString : (x.ValueNumeric.HasValue ? x.ValueNumeric.ToString() : (x.ValueDate.HasValue ? x.ValueDate.ToString() : null)) }).ToList();
                }
            }
        }
        /*
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
        */
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int Id { get; set; }

        public int? DocumentSubjectId { get; set; }
        /// <summary>
        /// Краткое содержание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ИД организации, обязательное для внешних документов
        /// </summary>
        public int? SenderAgentId { get; set; }
        /// <summary>
        /// ИД контакта, обязательное для внешних документов
        /// </summary>
        public int? SenderAgentPersonId { get; set; }
        /// <summary>
        /// Входящий номер документа
        /// </summary>
        public string SenderNumber { get; set; }
        /// <summary>
        /// Дата входящего документа
        /// </summary>
        public DateTime? SenderDate { get; set; }
        /// <summary>
        /// Кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public int? AccessLevelId { get; set; }

        public string TemplateDocumentName { get; set; }

        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }

        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
        public string ExecutorPositionExecutorNowAgentName { get; set; }
        public string ExecutorPositionAgentPhoneNumber { get; set; }

        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }


        public int? TemplateDocumentId { get; set; }
        public int? ExecutorPositionId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsRegistered { get; set; }
        public int? RegistrationJournalId { get; set; }
        public string NumerationPrefixFormula { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? LastChangeUserId { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public int? LinkId { get; set; }
        public bool? IsLaunchPlan { get; set; }
        public bool? IsHard { get; set; }
        public EnumDocumentDirections? DocumentDirection { get; set; }
        public int? DocumentTypeId { get; set; }

        public bool? IsFavourite { get; set; }
        public bool? IsInWork { get; set; }

        public DateTime DocumentDate { get; set; }
       // public DateTime DateOfControl { get; set; }
        public string RegistrationFullNumber { get; set; }

        public int? EventsCount { get; set; }

        public UICounters WaitCount { get; set; }

        public int? WaitOpenCount { get; set; }
        public int? WaitOverdueCount { get; set; }


        public int? NewEventCount { get; set; }
        public int? AttachedFilesCount { get; set; }
        public int? LinkedDocumentsCount { get; set; }

        public IEnumerable<FrontDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public IEnumerable<FrontDocumentSendList> SendLists { get; set; }
        public IEnumerable<FrontDocumentSendListStage> SendListStages { get; set; }
        public int? SendListStageMax { get; set; }
        //public IEnumerable<FrontDocumentEvent> Events { get; set; }
        public IEnumerable<FrontDocumentAccess> Accesses { get; set; }
        public IEnumerable<FrontDocumentAttachedFile> DocumentFiles { get; set; }
        public IEnumerable<FrontDocument> LinkedDocuments { get; set; }
        public IEnumerable<FrontDocumentLink> Links { get; set; }
        public IEnumerable<FrontDocumentTask> DocumentTasks { get; set; }
        public IEnumerable<FrontDocumentWait> DocumentWaits { get; set; }
        public IEnumerable<FrontDocumentTag> DocumentTags { get; set; }

        public IEnumerable<FrontDocumentPaper> DocumentPapers { get; set; }
        public IEnumerable<FrontDocumentPaperEvent> DocumentPaperEvents { get; set; }

        public IEnumerable<FrontDictionaryPosition> DocumentWorkGroup { get; set; }
        public IEnumerable<FrontDocumentSubscription> DocumentSubscriptions { get; set; }

        public IEnumerable<FrontPropertyValue> Properties { get; set; }

    }
}