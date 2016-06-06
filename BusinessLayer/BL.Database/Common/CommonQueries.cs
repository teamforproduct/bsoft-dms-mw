using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
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
using BL.CrossCutting.CryptographicWorker;
using LinqKit;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(DmsContext dbContext, IContext ctx, IQueryable<FrontDocumentAccess> userAccesses = null)
        {
            var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Accesses.AsQueryable().Any(filterContains));
                //qry = qry.Where(x => x.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }
            if (userAccesses != null)
            {
                qry = qry.Where(x => userAccesses.Select(a => a.DocumentId).Contains(x.Id));
            }
            return qry;
        }

        private static IQueryable<DocumentFiles> GetDocumentFilesMaxVersion(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var qry = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                qry = qry.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

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
                .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault());
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter, UIPaging paging = null)
        {
            var sq = GetDocumentFilesMaxVersion(ctx, dbContext, filter);

            if (paging != null)
            {
                paging.TotalItemsCount = sq.Count();

                if (paging.IsOnlyCounter)
                {
                    return new List<FrontDocumentAttachedFile>();
                }

                sq = sq.OrderByDescending(x => x.LastChangeDate)
                    .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
            }

            var qry = from file in sq
                      join agent in dbContext.DictionaryAgentsSet on file.LastChangeUserId equals agent.Id into agent
                      from agentAg in agent.DefaultIfEmpty()
                      select new FrontDocumentAttachedFile
                      {
                          Id = file.Id,
                          Date = file.Date,
                          DocumentId = file.DocumentId,
                          Extension = file.Extension,
                          FileContent = file.Content,
                          FileType = file.FileType,
                          FileSize = file.FileSize,
                          IsAdditional = file.IsAdditional,
                          Hash = file.Hash,
                          LastChangeDate = file.LastChangeDate,
                          LastChangeUserId = file.LastChangeUserId,
                          LastChangeUserName = agentAg.Name,
                          Name = file.Name,
                          OrderInDocument = file.OrderNumber,
                          Version = file.Version,
                          WasChangedExternal = false,
                          ExecutorPositionName = file.ExecutorPosition.Name,
                          ExecutorPositionExecutorAgentName = file.ExecutorPositionExecutorAgent.Name,

                          DocumentDate = file.Document.LinkId.HasValue ? file.Document.RegistrationDate ?? file.Document.CreateDate : (DateTime?)null,
                          RegistrationNumber = file.Document.LinkId.HasValue ? file.Document.RegistrationNumber : null,
                          RegistrationNumberPrefix = file.Document.LinkId.HasValue ? file.Document.RegistrationNumberPrefix : null,
                          RegistrationNumberSuffix = file.Document.LinkId.HasValue ? file.Document.RegistrationNumberSuffix : null,
                          RegistrationFullNumber = file.Document.LinkId.HasValue ? "#" + file.Document.Id : null,
                      };

            var files = qry.ToList();
            files.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));
            return files;
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(IContext ctx, DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFilesMaxVersion(ctx, dbContext, new FilterDocumentAttachedFile { DocumentId = new List<int> { documentId } });

            return
                sq.Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    Date = x.Date,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension,
                    FileContent = x.Content,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    IsAdditional = x.IsAdditional,
                    Hash = x.Hash,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    Name = x.Name,
                    OrderInDocument = x.OrderNumber,
                    Version = x.Version,
                    WasChangedExternal = false
                }).ToList();
        }

        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext, bool isAll = false)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!isAll && !ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }
            return
                qry.Select(acc => new FrontDocumentAccess
                {
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevelId = acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name
                });
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(DmsContext dbContext, IContext ctx, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            if (docAccesses == null || !docAccesses.Any()) return null;
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x => !accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
        }

        public static IQueryable<DocumentAccesses> GetDocumentAccessesesQry(DmsContext dbContext, int documentId, IContext ctx)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId);
            if (ctx != null)
            {
                qry = qry.Where(x => ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId));
            }
            return qry;
        }

        public static IEnumerable<InternalDocumentAccess> GetInternalDocumentAccesses(DmsContext dbContext, IContext ctx, int documentId)
        {
            return dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId).Select(acc => new InternalDocumentAccess
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

        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(DmsContext dbContext, IContext context, List<int> positionIds)
        {
            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId)
                .Where(x => positionIds.Contains(x.Id)).Select(x => new InternalPositionInfo
                {
                    PositionId = x.Id,
                    PositionName = x.Name,
                    AgentId = x.ExecutorAgentId ?? 0,
                    AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name : ""
                }).ToList();
        }

        public static IQueryable<DocumentEvents> GetDocumentEventsQuery(IContext ctx, DmsContext dbContext)
        {
            var qry = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                qry = qry.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

            return qry
                    .Where(x => ctx.IsAdmin || (x.TargetPositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.TargetPositionId.Value))
                    || (x.SourcePositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.SourcePositionId.Value))
                    || (x.IsAvailableWithinTask && x.TaskId.HasValue && x.Task.TaskAccesses.Any(a => ctx.CurrentPositionsIdList.Contains(a.PositionId)))
                    ).AsQueryable();
        }

        public static IQueryable<DocumentWaits> GetDocumentWaitsQuery(DmsContext dbContext, IContext ctx, int? documentId = null)
        {
            var qry = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                qry = qry.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId.Value);
            }
            if (ctx != null && !ctx.IsAdmin)
            {
                //TODO
                var currentPositionsIdList = ctx.CurrentPositionsIdList;


                qry = qry.Where(x =>
                           (x.OnEvent.TargetPositionId.HasValue &&
                            currentPositionsIdList.Contains(x.OnEvent.TargetPositionId.Value))
                           ||
                           (x.OnEvent.SourcePositionId.HasValue &&
                            currentPositionsIdList.Contains(x.OnEvent.SourcePositionId.Value))
                           // make weit available if onevent can be accesed through the task
                           ||
                           (x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue &&
                           x.OnEvent.Task.TaskAccesses.Any(a => currentPositionsIdList.Contains(a.PositionId)))
                             );
            }
            return qry;
        }

        public static IEnumerable<FrontDocumentTask> GetDocumentTasks(DmsContext dbContext, IContext ctx, FilterDocumentTask filter, UIPaging paging)
        {
            var tasksDb = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                tasksDb = tasksDb.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

            //var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
            //                .Where(x => x.TaskId.HasValue)
            //                .Where(x => !x.CloseEventId.HasValue)
            //                .Where(x => x.SendTypeId == (int)EnumSendTypes.SendForResponsibleExecution)
            //                .GroupBy(x => x.TaskId)
            //                .Select(x => x.FirstOrDefault())
            //                .AsQueryable();

            //var eventDb = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
            //                .Where(x => !x.OffEventId.HasValue)
            //                .Select(x => x.OnEvent)
            //                .Where(x => x.TaskId.HasValue)
            //                .Where(x => x.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution || x.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecutionChange)
            //                .GroupBy(x => x.TaskId)
            //                .Select(x => x.FirstOrDefault())
            //                .AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    tasksDb = tasksDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                    //sendListDb = sendListDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                    //eventDb = eventDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter?.Id?.Count() > 0)
                {
                    tasksDb = tasksDb.Where(x => filter.Id.Contains(x.Id));
                    //sendListDb = sendListDb.Where(x => filter.Id.Contains(x.TaskId ?? 0));
                    //eventDb = eventDb.Where(x => filter.Id.Contains(x.TaskId ?? 0));
                }
            }

            //var tasksRes = tasksDb;

            //var tasksRes = from task in tasksDb

            //               join sl in sendListDb on task.Id equals sl.TaskId into sl
            //               from slAg in sl.DefaultIfEmpty()

            //               join ev in eventDb on task.Id equals ev.TaskId into ev
            //               from evAg in ev.DefaultIfEmpty()
            //               select new
            //               {
            //                   Task = task,
            //                   SendListDb = slAg,
            //                   Event = evAg
            //               };

            if (paging != null)
            {
                paging.TotalItemsCount = tasksDb.Count();

                if (paging.IsOnlyCounter)
                {
                    return new List<FrontDocumentTask>();
                }

                tasksDb = tasksDb.OrderByDescending(x => x.LastChangeDate)
                    .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
            }

            var tasks = tasksDb.Select(x => new FrontDocumentTask
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                Name = x.Task,
                Description = x.Description,

                DocumentDate = x.Document.RegistrationDate ?? x.Document.CreateDate,
                RegistrationNumber = x.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Document.Id,

                DocumentDescription = x.Document.Description,
                DocumentTypeName = x.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Document.TemplateDocument.DocumentDirection.Name,

                PositionId = x.PositionId,
                PositionExecutorAgentId = x.PositionExecutorAgentId,
                AgentId = x.AgentId,

                PositionExecutorAgentName = x.PositionExecutorAgent.Name,
                AgentName = x.Agent.Name,
                PositionName = x.Position.Name,
                PositionExecutorNowAgentName = x.Position.ExecutorAgent.Name,
                PositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                //FactResponsibleExecutorPositionName = x.SendListDb.TargetPosition.Name,
                //FactResponsibleExecutorPositionExecutorAgentName = x.SendListDb.TargetPositionExecutorAgent.Name,

                //PlanResponsibleExecutorPositionName = x.Event.TargetPosition.Name,
                //PlanResponsibleExecutorPositionExecutorAgentName = x.Event.TargetPositionExecutorAgent.Name,
            }).ToList();

            tasks.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return tasks;

        }

        public static IEnumerable<FrontDocumentWait> GetDocumentWaits(DmsContext dbContext, FilterDocumentWait filter, IContext ctx, UIPaging paging = null)
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

            var waitsRes = waitsDb
                .OrderBy(x => x.DueDate ?? DateTime.MaxValue)
                .ThenBy(x => x.OnEvent.Date)
                .AsQueryable();

            if (paging != null)
            {
                //paging.TotalItemsCount = waitsRes.Count();

                paging.Counters = new UICounters
                {
                    Counter1 = waitsRes.Count(y => !y.OffEventId.HasValue),
                    Counter2 = waitsRes.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.Now),
                    Counter3 = waitsRes.Count(),
                };

                //TODO Подумать что лучше
                //paging.Counters = waitsRes.GroupBy(x => 1).Select(x => new UICounters
                //{
                //    Counter1 = x.Count(y => !y.OffEventId.HasValue),
                //    Counter2 = x.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.Now),
                //    Counter3 = x.Count()
                //}).FirstOrDefault();

                paging.TotalItemsCount = paging.Counters.Counter3.GetValueOrDefault();

                if (paging.IsOnlyCounter)
                {
                    return new List<FrontDocumentWait>();
                }

                waitsRes = waitsRes
                        .Skip(paging.PageSize * (paging.CurrentPage - 1))
                        .Take(paging.PageSize);
            }

            var waits = waitsRes.Select(x => new FrontDocumentWait
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                ParentId = x.ParentId,
                OnEventId = x.OnEventId,
                OffEventId = x.OffEventId,
                ResultTypeId = x.ResultTypeId,
                ResultTypeName = x.ResultType.Name,
                DueDate = x.DueDate,
                AttentionDate = x.AttentionDate,
                TargetDescription = x.TargetDescription,
                //TargetAttentionDate = x.TargetAttentionDate,
                IsClosed = x.OffEvent != null,

                DocumentDate = x.Document.LinkId.HasValue ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                RegistrationNumber = x.Document.LinkId.HasValue ? x.Document.RegistrationNumber : null,
                RegistrationNumberPrefix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberPrefix : null,
                RegistrationNumberSuffix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberSuffix : null,
                RegistrationFullNumber = x.Document.LinkId.HasValue ? "#" + x.Document.Id : null,
                DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                OnEvent = new FrontDocumentEvent
                {
                    Id = x.OnEvent.Id,
                    DocumentId = x.OnEvent.DocumentId,
                    Task = x.OnEvent.Task.Task,
                    Description = x.OnEvent.Description,
                    AddDescription = x.OnEvent.AddDescription,
                    EventType = x.OnEvent.EventTypeId,
                    EventTypeName = x.OnEvent.EventType.WaitDescription/*?? x.OnEvent.EventType.Name*/,
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
                    SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                    TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                },
                OffEvent = !x.OffEventId.HasValue
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        DocumentId = x.OffEvent.DocumentId,
                        Task = null,
                        Description = x.OffEvent.Description,
                        AddDescription = x.OffEvent.AddDescription,
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

        public static IQueryable<DocumentSubscriptions> GetDocumentSubscriptionsQuery(DmsContext dbContext, FilterDocumentSubscription filter, IContext ctx)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                subscriptionsDb = subscriptionsDb.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

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
        public static IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(DmsContext dbContext, FilterDocumentSubscription filter, IContext ctx, UIPaging paging = null)
        {

            var subscriptionsDb = GetDocumentSubscriptionsQuery(dbContext, filter, ctx);

            var subscriptionsRes = subscriptionsDb
                                    .OrderByDescending(x => x.LastChangeDate)
                                    .AsQueryable();

            if (paging != null)
            {
                paging.TotalItemsCount = subscriptionsRes.Count();

                if (paging.IsOnlyCounter)
                {
                    return new List<FrontDocumentSubscription>();
                }

                subscriptionsRes = subscriptionsRes
                        .Skip(paging.PageSize * (paging.CurrentPage - 1))
                        .Take(paging.PageSize);
            }

            var subscriptions = subscriptionsRes.Select(x => new FrontDocumentSubscription
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                SendEventId = x.SendEventId,
                DoneEventId = x.DoneEventId,
                SubscriptionStatesId = x.SubscriptionStateId,
                SubscriptionStatesName = x.SubscriptionState.Name,
                IsSuccess = x.SubscriptionState.IsSuccess,
                Description = x.Description,

                DocumentDate = x.Document.LinkId.HasValue ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                RegistrationNumber = x.Document.LinkId.HasValue ? x.Document.RegistrationNumber : null,
                RegistrationNumberPrefix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberPrefix : null,
                RegistrationNumberSuffix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberSuffix : null,
                RegistrationFullNumber = x.Document.LinkId.HasValue ? "#" + x.Document.Id : null,
                DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                SendEvent = x.SendEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.SendEvent.Id,
                        DocumentId = x.SendEvent.DocumentId,
                        EventTypeName = x.SendEvent.EventType.Name,
                        TargetPositionExecutorAgentName = x.SendEvent.TargetPositionExecutorAgent.Name,
                        DueDate = x.SendEvent.OnWait.FirstOrDefault().DueDate,

                        Date = x.SendEvent.Date,
                        SourcePositionExecutorAgentName = x.SendEvent.SourcePositionExecutorAgent.Name,
                        Description = x.SendEvent.Description,
                        AddDescription = x.SendEvent.AddDescription,
                        ReadAgentName = x.SendEvent.ReadAgent.Name,
                        ReadDate = x.SendEvent.ReadDate,
                        SourceAgentName = x.SendEvent.SourceAgent.Name,
                        SourcePositionName = x.SendEvent.SourcePosition.Name,
                        TargetPositionName = x.SendEvent.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.SendEvent.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = x.SendEvent.TargetPosition.ExecutorAgent.Name,
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

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
                        AddDescription = x.DoneEvent.AddDescription,


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

        public static IEnumerable<FrontDocumentTag> GetDocumentTags(DmsContext dbContext, IContext context, FilterDocumentTag filter)
        {
            var tagsDb = dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    tagsDb = tagsDb.Where(filterContains);

                    //tagsDb = tagsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = filter.CurrentPositionsId.Aggregate(filterContains,
                        (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    tagsDb = tagsDb.Where(filterContains);

                    //tagsDb = tagsDb.Where(x => !x.Tag.PositionId.HasValue || filter.CurrentPositionsId.Contains(x.Tag.PositionId ?? 0));
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

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.Object.Select(x=>(int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PropertyLink.ObjectId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);

                    //itemsDb = itemsDb.Where(x => filter.Object.Contains((EnumObjects)x.PropertyLink.ObjectId));
                }

                if (filter.RecordId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.RecordId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RecordId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);

                    //itemsDb = itemsDb.Where(x => filter.RecordId.Contains(x.RecordId));
                }
            }

            //var itemsRes = itemsDb;

            var itemsRes = itemsDb
                .Select(x => new
                {
                    Id = x.Id,
                    PropertyLinkId = x.PropertyLinkId,
                    ValueString = x.ValueString,
                    ValueNumeric = x.ValueNumeric,
                    ValueDate = x.ValueDate,
                    PropertyCode = x.PropertyLink.Property.Code,
                    PropertyLabel = x.PropertyLink.Property.Label,
                    PropertyValueTypeCode = x.PropertyLink.Property.ValueType.Code,
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
                    PropertyLinkId = itemRes.PropertyLinkId,
                    //Value = itemRes.Value,
                    PropertyCode = itemRes.PropertyCode,
                    PropertyLabel = itemRes.PropertyLabel,
                    PropertyValueTypeCode = itemRes.PropertyValueTypeCode,
                };

                if (itemRes.ValueString != null)
                {
                    item.Value = itemRes.ValueString;
                }
                else if (itemRes.ValueNumeric.HasValue)
                {
                    item.Value = itemRes.ValueNumeric;
                }
                else if (itemRes.ValueDate.HasValue)
                {
                    item.Value = itemRes.ValueDate;
                }
                else
                {
                    item.Value = null;
                }

                if (string.IsNullOrEmpty(itemRes.SelectAPI))
                {
                    item.DisplayValue = item.Value;
                }
                else
                {
                    try
                    {
                        Type entityType = Type.GetType(itemRes.SelectTable);

                        var values = dbContext.Set(entityType);
                        int key = 0;
                        int.TryParse(item.Value.ToString(), out key);
                        var value = key > 0 ? values.Find(key) : values.Find(item.Value);

                        item.DisplayValue = (string)value.GetType().GetProperty(itemRes.SelectDescriptionFieldCode).GetValue(value, null);
                    }
                    catch
                    {
                        item.DisplayValue = item.Value;
                    }
                }
                items.Add(item);
            }

            return items;

        }

        public static IEnumerable<InternalPropertyValue> GetInternalPropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId).AsQueryable();

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

        public static void DeletePropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId).AsQueryable();

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

        public static IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, IContext ctx, FilterDictionaryPosition filter)
        {
            //TODO пределать на аксессы
            var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId).Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
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
                PositionPhone = "(888)888-88-88",
                MaxSubordinationTypeId = (x.subordMax > 0 ? (int?)x.subordMax : null)
            }).ToList();

        }

        public static IEnumerable<FrontDocument> GetLinkedDocuments(IContext context, DmsContext dbContext, int linkId)
        {
            //var acc = CommonQueries.GetDocumentAccesses(context, dbContext, true);

            var items = CommonQueries.GetDocumentQuery(dbContext, context/*, acc*/)
                    .Where(x => x.LinkId == linkId /*&& context.CurrentPositionsIdList.Contains(x.Acc.PositionId)*/)
                        .OrderBy(x => x.RegistrationDate ?? x.CreateDate)
                        .Select(y => new FrontDocument
                        {
                            Id = y.Id,
                            DocumentDirectionName = y.TemplateDocument.DocumentDirection.Name,
                            DocumentTypeName = y.TemplateDocument.DocumentType.Name,

                            RegistrationNumber = y.RegistrationNumber,
                            RegistrationNumberPrefix = y.RegistrationNumberPrefix,
                            RegistrationNumberSuffix = y.RegistrationNumberSuffix,

                            DocumentDate = y.RegistrationDate ?? y.CreateDate,
                            IsRegistered = y.IsRegistered,
                            Description = y.Description,
                            ExecutorPositionExecutorAgentName = y.ExecutorPositionExecutorAgent.Name,
                            ExecutorPositionName = y.ExecutorPosition.Name,
                            Links = y.LinksDocuments.OrderBy(z => z.LastChangeDate).
                                Select(z => new FrontDocumentLink
                                {
                                    Id = z.Id,
                                    LinkTypeName = z.LinkType.Name,
                                    RegistrationNumber = z.ParentDocument.RegistrationNumber,
                                    RegistrationNumberPrefix = z.ParentDocument.RegistrationNumberPrefix,
                                    RegistrationNumberSuffix = z.ParentDocument.RegistrationNumberSuffix,
                                    RegistrationFullNumber = "#" + z.ParentDocument.Id.ToString(),
                                    DocumentDate = (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate),
                                })
                        }).ToList();

            items.ForEach(x =>
            {
                CommonQueries.ChangeRegistrationFullNumber(x);
                var links = x.Links.ToList();
                links.ForEach(y => CommonQueries.ChangeRegistrationFullNumber(y));
                x.Links = links;

                //TODO x.Accesses = acc.Where(y => y.DocumentId == x.Id).ToList();
            });

            return items;
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                    //sendListDb = sendListDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);

                    //sendListDb = sendListDb.Where(x => filter.Id.Contains(x.Id));
                }

            }

            var res = sendListDb.Select(y => new FrontDocumentSendList
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
                IsWorkGroup = y.IsWorkGroup,
                IsAddControl = y.IsAddControl,
                SelfDueDate = y.SelfDueDate,
                SelfDueDay = y.SelfDueDay,
                SelfAttentionDate = y.SelfAttentionDate,
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
                SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
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
                                            AddDescription = y.StartEvent.AddDescription,
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
                                            AddDescription = y.CloseEvent.AddDescription,
                                        },
            }).ToList();
            return res;
        }

        public static IEnumerable<FrontDocumentRestrictedSendList> GetDocumentRestrictedSendList(DmsContext dbContext, IContext ctx, FilterDocumentRestrictedSendList filter)
        {
            var sendListDb = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentRestrictedSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                    //sendListDb = sendListDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentRestrictedSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);

                    //sendListDb = sendListDb.Where(x => filter.Id.Contains(x.Id));
                }

            }
            return sendListDb.Select(y => new FrontDocumentRestrictedSendList
            {
                Id = y.Id,
                DocumentId = y.DocumentId,
                PositionId = y.PositionId,
                PositionName = y.Position.Name,
                PositionExecutorAgentName = y.Position.ExecutorAgent.Name,
                PositionExecutorAgentPhoneNumber = "(888)888-88-88",
                AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                AccessLevelName = y.AccessLevel.Name,

            }).ToList();
        }

        public static void ModifyPropertyValues(DmsContext dbContext, IContext ctx, InternalPropertyValues model)
        {
            var propertyValues = dbContext.PropertyValuesSet
                .Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId)
                .Where(x => x.PropertyLink.ObjectId == (int)model.Object && x.RecordId == model.RecordId)
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

        public static void AddFullTextCashInfo(DmsContext dbContext, int objectId, EnumObjects objType, EnumOperationType operationType)
        {
            var cashInfo = new FullTextIndexCash
            {
                ObjectId = objectId,
                ObjectType = (int)objType,
                OperationType = (int)operationType
            };

            dbContext.FullTextIndexCashSet.Add(cashInfo);
        }

        public static IEnumerable<FrontDocumentPaper> GetDocumentPapers(DmsContext dbContext, IContext ctx, FilterDocumentPaper filter, UIPaging paging)
        {
            var itemsDb = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                itemsDb = itemsDb.Where(x => x.Document.Accesses.Any(y => ctx.CurrentPositionsIdList.Contains(y.PositionId)));
            }

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

            itemsDb = itemsDb.OrderByDescending(x => x.LastChangeDate);

            if (paging != null)
            {
                paging.TotalItemsCount = itemsDb.Count();

                if (paging.IsOnlyCounter)
                {
                    return new List<FrontDocumentPaper>();
                }



                itemsDb = itemsDb
                        .Skip(paging.PageSize * (paging.CurrentPage - 1))
                        .Take(paging.PageSize);
            }

            //var itemsRes = itemsDb.Select(x => x);

            var items = itemsDb.Select(x => new FrontDocumentPaper
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

                DocumentDate = x.Document.LinkId.HasValue ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                RegistrationNumber = x.Document.LinkId.HasValue ? x.Document.RegistrationNumber : null,
                RegistrationNumberPrefix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberPrefix : null,
                RegistrationNumberSuffix = x.Document.LinkId.HasValue ? x.Document.RegistrationNumberSuffix : null,
                RegistrationFullNumber = x.Document.LinkId.HasValue ? "#" + x.Document.Id : null,
                DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                OwnerAgentName = x.LastPaperEvent.TargetAgent.Name,
                OwnerPositionExecutorAgentName = x.LastPaperEvent.TargetPositionExecutorAgent.Name,
                OwnerPositionName = x.LastPaperEvent.TargetPosition.Name,
                OwnerPositionExecutorNowAgentName = x.LastPaperEvent.TargetPosition.ExecutorAgent.Name,
                OwnerPositionExecutorAgentPhoneNumber = "(888)888-88-88",
                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,


            }).ToList();

            items.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return items;
        }

        public static IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(DmsContext dbContext, IContext ctx, FilterDocumentPaperList filter)
        {
            var itemsDb = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

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
            var subscriptions = CommonQueries.GetInternalDocumentSubscriptions(dbContext, ctx,
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

            var IsFilesIncorrect = false;

            if (isFull || isAddSubscription)
            {
                var fs = DmsResolver.Current.Get<IFileStore>();
                foreach (var file in document.DocumentFiles)
                {
                    if (!fs.IsFileCorrect(ctx, file))
                    {
                        //TODO
                        IsFilesIncorrect = true;
                        //throw new DocumentFileWasChangedExternally();
                    }
                }
            }

            document.Hash = CommonQueries.GetDocumentHash(document);

            if (isFull || isAddSubscription)
            {
                document.FullHash = CommonQueries.GetDocumentHash(document, true);
            }

            if (subscriptions.Any())
            {
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                foreach (var subscription in subscriptions)
                {
                    if (IsFilesIncorrect || !VerifyDocumentHash(subscription.Hash, document) ||
                        ((isFull || isAddSubscription) && !VerifyDocumentHash(subscription.FullHash, document, true)))
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

                        var sendList = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Where(x => x.StartEventId == subscription.SendEventId && x.IsInitial)
                            .FirstOrDefault();

                        if (sendList != null)
                        {
                            sendList.StartEventId = null;
                            sendList.CloseEventId = null;
                            sendList.LastChangeUserId = ctx.CurrentAgentId;
                            sendList.LastChangeDate = DateTime.Now;
                        }
                        //TODO проверить поля
                        //var eventDb = new DocumentEvents
                        //{
                        //    DocumentId = document.Id,
                        //    EventTypeId = (int)EnumEventTypes.LaunchPlan,
                        //    SourceAgentId = ctx.CurrentAgentId,
                        //    SourcePositionId = ctx.CurrentPositionId,
                        //    //SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId),
                        //    TargetPositionId = ctx.CurrentPositionId,
                        //    //TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId),
                        //    TargetAgentId = ctx.CurrentAgentId,
                        //    LastChangeUserId = ctx.CurrentAgentId,
                        //    LastChangeDate = DateTime.Now,
                        //    Date = DateTime.Now,
                        //    CreateDate = DateTime.Now,
                        //};

                        //dbContext.DocumentEventsSet.Add(eventDb);
                    }
                }
            }

            if (IsFilesIncorrect)
            {
                throw new DocumentFileWasChangedExternally();
            }

            return document;
        }

        public static InternalDocument GetDocumentHashPrepare(DmsContext dbContext, IContext ctx, int documentId)
        {
            var doc = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                    Description = x.Description
                }).FirstOrDefault();

            if (doc == null)
            {
                throw new Model.Exception.DocumentNotFoundOrUserHasNoAccess();
            }

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId);

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            return doc;
        }

        private static string GetStringDocumentForDocumentHash(InternalDocument doc, bool isFull = false)
        {
            StringBuilder stringDocument = new StringBuilder();

            stringDocument.Append("Document");
            stringDocument.Append(doc.Id);
            stringDocument.Append(doc.DocumentTypeId);
            stringDocument.Append(doc.Description);
            stringDocument.Append("Document");

            stringDocument.Append("File");
            if (doc.DocumentFiles?.Count() > 0)
            {
                foreach (var docFile in doc.DocumentFiles.OrderBy(x => x.Id))
                {
                    stringDocument.Append(docFile.Id);
                    stringDocument.Append(docFile.FileSize);
                    stringDocument.Append(docFile.LastChangeDate);
                    stringDocument.Append(docFile.Extension);
                    stringDocument.Append(docFile.Name);
                    if (isFull)
                        stringDocument.Append(docFile.Hash);
                }
            }
            stringDocument.Append("File");

            stringDocument.Append("SendList");
            if (doc.SendLists?.Count() > 0)
            {
                foreach (var docSendList in doc.SendLists.OrderBy(x => x.Id))
                {
                    stringDocument.Append(docSendList.Id);
                    stringDocument.Append(docSendList.TargetPositionId);
                    stringDocument.Append(docSendList.TargetAgentId);
                    stringDocument.Append(docSendList.SendType);
                    stringDocument.Append(docSendList.Description);
                    stringDocument.Append(docSendList.DueDate);
                    stringDocument.Append(docSendList.DueDay);
                }
            }
            stringDocument.Append("SendList");

            return stringDocument.ToString();
        }
        public static string GetDocumentHash(InternalDocument doc, bool isFull = false)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, isFull);

            string hash = DmsResolver.Current.Get<ICryptoService>().GetHash(stringDocument);

            return hash;
        }

        public static bool VerifyDocumentHash(string hash, InternalDocument doc, bool isFull = false)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, isFull);

            return DmsResolver.Current.Get<ICryptoService>().VerifyHash(stringDocument, hash);
        }

        public static List<InternalDictionaryPositionWithActions> GetPositionWithActions(IContext context, DmsContext dbContext, List<int> positionAccesses)
        {
            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId)
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

        public static IEnumerable<InternalDocumentSendList> GetInternalDocumentSendList(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

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

        public static IEnumerable<InternalDocumentSubscription> GetInternalDocumentSubscriptions(DmsContext dbContext, IContext context, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

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

            var subscriptions = subscriptionsDb.Select(x => new InternalDocumentSubscription
            {
                Id = x.Id,
                SendEventId = x.SendEventId,
                SubscriptionStates = (EnumSubscriptionStates)x.SubscriptionStateId,
                Hash = x.Hash,
                FullHash = x.FullHash
            }).ToList();

            return subscriptions;
        }

        public static void ModifyDocumentTaskAccesses(DmsContext dbContext, IContext ctx, int documentId)
        {
            var qry1 = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                .Where(x => x.DocumentId == documentId && x.IsAvailableWithinTask && x.TaskId.HasValue)
                .GroupBy(x => new { x.TaskId, x.SourcePositionId, x.TargetPositionId })
                .Select(x => new { x.Key.TaskId, x.Key.SourcePositionId, x.Key.TargetPositionId }).ToList();
            var qry2 = qry1.GroupBy(x => new { x.TaskId, x.SourcePositionId }).Where(x => x.Key.SourcePositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.SourcePositionId }).ToList();
            var qry3 = qry1.GroupBy(x => new { x.TaskId, x.TargetPositionId }).Where(x => x.Key.TargetPositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.TargetPositionId }).ToList();
            var taNew = qry2.Union(qry3).GroupBy(x => new { x.TaskId, x.PositionId })
                .Select(x => new InternalDocumentTaskAccesses { TaskId = x.Key.TaskId.Value, PositionId = x.Key.PositionId.Value }).ToList();

            var taOld = dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == documentId)
                .Select(x => new InternalDocumentTaskAccesses { Id = x.Id, TaskId = x.TaskId, PositionId = x.PositionId }).ToList();

            var delId = taOld.GroupJoin(taNew
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1.Id, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.Id).ToList();

            var insTA = taNew.GroupJoin(taOld
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.ta1).ToList();

            dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => delId.Contains(x.Id)));

            dbContext.DocumentTaskAccessesSet.AddRange(insTA.Select(x => new DocumentTaskAccesses { TaskId = x.TaskId, PositionId = x.PositionId }));

        }

    }
}