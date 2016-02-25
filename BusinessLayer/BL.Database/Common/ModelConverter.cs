using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Database.Common
{
    public static class ModelConverter
    {
        public static DocumentAccesses GetDbDocumentAccess(InternalDocumentAccess docAccess)
        {
            return docAccess == null ? null :
                new DocumentAccesses
                {
                    LastChangeDate = docAccess.LastChangeDate,
                    LastChangeUserId = docAccess.LastChangeUserId,
                    DocumentId = docAccess.DocumentId,
                    IsFavourite = docAccess.IsFavourite,
                    IsInWork = docAccess.IsInWork,
                    AccessLevelId = (int)docAccess.AccessLevel,
                    PositionId = docAccess.PositionId
                };
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(IEnumerable<InternalDocumentAccess> docAccesses)
        {
            return docAccesses.Select(GetDbDocumentAccess);
        }

        public static DocumentEvents GetDbDocumentEvent(InternalDocumentEvent evt)
        {
            return evt == null ? null :
                new DocumentEvents
                {
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
            return events.Select(GetDbDocumentEvent);
        }

        public static DocumentWaits GetDbDocumentWait(InternalDocumentWait docWait)
        {
            return docWait == null ? null :
                new DocumentWaits
            {
                AttentionDate = docWait.AttentionDate,
                Task = docWait.Task,
                DocumentId = docWait.DocumentId,
                DueDate = docWait.DueDate,
                LastChangeDate = docWait.LastChangeDate,
                LastChangeUserId = docWait.LastChangeUserId,
                OffEventId = docWait.OffEventId,
                OnEventId = docWait.OnEventId,
                ParentId = docWait.ParentId,
                ResultTypeId = docWait.ResultTypeId,
                OnEvent = GetDbDocumentEvent(docWait.OnEvent),
                OffEvent = GetDbDocumentEvent(docWait.OffEvent),
            };
        }

        public static IEnumerable<DocumentWaits> GetDbDocumentWaits(IEnumerable<InternalDocumentWait> docWaits)
        {
            return docWaits.Select(GetDbDocumentWait);
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
                Description = sl.Description,
                DueDate = sl.DueDate,
                DueDay = sl.DueDay,
                AccessLevelId = (int)sl.AccessLevel,
                IsInitial = sl.IsInitial,
                StartEventId = sl.StartEventId,
                CloseEventId = sl.CloseEventId,
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