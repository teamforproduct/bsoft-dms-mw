﻿using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Database.Common
{
    public static class ModelConverter
    {
        public static DocumentAccesses GetDbDocumentAccess(InternalDocumentAccesses docAccess)
        {
            return new DocumentAccesses
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

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(IEnumerable<InternalDocumentAccesses> docAccesses)
        {
            return docAccesses.Select(GetDbDocumentAccess);
        }

        public static DocumentEvents GetDbDocumentEvent(InternalDocumentEvents evt)
        {
            return new DocumentEvents
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

        public static IEnumerable<DocumentEvents> GetDbDocumentEvents(IEnumerable<InternalDocumentEvents> events)
        {
            return events.Select(GetDbDocumentEvent);
        }

        public static DocumentWaits GetDbDocumentWait(InternalDocumentWaits docWait)
        {
            return new DocumentWaits
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
                ResultTypeId = docWait.ResultTypeId
            };
        }

        public static IEnumerable<DocumentWaits> GetDbDocumentWaitses(IEnumerable<InternalDocumentWaits> docWaits)
        {
            return docWaits.Select(GetDbDocumentWait);
        }

        public static IEnumerable<DocumentSendLists> AddDocumentSendList(IEnumerable<InternalDocumentSendLists> docSendList)
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

        public static IEnumerable<DocumentRestrictedSendLists> AddDocumentRestrictedSendList(IEnumerable<InternalDocumentRestrictedSendLists> docRestrictedSendList)
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

    }
}