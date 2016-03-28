using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Database.DBModel.System;
using BL.Model.FullTextSerach;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        public static IQueryable<DocumentQuery> GetDocumentQuery(DmsContext dbContext, IContext ctx, IQueryable<FrontDocumentAccess> userAccesses = null)
        {
            var qry = from dc in dbContext.DocumentsSet
                      join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                      join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                      join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                      join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                      join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                      from exAg in ea.DefaultIfEmpty()

                      join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                      from docsubj in eg.DefaultIfEmpty()

                      join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                          egg
                      from regj in egg.DefaultIfEmpty()

                      join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                      from sendAg in ag.DefaultIfEmpty()

                      join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                      from sendAp in ap.DefaultIfEmpty()

                      where dbContext.DocumentAccessesSet.Where(x => ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId)).Select(x => x.DocumentId).Contains(dc.Id)

                      select new DocumentQuery
                      {
                          Doc = dc,
                          Templ = tmpl,
                          DirName = ddir.Name,
                          SubjName = docsubj.Name,
                          DocTypeName = doctp.Name,
                          RegistrationJournalName = regj.Name,
                          RegistrationJournalNumerationPrefixFormula = regj.NumerationPrefixFormula,
                          RegistrationJournalPrefixFormula = regj.PrefixFormula,
                          RegistrationJournalSuffixFormula = regj.SuffixFormula,
                          ExecutorPosName = executor.Name,
                          ExecutorPositionExecutorAgentName = dc.ExecutorPositionExecutorAgent.Name,
                          //ExecutorPositionExecutorNowAgentName = executor.ExecutorAgent.Name,
                          ExecutorPositionExecutorNowAgentName = exAg.Name,
                          SenderAgentname = sendAg.Name,
                          SenderPersonName = sendAp.FullName
                      };
            if (userAccesses != null)
            {
                qry = qry.Where(x => userAccesses.Select(a => a.DocumentId).Contains(x.Doc.Id));
            }
            return qry;
        }

        public static IQueryable<FilterDocumentFileIdentity> GetDocumentFilesMaxVersion(DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var qry = dbContext.DocumentFilesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.AttachedFileId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AttachedFileId.Contains(x.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            return qry
                .GroupBy(g => new { g.DocumentId, g.OrderNumber })
                .Select(x => new FilterDocumentFileIdentity { DocumentId = x.Key.DocumentId, OrderInDocument = x.Key.OrderNumber, Version = x.Max(s => s.Version) });
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var sq = GetDocumentFilesMaxVersion(dbContext, filter);

            return
                sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocumentId, OrderNumber = sub.OrderInDocument, sub.Version },
                    fl => new { fl.DocumentId, fl.OrderNumber, fl.Version },
                    (s, f) => new { fl = f })
                    .Join(dbContext.DictionaryAgentsSet, df => df.fl.LastChangeUserId, da => da.Id,
                        (d, a) => new { d.fl, agName = a.Name })
                    .Select(x => new FrontDocumentAttachedFile
                    {
                        Id = x.fl.Id,
                        Date = x.fl.Date,
                        DocumentId = x.fl.DocumentId,
                        Extension = x.fl.Extension,
                        FileContent = x.fl.Content,
                        FileType = x.fl.FileType,
                        FileSize = x.fl.FileSize,
                        IsAdditional = x.fl.IsAdditional,
                        Hash = x.fl.Hash,
                        LastChangeDate = x.fl.LastChangeDate,
                        LastChangeUserId = x.fl.LastChangeUserId,
                        LastChangeUserName = x.agName,
                        Name = x.fl.Name,
                        OrderInDocument = x.fl.OrderNumber,
                        Version = x.fl.Version,
                        WasChangedExternal = false
                    }).ToList();
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFilesMaxVersion(dbContext, new FilterDocumentAttachedFile { DocumentId = new List<int> { documentId } });

            return
                sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocumentId, OrderNumber = sub.OrderInDocument, sub.Version },
                    fl => new { fl.DocumentId, fl.OrderNumber, fl.Version },
                    (s, f) => new { fl = f })
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.fl.Id,
                        Date = x.fl.Date,
                        DocumentId = x.fl.DocumentId,
                        Extension = x.fl.Extension,
                        FileContent = x.fl.Content,
                        FileType = x.fl.FileType,
                        FileSize = x.fl.FileSize,
                        IsAdditional = x.fl.IsAdditional,
                        Hash = x.fl.Hash,
                        LastChangeDate = x.fl.LastChangeDate,
                        LastChangeUserId = x.fl.LastChangeUserId,
                        Name = x.fl.Name,
                        OrderInDocument = x.fl.OrderNumber,
                        Version = x.fl.Version,
                        WasChangedExternal = false
                    }).ToList();
        }

        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext)
        {
            return
                dbContext.DocumentAccessesSet.Where(x => ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId))
                .Select(acc => new FrontDocumentAccess
                {
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name
                });
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(DmsContext dbContext, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            if (docAccesses == null || !docAccesses.Any()) return null;
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x => !accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
        }

        public static InternalDocumentAccess GetInternalDocumentAccess(IContext ctx, DmsContext dbContext, int documentId)
        {

            var acc =
                dbContext.DocumentAccessesSet.FirstOrDefault(
                    x => x.DocumentId == documentId && x.PositionId == ctx.CurrentPositionId);
            if (acc != null)
            {
                return new InternalDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId,
                };
            }

            return null;
        }

        public static IQueryable<DocumentAccesses> GetDocumentAccessesesQry(DmsContext dbContext, int documentId, IContext ctx = null)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.DocumentId == documentId);
            if (ctx != null)
            {
                qry = qry.Where(x => ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId));
            }
            return qry;
        }

        public static IEnumerable<InternalDocumentAccess> GetInternalDocumentAccesses(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentAccessesSet.Where(
                x => x.DocumentId == documentId).Select(acc => new InternalDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId
                }).ToList();
        }

        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(DmsContext dbContext, List<int> positionIds)
        {
            return dbContext.DictionaryPositionsSet.Where(x => positionIds.Contains(x.Id)).Select(x => new InternalPositionInfo
            {
                PositionId = x.Id,
                PositionName = x.Name,
                AgentId = x.ExecutorAgentId ?? 0,
                AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name : ""
            }).ToList();
        }

        public static IQueryable<DocumentEvents> GetDocumentEventsQuery(IContext ctx, DmsContext dbContext)
        {
            return dbContext.DocumentEventsSet
                    .Where(x => ctx.IsAdmin || (x.TargetPositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.TargetPositionId.Value))
                    || (x.SourcePositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.SourcePositionId.Value))
                    //|| ctx.CurrentAgentId == x.SourceAgentId
                    //|| (x.TargetAgentId.HasValue && ctx.CurrentAgentId == x.TargetAgentId.Value)
                    )
                    .AsQueryable();
        }

        //        public static IEnumerable<FrontDocumentEvent> GetDocumentEvents(DmsContext dbContext, FilterDocumentEvent filter)
        //        {
        //            var qry = dbContext.DocumentEventsSet.AsQueryable();

        //            if (filter != null)
        //            {
        //                if (filter.EventId?.Count > 0)
        //                {
        //                    qry = qry.Where(x => filter.EventId.Contains(x.Id));
        //                }

        //                if (filter.DocumentId?.Count > 0)
        //                {
        //                    qry = qry.Where(x => filter.DocumentId.Contains(x.DocumentId));
        //                }
        //            }
        //            return qry.Select(x => new FrontDocumentEvent
        //            {
        //                Id = x.Id,
        //                DocumentId = x.DocumentId,
        //                TaskName = x.TaskName,
        //                Description = x.Description,
        //                EventType = (EnumEventTypes)x.EventTypeId,
        //                EventTypeName = x.EventType.Name,
        //                ImportanceEventType = (EnumImportanceEventTypes)x.EventType.ImportanceEventTypeId,
        ////                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
        //                CreateDate = x.CreateDate,
        //                Date = x.Date,
        //                SourceAgentName = x.SourceAgent.Name,
        //                SourceAgentId = x.SourceAgentId,
        //                SourcePositionId = x.SourcePositionId,
        //                SourcePositionName = x.SourcePosition.Name,
        //                SourcePositionExecutorAgentName = x.SourcePosition.ExecutorAgent.Name,
        //                TargetAgentName = x.TargetAgent.Name,
        //                TargetAgentId = x.TargetAgentId,
        //                TargetPositionId = x.TargetPositionId,
        //                TargetPositionName = x.TargetPosition.Name,
        //                TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
        //            }).ToList();

        //        }

        public static IQueryable<DocumentWaits> GetDocumentWaitsQuery(DmsContext dbContext, IContext ctx = null, int? documentId = null)
        {
            var qry = dbContext.DocumentWaitsSet.AsQueryable();
            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId.Value);
            }
            if (ctx != null)
            {
                qry = qry.Where(x => ctx.IsAdmin ||
                           (x.OnEvent.TargetPositionId.HasValue &&
                            ctx.CurrentPositionsIdList.Contains(x.OnEvent.TargetPositionId.Value))
                           ||
                           (x.OnEvent.SourcePositionId.HasValue &&
                            ctx.CurrentPositionsIdList.Contains(x.OnEvent.SourcePositionId.Value))
                            ||
                            (x.OffEventId.HasValue && (
                            (x.OffEvent.TargetPositionId.HasValue &&
                             ctx.CurrentPositionsIdList.Contains(x.OffEvent.TargetPositionId.Value))
                            ||
                            (x.OffEvent.SourcePositionId.HasValue &&
                             ctx.CurrentPositionsIdList.Contains(x.OffEvent.SourcePositionId.Value)))));
            }
            return qry;
        }

        public static IEnumerable<FrontDocumentTask> GetDocumentTasks(DmsContext dbContext, FilterDocumentTask filter)
        {
            var tasksDb = dbContext.DocumentTasksSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    tasksDb = tasksDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            var tasksRes = tasksDb.Select(x => new { Task = x });

            var tasks = tasksRes.Select(x => new FrontDocumentTask
            {
                Id = x.Task.Id,
                DocumentId = x.Task.DocumentId,
                Name = x.Task.Task,
                Description = x.Task.Description,
                DocumentDate = x.Task.Document.RegistrationDate ?? x.Task.Document.CreateDate,
                RegistrationFullNumber = (x.Task.Document.RegistrationNumber != null
                                           ? x.Task.Document.RegistrationNumberPrefix + x.Task.Document.RegistrationNumber +
                                             x.Task.Document.RegistrationNumberSuffix
                                           : "#" + x.Task.Document.Id),
                DocumentDescription = x.Task.Document.Description,
                DocumentTypeName = x.Task.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Task.Document.TemplateDocument.DocumentDirection.Name,

                PositionId = x.Task.PositionId,
                PositionExecutorAgentId = x.Task.PositionExecutorAgentId,
                AgentId = x.Task.AgentId,

                PositionExecutorAgentName = x.Task.PositionExecutorAgent.Name,
                AgentName = x.Task.Agent.Name,
                PositionName = x.Task.Position.Name,
                PositionExecutorNowAgentName = x.Task.Position.ExecutorAgent.Name,
                PositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
            }).ToList();

            return tasks;

        }


        public static IEnumerable<FrontDocumentWait> GetDocumentWaits(DmsContext dbContext, FilterDocumentWait filter)
        {
            var waitsDb = dbContext.DocumentWaitsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    waitsDb = waitsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                if (filter.OnEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OnEventId == filter.OnEventId.Value);
                }

                if (filter.OffEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OffEventId.HasValue && x.OffEventId.Value == filter.OffEventId.Value);
                }

                if (filter.Opened)
                {
                    waitsDb = waitsDb.Where(x => !x.OffEventId.HasValue);
                }
            }

            var waitsRes = waitsDb.Select(x => new { Wait = x, x.OnEvent, x.OffEvent });

            var waits = waitsRes.Select(x => new FrontDocumentWait
            {
                Id = x.Wait.Id,
                DocumentId = x.Wait.DocumentId,
                ParentId = x.Wait.ParentId,
                OnEventId = x.Wait.OnEventId,
                OffEventId = x.Wait.OffEventId,
                ResultTypeId = x.Wait.ResultTypeId,
                ResultTypeName = x.Wait.ResultType.Name,
                DueDate = x.Wait.DueDate,
                AttentionDate = x.Wait.AttentionDate,
                TargetDescription = x.Wait.TargetDescription,
                TargetAttentionDate = x.Wait.TargetAttentionDate,
                IsClosed = x.OffEvent != null,
                DocumentDate = x.Wait.Document.RegistrationDate ?? x.Wait.Document.CreateDate,
                RegistrationFullNumber = (x.Wait.Document.RegistrationNumber != null
                                           ? x.Wait.Document.RegistrationNumberPrefix + x.Wait.Document.RegistrationNumber +
                                             x.Wait.Document.RegistrationNumberSuffix
                                           : "#" + x.Wait.Document.Id),
                DocumentDescription = x.Wait.Document.Description,
                DocumentTypeName = x.Wait.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Wait.Document.TemplateDocument.DocumentDirection.Name,
                OnEvent = x.OnEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OnEvent.Id,
                        DocumentId = x.OnEvent.DocumentId,
                        Task = x.OnEvent.Task.Task,
                        Description = x.OnEvent.Description,
                        EventType = x.OnEvent.EventTypeId,
                        EventTypeName = x.OnEvent.EventType.Name,
                        Date = x.OnEvent.Date,
                        SourcePositionExecutorAgentName = x.OnEvent.SourcePositionExecutorAgent.Name,
                        TargetPositionExecutorAgentName = x.OnEvent.TargetPositionExecutorAgent.Name,

                        ReadAgentName = x.OnEvent.ReadAgent.Name,
                        ReadDate = x.OnEvent.ReadDate,
                        SourceAgentName = x.OnEvent.SourceAgent.Name,

                        SourcePositionName = x.OnEvent.SourcePosition.Name,
                        TargetPositionName = x.OnEvent.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.OnEvent.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = x.OnEvent.TargetPosition.ExecutorAgent.Name,
                        SourcePositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "TargetPositionAgentPhoneNumber", //TODO 

                    },
                OffEvent = x.OffEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        DocumentId = x.OffEvent.DocumentId,
                        Task = null,
                        Description = x.OffEvent.Description,
                        EventType = x.OffEvent.EventTypeId,
                        EventTypeName = x.OffEvent.EventType.Name,
                        Date = x.OffEvent.Date,
                        SourcePositionExecutorAgentName = x.OffEvent.SourcePositionExecutorAgent.Name,
                        TargetPositionExecutorAgentName = x.OffEvent.TargetPositionExecutorAgent.Name,

                        ReadAgentName = x.OnEvent.ReadAgent.Name,
                        ReadDate = x.OnEvent.ReadDate,
                        SourceAgentName = x.OffEvent.SourceAgent.Name,

                        SourcePositionName = null,
                        TargetPositionName = null,
                        SourcePositionExecutorNowAgentName = null,
                        TargetPositionExecutorNowAgentName = null,
                        SourcePositionExecutorAgentPhoneNumber = null,
                        TargetPositionExecutorAgentPhoneNumber = null,

                    }
            }).ToList();

            return waits;

        }

        public static IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(DmsContext dbContext, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.Any())
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            var subscriptionsRes = subscriptionsDb.Select(x => new { Subscription = x, x.SendEvent, x.DoneEvent });

            var subscriptions = subscriptionsRes.Select(x => new FrontDocumentSubscription
            {
                Id = x.Subscription.Id,
                DocumentId = x.Subscription.DocumentId,
                SendEventId = x.Subscription.SendEventId,
                DoneEventId = x.Subscription.DoneEventId,
                SubscriptionStatesId = x.Subscription.SubscriptionStateId,
                SubscriptionStatesName = x.Subscription.SubscriptionState.Name,
                Description = x.Subscription.Description,
                DocumentDate = x.Subscription.Document.RegistrationDate ?? x.Subscription.Document.CreateDate,
                RegistrationFullNumber = (x.Subscription.Document.RegistrationNumber != null
                                           ? x.Subscription.Document.RegistrationNumberPrefix + x.Subscription.Document.RegistrationNumber +
                                             x.Subscription.Document.RegistrationNumberSuffix
                                           : "#" + x.Subscription.Document.Id),
                DocumentDescription = x.Subscription.Document.Description,
                DocumentTypeName = x.Subscription.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Subscription.Document.TemplateDocument.DocumentDirection.Name,
                SendEvent = x.SendEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.SendEvent.Id,
                        DocumentId = x.SendEvent.DocumentId,
                        EventTypeName = x.SendEvent.EventType.Name,
                        TargetPositionExecutorAgentName = x.SendEvent.TargetPositionExecutorAgent.Name,
                        DueDate = x.Subscription.SendEvent.OnWait.FirstOrDefault().DueDate,

                        Date = x.SendEvent.Date,
                        SourcePositionExecutorAgentName = x.SendEvent.SourcePositionExecutorAgent.Name,
                        Description = x.SendEvent.Description,
                        ReadAgentName = x.SendEvent.ReadAgent.Name,
                        ReadDate = x.SendEvent.ReadDate,
                        SourceAgentName = x.SendEvent.SourceAgent.Name,
                        SourcePositionName = x.SendEvent.SourcePosition.Name,
                        TargetPositionName = x.SendEvent.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.SendEvent.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = x.SendEvent.TargetPosition.ExecutorAgent.Name,
                        SourcePositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "TargetPositionAgentPhoneNumber", //TODO 

                    },
                DoneEvent = x.DoneEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.DoneEvent.Id,
                        DocumentId = x.DoneEvent.DocumentId,
                        EventTypeName = x.DoneEvent.EventType.Name,
                        TargetPositionExecutorAgentName = x.DoneEvent.TargetPositionExecutorAgent.Name,
                        DueDate = null,
                        Date = x.DoneEvent.Date,
                        SourcePositionExecutorAgentName = null,
                        Description = x.DoneEvent.Description,


                        ReadAgentName = x.SendEvent.ReadAgent.Name,
                        ReadDate = x.SendEvent.ReadDate,
                        SourceAgentName = x.SendEvent.SourceAgent.Name,

                        SourcePositionName = null,
                        TargetPositionName = null,

                        SourcePositionExecutorNowAgentName = x.SendEvent.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = null,
                        SourcePositionExecutorAgentPhoneNumber = null,
                        TargetPositionExecutorAgentPhoneNumber = null,
                    }
            }).ToList();

            return subscriptions;

        }

        public static IEnumerable<FrontDocumentTag> GetDocumentTags(DmsContext dbContext, FilterDocumentTag filter)
        {
            var tagsDb = dbContext.DocumentTagsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId?.Count() > 0)
                {
                    tagsDb = tagsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    tagsDb = tagsDb.Where(x => !x.Tag.PositionId.HasValue || filter.CurrentPositionsId.Contains(x.Tag.PositionId ?? 0));
                }
            }

            var tagsRes = tagsDb;

            var tags = tagsRes.Select(x => new FrontDocumentTag
            {
                TagId = x.TagId,
                DocumentId = x.DocumentId,
                PositionId = x.Tag.PositionId,
                PositionName = x.Tag.Position.Name,
                Color = x.Tag.Color,
                Name = x.Tag.Name,
                IsSystem = !x.Tag.PositionId.HasValue
            }).ToList();

            return tags;

        }

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(DmsContext dbContext, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.Object.Contains((EnumObjects)x.PropertyLink.ObjectId));
                }

                if (filter.RecordId?.Count > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.RecordId.Contains(x.RecordId));
                }
            }

            var itemsRes = itemsDb;

            var items = itemsRes.Select(x => new FrontPropertyValue
            {
                Id = x.Id,
                RecordId = x.RecordId,
                PropertyLinkId = x.PropertyLinkId,
                Value = x.ValueString != null ? x.ValueString : (x.ValueNumeric.HasValue ? x.ValueNumeric.ToString() : (x.ValueDate.HasValue ? x.ValueDate.ToString() : null)),
                PropertyId = x.PropertyLink.PropertyId,
                ObjectId = x.PropertyLink.ObjectId,
                Filers = x.PropertyLink.Filers,
                IsMandatory = x.PropertyLink.IsMandatory,
                PropertyCode = x.PropertyLink.Property.Code,
                PropertyDescription = x.PropertyLink.Property.Description,
                PropertyLabel = x.PropertyLink.Property.Label,
                PropertyHint = x.PropertyLink.Property.Hint,
                PropertyValueTypeId = x.PropertyLink.Property.ValueTypeId,
                PropertyOutFormat = x.PropertyLink.Property.OutFormat,
                PropertyInputFormat = x.PropertyLink.Property.InputFormat,
                PropertySelectAPI = x.PropertyLink.Property.SelectAPI,
                PropertySelectFilter = x.PropertyLink.Property.SelectFilter,
                PropertySelectFieldCode = x.PropertyLink.Property.SelectFieldCode,
                PropertySelectDescriptionFieldCode = x.PropertyLink.Property.SelectDescriptionFieldCode,
                PropertyValueTypeCode = x.PropertyLink.Property.ValueType.Code,
                PropertyValueTypeDescription = x.PropertyLink.Property.ValueType.Description,
            }).ToList();

            return items;

        }

        public static IEnumerable<BaseDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }

                if (filter.SubordinatedPositions?.Count > 0)
                {
                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(y => filter.SubordinatedPositions.Contains(y.SourcePositionId)),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
                                        )
                             .Where(x => x.subordMax > 0);
                }
            }

            return qry.Select(x => new BaseDictionaryPosition
            {
                Id = x.pos.Id,
                Name = x.pos.Name,
                DepartmentId = x.pos.DepartmentId,
                ExecutorAgentId = x.pos.ExecutorAgentId,
                DepartmentName = x.pos.Department.Name,
                ExecutorAgentName = x.pos.ExecutorAgent.Name,
                PositionPhone = "PositionPhone",
                MaxSubordinationTypeId = (x.subordMax > 0 ? (int?)x.subordMax : null)
            }).ToList();

        }

        public static IEnumerable<FrontDocument> GetLinkedDocuments(IContext context, DmsContext dbContext, int linkId)
        {
            return CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.LinkId == linkId /*&& context.CurrentPositionsIdList.Contains(x.Acc.PositionId)*/)
                        .Select(y => new FrontDocument
                        {
                            Id = y.Doc.Id,
                            DocumentDirectionName = y.DirName,
                            DocumentTypeName = y.DocTypeName,
                            RegistrationFullNumber = (y.Doc.RegistrationNumber != null
                                                           ? y.Doc.RegistrationNumberPrefix + y.Doc.RegistrationNumber +
                                                             y.Doc.RegistrationNumberSuffix
                                                           : "#" + y.Doc.Id),
                            DocumentDate = y.Doc.RegistrationDate ?? y.Doc.CreateDate,
                            IsRegistered = y.Doc.IsRegistered,
                            Description = y.Doc.Description,
                            ExecutorPositionExecutorAgentName = y.ExecutorPositionExecutorAgentName,
                            ExecutorPositionName = y.ExecutorPosName,
                            Links = dbContext.DocumentLinksSet.Where(z => z.DocumentId == y.Doc.Id).
                                Select(z => new FrontDocumentLink
                                {
                                    Id = z.Id,
                                    LinkTypeName = z.LinkType.Name,
                                    RegistrationFullNumber =
                                                (z.ParentDocument.RegistrationNumber != null
                                                        ? (z.ParentDocument.RegistrationNumberPrefix + z.ParentDocument.RegistrationNumber.ToString() + z.ParentDocument.RegistrationNumberSuffix)
                                                        : ("#" + z.ParentDocument.Id.ToString())),
                                    DocumentDate = (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate),
                                }).ToList()
                        }).ToList();
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(DmsContext dbContext, FilterDocumentSendList filter)
        {
            var sendListDb = dbContext.DocumentSendListsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    sendListDb = sendListDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter?.Id?.Count() > 0)
                {
                    sendListDb = sendListDb.Where(x => filter.Id.Contains(x.Id));
                }

            }

            return sendListDb.Select(y => new FrontDocumentSendList
            {
                Id = y.Id,
                DocumentId = y.DocumentId,
                Stage = y.Stage,
                SendType = (EnumSendTypes)y.SendTypeId,
                SendTypeName = y.SendType.Name,
                SendTypeCode = y.SendType.Code,
                SendTypeIsImportant = y.SendType.IsImportant,
                SourcePositionExecutorAgentName = y.SourcePosition.ExecutorAgent.Name,
                TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name ?? y.TargetAgent.Name,

                Task = y.Task.Task,
                IsAvailableWithinTask = y.IsAvailableWithinTask,
                IsAddControl = y.IsAddControl,
                Description = y.Description,
                DueDate = y.DueDate,
                DueDay = y.DueDay,
                StartEventId = y.StartEventId,
                CloseEventId = y.CloseEventId,
                IsInitial = y.IsInitial,

                SourceAgentName = y.SourceAgent.Name,

                SourceAgentId = y.SourceAgentId,
                SourcePositionId = y.SourcePositionId,

                TargetAgentId = y.TargetAgentId,
                TargetPositionId = y.TargetPositionId,

                SourcePositionName = y.SourcePosition.Name,
                TargetPositionName = y.TargetPosition.Name,
                SourcePositionExecutorNowAgentName = y.SourcePosition.ExecutorAgent.Name,
                TargetPositionExecutorNowAgentName = y.TargetPosition.ExecutorAgent.Name,
                SourcePositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
                TargetPositionExecutorAgentPhoneNumber = "TargetPositionAgentPhoneNumber", //TODO 
                AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                AccessLevelName = y.AccessLevel.Name,
                StartEvent = y.StartEvent == null
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = y.StartEvent.Id,
                                            EventTypeName = y.StartEvent.EventType.Name,
                                            Date = y.StartEvent.Date,
                                            SourcePositionExecutorAgentName = y.StartEvent.SourcePositionExecutorAgent.Name,
                                            TargetPositionExecutorAgentName = y.StartEvent.TargetPositionExecutorAgent.Name ?? y.StartEvent.TargetAgent.Name,
                                            Description = y.StartEvent.Description,
                                        },
                CloseEvent = y.CloseEvent == null || y.StartEventId == y.CloseEventId
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = y.CloseEvent.Id,
                                            EventTypeName = y.CloseEvent.EventType.Name,
                                            Date = y.CloseEvent.Date,
                                            SourcePositionExecutorAgentName = y.CloseEvent.SourcePositionExecutorAgent.Name,
                                            TargetPositionExecutorAgentName = y.CloseEvent.TargetPositionExecutorAgent.Name ?? y.StartEvent.TargetAgent.Name,
                                            Description = y.CloseEvent.Description,
                                        },
            }).ToList();
        }

        public static IEnumerable<FrontDocumentRestrictedSendList> GetDocumentRestrictedSendList(DmsContext dbContext, FilterDocumentRestrictedSendList filter)
        {
            var sendListDb = dbContext.DocumentRestrictedSendListsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    sendListDb = sendListDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter?.Id?.Count() > 0)
                {
                    sendListDb = sendListDb.Where(x => filter.Id.Contains(x.Id));
                }

            }
            return sendListDb.Select(y => new FrontDocumentRestrictedSendList
            {
                Id = y.Id,
                DocumentId = y.DocumentId,
                PositionId = y.PositionId,
                PositionName = y.Position.Name,
                PositionExecutorAgentName = y.Position.ExecutorAgent.Name,
                PositionExecutorAgentPhoneNumber = "PositionAgentPhone",
                AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                AccessLevelName = y.AccessLevel.Name,

            }).ToList();
        }

        public static void ModifyPropertyValues(DmsContext dbContext, InternalPropertyValues model)
        {
            var propertyValues = dbContext.PropertyValuesSet.
                Where(x => x.PropertyLink.ObjectId == (int)model.Object && x.RecordId == model.RecordId)
                .Select(x => new { x.Id, x.PropertyLinkId }).ToList();

            var groupJoinItems = propertyValues
               .GroupJoin(model.PropertyValues,
                   x => x.PropertyLinkId,
                   y => y.PropertyLinkId,
                   (x, y) => new { propertyValueId = x.Id, values = y })
               .ToList();

            #region modify
            var modifyItems = groupJoinItems
                .Where(x => x.values.Count() > 0)
                .Select(x => new { x.propertyValueId, value = x.values.First() })
                .Select(x => new PropertyValues
                {
                    Id = x.propertyValueId,
                    ValueString = x.value.ValueString,
                    ValueDate = x.value.ValueDate,
                    ValueNumeric = x.value.ValueNumeric,
                    LastChangeDate = x.value.LastChangeDate,
                    LastChangeUserId = x.value.LastChangeUserId,
                });

            foreach (var item in modifyItems)
            {
                dbContext.PropertyValuesSet.Attach(item);
                var entry = dbContext.Entry(item);
                entry.Property(x => x.ValueString).IsModified = true;
                entry.Property(x => x.ValueDate).IsModified = true;
                entry.Property(x => x.ValueNumeric).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
            }
            #endregion

            #region add
            var newItems = model.PropertyValues
                .Where(x => !propertyValues.Select(y => y.PropertyLinkId).Contains(x.PropertyLinkId))
                .Select(x => new PropertyValues
                {
                    PropertyLinkId = x.PropertyLinkId,
                    RecordId = model.RecordId,
                    ValueString = x.ValueString,
                    ValueDate = x.ValueDate,
                    ValueNumeric = x.ValueNumeric,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

            dbContext.PropertyValuesSet.AddRange(newItems);
            #endregion

            #region delete
            foreach (var item in groupJoinItems.Where(x => x.values.Count() == 0).Select(x => x.propertyValueId))
            {
                var itemAtt = dbContext.PropertyValuesSet.Attach(new PropertyValues { Id = item });
                dbContext.Entry(itemAtt).State = System.Data.Entity.EntityState.Deleted;
            }
            #endregion
        }

        public static void AddFullTextCashInfo(DmsContext dbContext, int objectId, EnumSearchObjectType objType, EnumOperationType operationType)
        {
            var cashInfo = new FullTextIndexCash
            {
                ObjectId = objectId,
                ObjectType = (int)objType,
                OperationType = (int)operationType
            };

            dbContext.FullTextIndexCashSet.Add(cashInfo);
        }

        public static IEnumerable<FrontDocumentPaper> GetDocumentPapers(DmsContext dbContext, FilterDocumentPaper filter)
        {
            var itemsDb = dbContext.DocumentPapersSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            var itemsRes = itemsDb.Select(x => x);

            var items = itemsRes.Select(x => new FrontDocumentPaper
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                Name = x.Name,
                Description = x.Description,
                IsMain = x.IsMain,
                IsOriginal = x.IsOriginal,
                IsCopy = x.IsCopy,
                PageQuantity = x.PageQuantity,
                OrderNumber = x.OrderNumber,
                LastPaperEventId = x.LastPaperEventId
            }).ToList();

            return items;
        }

        public static IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(DmsContext dbContext, FilterDocumentPaperList filter)
        {
            var itemsDb = dbContext.DocumentPaperListsSet.AsQueryable();

            if (filter != null)
            {
              
            }

            var itemsRes = itemsDb.Select(x => x);

            var items = itemsRes.Select(x => new FrontDocumentPaperList
            {
                Id = x.Id,
                Date = x.Date,
                Description = x.Description
            }).ToList();

            return items;
        }
    }
}