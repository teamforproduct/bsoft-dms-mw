using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Template;
using BL.Model.DocumentCore.InternalModel;
using BL.Database.DBModel.System;
using BL.Model.SystemCore.InternalModel;
using System;
using BL.Model.EncryptionCore.InternalModel;
using BL.Database.DBModel.Encryption;
using BL.CrossCutting.Interfaces;

namespace BL.Database.Common
{
    public static class ModelConverter
    {

        public static DBModel.Document.Documents GetDbDocument(InternalDocument document)
        {
            return document == null ? null :
                new DBModel.Document.Documents
                {
                    Id = document.Id,
                    EntityTypeId = document.EntityTypeId,
                    ClientId = document.ClientId,
                    DocumentSubject = document.DocumentSubject,
                    TemplateDocumentId = document.TemplateDocumentId,
                    CreateDate = document.CreateDate,
                    DocumentTypeId = document.DocumentTypeId,
                    DocumentDirectionId = (int)document.DocumentDirection,
                    Description = document.Description,
                    IsRegistered = document.IsRegistered,
                    IsLaunchPlan = document.IsLaunchPlan,
                    LinkId = document.LinkId,
                    NumerationPrefixFormula = document.NumerationPrefixFormula,
                    RegistrationJournalId = document.RegistrationJournalId,
                    RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                    RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                    RegistrationNumber = document.RegistrationNumber,
                    RegistrationDate = document.RegistrationDate,
                    ExecutorPositionId = document.ExecutorPositionId,
                    ExecutorPositionExecutorAgentId = document.ExecutorPositionExecutorAgentId,
                    ExecutorPositionExecutorTypeId = document.ExecutorPositionExecutorTypeId,
                    LastChangeUserId = document.LastChangeUserId,
                    LastChangeDate = document.LastChangeDate,
                    SenderAgentId = document.SenderAgentId,
                    SenderAgentPersonId = document.SenderAgentPersonId,
                    SenderNumber = document.SenderNumber,
                    SenderDate = document.SenderDate,
                    Addressee = document.Addressee,
                };
        }

        public static DocumentAccesses GetDbDocumentAccess(InternalDocumentAccess access)
        {
            return access == null ? null :
                new DocumentAccesses
                {
                    Id = access.Id,
                    EntityTypeId = access.EntityTypeId,
                    ClientId = access.ClientId,
                    LastChangeDate = access.LastChangeDate,
                    LastChangeUserId = access.LastChangeUserId,
                    DocumentId = access.DocumentId,
                    IsFavourite = access.IsFavourite,
                    IsInWork = access.IsInWork,
                    AccessLevelId = (int)access.AccessLevel,
                    PositionId = access.PositionId
                };
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(IEnumerable<InternalDocumentAccess> accesses)
        {
            return accesses?.Any() ?? false ? accesses.Select(GetDbDocumentAccess) : null;
        }

        public static DocumentEventAccesses GetDbDocumentEventAccess(InternalDocumentEventAccess access)
        {
            return access == null ? null :
                new DocumentEventAccesses
                {
                    Id = access.Id,
                    EntityTypeId = access.EntityTypeId,
                    ClientId = access.ClientId,
                    LastChangeDate = access.LastChangeDate,
                    LastChangeUserId = access.LastChangeUserId,
                    DocumentId = access.DocumentId,
                    IsFavourite = access.IsFavourite,
                    AccessTypeId = (int)access.AccessType,
                    PositionId = access.PositionId,
                    AgentId = access.AgentId,
                    PositionExecutorTypeId = access.PositionExecutorTypeId,
                    EventId = access.EventId,
                    IsActive = access.IsActive,
                    IsAddLater = access.IsAddLater,
                    ReadAgentId = access.ReadAgentId,
                    ReadDate = access.ReadDate,
                    SendDate = access.SendDate,
                };
        }

        public static IEnumerable<DocumentEventAccesses> GetDbDocumentEventAccesses(IEnumerable<InternalDocumentEventAccess> accesses)
        {
            return accesses?.Any() ?? false ? accesses.Select(GetDbDocumentEventAccess) : null;
        }

        public static DocumentEventAccessGroups GetDbDocumentEventAccessGroup(InternalDocumentEventAccessGroup access)
        {
            return access == null ? null :
                new DocumentEventAccessGroups
                {
                    Id = access.Id,
                    EntityTypeId = access.EntityTypeId,
                    ClientId = access.ClientId,
                    LastChangeDate = access.LastChangeDate,
                    LastChangeUserId = access.LastChangeUserId,
                    DocumentId = access.DocumentId,
                    EventId = access.EventId,
                    AccessTypeId = (int)access.AccessType,
                    AccessGroupTypeId = (int)access.AccessGroupType,
                    PositionId = access.PositionId,
                    AgentId = access.AgentId,
                    CompanyId = access.CompanyId,
                    DepartmentId = access.DepartmentId,
                    StandartSendListId = access.StandartSendListId,
                    IsActive = access.IsActive,
                };
        }

        public static IEnumerable<DocumentEventAccessGroups> GetDbDocumentEventAccessGroups(IEnumerable<InternalDocumentEventAccessGroup> accesses)
        {
            return accesses?.Any() ?? false ? accesses.Select(GetDbDocumentEventAccessGroup) : null;
        }

        public static DocumentSendListAccessGroups GetDbDocumentSendListAccessGroup(InternalDocumentSendListAccessGroup access)
        {
            return access == null ? null :
                new DocumentSendListAccessGroups
                {
                    Id = access.Id,
                    EntityTypeId = access.EntityTypeId,
                    ClientId = access.ClientId,
                    LastChangeDate = access.LastChangeDate,
                    LastChangeUserId = access.LastChangeUserId,
                    DocumentId = access.DocumentId,
                    SendListId = access.SendListId,
                    AccessTypeId = (int)access.AccessType,
                    AccessGroupTypeId = (int)access.AccessGroupType,
                    PositionId = access.PositionId,
                    AgentId = access.AgentId,
                    CompanyId = access.CompanyId,
                    DepartmentId = access.DepartmentId,
                    StandartSendListId = access.StandartSendListId,
                    IsActive = access.IsActive,
                };
        }

        public static IEnumerable<DocumentSendListAccessGroups> GetDbDocumentSendListAccessGroups(IEnumerable<InternalDocumentSendListAccessGroup> accesses)
        {
            return accesses?.Any() ?? false ? accesses.Select(GetDbDocumentSendListAccessGroup) : null;
        }

        public static DocumentEvents GetDbDocumentEvent(InternalDocumentEvent evt)
        {
            return evt == null ? null :
                new DocumentEvents
                {
                    Id = evt.Id,
                    EntityTypeId = evt.EntityTypeId,
                    ClientId = evt.ClientId,
                    TaskId = evt.TaskId,
                    Description = evt.Description,
                    AddDescription = evt.AddDescription,
                    Date = evt.Date,
                    CreateDate = evt.CreateDate,
                    DocumentId = evt.DocumentId,
                    EventTypeId = (int)evt.EventType,
                    LastChangeDate = evt.LastChangeDate,
                    LastChangeUserId = evt.LastChangeUserId,
                    TargetAgentId = evt.TargetAgentId,
                    TargetPositionId = evt.TargetPositionId,
                    TargetPositionExecutorAgentId = evt.TargetPositionExecutorAgentId,
                    TargetPositionExecutorTypeId = evt.TargetPositionExecutorTypeId,
                    SourceAgentId = evt.SourceAgentId,
                    SourcePositionId = evt.SourcePositionId,
                    SourcePositionExecutorAgentId = evt.SourcePositionExecutorAgentId,
                    SourcePositionExecutorTypeId = evt.SourcePositionExecutorTypeId,
                    ReadAgentId = evt.ReadAgentId,
                    ReadDate = evt.ReadDate,
                    Accesses = GetDbDocumentEventAccesses(evt.Accesses)?.ToList(),
                    AccessGroups = GetDbDocumentEventAccessGroups(evt.AccessGroups)?.ToList(),
                    PaperId = evt.PaperId,
                    PaperPlanAgentId = evt.PaperPlanAgentId,
                    PaperPlanDate = evt.PaperPlanDate,
                    PaperSendAgentId = evt.PaperSendAgentId,
                    PaperSendDate = evt.PaperSendDate,
                    PaperRecieveAgentId = evt.PaperRecieveAgentId,
                    PaperRecieveDate = evt.PaperRecieveDate,
                    SendListId = evt.SendListId,
                    ParentEventId = evt.ParentEventId,
                };
        }

        public static IEnumerable<DocumentEvents> GetDbDocumentEvents(IEnumerable<InternalDocumentEvent> events)
        {
            return events?.Any() ?? false ? events.Select(GetDbDocumentEvent) : null;
        }

        public static DocumentTasks GetDbDocumentTask(InternalDocumentTask task)
        {
            return task == null ? null :
                new DocumentTasks
                {
                    Id = task.Id,
                    EntityTypeId = task.EntityTypeId,
                    ClientId = task.ClientId,
                    Task = task.Name,
                    Description = task.Description,
                    DocumentId = task.DocumentId,
                    LastChangeDate = task.LastChangeDate,
                    LastChangeUserId = task.LastChangeUserId,
                    AgentId = task.AgentId,
                    PositionId = task.PositionId,
                    PositionExecutorAgentId = task.PositionExecutorAgentId,
                    PositionExecutorTypeId = task.PositionExecutorTypeId,
                };
        }

        public static IEnumerable<DocumentTasks> GetDbDocumentTasks(IEnumerable<InternalDocumentTask> tasks)
        {
            return tasks?.Any() ?? false ? tasks.Select(GetDbDocumentTask) : null;
        }

        public static PropertyValues GetDbPropertyValue(InternalPropertyValue propVal)
        {
            return propVal == null ? null :
                new PropertyValues
                {
                    Id = propVal.Id,
                    PropertyLinkId = propVal.PropertyLinkId,
                    RecordId = propVal.RecordId,
                    ValueString = propVal.ValueString,
                    ValueDate = propVal.ValueDate,
                    ValueNumeric = propVal.ValueNumeric,
                    LastChangeDate = propVal.LastChangeDate,
                    LastChangeUserId = propVal.LastChangeUserId,
                };
        }

        public static IEnumerable<PropertyValues> GetDbPropertyValue(IEnumerable<InternalPropertyValue> propVals)
        {
            return propVals?.Any() ?? false ? propVals.Select(GetDbPropertyValue) : null;
        }
        public static DocumentWaits GetDbDocumentWait(InternalDocumentWait wait)
        {
            var waitDb = wait == null ? null :
                new DocumentWaits
                {
                    Id = wait.Id,
                    EntityTypeId = wait.EntityTypeId,
                    ClientId = wait.ClientId,
                    AttentionDate = wait.AttentionDate,
                    DocumentId = wait.DocumentId,
                    DueDate = wait.DueDate ?? DateTime.MaxValue,
                    PlanDueDate = wait.PlanDueDate,
                    LastChangeDate = wait.LastChangeDate,
                    LastChangeUserId = wait.LastChangeUserId,
                    OffEventId = wait.OffEventId,
                    OnEventId = wait.OnEventId,
                    ParentId = wait.ParentId,
                    ResultTypeId = wait.ResultTypeId,
                    ParentWait = GetDbDocumentWait(wait.ParentWait),
                    OnEvent = GetDbDocumentEvent(wait.OnEvent),
                    OffEvent = GetDbDocumentEvent(wait.OffEvent),
                    TargetDescription = wait.TargetDescription,
                    //TargetAttentionDate = wait.TargetAttentionDate,
                };
            return waitDb;
        }

        public static IEnumerable<DocumentWaits> GetDbDocumentWaits(IEnumerable<InternalDocumentWait> waits)
        {
            return waits?.Any() ?? false ? waits.Select(GetDbDocumentWait) : null;
        }

        public static DocumentSubscriptions GetDbDocumentSubscription(InternalDocumentSubscription subscription)
        {
            return subscription == null ? null :
                new DocumentSubscriptions
                {
                    Id = subscription.Id,
                    EntityTypeId = subscription.EntityTypeId,
                    ClientId = subscription.ClientId,
                    DocumentId = subscription.DocumentId,
                    LastChangeDate = subscription.LastChangeDate,
                    LastChangeUserId = subscription.LastChangeUserId,
                    DoneEventId = subscription.DoneEventId,
                    DoneEvent = GetDbDocumentEvent(subscription.DoneEvent),
                    SendEventId = subscription.SendEventId,
                    SendEvent = GetDbDocumentEvent(subscription.SendEvent),
                    Description = subscription.Description,
                    SubscriptionStateId = (int)subscription.SubscriptionStates,
                    Hash = subscription.Hash,
                    ChangedHash = subscription.ChangedHash,
                    SigningTypeId = (int)subscription.SigningType,
                };
        }

        public static IEnumerable<DocumentSubscriptions> GetDbDocumentSubscriptions(IEnumerable<InternalDocumentSubscription> subscriptions)
        {
            return subscriptions?.Any() ?? false ? subscriptions.Select(GetDbDocumentSubscription) : null;
        }

        public static DocumentSendLists GetDbDocumentSendList(InternalDocumentSendList sendList, bool isIncludeAccessGroups)
        {
            return sendList == null ? null :
                new DocumentSendLists
                {
                    Id = sendList.Id,
                    EntityTypeId = sendList.EntityTypeId,
                    ClientId = sendList.ClientId,
                    DocumentId = sendList.DocumentId,
                    Stage = sendList.Stage.Value,
                    SendTypeId = (int)sendList.SendType,
                    StageTypeId = (int?)sendList.StageType,
                    TaskId = sendList.TaskId,
                    IsWorkGroup = sendList.IsWorkGroup,
                    IsAddControl = sendList.IsAddControl,
                    SelfDescription = sendList.SelfDescription,
                    SelfDueDate = sendList.SelfDueDate,
                    SelfDueDay = sendList.SelfDueDay,
                    SelfAttentionDate = sendList.SelfAttentionDate,
                    SelfAttentionDay = sendList.SelfAttentionDay,
                    Description = sendList.Description,
                    AddDescription = sendList.AddDescription,
                    DueDate = sendList.DueDate,
                    DueDay = sendList.DueDay,
                    AccessLevelId = (int)sendList.AccessLevel,
                    IsInitial = sendList.IsInitial,
                    StartEventId = sendList.StartEventId,
                    CloseEventId = sendList.CloseEventId,

                    SourceAgentId = sendList.SourceAgentId,
                    SourcePositionId = sendList.SourcePositionId.Value,
                    SourcePositionExecutorAgentId = sendList.SourcePositionExecutorAgentId,
                    SourcePositionExecutorTypeId = sendList.SourcePositionExecutorTypeId,
                    TargetAgentId = sendList.TargetAgentId,
                    TargetPositionId = sendList.TargetPositionId,
                    TargetPositionExecutorAgentId = sendList.TargetPositionExecutorAgentId,
                    TargetPositionExecutorTypeId = sendList.TargetPositionExecutorTypeId,

                    AccessGroups = isIncludeAccessGroups ? GetDbDocumentSendListAccessGroups(sendList.AccessGroups)?.ToList() : null,

                    LastChangeUserId = sendList.LastChangeUserId,
                    LastChangeDate = sendList.LastChangeDate
                };
        }

        public static IEnumerable<DocumentSendLists> GetDbDocumentSendLists(IEnumerable<InternalDocumentSendList> sendLists, bool isIncludeAccessGroups)
        {
            return sendLists?.Any() ?? false ? sendLists.Select(x=>GetDbDocumentSendList(x,isIncludeAccessGroups)) : null;
        }

        public static DocumentRestrictedSendLists GetDbDocumentRestrictedSendList(InternalDocumentRestrictedSendList sendList)
        {
            return sendList == null ? null :
                new DocumentRestrictedSendLists
                {
                    Id = sendList.Id,
                    EntityTypeId = sendList.EntityTypeId,
                    ClientId = sendList.ClientId,
                    PositionId = sendList.PositionId,
                    AccessLevelId = (int)sendList.AccessLevel,
                    LastChangeUserId = sendList.LastChangeUserId,
                    LastChangeDate = sendList.LastChangeDate,
                    DocumentId = sendList.DocumentId,
                };
        }

        public static IEnumerable<DocumentRestrictedSendLists> GetDbDocumentRestrictedSendLists(IEnumerable<InternalDocumentRestrictedSendList> sendLists)
        {
            return sendLists?.Any() ?? false ? sendLists.Select(GetDbDocumentRestrictedSendList) : null;
        }

        public static DocumentFiles GetDbDocumentFile(InternalDocumentAttachedFile docFile)
        {

            var res = new DocumentFiles
            {
                Id = docFile.Id,
                EntityTypeId = docFile.EntityTypeId,
                ClientId = docFile.ClientId,
                DocumentId = docFile.DocumentId,
                OrderNumber = docFile.OrderInDocument,
                Version = docFile.Version,
                Extension = docFile.Extension,
                Hash = docFile.Hash,
                FileType = docFile.FileType,
                FileSize = docFile.FileSize,
                TypeId = (int)docFile.Type,
                IsDeleted = docFile.IsDeleted,
                IsMainVersion = docFile.IsMainVersion,
                IsWorkedOut = docFile.IsWorkedOut,
                Description = docFile.Description,
                LastChangeDate = docFile.LastChangeDate,
                LastChangeUserId = docFile.LastChangeUserId,
                Name = docFile.Name,
                Date = docFile.Date,
                ExecutorPositionId = docFile.ExecutorPositionId,
                ExecutorPositionExecutorAgentId = docFile.ExecutorPositionExecutorAgentId,
                ExecutorPositionExecutorTypeId = docFile.ExecutorPositionExecutorTypeId,
                LastPdfAccessDate = docFile.LastPdfAccess,
                IsPdfCreated = docFile.PdfCreated,
            };

            return res;
        }

        public static IEnumerable<DocumentFiles> GetDbDocumentFiles(IEnumerable<InternalDocumentAttachedFile> docFile)
        {
            return docFile.Select(GetDbDocumentFile);
        }

        public static DocumentPapers GetDbDocumentPaper(InternalDocumentPaper item)
        {
            return item == null ? null :
                new DocumentPapers
                {
                    Id = item.Id,
                    EntityTypeId = item.EntityTypeId,
                    ClientId = item.ClientId,
                    DocumentId = item.DocumentId,
                    Name = item.Name,
                    Description = item.Description,
                    IsMain = item.IsMain,
                    IsOriginal = item.IsOriginal,
                    IsCopy = item.IsCopy,
                    PageQuantity = item.PageQuantity,
                    OrderNumber = item.OrderNumber,
                    LastPaperEventId = item.LastPaperEventId,
                    IsInWork = item.IsInWork,
                    LastChangeDate = item.LastChangeDate,
                    LastChangeUserId = item.LastChangeUserId
                };
        }

        public static IEnumerable<DocumentPapers> GetDbDocumentPapers(IEnumerable<InternalDocumentPaper> papers)
        {
            return papers?.Any() ?? false ? papers.Select(GetDbDocumentPaper) : null;
        }

        public static TemplateDocumentPapers GetDbTemplateDocumentPaper(InternalTemplateDocumentPaper item)
        {
            return item == null ? null :
                new TemplateDocumentPapers
                {
                    Id = item.Id,
                    DocumentId = item.DocumentId,
                    Name = item.Name,
                    Description = item.Description,
                    IsMain = item.IsMain,
                    IsOriginal = item.IsOriginal,
                    IsCopy = item.IsCopy,
                    PageQuantity = item.PageQuantity,
                    OrderNumber = item.OrderNumber,
                    LastChangeDate = item.LastChangeDate,
                    LastChangeUserId = item.LastChangeUserId
                };
        }

        public static IEnumerable<TemplateDocumentPapers> GetDbTemplateDocumentPapers(IEnumerable<InternalTemplateDocumentPaper> papers)
        {
            return papers?.Any() ?? false ? papers.Select(GetDbTemplateDocumentPaper) : null;
        }

        public static DocumentPaperLists GetDbDocumentPaperList(InternalDocumentPaperList item)
        {
            return item == null ? null :
                new DocumentPaperLists
                {
                    Id = item.Id,
                    ClientId = item.ClientId,
                    Date = item.Date,
                    Description = item.Description,
                    LastChangeDate = item.LastChangeDate,
                    LastChangeUserId = item.LastChangeUserId
                };
        }

        public static TemplateDocumentFiles GetDbTemplateFile(InternalTemplateAttachedFile docFile)
        {
            return new TemplateDocumentFiles
            {
                Id = docFile.Id,
                DocumentId = docFile.DocumentId,
                OrderNumber = docFile.OrderInDocument,
                Extention = docFile.Extension,
                Hash = docFile.Hash,
                FileType = docFile.FileType,
                FileSize = docFile.FileSize,
                TypeId = (int)docFile.Type,
                LastChangeDate = docFile.LastChangeDate,
                LastChangeUserId = docFile.LastChangeUserId,
                Name = docFile.Name,
                Description = docFile.Description,
                IsPdfCreated = docFile.PdfCreated,
                LastPdfAccessDate = docFile.LastPdfAccess
            };
        }

        public static EncryptionCertificates GetDbEncryptionCertificate(InternalEncryptionCertificate item)
        {
            return new EncryptionCertificates
            {
                Id = item.Id,
                Name = item.Name,
                Thumbprint = item.Thumbprint,
                CreateDate = item.CreateDate,
                NotBefore = item.NotBefore,
                NotAfter = item.NotAfter,
                AgentId = item.AgentId,
                Certificate = item.CertificateZip,
                IsRememberPassword = item.IsRememberPassword,

                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
            };
        }

        public static IEnumerable<EncryptionCertificates> GetDbEncryptionCertificates(IEnumerable<InternalEncryptionCertificate> items)
        {
            return items.Select(GetDbEncryptionCertificate);
        }

    }
}