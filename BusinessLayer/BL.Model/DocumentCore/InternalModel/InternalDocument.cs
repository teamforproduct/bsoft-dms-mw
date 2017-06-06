using System;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Reports.Interfaces;
using BL.Model.DocumentCore.Filters;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocument : LastChangeInfo, IReports
    {
        public InternalDocument()
        {
        }
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        /// <summary>
        /// Тематика документа
        public string DocumentSubject { get; set; }
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
        public EnumAccessLevels? AccessLevel { get; set; }

        public int TemplateId { get; set; }
        public int? ExecutorPositionId { get; set; }
        public int ExecutorPositionExecutorAgentId { get; set; }
        public int? ExecutorPositionExecutorTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? IsRegistered { get; set; }
        public int? RegistrationJournalId { get; set; }
        public string NumerationPrefixFormula { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? LinkId { get; set; }
        public bool IsLaunchPlan { get; set; }
        public bool IsHard { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public int? DocumentTypeId { get; set; }

        public bool IsFavourite { get; set; }
        public bool IsInWork { get; set; }

        public string RegistrationJournalPrefixFormula { get; set; }
        public string RegistrationJournalSuffixFormula { get; set; }

        public DateTime DocumentDate { get; set; }
        public DateTime DateOfControl { get; set; }
        //      public string RegistrationFullNumber { get; set; }

        public int ParentDocumentId { get; set; }
        public int? ParentDocumentLinkId { get; set; }
        public int LinkTypeId { get; set; }

        public int? WaitsCount { get; set; }
        public int? SendListsCount { get; set; }
        public int? SubscriptionsCount { get; set; }
        public int? AccessesCount { get; set; }
        // public int EventsCount { get; set; }
        // public int NewEventCount { get; set; }
        // public int FilesCount { get; set; }
        public int LinkedDocumentsCount { get; set; }


        public string Hash { get; set; }
        public string FullHash { get; set; }
        public string InternalSign { get; set; }
        public string CertificateSign { get; set; }
        public int? OldLinkId { get; set; }
        public int? NewLinkId { get; set; }
        public bool IsRestrictedSendListsLoaded { get; set; }        
        public InternalTemplate Template { get; set; }
        public IEnumerable<InternalDocumentRestrictedSendList> RestrictedSendLists { get; set; }
        public IEnumerable<InternalDocumentSendList> SendLists { get; set; }
        public IEnumerable<InternalDocumentSendListStage> SendListStages { get; set; }
        public int SendListStageMax { get; set; }
        public IEnumerable<InternalDocumentEvent> Events { get; set; }
        public IEnumerable<InternalDocumentAccess> Accesses { get; set; }
        public IEnumerable<InternalDocumentEventAccess> ParentEventAccesses { get; set; }
        public IEnumerable<InternalDocumentFile> DocumentFiles { get; set; }
        public IEnumerable<InternalDocumentLink> Links { get; set; }
        public IEnumerable<InternalDocumentLink> OldLinks { get; set; }
        public IEnumerable<int> OldLinkSet { get; set; }
        public IEnumerable<int> NewLinkSet { get; set; }
        public IEnumerable<InternalDocumentWait> Waits { get; set; }
        public IEnumerable<InternalDocumentSubscription> Subscriptions { get; set; }
        public IEnumerable<InternalDocumentTask> Tasks { get; set; }
        public IEnumerable<InternalPropertyValue> Properties { get; set; }

        public IEnumerable<InternalDocumentEvent> PaperEvents { get; set; }
        public IEnumerable<InternalDocumentPaper> Papers { get; set; }
        public int? MaxPaperOrderNumber { get; set; }
        public int? FileOrderNumber { get; set; }

        #region For Report
        public string DocumentTypeName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }
        public string RegistrationFullNumber { get; set; }
        #endregion

        #region
        public FilterDocumentFileIdentity CertificateSignPdfFileIdentity { get; set; }
        #endregion
    }
}