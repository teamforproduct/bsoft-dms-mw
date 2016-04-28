using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.InternalModel;
using BL.Database.DBModel.System;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.InternalModel;
using System.Text;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Database.FileWorker;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;

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

        private static IQueryable<FilterDocumentFileIdentity> GetDocumentFilesMaxVersion(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter)
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

            qry = from fl in qry
                  where dbContext.DocumentAccessesSet.Where(x => ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId)).Select(x => x.DocumentId).Contains(fl.DocumentId)
                  select fl;

            return qry
                .GroupBy(g => new { g.DocumentId, g.OrderNumber })
                .Select(x => new FilterDocumentFileIdentity { DocumentId = x.Key.DocumentId, OrderInDocument = x.Key.OrderNumber, Version = x.Max(s => s.Version) });
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter, UIPaging paging = null)
        {
            var sq = GetDocumentFilesMaxVersion(ctx, dbContext, filter);

            var qry =
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
                        WasChangedExternal = false,
                        DocumentDate = x.fl.Document.RegistrationDate ?? x.fl.Document.CreateDate,
                        RegistrationNumber = x.fl.Document.RegistrationNumber,
                        RegistrationNumberPrefix = x.fl.Document.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.fl.Document.RegistrationNumberSuffix,
                        RegistrationFullNumber = "#" + x.fl.Document.Id,
                        ExecutorPositionName = x.fl.ExecutorPosition.Name,
                        ExecutorPositionExecutorAgentName = x.fl.ExecutorPositionExecutorAgent.Name,
                    });

            if (paging != null)
            {
                paging.TotalItemsCount = qry.Count();

                qry = qry.OrderByDescending(x => x.LastChangeDate)
                    .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
            }

            var files = qry.ToList();
            files.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));
            return files;
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(IContext ctx, DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFilesMaxVersion(ctx, dbContext, new FilterDocumentAttachedFile { DocumentId = new List<int> { documentId } });

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

        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext, bool isAll = false)
        {
            var qry = dbContext.DocumentAccessesSet.AsQueryable();
            if (!isAll && !ctx.IsAdmin)
            {
                qry = qry.Where(x => ctx.CurrentPositionsIdList.Contains(x.PositionId));
            }
            return
                qry.Select(acc => new FrontDocumentAccess
                {
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevelId = (int)acc.AccessLevelId,
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
                    || (x.IsAvailableWithinTask && x.TaskId.HasValue && dbContext.DocumentTaskAccessesSet.Any(a => a.TaskId == x.TaskId.Value && ctx.CurrentPositionsIdList.Contains(a.PositionId)))
                    ).AsQueryable();
        }

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
                           // make weit available if onevent can be accesed through the task
                           ||
                           (x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue &&
                            dbContext.DocumentTaskAccessesSet.Any(a => a.TaskId == x.OnEvent.TaskId.Value && ctx.CurrentPositionsIdList.Contains(a.PositionId)))

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
                if (filter?.Id?.Count() > 0)
                {
                    tasksDb = tasksDb.Where(x => filter.Id.Contains(x.Id));
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
                RegistrationNumber = x.Task.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Task.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Task.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Task.Document.Id,

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

            tasks.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return tasks;

        }

        public static IEnumerable<FrontDocumentWait> GetDocumentWaits(DmsContext dbContext, FilterDocumentWait filter, IContext ctx = null)
        {
            var waitsDb = GetDocumentWaitsQuery(dbContext, ctx);

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

            var waitsRes = waitsDb.OrderByDescending(x => x.LastChangeDate).Select(x => new { Wait = x, x.OnEvent, x.OffEvent });

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

                RegistrationNumber = x.Wait.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Wait.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Wait.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Wait.Document.Id,

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

            waits.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return waits;

        }

        public static IQueryable<DocumentSubscriptions> GetDocumentSubscriptionsQuery(DmsContext dbContext, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.Any())
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.SubscriptionStates.Cast<int>().Contains(x.SubscriptionStateId ?? 0));
                }
            }

            return subscriptionsDb;

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

            var subscriptionsRes = subscriptionsDb.OrderByDescending(x => x.LastChangeDate).Select(x => new { Subscription = x, x.SendEvent, x.DoneEvent });

            var subscriptions = subscriptionsRes.Select(x => new FrontDocumentSubscription
            {
                Id = x.Subscription.Id,
                DocumentId = x.Subscription.DocumentId,
                SendEventId = x.Subscription.SendEventId,
                DoneEventId = x.Subscription.DoneEventId,
                SubscriptionStatesId = x.Subscription.SubscriptionStateId,
                SubscriptionStatesName = x.Subscription.SubscriptionState.Name,
                IsSuccess = x.Subscription.SubscriptionState.IsSuccess,
                Description = x.Subscription.Description,
                DocumentDate = x.Subscription.Document.RegistrationDate ?? x.Subscription.Document.CreateDate,

                RegistrationNumber = x.Subscription.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Subscription.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Subscription.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Subscription.Document.Id,

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

            subscriptions.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

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

            //var itemsRes = itemsDb;

            var itemsRes = itemsDb
                .Select(x => new
                {
                    Id = x.Id,
                    PropertyLinkId = x.PropertyLinkId,
                    Value = x.ValueString != null ? x.ValueString : (x.ValueNumeric.HasValue ? x.ValueNumeric.ToString() : (x.ValueDate.HasValue ? x.ValueDate.ToString() : null)),
                    PropertyCode = x.PropertyLink.Property.Code,
                    PropertyLabel = x.PropertyLink.Property.Label,
                    DisplayValue = string.Empty,
                    SelectAPI = x.PropertyLink.Property.SelectAPI,
                    SelectFilter = x.PropertyLink.Property.SelectFilter,
                    SelectDescriptionFieldCode = x.PropertyLink.Property.SelectDescriptionFieldCode,
                    SelectTable = x.PropertyLink.Property.SelectTable,
                }).ToList();

            var items = new List<FrontPropertyValue>();

            foreach (var itemRes in itemsRes)
            {
                var item = new FrontPropertyValue
                {
                    Id = itemRes.Id,
                    PropertyLinkId = itemRes.PropertyLinkId,
                    Value = itemRes.Value,
                    PropertyCode = itemRes.PropertyCode,
                    PropertyLabel = itemRes.PropertyLabel,
                };
                if (string.IsNullOrEmpty(itemRes.SelectAPI))
                {
                    item.DisplayValue = itemRes.Value;
                }
                else
                {
                    try
                    {
                        Type entityType = Type.GetType(itemRes.SelectTable);

                        var values = dbContext.Set(entityType);
                        int key = 0;
                        int.TryParse(item.Value, out key);
                        var value = key > 0 ? values.Find(key) : values.Find(item.Value);

                        item.DisplayValue = (string)value.GetType().GetProperty(itemRes.SelectDescriptionFieldCode).GetValue(value, null);
                    }
                    catch
                    {
                        item.DisplayValue = itemRes.Value;
                    }
                }
                items.Add(item);
            }

            return items;

        }

        public static IEnumerable<InternalPropertyValue> GetInternalPropertyValues(DmsContext dbContext, FilterPropertyValue filter)
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

            var items = itemsRes.Select(x => new InternalPropertyValue
            {
                Id = x.Id,
                PropertyLinkId = x.PropertyLinkId,
                RecordId = x.RecordId,
                ValueString = x.ValueString,
                ValueDate = x.ValueDate,
                ValueNumeric = x.ValueNumeric,
                PropertyLink = new InternalPropertyLink
                {
                    Id = x.PropertyLink.Id,
                    PropertyId = x.PropertyLink.PropertyId,
                    Object = (EnumObjects)x.PropertyLink.ObjectId,
                    Filers = x.PropertyLink.Filers,
                    IsMandatory = x.PropertyLink.IsMandatory
                }
            }).ToList();

            return items;

        }

        public static void DeletePropertyValues(DmsContext dbContext, FilterPropertyValue filter)
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

            dbContext.PropertyValuesSet.RemoveRange(itemsRes);
        }

        public static IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
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

            return qry.Select(x => new FrontDictionaryPosition
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
            var acc = CommonQueries.GetDocumentAccesses(context, dbContext, true);

            var items = CommonQueries.GetDocumentQuery(dbContext, context, acc)
                    .Where(x => x.Doc.LinkId == linkId /*&& context.CurrentPositionsIdList.Contains(x.Acc.PositionId)*/)
                        .Select(y => new FrontDocument
                        {
                            Id = y.Doc.Id,
                            DocumentDirectionName = y.DirName,
                            DocumentTypeName = y.DocTypeName,

                            RegistrationNumber = y.Doc.RegistrationNumber,
                            RegistrationNumberPrefix = y.Doc.RegistrationNumberPrefix,
                            RegistrationNumberSuffix = y.Doc.RegistrationNumberSuffix,

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
                                    RegistrationNumber = z.ParentDocument.RegistrationNumber,
                                    RegistrationNumberPrefix = z.ParentDocument.RegistrationNumberPrefix,
                                    RegistrationNumberSuffix = z.ParentDocument.RegistrationNumberSuffix,
                                    RegistrationFullNumber = "#" + z.ParentDocument.Id.ToString(),
                                    DocumentDate = (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate),
                                }).ToList()
                        }).ToList();

            items.ForEach(x =>
            {
                CommonQueries.ChangeRegistrationFullNumber(x);
                var links = x.Links.ToList();
                links.ForEach(y => CommonQueries.ChangeRegistrationFullNumber(y));
                x.Links = links;

                x.Accesses = acc.Where(y => y.DocumentId == x.Id).ToList();
            });

            return items;
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
                if (filter?.Id?.Count() > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.Id.Contains(x.Id));
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
                LastPaperEventId = x.LastPaperEventId,
                IsInWork = x.IsInWork,
                DocumentDate = x.Document.RegistrationDate ?? x.Document.CreateDate,
                RegistrationNumber = x.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Document.Id,
                DocumentDescription = x.Document.Description,
                DocumentTypeName = x.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Document.TemplateDocument.DocumentDirection.Name,
                OwnerAgentName = x.LastPaperEvent.TargetAgent.Name,
                OwnerPositionExecutorAgentName = x.LastPaperEvent.TargetPositionExecutorAgent.Name,
                OwnerPositionName = x.LastPaperEvent.TargetPosition.Name,
                OwnerPositionExecutorNowAgentName = x.LastPaperEvent.TargetPosition.ExecutorAgent.Name,
                OwnerPositionExecutorAgentPhoneNumber = "OwnerPositionExecutorAgentPhoneNumber",
                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,


            }).ToList();

            items.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return items;
        }

        public static IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(DmsContext dbContext, FilterDocumentPaperList filter)
        {
            var itemsDb = dbContext.DocumentPaperListsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.PaperListId?.Count() > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.PaperListId.Contains(x.Id));
                }
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

        public static void ChangeRegistrationFullNumber(FrontDocument item, bool isClearFields = true)
        {
            if (item.RegistrationNumber != null)
            {
                item.RegistrationFullNumber = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber + (item.RegistrationNumberSuffix ?? "");
            }
            else
            {
                item.RegistrationFullNumber = "#" + item.Id;
            }

            if (isClearFields)
            {
                item.RegistrationNumber = null;
                item.RegistrationNumberPrefix = null;
                item.RegistrationNumberSuffix = null;
            }
        }

        public static string GetRegistrationFullNumber(InternalDocument item)
        {
            string res = null;
            try
            {
                if ((item.IsRegistered.HasValue && item.IsRegistered.Value) || item.RegistrationNumber.HasValue)
                    res = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber + (item.RegistrationNumberSuffix ?? "");
            }
            catch { }
            return res;
        }

        public static void ChangeRegistrationFullNumber(FrontRegistrationFullNumber item, bool isClearFields = true)
        {
            if (item.RegistrationNumber != null)
            {
                item.RegistrationFullNumber = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber + (item.RegistrationNumberSuffix ?? "");
            }

            if (isClearFields)
            {
                item.RegistrationNumber = null;
                item.RegistrationNumberPrefix = null;
                item.RegistrationNumberSuffix = null;
            }
        }

        public static InternalDocument GetDocumentHash(DmsContext dbContext, IContext ctx, int documentId, bool isAddSubscription = false, bool isFull = false)
        {
            var subscriptions = CommonQueries.GetInternalDocumentSubscriptions(dbContext,
                new FilterDocumentSubscription
                {
                    DocumentId = new List<int> { documentId },
                    SubscriptionStates = new List<EnumSubscriptionStates> {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                        }
                });

            if (!isAddSubscription)
            {
                if (!subscriptions.Any())
                    return null;
            }

            var document = CommonQueries.GetDocumentHashPrepare(dbContext, ctx, documentId);
            document.Subscriptions = subscriptions;

            if (isFull || isAddSubscription)
            {
                var fs = DmsResolver.Current.Get<IFileStore>();
                foreach (var file in document.DocumentFiles)
                {
                    if (!fs.IsFileCorrect(ctx, file))
                    {
                        //TODO
                        //throw new DocumentFileWasChangedExternally();
                    }
                }
            }

            document.Hash = CommonQueries.GetStringDocumentHash(document);

            if (isFull || isAddSubscription)
            {
                document.FullHash = CommonQueries.GetStringDocumentHash(document, true);
            }

            if (subscriptions.Any())
            {
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                foreach (var subscription in subscriptions)
                {
                    if (comparer.Compare(subscription.Hash, document.Hash) != 0 ||
                        ((isFull || isAddSubscription) && comparer.Compare(subscription.FullHash, document.FullHash) != 0))
                    {
                        var subscriptionDb = new DocumentSubscriptions
                        {
                            Id = subscription.Id,
                            SubscriptionStateId = (int)EnumSubscriptionStates.Violated,
                            LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                            LastChangeDate = DateTime.Now
                        };
                        dbContext.DocumentSubscriptionsSet.Attach(subscriptionDb);
                        var entry = dbContext.Entry(subscriptionDb);
                        entry.Property(x => x.SubscriptionStateId).IsModified = true;
                        entry.Property(x => x.LastChangeUserId).IsModified = true;
                        entry.Property(x => x.LastChangeDate).IsModified = true;
                    }
                }
            }

            return document;
        }

        public static InternalDocument GetDocumentHashPrepare(DmsContext dbContext, IContext ctx, int documentId)
        {
            var doc = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Doc.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Doc.Id,
                    DocumentTypeId = x.Templ.DocumentTypeId,
                    Description = x.Doc.Description
                }).FirstOrDefault();

            if (doc == null)
            {
                throw new Model.Exception.DocumentNotFoundOrUserHasNoAccess();
            }

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId);

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(dbContext, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            return doc;
        }

        public static string GetStringDocumentHash(InternalDocument doc, bool isFull = false)
        {
            var hashPrepare = new StringBuilder();

            hashPrepare.Append("Document");
            hashPrepare.Append(doc.Id);
            hashPrepare.Append(doc.DocumentTypeId);
            hashPrepare.Append(doc.Description);
            hashPrepare.Append("Document");

            hashPrepare.Append("File");
            if (doc.DocumentFiles?.Count() > 0)
            {
                foreach (var docFile in doc.DocumentFiles.OrderBy(x => x.Id))
                {
                    hashPrepare.Append(docFile.Id);
                    hashPrepare.Append(docFile.FileSize);
                    hashPrepare.Append(docFile.LastChangeDate);
                    hashPrepare.Append(docFile.Extension);
                    hashPrepare.Append(docFile.Name);
                    if (isFull)
                        hashPrepare.Append(docFile.Hash);
                }
            }
            hashPrepare.Append("File");

            hashPrepare.Append("SendList");
            if (doc.SendLists?.Count() > 0)
            {
                foreach (var docSendList in doc.SendLists.OrderBy(x => x.Id))
                {
                    hashPrepare.Append(docSendList.Id);
                    hashPrepare.Append(docSendList.TargetPositionId);
                    hashPrepare.Append(docSendList.TargetAgentId);
                    hashPrepare.Append(docSendList.SendType);
                    hashPrepare.Append(docSendList.Description);
                    hashPrepare.Append(docSendList.DueDate);
                    hashPrepare.Append(docSendList.DueDay);
                }
            }
            hashPrepare.Append("SendList");

            var hash = CrossCutting.Helpers.DmsHash.GetSha1(hashPrepare.ToString());

            return hash;
        }

        public static List<InternalDictionaryPositionWithActions> GetPositionWithActions(IContext context, DmsContext dbContext, List<int> positionAccesses)
        {
            return dbContext.DictionaryPositionsSet
                .Where(
                    x =>
                        context.CurrentPositionsIdList.Contains(x.Id) &&
                        positionAccesses.Contains(x.Id))
                .Select(x => new InternalDictionaryPositionWithActions
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    ExecutorAgentId = x.ExecutorAgentId,
                    DepartmentName = x.Department.Name,
                    ExecutorAgentName = x.ExecutorAgent.Name,
                }).ToList();
        }

        public static Dictionary<int, List<InternalSystemAction>> GetActionsListForCurrentPositionsList(IContext context, DmsContext dbContext, IEnumerable<EnumObjects> objects, List<int> positionAccesses)
        {
            var res = new Dictionary<int, List<InternalSystemAction>>();
            foreach (var posId in context.CurrentPositionsIdList)
            {
                var qry = dbContext.SystemActionsSet.Where(x => objects.Select(y => (int)y).Contains(x.ObjectId)
                                                                && positionAccesses.Contains(posId)
                                                                && x.IsVisible &&
                                                                (!x.IsGrantable ||
                                                                 x.RoleActions.Any(y => y.Role.PositionRoles.Any(pr => pr.PositionId == posId) &&
                                                                                        y.Role.UserRoles.Any(z => z.UserId == context.CurrentAgentId)))
                    );

                var actLst = qry.Select(a => new InternalSystemAction
                {
                    DocumentAction = (EnumDocumentActions)a.Id,
                    Object = (EnumObjects)a.ObjectId,
                    ActionCode = a.Code,
                    ObjectCode = a.Object.Code,
                    API = a.API,
                    Description = a.Description,
                    Category = a.Category
                }).ToList();
                res.Add(posId, actLst);
            }
            return res;
        }

        public static IEnumerable<InternalDocumentSendList> GetInternalDocumentSendList(DmsContext dbContext, FilterDocumentSendList filter)
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

            return sendListDb.Select(y => new InternalDocumentSendList
            {
                Id = y.Id,
                TargetAgentId = y.TargetAgentId,
                TargetPositionId = y.TargetPositionId,
                SendType = (EnumSendTypes)y.SendTypeId,
                Description = y.Description,
                DueDate = y.DueDate,
                DueDay = y.DueDay,
            }).ToList();
        }

        public static IEnumerable<InternalDocumentSubscription> GetInternalDocumentSubscriptions(DmsContext dbContext, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.Any())
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.SubscriptionStates.Cast<int>().Contains(x.SubscriptionStateId ?? 0));
                }
            }

            var subscriptionsRes = subscriptionsDb.Select(x => new { Subscription = x });

            var subscriptions = subscriptionsRes.Select(x => new InternalDocumentSubscription
            {
                Id = x.Subscription.Id,
                SubscriptionStates = (EnumSubscriptionStates)x.Subscription.SubscriptionStateId,
                Hash = x.Subscription.Hash,
                FullHash = x.Subscription.FullHash
            }).ToList();

            return subscriptions;
        }

        public static void ModifyDocumentTaskAccesses(DmsContext dbContext, int documentId)
        {
            var qry1 = dbContext.DocumentEventsSet.Where(x => x.DocumentId == documentId && x.IsAvailableWithinTask && x.TaskId.HasValue)
                .GroupBy(x => new { x.TaskId, x.SourcePositionId, x.TargetPositionId })
                .Select(x => new { x.Key.TaskId, x.Key.SourcePositionId, x.Key.TargetPositionId }).ToList();
            var qry2 = qry1.GroupBy(x => new { x.TaskId, x.SourcePositionId }).Where(x => x.Key.SourcePositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.SourcePositionId }).ToList();
            var qry3 = qry1.GroupBy(x => new { x.TaskId, x.TargetPositionId }).Where(x => x.Key.TargetPositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.TargetPositionId }).ToList();
            var taNew = qry2.Union(qry3).GroupBy(x => new { x.TaskId, x.PositionId })
                .Select(x => new InternalDocumentTaskAccesses { TaskId = x.Key.TaskId.Value, PositionId = x.Key.PositionId.Value }).ToList();

            var taOld = dbContext.DocumentTaskAccessesSet.Where(x => x.Task.DocumentId == documentId)
                .Select(x => new InternalDocumentTaskAccesses { Id = x.Id, TaskId = x.TaskId, PositionId = x.PositionId }).ToList();

            var delId = taOld.GroupJoin(taNew
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1.Id, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.Id).ToList();

            var insTA = taNew.GroupJoin(taOld
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.ta1).ToList();

            dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => delId.Contains(x.Id)));

            dbContext.DocumentTaskAccessesSet.AddRange(insTA.Select(x => new DocumentTaskAccesses { TaskId = x.TaskId, PositionId = x.PositionId }));

        }

    }
}