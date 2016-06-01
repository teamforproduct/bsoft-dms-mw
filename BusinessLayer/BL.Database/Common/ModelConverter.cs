using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Template;
using BL.Model.DocumentCore.InternalModel;
using BL.Database.DBModel.System;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Common
{
    public static class ModelConverter
    {

        public static DBModel.Document.Documents GetDbDocument(InternalDocument document)
        {
            return document == null ? null :
                new DBModel.Document.Documents
                {
                    TemplateDocumentId = document.TemplateDocumentId,
                    CreateDate = document.CreateDate,
                    DocumentSubjectId = document.DocumentSubjectId,
                    Description = document.Description,
                    IsRegistered = document.IsRegistered,
                    RegistrationJournalId = document.RegistrationJournalId,
                    RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                    RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                    RegistrationDate = document.RegistrationDate,
                    ExecutorPositionId = document.ExecutorPositionId,
                    ExecutorPositionExecutorAgentId = document.ExecutorPositionExecutorAgentId,
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

        public static DocumentEvents GetDbDocumentEvent(InternalDocumentEvent evt)
        {
            return evt == null ? null :
                new DocumentEvents
                {
                    TaskId = evt.TaskId,
                    IsAvailableWithinTask = evt.IsAvailableWithinTask,
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
                    SourceAgentId = evt.SourceAgentId,
                    SourcePositionId = evt.SourcePositionId,
                    SourcePositionExecutorAgentId = evt.SourcePositionExecutorAgentId,
                    ReadAgentId = evt.ReadAgentId,
                    ReadDate = evt.ReadDate,

                    Id = evt.Id,
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
                    Task = task.Name,
                    Description = task.Description,
                    DocumentId = task.DocumentId,
                    LastChangeDate = task.LastChangeDate,
                    LastChangeUserId = task.LastChangeUserId,
                    AgentId = task.AgentId,
                    PositionId = task.PositionId,
                    PositionExecutorAgentId = task.PositionExecutorAgentId,
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
                    AttentionDate = wait.AttentionDate,
                    DocumentId = wait.DocumentId,
                    DueDate = wait.DueDate,
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
                };
        }

        public static IEnumerable<DocumentSubscriptions> GetDbDocumentSubscriptions(IEnumerable<InternalDocumentSubscription> subscriptions)
        {
            return subscriptions?.Any() ?? false ? subscriptions.Select(GetDbDocumentSubscription) : null;
        }

        public static DocumentSendLists GetDbDocumentSendList(InternalDocumentSendList sendList)
        {
            return sendList == null ? null :
                new DocumentSendLists
                {
                    Id = sendList.Id,
                    DocumentId = sendList.DocumentId,
                    Stage = sendList.Stage,
                    SendTypeId = (int)sendList.SendType,

                    TaskId = sendList.TaskId,
                    IsAvailableWithinTask = sendList.IsAvailableWithinTask,
                    IsWorkGroup = sendList.IsWorkGroup,
                    IsAddControl = sendList.IsAddControl,
                    SelfDueDate = sendList.SelfDueDate,
                    SelfDueDay = sendList.SelfDueDay,
                    SelfAttentionDate = sendList.SelfAttentionDate,

                    Description = sendList.Description,
                    DueDate = sendList.DueDate,
                    DueDay = sendList.DueDay,
                    AccessLevelId = (int)sendList.AccessLevel,
                    IsInitial = sendList.IsInitial,
                    StartEventId = sendList.StartEventId,
                    CloseEventId = sendList.CloseEventId,

                    SourceAgentId = sendList.SourceAgentId,
                    SourcePositionId = sendList.SourcePositionId,
                    SourcePositionExecutorAgentId = sendList.SourcePositionExecutorAgentId,
                    TargetAgentId = sendList.TargetAgentId,
                    TargetPositionId = sendList.TargetPositionId,
                    TargetPositionExecutorAgentId = sendList.TargetPositionExecutorAgentId,
                    LastChangeUserId = sendList.LastChangeUserId,
                    LastChangeDate = sendList.LastChangeDate
                };
        }

        public static IEnumerable<DocumentSendLists> GetDbDocumentSendLists(IEnumerable<InternalDocumentSendList> sendLists)
        {
            return sendLists?.Any() ?? false ? sendLists.Select(GetDbDocumentSendList) : null;
        }

        public static DocumentRestrictedSendLists GetDbDocumentRestrictedSendList(InternalDocumentRestrictedSendList sendList)
        {
            return sendList == null ? null :
                new DocumentRestrictedSendLists
                {
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
            return new DocumentFiles
            {
                Id = docFile.Id,
                DocumentId = docFile.DocumentId,
                OrderNumber = docFile.OrderInDocument,
                Version = docFile.Version,
                Extension = docFile.Extension,
                Hash = docFile.Hash,
                FileType = docFile.FileType,
                FileSize = docFile.FileSize,
                IsAdditional = docFile.IsAdditional,
                LastChangeDate = docFile.LastChangeDate,
                LastChangeUserId = docFile.LastChangeUserId,
                Name = docFile.Name,
                Date = docFile.Date,
                ExecutorPositionId = docFile.ExecutorPositionId,
                ExecutorPositionExecutorAgentId = docFile.ExecutorPositionExecutorAgentId
            };
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

        public static DocumentPaperLists GetDbDocumentPaperList(InternalDocumentPaperList item)
        {
            return item == null ? null :
                new DocumentPaperLists
                {
                    Id = item.Id,
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
                IsAdditional = docFile.IsAdditional,
                LastChangeDate = docFile.LastChangeDate,
                LastChangeUserId = docFile.LastChangeUserId,
                Name = docFile.Name

            };
        }

    }
}