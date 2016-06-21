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
using BL.Database.DBModel.Dictionary;
using System.Data.Entity;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(DmsContext dbContext, IContext ctx, IQueryable<FrontDocumentAccess> userAccesses = null, bool isVerifyExecutorPosition = false)
        {
            var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                if (userAccesses == null)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());
                    qry = qry.Where(x => x.Accesses.AsQueryable().Any(filterContains));
                }

                if (isVerifyExecutorPosition)
                {
                    var filterExecutorPositionContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterExecutorPositionContains = ctx.CurrentPositionsIdList.Aggregate(filterExecutorPositionContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterExecutorPositionContains);
                }
            }
            if (userAccesses != null)
            {
                qry = qry.Where(x => userAccesses.Select(a => a.DocumentId).Contains(x.Id));
            }
            return qry;
        }

        private static IQueryable<DocumentFiles> GetDocumentFilesQuery(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var qry = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.AttachedFileId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.AttachedFileId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsAdditional.HasValue)
                {
                    qry = qry.Where(x => x.IsAdditional == filter.IsAdditional);
                }

                if (filter.IsDeleted.HasValue)
                {
                    qry = qry.Where(x => x.IsDeleted == filter.IsDeleted);
                }

                if (filter.IsMainVersion.HasValue)
                {
                    qry = qry.Where(x => x.IsMainVersion == filter.IsMainVersion);
                }

                if (filter.IsLastVersion.HasValue)
                {
                    qry = qry.Where(x => x.IsLastVersion == filter.IsLastVersion);
                }
            }

            return qry;
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, DmsContext dbContext, FilterDocumentAttachedFile filter, UIPaging paging = null)
        {
            var sq = GetDocumentFilesQuery(ctx, dbContext, filter);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = sq.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentAttachedFile>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    sq = sq.OrderByDescending(x => x.LastChangeDate)
                        .Skip(() => skip).Take(() => take);
                }
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
                          IsMainVersion = file.IsMainVersion,
                          IsLastVersion = file.IsLastVersion,
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
            var sq = GetDocumentFilesQuery(ctx, dbContext, new FilterDocumentAttachedFile { DocumentId = new List<int> { documentId } });

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
                    
                    IsMainVersion = x.IsMainVersion,
                    IsLastVersion = x.IsLastVersion,

                    Hash = x.Hash,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    Name = x.Name,
                    OrderInDocument = x.OrderNumber,
                    Version = x.Version,
                    WasChangedExternal = false
                }).ToList();
        }

        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext, bool isAll = false, bool isAddClientFilter = true)
        {
            var qry = dbContext.DocumentAccessesSet.AsQueryable();
            if (isAddClientFilter)
            {
                qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);
            }
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
            if (ctx != null && !ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
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
            var filterContains = PredicateBuilder.False<DictionaryPositions>();
            filterContains = positionIds.Aggregate(filterContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId)
                .Where(filterContains).Select(x => new InternalPositionInfo
                {
                    PositionId = x.Id,
                    PositionName = x.Name,
                    AgentId = x.ExecutorAgentId ?? 0,
                    AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name : ""
                }).ToList();
        }

        public static IQueryable<DocumentEvents> GetDocumentEventsQuery(IContext ctx, DmsContext dbContext)
        {
            var qry = dbContext.DocumentEventsSet.AsQueryable();

            //TODO Что то придумать с union

            if (!ctx.IsAdmin)
            {
                var filterPositionContains = PredicateBuilder.False<DocumentEvents>();
                filterPositionContains = ctx.CurrentPositionsIdList.Aggregate(filterPositionContains,
                    (current, value) => current.Or(e =>
                        e.TargetPositionId == value
                        || e.SourcePositionId == value).Expand());

                var filterTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterTaskAccessesContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => !x.IsAvailableWithinTask).Where(filterPositionContains)
                         .Concat(qry.Where(x => x.IsAvailableWithinTask && x.TaskId.HasValue && x.Task.TaskAccesses.AsQueryable().Any(filterTaskAccessesContains)));

                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            return qry;
        }

        public static IQueryable<DocumentWaits> GetDocumentWaitsQuery(DmsContext dbContext, IContext ctx, int? documentId = null)
        {
            var qry = dbContext.DocumentWaitsSet.AsQueryable();

            //TODO Что то придумать с union

            if (ctx != null && !ctx.IsAdmin)
            {
                var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
                filterOnEventPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventPositionsContains,
                    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value).Expand());

                var filterOnEventTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterOnEventTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => !x.OnEvent.IsAvailableWithinTask).Where(filterOnEventPositionsContains)
                        .Concat(
                                    qry.Where(x => x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue && x.OnEvent.Task.TaskAccesses.AsQueryable().Any(filterOnEventTaskAccessesContains))
                                );

                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId.Value);
            }
            return qry;
        }


        public static IQueryable<DocumentWaits> GetDocumentWaitsQuery1(DmsContext dbContext, IContext ctx, int? documentId = null)
        {
            var qry = dbContext.DocumentWaitsSet.AsQueryable();

            //TODO Что то придумать с union

            if (ctx != null && !ctx.IsAdmin)
            {
                var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
                filterOnEventPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventPositionsContains,
                    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value).Expand());

                qry = qry.Where(x => !x.OnEvent.IsAvailableWithinTask).Where(filterOnEventPositionsContains);

                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId.Value);
            }
            return qry;
        }


        public static IQueryable<DocumentWaits> GetDocumentWaitsQuery2(DmsContext dbContext, IContext ctx, int? documentId = null)
        {
            var qry = dbContext.DocumentWaitsSet.AsQueryable();

            //TODO Что то придумать с union

            if (ctx != null && !ctx.IsAdmin)
            {
                var filterOnEventTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterOnEventTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue && x.OnEvent.Task.TaskAccesses.AsQueryable().Any(filterOnEventTaskAccessesContains));

                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId.Value);
            }
            return qry;
        }

        public static IEnumerable<FrontDocumentTask> GetDocumentTasks(DmsContext dbContext, IContext ctx, FilterDocumentTask filter, UIPaging paging)
        {
            var tasksDb = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                tasksDb = tasksDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTasks>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    tasksDb = tasksDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTasks>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    tasksDb = tasksDb.Where(filterContains);
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
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = tasksDb.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentTask>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    tasksDb = tasksDb.OrderByDescending(x => x.LastChangeDate)
                        .Skip(() => skip).Take(() => take);
                }
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
            var waitsDb1 = GetDocumentWaitsQuery1(dbContext, ctx);
            var waitsDb2 = GetDocumentWaitsQuery2(dbContext, ctx);

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    waitsDb = waitsDb.Where(filterContains);
                    waitsDb1 = waitsDb1.Where(filterContains);
                    waitsDb2 = waitsDb2.Where(filterContains);
                }

                if (filter.OnEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OnEventId == filter.OnEventId.Value);
                    waitsDb1 = waitsDb1.Where(x => x.OnEventId == filter.OnEventId.Value);
                    waitsDb2 = waitsDb2.Where(x => x.OnEventId == filter.OnEventId.Value);
                }

                if (filter.OffEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OffEventId.HasValue && x.OffEventId.Value == filter.OffEventId.Value);
                    waitsDb1 = waitsDb1.Where(x => x.OffEventId.HasValue && x.OffEventId.Value == filter.OffEventId.Value);
                    waitsDb2 = waitsDb2.Where(x => x.OffEventId.HasValue && x.OffEventId.Value == filter.OffEventId.Value);
                }

                if (filter.Opened)
                {
                    waitsDb = waitsDb.Where(x => !x.OffEventId.HasValue);
                    waitsDb1 = waitsDb1.Where(x => !x.OffEventId.HasValue);
                    waitsDb2 = waitsDb2.Where(x => !x.OffEventId.HasValue);
                }
            }

            var waitsRes = waitsDb.OrderBy(x => x.DueDate).AsQueryable();
            var waitsRes1 = waitsDb1.OrderBy(x => x.DueDate).AsQueryable();
            var waitsRes2 = waitsDb2.OrderBy(x => x.DueDate).AsQueryable();


            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.Counters = new UICounters
                    {
                        Counter1 = waitsRes.Count(y => !y.OffEventId.HasValue),
                        Counter2 = waitsRes.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.Now),
                        Counter3 = waitsRes.Count(),
                    };

                    //TODO Подумать что лучше
                    //Нужно в одном запросе посчитать несколько каунтов
                    //paging.Counters = waitsRes.GroupBy(x => 1).Select(x => new UICounters
                    //{
                    //    Counter1 = x.Count(y => !y.OffEventId.HasValue),
                    //    Counter2 = x.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.Now),
                    //    Counter3 = x.Count()
                    //}).FirstOrDefault();

                    paging.TotalItemsCount = paging.Counters.Counter3.GetValueOrDefault();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentWait>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;
                    var skip1 = 0;
                    var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                    waitsRes = waitsRes
                        .Skip(() => skip).Take(() => take);

                    waitsRes = waitsRes1.Skip(() => skip1).Take(() => take1)
                        .Concat(waitsRes2.Skip(() => skip1).Take(() => take1))
                        .OrderBy(x => x.DueDate).Skip(() => skip).Take(() => take);

                }
            }

            var waitsResF =
                //waitsRes
                dbContext.DocumentWaitsSet.Where(x => waitsRes.Select(y => y.Id).Contains(x.Id))
                .Select(x => new FrontDocumentWait
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    ParentId = x.ParentId,
                    OnEventId = x.OnEventId,
                    OffEventId = x.OffEventId,
                    ResultTypeId = x.ResultTypeId,
                    ResultTypeName = x.ResultType.Name,
                    DueDate = x.DueDate > DateTime.Now.AddYears(50) ? null : x.DueDate,
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
                });
            var waits = waitsResF.ToList();

            waits.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return waits;

        }

        public static IQueryable<DocumentSubscriptions> GetDocumentSubscriptionsQuery(DmsContext dbContext, FilterDocumentSubscription filter, IContext ctx)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                subscriptionsDb = subscriptionsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
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
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = subscriptionsRes.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentSubscription>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    subscriptionsRes = subscriptionsRes
                        .Skip(() => skip).Take(() => take);
                }
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
                        DueDate = x.SendEvent.OnWait.FirstOrDefault().DueDate > DateTime.Now.AddYears(50) ? null : x.SendEvent.OnWait.FirstOrDefault().DueDate,

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
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = filter.CurrentPositionsId.Aggregate(filterContains,
                        (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    tagsDb = tagsDb.Where(filterContains);
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

        public static IQueryable<PropertyValues> GetPropertyValuesQuery(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.Object.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PropertyLink.ObjectId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }

                if (filter.RecordId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.RecordId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RecordId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            return itemsDb;

        }

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

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
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

            var items = itemsDb.Select(x => new InternalPropertyValue
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
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

            dbContext.PropertyValuesSet.RemoveRange(itemsDb);
        }

        public static IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, IContext ctx, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry.Select(x => new FrontDictionaryPosition
            {
                Id = x.PositionId,
                Name = x.Position.Name,
                DepartmentId = x.Position.DepartmentId,
                ExecutorAgentId = x.Position.ExecutorAgentId,
                DepartmentName = x.Position.Department.Name,
                ExecutorAgentName = x.Position.ExecutorAgent.Name,
                PositionPhone = "(888)888-88-88"
            }).ToList();

        }

        public static IEnumerable<FrontDocument> GetLinkedDocuments(IContext context, DmsContext dbContext, int linkId)
        {
            //var acc = CommonQueries.GetDocumentAccesses(context, dbContext, true);

            var items = CommonQueries.GetDocumentQuery(dbContext, context/*, acc*/)
                    .Where(x => x.LinkId == linkId)
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

        private static IQueryable<DocumentSendLists> GetDocumentSendListQuery(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
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
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }

            return sendListDb;
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = GetDocumentSendListQuery(dbContext, context, filter);

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
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentRestrictedSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
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
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                itemsDb = itemsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPapers>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPapers>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            itemsDb = itemsDb.OrderByDescending(x => x.LastChangeDate);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = itemsDb.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentPaper>();
                }


                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    itemsDb = itemsDb
                        .Skip(() => skip).Take(() => take);
                }
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
                    var filterContains = PredicateBuilder.False<DocumentPaperLists>();
                    filterContains = filter.PaperListId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
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

        public static List<InternalDictionaryPositionWithActions> GetPositionWithActions(IContext ctx, DmsContext dbContext, List<int> positionAccesses)
        {
            var filterCurrentPositionsContains = PredicateBuilder.False<DictionaryPositions>();
            filterCurrentPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterCurrentPositionsContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            var filterPositionAccessesContains = PredicateBuilder.False<DictionaryPositions>();
            filterPositionAccessesContains = positionAccesses.Aggregate(filterPositionAccessesContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                .Where(filterCurrentPositionsContains)
                .Where(filterPositionAccessesContains)
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
            var filterObjectsContains = PredicateBuilder.False<SystemActions>();
            filterObjectsContains = objects.Aggregate(filterObjectsContains,
                (current, value) => current.Or(e => (EnumObjects)e.ObjectId == value).Expand());

            var res = new Dictionary<int, List<InternalSystemAction>>();
            foreach (var posId in context.CurrentPositionsIdList)
            {
                if (positionAccesses.Contains(posId))
                {
                    var qry = dbContext.SystemActionsSet
                        .Where(filterObjectsContains)
                        .Where(x => x.IsVisible &&
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
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
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
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
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

            {
                var filterContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterContains = delId.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(filterContains));
            }

            dbContext.DocumentTaskAccessesSet.AddRange(insTA.Select(x => new DocumentTaskAccesses { TaskId = x.TaskId, PositionId = x.PositionId }));

        }

    }
}