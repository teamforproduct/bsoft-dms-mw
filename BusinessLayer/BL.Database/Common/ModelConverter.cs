using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Database.Common
{
    public static class ModelConverter
    {
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
                    Task = evt.Task,
                    Description = evt.Description,
                    Date = evt.Date,
                    CreateDate = evt.CreateDate,
                    DocumentId = evt.DocumentId,
                    EventTypeId = (int)evt.EventType,
                    LastChangeDate = evt.LastChangeDate,
                    LastChangeUserId = evt.LastChangeUserId,
                    TargetAgentId = evt.TargetAgentId,
                    TargetPositionId = evt.TargetPositionId,
                    SourceAgentId = evt.SourceAgentId,
                    SourcePositionId = evt.SourcePositionId,
                    ReadAgentId = evt.ReadAgentId,
                    ReadDate = evt.ReadDate
                };
        }

        public static IEnumerable<DocumentEvents> GetDbDocumentEvents(IEnumerable<InternalDocumentEvent> events)
        {
            return events?.Any() ?? false ? events.Select(GetDbDocumentEvent) : null;
        }

        public static DocumentWaits GetDbDocumentWait(InternalDocumentWait wait)
        {
            return wait == null ? null :
                new DocumentWaits
                {
                    AttentionDate = wait.AttentionDate,
                    DocumentId = wait.DocumentId,
                    DueDate = wait.DueDate,
                    LastChangeDate = wait.LastChangeDate,
                    LastChangeUserId = wait.LastChangeUserId,
                    OffEventId = wait.OffEventId,
                    OnEventId = wait.OnEventId,
                    ParentId = wait.ParentId,
                    ResultTypeId = wait.ResultTypeId,
                    OnEvent = GetDbDocumentEvent(wait.OnEvent),
                    OffEvent = GetDbDocumentEvent(wait.OffEvent),
                };
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
                    Hash = subscription.Hash,
                    ChangedHash = subscription.ChangedHash,
                };
        }

        public static IEnumerable<DocumentSubscriptions> GetDbDocumentSubscriptions(IEnumerable<InternalDocumentSubscription> subscriptions)
        {
            return subscriptions?.Any() ?? false ? subscriptions.Select(GetDbDocumentSubscription) : null;
        }

        public static void UpdateDbDocumentWaitByEvents(DocumentWaits docWait, InternalDocumentWait documentWait)
        {
            if (documentWait.OnEvent != null)
            {
                docWait.OnEvent = GetDbDocumentEvent(documentWait.OnEvent);
            }
            if (documentWait.OffEvent != null)
            {
                docWait.OffEvent = GetDbDocumentEvent(documentWait.OffEvent);
            }
        }

        public static IEnumerable<DocumentSendLists> AddDocumentSendList(IEnumerable<InternalDocumentSendList> docSendList)
        {
            return docSendList.Select(sl => new DocumentSendLists()
            {
                DocumentId = sl.DocumentId,
                Stage = sl.Stage,
                SendTypeId = (int)sl.SendType,
                TargetPositionId = sl.TargetPositionId,
                Task = sl.Task,
                Description = sl.Description,
                DueDate = sl.DueDate,
                DueDay = sl.DueDay,
                AccessLevelId = (int)sl.AccessLevel,
                IsInitial = sl.IsInitial,
                StartEventId = sl.StartEventId,
                CloseEventId = sl.CloseEventId,

                SourceAgentId = sl.SourceAgentId,
                SourcePositionId = sl.SourcePositionId,
                TargetAgentId = sl.TargetAgentId,

                LastChangeUserId = sl.LastChangeUserId,
                LastChangeDate = sl.LastChangeDate
            });
        }

        public static IEnumerable<DocumentRestrictedSendLists> AddDocumentRestrictedSendList(IEnumerable<InternalDocumentRestrictedSendList> docRestrictedSendList)
        {
            return docRestrictedSendList.Select(sl => new DocumentRestrictedSendLists
            {
                PositionId = sl.PositionId,
                AccessLevelId = (int)sl.AccessLevel,
                LastChangeUserId = sl.LastChangeUserId,
                LastChangeDate = sl.LastChangeDate,
                DocumentId = sl.DocumentId,
            });
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
                Date = docFile.Date
            };
        }

        public static IEnumerable<DocumentFiles> GetDbDocumentFiles(IEnumerable<InternalDocumentAttachedFile> docFile)
        {
            return docFile.Select(GetDbDocumentFile);
        }
    }
}