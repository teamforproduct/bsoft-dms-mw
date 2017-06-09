using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.IncomingModel;
using System.Data.Entity;
using BL.Model.AdminCore;
using BL.Model.SystemCore;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Exception;
using LinqKit;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.Helpers;
using EntityFramework.Extensions;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.FullTextSearch;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.System;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        #region DocumentAction

        public DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int? documentId, int? id = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();
                var qry = CommonQueries.GetDocumentQuery(context, null);
                if (documentId.HasValue)
                {
                    qry = qry.Where(x => x.Id == documentId);
                }
                if (id.HasValue)
                {
                    qry = qry.Where(x => x.Events.Any(y => y.Id == id.Value));
                }
                res.Document = qry.Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    IsRegistered = x.IsRegistered,
                    IsLaunchPlan = x.IsLaunchPlan,
                    ExecutorPositionId = x.ExecutorPositionId,
                    LinkId = x.LinkId,
                    DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                    AccessesCount = x.Accesses.Count(),
                }).FirstOrDefault();

                if (res.Document != null)
                {
                    documentId = res.Document.Id;
                    var strDocumentDirection = ((int)res.Document.DocumentDirection).ToString();
                    res.Document.Accesses = CommonQueries.GetDocumentAccessesesQry(context, res.Document.Id, true)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                            IsFavourite = x.IsFavourite,
                            CountWaits = x.CountWaits,
                            IsCanRegisterDoc = dbContext.AdminRegistrationJournalPositionsSet
                                .Any(y => y.PositionId == x.PositionId &&
                                            y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration &&
                                            y.RegistrationJournal.DirectionCodes.Contains(strDocumentDirection) &&
                                            y.RegistrationJournal.ClientId == context.Client.Id
                                    )
                        }
                        ).ToList();
                    res.Document.IsInWork = res.Document.Accesses.Any(x => x.IsInWork);
                    res.Document.IsFavourite = res.Document.Accesses.Any(x => x.IsFavourite);

                    var qryEvents = CommonQueries.GetDocumentEventQuery(context,null);
                    if (id.HasValue)
                        qryEvents = qryEvents.Where(x => x.Id == id);
                    else
                        qryEvents = qryEvents.Where(x => x.DocumentId == documentId);
                    res.Document.Events = qryEvents.Select(x => new InternalDocumentEvent
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }
                        ).ToList();

                    var qryWaits = CommonQueries.GetDocumentWaitQuery(context, null);
                    if (id.HasValue)
                        qryWaits = qryWaits.Where(x => x.OnEventId == id);
                    else
                        qryWaits = qryWaits.Where(x => x.DocumentId == documentId);
                    res.Document.Waits = qryWaits.Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        OffEventId = x.OffEventId,
                        ParentId = x.ParentId,
                        IsHasMarkExecution = x.ChildWaits.Any(y => y.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution && !y.OffEventId.HasValue),
                        IsHasAskPostponeDueDate = x.ChildWaits.Any(y => y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate && !y.OffEventId.HasValue),
                        OnEvent = new InternalDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            TaskId = x.OnEvent.TaskId,
                            SourcePositionId = x.OnEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                            ControllerPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Controller)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                        }

                    }
                        ).ToList();

                    res.Document.Subscriptions =
                        dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == documentId)
                            .Select(x => new InternalDocumentSubscription
                            {
                                Id = x.Id,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                SubscriptionStatesId = x.SubscriptionStateId,
                                SubscriptionStatesIsSuccess = x.SubscriptionState.IsSuccess,
                            }
                            ).ToList();
                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentSendList
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                        }
                        ).ToList();
                    res.Document.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentTask
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                        }
                        ).ToList();

                    var qryFiles = CommonQueries.GetDocumentFileQuery(context, null);
                    if (id.HasValue)
                        qryFiles = qryFiles.Where(x => x.EventId == id);
                    else
                        qryFiles = qryFiles.Where(x => x.DocumentId == documentId);
                    res.Document.DocumentFiles = qryFiles.Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        Type = (EnumFileTypes)x.TypeId,
                        IsWorkedOut = x.IsWorkedOut,
                        IsMainVersion = x.IsMainVersion,
                        IsDeleted = x.IsDeleted,
                    }).ToList();

                    var positionAccesses = res.Document?.Accesses.Where(y => y.PositionId.HasValue).Select(y => y.PositionId.Value).ToList();

                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = GetBlankPositionWithActions(context, positionAccesses);
                        res.ActionsList = GetActionsListForCurrentPositionsList(context, new List<EnumObjects> { EnumObjects.Documents, EnumObjects.DocumentEvents, EnumObjects.DocumentWaits, EnumObjects.DocumentSubscriptions, EnumObjects.DocumentFiles }, positionAccesses, true);
                    }
                }
                transaction.Complete();
                return res;
            }
        }
        public DocumentActionsModel GetDocumentSendListActionsModelPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

                res.Document = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId } })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                        }
                        ).ToList();

                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == documentId)
                         .Select(x => new InternalDocumentSendList
                         {
                             Id = x.Id,
                             ClientId = x.ClientId,
                             EntityTypeId = x.EntityTypeId,
                             SourcePositionId = x.AccessGroups.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                             StartEventId = x.StartEventId,
                             CloseEventId = x.CloseEventId,
                         }
                         ).ToList();


                    var positionAccesses = res.Document?.Accesses.Where(y => y.PositionId.HasValue).Select(y => y.PositionId.Value).ToList();

                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = GetBlankPositionWithActions(context, positionAccesses);
                        res.ActionsList = GetActionsListForCurrentPositionsList(context, new List<EnumObjects> { EnumObjects.DocumentSendLists, EnumObjects.DocumentSendListStages }, positionAccesses);
                    }
                }
                transaction.Complete();
                return res;
            }
        }
        public DocumentActionsModel GetDocumentFileActionsModelPrepare(IContext context, int? documentId, int? id = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

                var qry = CommonQueries.GetDocumentQuery(context, null);

                if (documentId.HasValue)
                {
                    qry = qry.Where(x => x.Id == documentId);
                }
                if (id.HasValue)
                {
                    qry = qry.Where(x => x.Files.Any(y => y.Id == id.Value));
                }
                res.Document = qry.Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    IsRegistered = x.IsRegistered,
                    ExecutorPositionId = x.ExecutorPositionId,
                    LinkId = x.LinkId,
                }).FirstOrDefault();

                if (res.Document != null)
                {
                    documentId = res.Document.Id;
                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                        }
                        ).ToList();
                    var qryFiles = CommonQueries.GetDocumentFileQuery(context, null);
                    if (id.HasValue)
                    {
                        qryFiles = qryFiles.Where(x => x.Id == id);
                    }
                    else
                    {
                        qryFiles = qryFiles.Where(x => x.DocumentId == documentId);
                    }
                    res.Document.DocumentFiles = qryFiles.Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        Type = (EnumFileTypes)x.TypeId,
                        IsWorkedOut = x.IsWorkedOut,
                        IsMainVersion = x.IsMainVersion,
                        IsDeleted = x.IsDeleted,
                    }).ToList();
                    var positionAccesses = res.Document?.Accesses.Where(y => y.PositionId.HasValue).Select(y => y.PositionId.Value).ToList();
                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = GetBlankPositionWithActions(context, positionAccesses);
                        res.ActionsList = GetActionsListForCurrentPositionsList(context, new List<EnumObjects> { EnumObjects.DocumentFiles }, positionAccesses);
                    }
                }
                transaction.Complete();
                return res;
            }
        }
        public DocumentActionsModel GetDocumentPaperActionsModelPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

                res.Document = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId } })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        //IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        //LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {
                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                        }
                        ).ToList();

                    res.Document.Papers = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == documentId)
                         .Select(x => new InternalDocumentPaper
                         {
                             Id = x.Id,
                             ClientId = x.ClientId,
                             EntityTypeId = x.EntityTypeId,
                             IsInWork = x.IsInWork,
                             LastPaperEvent = !x.LastPaperEventId.HasValue ? null :
                                            new InternalDocumentEvent
                                            {
                                                Id = x.LastPaperEvent.Id,
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                                SourcePositionId = x.LastPaperEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                                TargetPositionId = x.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                                                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,

                                            }
                         }
                         ).ToList();

                    var positionAccesses = res.Document?.Accesses.Where(y => y.PositionId.HasValue).Select(y => y.PositionId.Value).ToList();
                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = GetBlankPositionWithActions(context, positionAccesses);
                        res.ActionsList = GetActionsListForCurrentPositionsList(context, new List<EnumObjects> { EnumObjects.DocumentPapers, EnumObjects.DocumentPaperEvents }, positionAccesses);
                    }
                }
                transaction.Complete();
                return res;
            }
        }
        private List<InternalDictionaryPositionWithActions> GetBlankPositionWithActions(IContext context, List<int> positionAccesses)
        {
            var dbContext = context.DbContext as DmsContext;
            var filterCurrentPositionsContains = PredicateBuilder.New<DictionaryPositions>(false);
            filterCurrentPositionsContains = context.CurrentPositionsIdList.Aggregate(filterCurrentPositionsContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            var filterPositionAccessesContains = PredicateBuilder.New<DictionaryPositions>(false);
            filterPositionAccessesContains = positionAccesses.Aggregate(filterPositionAccessesContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.Client.Id)
                .Where(filterCurrentPositionsContains)
                .Where(filterPositionAccessesContains)
                .Select(x => new InternalDictionaryPositionWithActions
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    ExecutorAgentId = x.ExecutorAgentId,
                    DepartmentName = x.Department.Name,
                    ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                }).ToList();
        }
        private Dictionary<int, List<InternalSystemActionForDocument>> GetActionsListForCurrentPositionsList(IContext context, IEnumerable<EnumObjects> objects, List<int> positionAccesses, bool IsNotEmptyCategory = false)
        {
            var dbContext = context.DbContext as DmsContext;
            var filterObjectsContains = PredicateBuilder.New<SystemActions>(false);
            filterObjectsContains = objects.Aggregate(filterObjectsContains,
                (current, value) => current.Or(e => (EnumObjects)e.ObjectId == value).Expand());

            var res = new Dictionary<int, List<InternalSystemActionForDocument>>();
            foreach (var posId in context.CurrentPositionsIdList)
            {
                if (positionAccesses.Contains(posId))
                {
                    var qry = dbContext.SystemActionsSet.Where(filterObjectsContains);
                    if (IsNotEmptyCategory)
                        qry = qry.Where(x => x.CategoryId.HasValue);
                    qry = qry.Where(x => (x.Permission.RolePermissions.Any(y => y.Role.PositionRoles.Any(pr => pr.PositionId == posId) &&
                                        y.Role.UserRoles.Any(z => z.PositionExecutor.AgentId == context.CurrentAgentId))));
                    var qryActLst = qry.OrderBy(x => x.CategoryId).ThenBy(x => x.Id).Select(x => new InternalSystemActionForDocument
                    {
                        DocumentAction = (EnumActions)x.Id,
                        Object = (EnumObjects)x.ObjectId,
                        ActionCode = x.Code,
                        ObjectCode = x.Object.Code,
                        Description = "##l@Actions:" + ((EnumActions)x.Id).ToString() + "@l##",
                        Category = (EnumActionCategories)x.CategoryId,
                        
                    });
                    var actLst = qryActLst.ToList();
                    res.Add(posId, actLst);
                }
            }
            return res;
        }


        #endregion DocumentAction   

        #region Get

        public FrontDocumentEvent GetDocumentEvent(IContext context, int eventId)
        {

            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentEventQuery(context, new FilterDocumentEvent { EventId = new List<int> { eventId } })
                    .Select(x => new FrontDocumentEvent
                    {
                        Id = x.Id,
                        DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                        DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.DocumentType.Name : null,
                        DocumentDirectionName = x.Document.LinkId.HasValue ? "##l@DocumentDirections:" + ((EnumDocumentDirections)x.Document.DocumentDirectionId).ToString() + "@l##" : null,
                        PaperPlanAgentName = x.PaperPlanAgent.Name,
                        PaperSendAgentName = x.PaperSendAgent.Name,
                        PaperRecieveAgentName = x.PaperRecieveAgent.Name,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
                if (res != null)
                    CommonQueries.SetAccesses(context, new List<FrontDocumentEvent> { res });
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext context, FilterBase filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                #region qry
                var qrys = CommonQueries.GetDocumentEventQueries(context, filter?.Event);

                if (filter?.Document != null)
                {
                    var documentIds = CommonQueries.GetDocumentQuery(context, filter?.Document).Select(x => x.Id);
                    qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
                }

                if (filter?.File != null)
                {
                    var documentIds = CommonQueries.GetDocumentFileQuery(context, filter?.File).Select(x => x.DocumentId);

                    qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
                }

                if (filter?.Wait != null)
                {
                    var waits = CommonQueries.GetDocumentWaitQuery(context, filter?.Wait);

                    var waitOnEventIds = waits.Select(x => x.OnEventId);
                    var waitOffEventIds = waits.Select(x => x.OffEventId);

                    qrys = qrys.Select(qry => { return qry.Where(x => waitOnEventIds.Contains(x.Id) || waitOffEventIds.Contains(x.Id)); }).ToList();
                }

                //TODO Sort
                qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.Date).AsQueryable(); }).ToList();
                #endregion qry   

                #region paging
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        //var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                        //filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                        //paging.Counters = new UICounters
                        //{
                        //    Counter1 = qrys.Sum(qry => qry.Where(filterContains).Count(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)),
                        //    Counter3 = qrys.Sum(qry => qry.Count()),
                        //};

                        paging.TotalItemsCount = qrys.Sum(qry => qry.Count());//paging.Counters.Counter3.GetValueOrDefault();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDocumentEvent>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        if (qrys.Count > 1)
                        {
                            var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                            qrys = qrys.Select(qry => qry.Take(() => take1)).ToList();

                            var qryConcat = qrys.First();

                            foreach (var qry in qrys.Skip(1).ToList())
                            {
                                qryConcat = qryConcat.Concat(qry);
                            }

                            qrys.Clear();
                            qrys.Add(qryConcat);
                        }

                        //TODO Sort
                        qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.Date).AsQueryable(); }).ToList();

                        qrys = qrys.Select(qry => qry.Skip(() => skip).Take(() => take)).ToList();
                    }
                }

                if ((paging?.IsAll ?? true) && (filter == null || filter.Event == null || ((filter.Event.DocumentId?.Count ?? 0) == 0 && (filter.Event.EventId?.Count ?? 0) == 0)))
                {
                    throw new WrongAPIParameters();
                }
                #endregion paging

                #region filling
                IQueryable<DocumentEvents> qryRes = qrys.First(); ;

                if (qrys.Count > 1)
                {
                    foreach (var qry in qrys.Skip(1).ToList())
                    {
                        qryRes = qryRes.Concat(qry);
                    }
                }

                var isNeedRegistrationFullNumber = !(filter?.Event?.DocumentId?.Any() ?? false);

                var filterPositionContains = PredicateBuilder.New<DocumentEventAccesses>(false);
                filterPositionContains = context.CurrentPositionsAccessLevel.Aggregate(filterPositionContains,
                    (current, value) => current.Or(e => e.PositionId == value.Key && e.Document.Accesses.Any(x => x.PositionId == value.Key && x.AccessLevelId >= value.Value)).Expand());

                var qryView = dbContext.DocumentEventsSet  //Without security restrictions
                    .Where(x => qryRes.Select(y => y.Id).Contains(x.Id))
                    .OrderByDescending(x => x.Date)
                    .Select(x => new FrontDocumentEvent
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        EventType = (EnumEventTypes)x.EventTypeId,
                        EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.EventTypeId).ToString() + "@l##",
                        Date = x.Date,
                        CreateDate = x.Date != x.CreateDate ? (DateTime?)x.CreateDate : null,
                        Task = x.Task.Task,
                        Description = x.Description,
                        AddDescription = x.AddDescription,
                        //SourcePositionExecutorAgentName = x.SourcePositionExecutorAgent.Name + (x.SourcePositionExecutorType.Suffix != null ? " (" + x.SourcePositionExecutorType.Suffix + ")" : null),
                        //TargetPositionExecutorAgentName = (x.TargetPositionExecutorAgent.Name + (x.TargetPositionExecutorType.Suffix != null ? " (" + x.TargetPositionExecutorType.Suffix + ")" : null))
                        //                                  ?? x.TargetAgent.Name,
                        DocumentDate = (x.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                        RegistrationNumber = x.Document.RegistrationNumber,
                        RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                        RegistrationFullNumber = "#" + x.Document.Id,

                        OnWait = x.OnWait.Select(y => new FrontDocumentWait { DueDate = y.DueDate, OffEventDate = (DateTime?)y.OffEvent.Date }).FirstOrDefault(),

                        //For IsRead
                        //TargetPositionId = x.TargetPositionId,
                        //SourcePositionId = x.SourcePositionId,
                        //ReadDate = x.ReadDate,
                        IsRead = !x.Accesses.AsQueryable().Where(filterPositionContains).Any(y => !y.ReadDate.HasValue),

                        PaperId = x.Paper.Id,
                        PaperName = x.Paper.Name,
                        PaperIsMain = x.Paper.IsMain,
                        PaperIsOriginal = x.Paper.IsOriginal,
                        PaperIsCopy = x.Paper.IsCopy,
                        PaperOrderNumber = x.Paper.OrderNumber,

                        PaperPlanDate = x.PaperPlanDate,
                        PaperSendDate = x.PaperSendDate,
                        PaperRecieveDate = x.PaperRecieveDate,
                    });

                var res = qryView.ToList();

                res.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                CommonQueries.SetAccessGroups(context, res);
                CommonQueries.SetFiles(context, res);
                CommonQueries.SetWaitInfo(context, res);

                #endregion filling

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext context, FilterBase filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                #region qry
                var qrys = CommonQueries.GetDocumentWaitQueries(context, filter?.Wait);

                if (filter?.Document != null)
                {
                    var documentIds = CommonQueries.GetDocumentQuery(context, filter?.Document).Select(x => x.Id);
                    qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
                }

                if (filter?.Event != null)
                {
                    var eventIds = CommonQueries.GetDocumentEventQuery(context, filter?.Event).Select(x => x.Id);

                    qrys = qrys.Select(qry => { return qry.Where(x => eventIds.Contains(x.OnEventId) || eventIds.Contains(x.OffEventId.Value)); }).ToList();
                }

                if (filter?.File != null)
                {
                    var documentIds = CommonQueries.GetDocumentFileQuery(context, filter?.File).Select(x => x.DocumentId);

                    qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
                }

                //TODO Sort
                qrys = qrys.Select(qry => { return qry.OrderBy(x => x.DueDate).AsQueryable(); }).ToList();
                #endregion qry  

                #region paging
                if (paging != null)
                {
                    var isDetail = (filter?.Wait?.DocumentId == null) && (paging.IsOnlyCounter ?? false);
                    List<FrontDocumentWait> groupsCounter = null;
                    if (paging.IsOnlyCounter ?? true)
                    {
                        var qryGroupsCounter = qrys.Select(qry => qry.GroupBy(y => new
                        {
                            IsClosed = y.OffEventId.HasValue,
                            IsOverDue = !y.OffEventId.HasValue && y.DueDate.HasValue && y.DueDate.Value <= DateTime.UtcNow,
                            DueDate = isDetail ? DbFunctions.TruncateTime(y.DueDate) : null,
                            SourcePositionExecutorAgentName = isDetail ?
                            y.OnEvent.Accesses.Where(z => z.AccessTypeId == (int)EnumEventAccessTypes.Source)
                                .Select(z => z.Agent.Name + (z.PositionExecutorType.Suffix != null ? " (" + z.PositionExecutorType.Suffix + ")" : null)).FirstOrDefault()
                            : null,
                            TargetPositionExecutorAgentName = isDetail ?
                            y.OnEvent.Accesses.Where(z => z.AccessTypeId <= (int)EnumEventAccessTypes.Target).OrderByDescending(z => z.AccessTypeId)
                                .Select(z => z.Agent.Name + (z.PositionExecutorType.Suffix != null ? " (" + z.PositionExecutorType.Suffix + ")" : null)).FirstOrDefault()
                            : null,
                        })
                        .Select(y => new { Group = y.Key, RecordCount = y.Count() }).ToList()
                                            ).ToList();
                        groupsCounter = qryGroupsCounter
                                           .SelectMany(z => z)
                                           .GroupBy(z => z.Group)
                                           .Select(z => new FrontDocumentWait
                                           {
                                               IsClosed = z.Key.IsClosed,
                                               IsOverDue = z.Key.IsOverDue,
                                               DueDate = z.Key.DueDate,
                                               SourcePositionExecutorAgentName = z.Key.SourcePositionExecutorAgentName,
                                               TargetPositionExecutorAgentName = z.Key.TargetPositionExecutorAgentName,
                                               RecordCount = z.Sum(c => c.RecordCount)
                                           }).ToList();

                        paging.Counters = new UICounters
                        {
                            //Counter1 = qrys.Sum(qry => qry.Count(y => !y.OffEventId.HasValue)),
                            //Counter2 = qrys.Sum(qry => qry.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow)),
                            //Counter3 = qrys.Sum(qry => qry.Count()),
                            Counter1 = groupsCounter.Where(y => !y.IsClosed).Sum(y => y.RecordCount),
                            Counter2 = groupsCounter.Where(y => y.IsOverDue).Sum(y => y.RecordCount),
                            Counter3 = groupsCounter.Sum(y => y.RecordCount),
                        };

                        paging.TotalItemsCount = paging.Counters.Counter3.GetValueOrDefault();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return isDetail && groupsCounter != null ? groupsCounter : new List<FrontDocumentWait>();
                    }

                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    if (qrys.Count > 1)
                    {
                        var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                        qrys = qrys.Select(qry => qry.Take(() => take1)).ToList();

                        var qryConcat = qrys.First();

                        foreach (var qry in qrys.Skip(1).ToList())
                        {
                            qryConcat = qryConcat.Concat(qry);
                        }

                        qrys.Clear();
                        qrys.Add(qryConcat);
                    }

                    //TODO Sort
                    qrys = qrys.Select(qry => { return qry.OrderBy(x => x.DueDate).AsQueryable(); }).ToList();

                    qrys = qrys.Select(qry => qry.Skip(() => skip).Take(() => take)).ToList();
                }
                #endregion paging

                #region filling
                IQueryable<DocumentWaits> qryRes = qrys.First();

                if (qrys.Count > 1)
                {
                    foreach (var qry in qrys.Skip(1).ToList())
                    {
                        qryRes = qryRes.Concat(qry);
                    }
                }

                var maxDateTime = DateTime.UtcNow.AddYears(50);
                var isNeedRegistrationFullNumber = !(filter?.Wait?.DocumentId?.Any() ?? false);

                var qryFE = dbContext.DocumentWaitsSet  //Without security restrictions
                    .Where(x => qryRes.Select(y => y.Id).Contains(x.Id))
                    .OrderBy(x => x.DueDate)
                    .Select(x => new FrontDocumentWait
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        ParentId = x.ParentId,
                        OnEventId = x.OnEventId,
                        OffEventId = x.OffEventId,
                        ResultType = (EnumResultTypes)x.ResultTypeId,
                        ResultTypeName = x.ResultType.Name,
                        PlanDueDate = x.PlanDueDate,
                        DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                        AttentionDate = x.AttentionDate,
                        TargetDescription = x.TargetDescription,
                        //TargetAttentionDate = x.TargetAttentionDate,
                        IsClosed = x.OffEvent != null,
                        IsOverDue = !x.OffEventId.HasValue && x.DueDate.HasValue && x.DueDate.Value <= DateTime.UtcNow,
                        DocumentDate = (x.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                        RegistrationNumber = x.Document.RegistrationNumber,
                        RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                        RegistrationFullNumber = "#" + x.Document.Id,
                        //DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                        //DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                        //DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                        OnEvent = new FrontDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            DocumentId = x.OnEvent.DocumentId,
                            Task = x.OnEvent.Task.Task,
                            Description = x.OnEvent.Description,
                            AddDescription = x.OnEvent.AddDescription,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                            EventTypeName = x.OnEvent.EventType.WaitDescription/*?? x.OnEvent.EventType.Name*/,
                            Date = x.OnEvent.Date,
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
                            EventType = (EnumEventTypes)x.OffEvent.EventTypeId,
                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.OffEvent.EventTypeId).ToString() + "@l##",
                            Date = x.OffEvent.Date,
                        }
                    });

                var res = qryFE.ToList();

                res.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                CommonQueries.SetAccessGroups(context, res.Where(x => x.OnEvent != null).Select(x => x.OnEvent).Concat(res.Where(x => x.OffEvent != null).Select(x => x.OffEvent)).ToList());
                #endregion filling

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext context, FilterDocumentSubscription filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var subscriptionsDb = CommonQueries.GetDocumentSubscriptionsQuery(context, filter);

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
                if ((paging?.IsAll ?? true) && (filter == null || (filter.DocumentId?.Count ?? 0) == 0))
                {
                    throw new WrongAPIParameters();
                }

                var maxDateTime = DateTime.UtcNow.AddYears(50);

                var res = subscriptionsRes.Select(x => new FrontDocumentSubscription
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
                    RegistrationNumber = x.Document.RegistrationNumber,
                    RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                    RegistrationFullNumber = "#" + x.Document.Id,
                    //DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                    //DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                    //DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                    SendEvent = x.SendEvent == null
                        ? null
                        : new FrontDocumentEvent
                        {
                            Id = x.SendEvent.Id,
                            DocumentId = x.SendEvent.DocumentId,
                            EventType = (EnumEventTypes)x.SendEvent.EventTypeId,
                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.SendEvent.EventTypeId).ToString() + "@l##",
                            DueDate = x.SendEvent.OnWait.FirstOrDefault().DueDate > maxDateTime ? null : x.SendEvent.OnWait.FirstOrDefault().DueDate,
                            Date = x.SendEvent.Date,
                            Description = x.SendEvent.Description,
                            AddDescription = x.SendEvent.AddDescription,
                        },
                    DoneEvent = x.DoneEvent == null
                        ? null
                        : new FrontDocumentEvent
                        {
                            Id = x.DoneEvent.Id,
                            DocumentId = x.DoneEvent.DocumentId,
                            EventType = (EnumEventTypes)x.DoneEvent.EventTypeId,
                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.DoneEvent.EventTypeId).ToString() + "@l##",
                            DueDate = null,
                            Date = x.DoneEvent.Date,
                            Description = x.DoneEvent.Description,
                            AddDescription = x.DoneEvent.AddDescription,
                        },

                    SigningType = (EnumSigningTypes)x.SigningTypeId,
                    CertificateId = x.CertificateId,
                    CertificateName = x.Certificate.Name,
                    CertificatePositionId = x.CertificatePositionId,
                    CertificatePositionExecutorAgentId = x.CertificatePositionExecutorAgentId,
                    CertificatePositionName = x.CertificatePosition.Name,
                    CertificatePositionExecutorAgentName = x.CertificatePositionExecutorAgent.Name + (x.CertificatePositionExecutorType.Suffix != null ? " (" + x.CertificatePositionExecutorType.Suffix + ")" : null),
                    CertificateSignCreateDate = x.CertificateSignCreateDate,


                }).ToList();

                res.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                CommonQueries.SetAccessGroups(context, res.Where(x => x.SendEvent != null).Select(x => x.SendEvent).Concat(res.Where(x => x.DoneEvent != null).Select(x => x.DoneEvent)).ToList());
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(IContext context, FilterDictionaryPosition filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentWorkGroupQuery(context, filter);
                qry = qry.OrderBy(x => x.Position.Name);
                var res = qry.Where(x => x.PositionId.HasValue).Select(x => new FrontDictionaryPosition
                {
                    Id = x.PositionId.Value,
                    Name = x.Position.Name,
                    DepartmentId = x.Position.DepartmentId,
                    ExecutorAgentId = x.Position.ExecutorAgentId,
                    DepartmentName = x.Position.Department.Name,
                    ExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : (string)null),
                    IsActive = x.IsActive,
                    Order = x.Position.Order,
                    IsChoosen = x.IsInWork,
                    AccessLevelId = x.AccessLevelId,
                }).Distinct().ToList();
                transaction.Complete();
                res.ForEach(x => x.IsChoosen = (((x.IsChoosen ?? false) && (context.CurrentPositionsAccessLevel.Any(y => y.Key == x.Id && y.Value <= x.AccessLevelId))) ? true : (bool?)null));
                return res;
            }
        }
        public int GetDocumentWorkGroupCounter(IContext context, FilterDictionaryPosition filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentWorkGroupQuery(context, filter);
                var res = qry.Count();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetInternalDocumentAccesses(context, documentId);
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext context, List<int> positionIds)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetInternalPositionsInfo(context, positionIds);
                transaction.Complete();
                return res;
            }
        }

        #endregion Get  

        #region DocumentMainLogic  

        public void AddDocumentWaits(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                {
                    var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First(x => x.Id == 0));
                    dbContext.DocumentTasksSet.Add(taskDb);
                    dbContext.SaveChanges();
                    ((List<InternalDocumentWait>)document.Waits).ForEach(x => x.OnEvent.TaskId = taskDb.Id);
                }
                if (document.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();
                }
                if (document.Waits?.Any() ?? false)
                {
                    var waitDb = ModelConverter.GetDbDocumentWaits(document.Waits).ToList();
                    dbContext.DocumentWaitsSet.AddRange(waitDb);
                    dbContext.SaveChanges();
                    var waits = document.Waits.ToList();
                    for (var i = 0; i < waitDb.Count(); i++) waits[i].OnEventId = waitDb[i].OnEventId;

                    var positions = document.Waits.SelectMany(x => x.OnEvent.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                    CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id, positions);
                }
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);

                transaction.Complete();

            }
        }

        public void ChangeDocumentWait(IContext context, InternalDocumentWait wait)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var waitParentDb = ModelConverter.GetDbDocumentWait(wait.ParentWait);
                dbContext.DocumentWaitsSet.Add(waitParentDb);
                dbContext.SaveChanges();
                dbContext.DocumentFilesSet  //Without security restrictions
                    .Where(x => x.EventId == wait.OnEvent.Id)
                    .Update(x => new DocumentFiles { EventId = waitParentDb.OnEventId }); //перекидываем файлы на новый ИД

                var eventDb = ModelConverter.GetDbDocumentEvent(wait.OnEvent);
                eventDb.Id = wait.OnEvent.Id;
                eventDb.ParentEventId = waitParentDb.OnEventId;
                dbContext.SafeAttach(eventDb);
                dbContext.Entry(eventDb).State = EntityState.Modified;
                dbContext.SaveChanges();

                wait.OnEvent = null;

                var waitDb = ModelConverter.GetDbDocumentWait(wait);
                waitDb.Id = wait.Id;
                waitDb.ParentId = waitParentDb.Id;
                waitDb.ParentWait = null;
                dbContext.SafeAttach(waitDb);
                dbContext.Entry(waitDb).State = EntityState.Modified;
                dbContext.SaveChanges();

                if (wait.AskPostponeDueDateWait != null)
                {
                    var askWaitDb = ModelConverter.GetDbDocumentWait(wait.AskPostponeDueDateWait);
                    askWaitDb.ParentId = waitParentDb.Id;
                    askWaitDb.OnEvent = null;
                    dbContext.SafeAttach(askWaitDb);
                    var entry = dbContext.Entry(askWaitDb);
                    entry.Property(x => x.ResultTypeId).IsModified = true;
                    entry.Property(x => x.ParentId).IsModified = true;
                    entry.Property(x => x.OffEventId).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    dbContext.SaveChanges();
                }
                var positions = wait.ParentWait.OnEvent.Accesses.Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                CommonQueries.ModifyDocumentAccessesStatistics(context, wait.DocumentId, positions);
                CommonQueries.AddFullTextCacheInfo(context, wait.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void ChangeTargetDocumentWait(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (document.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();
                }
                var eventsDb = ModelConverter.GetDbDocumentEvents(document.Events).ToList();
                dbContext.DocumentEventsSet.AddRange(eventsDb);
                dbContext.SaveChanges();
                var events = document.Events.ToList();
                for (var i = 0; i < eventsDb.Count(); i++) events[i].Id = eventsDb[i].Id;

                var waitDb = ModelConverter.GetDbDocumentWait(document.Waits.First());
                dbContext.SafeAttach(waitDb);
                var entry = dbContext.Entry(waitDb);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.TargetDescription).IsModified = true;
                entry.Property(x => x.AttentionDate).IsModified = true;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void CloseDocumentWait(IContext context, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign, string serverMapPath)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (document.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();
                }
                var offEventDb = ModelConverter.GetDbDocumentEvent(document.Waits.First().OffEvent);
                foreach (var docWait in document.Waits)
                {
                    var wait = ModelConverter.GetDbDocumentWait(docWait);
                    wait.OffEvent = null;
                    dbContext.SafeAttach(wait);
                    wait.OffEvent = offEventDb;
                    var entry = dbContext.Entry(wait);
                    entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.ResultTypeId).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }
                var sendList = document.SendLists?.FirstOrDefault();
                if (sendList != null)
                {

                    var sendListDb = new DocumentSendLists
                    {
                        Id = sendList.Id,
                        LastChangeDate = sendList.LastChangeDate,
                        LastChangeUserId = sendList.LastChangeUserId
                    };
                    dbContext.SafeAttach(sendListDb);
                    var entry = dbContext.Entry(sendListDb);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    if (sendList.StartEventId != null)
                    {
                        sendListDb.CloseEvent = offEventDb;
                    }
                    else
                    {
                        sendListDb.StartEventId = null;
                        entry.Property(x => x.StartEventId).IsModified = true;
                    }
                }
                var subscription = document.Subscriptions?.FirstOrDefault();
                if (subscription != null)
                {
                    var docHash = CommonQueries.GetDocumentHash(context, document.Id,
                                                                isUseInternalSign, isUseCertificateSign, subscription, serverMapPath,
                                                                 subscription.SubscriptionStates == EnumSubscriptionStates.Sign ||
                                                                 subscription.SubscriptionStates == EnumSubscriptionStates.Visa ||
                                                                 subscription.SubscriptionStates == EnumSubscriptionStates.Аgreement ||
                                                                 subscription.SubscriptionStates == EnumSubscriptionStates.Аpproval,
                                                                 true);
                    var subscriptionDb = new DocumentSubscriptions
                    {
                        Id = subscription.Id,
                        Description = subscription.Description,
                        SubscriptionStateId = (int)subscription.SubscriptionStates,
                        Hash = docHash?.Hash,
                        FullHash = docHash?.FullHash,

                        SigningTypeId = (int)subscription.SigningType,

                        InternalSign = docHash?.InternalSign,
                        CertificateSign = docHash?.CertificateSign,
                        CertificateId = subscription.CertificateId,
                        CertificateSignCreateDate = subscription.SigningType == EnumSigningTypes.CertificateSign ? DateTime.UtcNow : (DateTime?)null,
                        CertificatePositionId = subscription.CertificatePositionId,
                        CertificatePositionExecutorAgentId = subscription.CertificatePositionExecutorAgentId,
                        CertificatePositionExecutorTypeId = subscription.CertificatePositionExecutorTypeId,
                        LastChangeDate = subscription.LastChangeDate,
                        LastChangeUserId = subscription.LastChangeUserId
                    };
                    dbContext.SafeAttach(subscriptionDb);
                    if (subscription.DoneEvent != null)
                    {
                        subscriptionDb.DoneEvent = offEventDb;
                    }
                    var entry = dbContext.Entry(subscriptionDb);
                    entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.Description).IsModified = true;
                    entry.Property(x => x.SubscriptionStateId).IsModified = true;
                    entry.Property(x => x.Hash).IsModified = true;
                    entry.Property(x => x.FullHash).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;

                    entry.Property(x => x.SigningTypeId).IsModified = true;

                    entry.Property(x => x.InternalSign).IsModified = true;
                    entry.Property(x => x.CertificateSign).IsModified = true;
                    entry.Property(x => x.CertificateId).IsModified = true;
                    entry.Property(x => x.CertificateSignCreateDate).IsModified = true;
                    entry.Property(x => x.CertificatePositionId).IsModified = true;
                    entry.Property(x => x.CertificatePositionExecutorAgentId).IsModified = true;
                    entry.Property(x => x.CertificatePositionExecutorTypeId).IsModified = true;
                }
                dbContext.SaveChanges();
                var waits = document.Waits.ToList();
                for (var i = 0; i < waits.Count(); i++) waits[i].OffEventId = offEventDb.Id;
                var positions = document.Waits.SelectMany(x => x.OffEvent.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id, positions);
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void VerifySigningDocument(IContext context, int documentId, bool isUseInternalSign, bool isUseCertificateSign, string serverMapPath)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                CommonQueries.GetDocumentHash(context, documentId, isUseInternalSign, isUseCertificateSign, null, serverMapPath, false, true);
                transaction.Complete();
            }
        }

        public void SelfAffixSigningDocument(IContext context, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign, string serverMapPath)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var eventDb = ModelConverter.GetDbDocumentEvent(document.Events.First());

                var subscription = document.Subscriptions.First();

                var docHash = CommonQueries.GetDocumentHash(context, document.Id,
                                                            isUseInternalSign, isUseCertificateSign, subscription, serverMapPath,
                                                             subscription.SubscriptionStates == EnumSubscriptionStates.Sign ||
                                                             subscription.SubscriptionStates == EnumSubscriptionStates.Visa ||
                                                             subscription.SubscriptionStates == EnumSubscriptionStates.Аgreement ||
                                                             subscription.SubscriptionStates == EnumSubscriptionStates.Аpproval,
                                                             true);

                var subscriptionDb = ModelConverter.GetDbDocumentSubscription(subscription);

                subscriptionDb.Hash = docHash.Hash;
                subscriptionDb.FullHash = docHash.FullHash;
                subscriptionDb.InternalSign = docHash.InternalSign;
                subscriptionDb.CertificateSign = docHash.CertificateSign;
                subscriptionDb.CertificateId = subscription.CertificateId;
                subscriptionDb.CertificateSignCreateDate = DateTime.UtcNow;
                subscriptionDb.CertificatePositionId = subscription.CertificatePositionId;
                subscriptionDb.CertificatePositionExecutorAgentId = subscription.CertificatePositionExecutorAgentId;
                subscriptionDb.CertificatePositionExecutorTypeId = subscription.CertificatePositionExecutorTypeId;

                dbContext.DocumentEventsSet.Add(eventDb);
                dbContext.SaveChanges();

                subscriptionDb.SendEventId = eventDb.Id;
                subscriptionDb.DoneEventId = eventDb.Id;

                dbContext.DocumentSubscriptionsSet.Add(subscriptionDb);
                dbContext.SaveChanges();
                var positions = document.Events.SelectMany(x => x.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id, positions);
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);

                transaction.Complete();

            }
        }

        public InternalDocument ControlChangeDocumentPrepare(IContext context, int eventId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var maxDateTime = DateTime.UtcNow.AddYears(50);

                var doc = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { OnEventId = new List<int> { eventId } })
                    .Where(x => x.OnEvent.EventTypeId != (int)EnumEventTypes.AskPostponeDueDate)
                    .Concat(CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait {Id = dbContext.DocumentWaitsSet //Without security restrictions
                                                        .Where(y => y.ParentId.HasValue && y.OnEventId == eventId && y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate)
                                                        .Select(y => y.ParentId.Value).ToList() })     )
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            ParentId = x.ParentId,
                                            OnEventId = x.OnEventId,
                                            OffEventId = x.OffEventId,
                                            DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                                            AttentionDate = x.AttentionDate,
                                            IsHasAskPostponeDueDate = x.ChildWaits.Any(y=>y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate && !y.OffEventId.HasValue),
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                                DocumentId = x.OnEvent.DocumentId,
                                                SourcePositionId = x.OnEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                                ControllerPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Controller)
                                                                    .OrderByDescending(y=> y.AccessTypeId).FirstOrDefault().PositionId,
                                                TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                    .OrderByDescending(y=> y.AccessTypeId).FirstOrDefault().PositionId,
                                                TaskId = x.OnEvent.TaskId,
                                                Description = x.OnEvent.Description,
                                                AddDescription = x.OnEvent.AddDescription,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                                CreateDate = x.OnEvent.CreateDate,
                                                Date = x.OnEvent.Date,
                                                LastChangeUserId = x.OnEvent.LastChangeUserId,
                                                LastChangeDate = x.OnEvent.LastChangeDate,
                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument ControlTargetChangeDocumentPrepare(IContext context, int eventId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { OnEventId = new List<int> { eventId } })
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            OnEventId = x.OnEventId,
                                            OffEventId = x.OffEventId,
                                            TargetDescription = x.TargetDescription,
                                            AttentionDate = x.AttentionDate,
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                                TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                                TaskId = x.OnEvent.TaskId,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument ControlOffDocumentPrepare(IContext context, int eventId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { OnEventId = new List<int> { eventId } })
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsLaunchPlan = x.Document.IsLaunchPlan,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            ParentId = x.ParentId,
                                            ParentOnEventId = x.ParentWait.OnEventId,
                                            OffEventId = x.OffEventId,
                                            OnEventId = x.OnEventId,
                                            IsHasMarkExecution = x.ChildWaits.Any(y=>y.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution && !y.OffEventId.HasValue),
                                            IsHasAskPostponeDueDate = x.ChildWaits.Any(y=>y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate && !y.OffEventId.HasValue),
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                                DocumentId = x.OnEvent.DocumentId,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                                SourcePositionId = x.OnEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                                ControllerPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Controller)
                                                                    .OrderByDescending(y=> y.AccessTypeId).FirstOrDefault().PositionId,
                                                TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                    .OrderByDescending(y=> y.AccessTypeId).FirstOrDefault().PositionId,
                                                TaskId = x.OnEvent.TaskId,
                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument SelfAffixSigningDocumentPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true });
                var doc = qry.Select(x => new InternalDocument
                            {
                                Id = x.Id,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                ExecutorPositionId = x.ExecutorPositionId
                            }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }
        public InternalDocument ModifyDocumentTagsPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true });
                var doc = qry.Select(x => new InternalDocument
                            {
                                Id = x.Id,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                            }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }
        public void SetSendListForControlOffPrepare(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var filterContains = PredicateBuilder.New<DocumentSendLists>(false);
                filterContains = document.Waits.Select(x => x.OnEventId).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.StartEventId == value && !e.CloseEventId.HasValue).Expand());

                document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                    }
                    ).ToList();
                transaction.Complete();
            }
        }


        public void ControlOffAskPostponeDueDateWaitPrepare(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                 var waitRes = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { IsOpened = true, ParentId = document.Waits.Select(x => x.Id).ToList() })
                    .Where(x => x.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate)
                    .Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ParentId = x.ParentId,
                        DocumentId = x.OnEvent.DocumentId,
                        OnEvent = new InternalDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.OnEvent.DocumentId,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                            SourcePositionId = x.OnEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                            ControllerPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Controller)
                                                                    .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                    .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            TaskId = x.OnEvent.TaskId,
                        }
                    }
                    ).ToList();
                ((List<InternalDocumentWait>)document.Waits).AddRange(waitRes);
                transaction.Complete();
            }
        }
        public void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var waitRes = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { IsOpened = true, ParentId = document.Waits.Select(x => x.Id).ToList() })
                    .Where(x => x.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution)
                    .Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ParentId = x.ParentId,
                        DocumentId = x.OnEvent.DocumentId,
                        OnEvent = new InternalDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.OnEvent.DocumentId,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                            SourcePositionId = x.OnEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                            ControllerPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Controller)
                                                                    .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            TargetPositionId = x.OnEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                    .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            TaskId = x.OnEvent.TaskId,
                        }
                    }
                    ).ToList();
                ((List<InternalDocumentWait>)document.Waits).AddRange(waitRes);
                transaction.Complete();
            }
        }

        public void SetSubscriptionForControlOffPrepare(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                filterContains = document.Waits.Select(x => x.OnEventId).ToList().Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SendEventId == value).Expand());

                document.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => !x.DoneEventId.HasValue)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentSubscription
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }
                    ).ToList();
                transaction.Complete();
            }
        }

        public void AddDocumentEvents(IContext context, InternalDocument document)
        {
            List<int> res = null;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                {
                    var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First(x => x.Id == 0));
                    dbContext.DocumentTasksSet.Add(taskDb);
                    dbContext.SaveChanges();
                    ((List<InternalDocumentEvent>)document.Events).ForEach(x => x.TaskId = taskDb.Id);
                }
                if (document.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();
                }
                if (document.Events?.Any() ?? false)
                {
                    var eventsDb = ModelConverter.GetDbDocumentEvents(document.Events).ToList();
                    dbContext.DocumentEventsSet.AddRange(eventsDb);
                    dbContext.SaveChanges();
                    var events = document.Events.ToList();
                    for (var i = 0; i < eventsDb.Count(); i++) events[i].Id = eventsDb[i].Id;
                    var positions = document.Events.SelectMany(x => x.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                    CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id, positions);
                }
                //CommonQueries.ModifyDocumentTaskAccesses(dbContext, context, document.Id);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }


        public IEnumerable<InternalDocumentEventAccess> MarkDocumentEventsAsReadPrepare(IContext context, MarkDocumentEventAsRead model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (model.EventIds == null)
                    return new List<InternalDocumentEventAccess>();

                var filterEventContains = PredicateBuilder.New<DocumentEventAccesses>(false);
                filterEventContains = model.EventIds.Aggregate(filterEventContains,
                    (current, value) => current.Or(e => e.EventId == value).Expand());
                var qry = dbContext.DocumentEventAccessesSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => !x.ReadDate.HasValue).Where(filterEventContains);

                if (!context.IsAdmin)
                {
                    var filterPositionContains = PredicateBuilder.New<DocumentEventAccesses>(false);
                    filterPositionContains = context.CurrentPositionsAccessLevel.Aggregate(filterPositionContains,
                        (current, value) => current.Or(e => e.PositionId == value.Key && e.Document.Accesses.Any(x => x.PositionId == value.Key && x.AccessLevelId >= value.Value)).Expand());

                    qry = qry.Where(filterPositionContains);
                }

                var res = qry.Select(x => new InternalDocumentEventAccess
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public void MarkDocumentEventAsRead(IContext context, IEnumerable<InternalDocumentEventAccess> eventAccList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var bdev in ModelConverter.GetDbDocumentEventAccesses(eventAccList))
                {
                    dbContext.SafeAttach(bdev);
                    var entry = dbContext.Entry(bdev);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.ReadAgentId).IsModified = true;
                    entry.Property(x => x.ReadDate).IsModified = true;
                }
                dbContext.SaveChanges();
                //eventAccList.ToList().ForEach(x => CommonQueries.ModifyDocumentAccessesStatistics(context, x.DocumentId, CommonQueries.GetEventsSourceTarget(x)));                             //TODO ver TargetPositionId remove
                eventAccList.Where(x => x.PositionId.HasValue).ToList().ForEach(x => CommonQueries.ModifyDocumentAccessesStatistics(context, x.DocumentId, x.PositionId.Value)); //TODO ver TargetPositionId remove
                transaction.Complete();
            }
        }

        public void ModifyDocumentAccessesStatistics(IContext context, int? documentId = null, List<int> positionId = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                CommonQueries.ModifyDocumentAccessesStatistics(context, documentId, positionId);
                transaction.Complete();
            }

        }

        //public void MarkDocumentEventAsReadAuto(IContext context)
        //{
        //    var dbContext = context.DbContext as DmsContext;
        //    using (var transaction = Transactions.GetTransaction())
        //    {
        //        dbContext.DocumentEventsSet.Where(x => !x.ReadDate.HasValue && (!x.TargetPositionId.HasValue || x.TargetPositionId == x.SourcePositionId))
        //            .Update(x => new DocumentEvents { ReadDate = x.CreateDate, ReadAgentId = x.SourceAgentId });
        //        //dbContext.SaveChanges();
        //        transaction.Complete();
        //    }
        //}
        public void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var acc = ModelConverter.GetDbDocumentAccess(docAccess);
                dbContext.SafeAttach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsFavourite).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsFavourite = x.IsFavourite,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            IsFavourite = x.IsFavourite,
                                            PositionId = x.PositionId,
                                            IsInWork = x.IsInWork,
                                        }
                                    }

                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public void ChangeIsInWorkAccess(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var docAccess = document.Accesses.FirstOrDefault();
                if (docAccess == null)
                {
                    throw new WrongParameterValueError();
                }
                var acc = ModelConverter.GetDbDocumentAccess(docAccess);
                dbContext.SafeAttach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsInWork).IsModified = true;
                dbContext.DocumentEventsSet.Add(ModelConverter.GetDbDocumentEvent(document.Events.FirstOrDefault()));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalDocument ChangeIsInWorkAccessPrepare(IContext context, ChangeWorkStatus Model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var acc = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == Model.DocumentId && x.PositionId == Model.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            IsInWork = x.IsInWork,
                                            CountWaits = x.CountWaits,
                                            PositionId = x.PositionId,
                                        }
                                    }

                    }).FirstOrDefault();
                transaction.Complete();
                return acc;

            }
        }

        public void SendBySendList(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            List<int> recalcPositions = new List<int>();
            using (var transaction = Transactions.GetTransaction())
            {
                var sendList = document.SendLists.First();
                var sendListDb = new DocumentSendLists
                {
                    Id = sendList.Id,
                    LastChangeDate = sendList.LastChangeDate,
                    LastChangeUserId = sendList.LastChangeUserId
                };
                var startEventDb = ModelConverter.GetDbDocumentEvent(sendList.StartEvent);
                recalcPositions = recalcPositions.Concat(sendList.StartEvent.Accesses.Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value)).ToList();
                if (sendList.Stage.HasValue)
                {
                    dbContext.SafeAttach(sendListDb);
                    sendListDb.StartEvent = startEventDb;
                    if (sendList.CloseEvent != null)
                    {
                        sendListDb.CloseEvent = sendListDb.StartEvent;
                    }

                    var entry = dbContext.Entry(sendListDb);
                    //entry.Property(x => x.Id).IsModified = true;
                    entry.Property(e => e.AddDescription).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.DocumentEventsSet.Add(startEventDb);
                    dbContext.SaveChanges();
                    sendListDb.StartEventId = startEventDb.Id;
                }
                if (document.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();
                }

                if (document.Waits?.Any() ?? false)
                {
                    foreach (var wait in document.Waits)
                    {
                        var waitDb = ModelConverter.GetDbDocumentWait(wait);
                        if (wait.OnEvent == sendList.StartEvent)
                        {
                            waitDb.OnEventId = sendListDb.StartEventId.Value;
                            waitDb.OnEvent = null;
                        }
                        else
                        {
                            if (waitDb.OnEvent != null)
                            {
                                waitDb.OnEvent.ParentEventId = sendListDb.StartEventId.Value;
                            }
                        }
                        dbContext.DocumentWaitsSet.Add(waitDb);
                        dbContext.SaveChanges();
                        recalcPositions = recalcPositions.Concat(document.Waits.SelectMany(x => x.OnEvent.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value)).ToList();
                    }
                }

                if (document.Subscriptions?.Any() ?? false)
                {
                    var subscription = document.Subscriptions.First();
                    var subscriptionDb = ModelConverter.GetDbDocumentSubscription(subscription);
                    if (subscription.SendEvent == sendList.StartEvent)
                    {
                        subscriptionDb.SendEventId = sendListDb.StartEventId.Value;
                        subscriptionDb.SendEvent = null;
                    }
                    dbContext.DocumentSubscriptionsSet.Add(subscriptionDb);
                    dbContext.SaveChanges();
                }

                if (document.Events?.Any() ?? false)
                {
                    var eventDb = ModelConverter.GetDbDocumentEvent(document.Events.First());
                    eventDb.ParentEventId = sendListDb.StartEventId.Value;

                    dbContext.DocumentEventsSet.Add(eventDb);
                    dbContext.SaveChanges();
                    recalcPositions = recalcPositions.Concat(document.Events.SelectMany(x => x.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value)).ToList();
                }

                if (document.Papers?.Any() ?? false)
                {
                    foreach (var paper in document.Papers.ToList())
                    {
                        var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                        paperEventDb.ParentEventId = sendListDb.StartEventId.Value;
                        dbContext.SafeAttach(paperEventDb);
                        var entryEventDb = dbContext.Entry(paperEventDb);
                        //entryEventDb.Property(e => e.SourcePositionExecutorAgentId).IsModified = true;
                        //entryEventDb.Property(e => e.SourcePositionExecutorTypeId).IsModified = true;
                        //entryEventDb.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                        //entryEventDb.Property(e => e.TargetPositionExecutorTypeId).IsModified = true;
                        //entryEventDb.Property(e => e.SourceAgentId).IsModified = true;
                        //entryEventDb.Property(e => e.TargetAgentId).IsModified = true;
                        entryEventDb.Property(e => e.ParentEventId).IsModified = true;
                        entryEventDb.Property(e => e.PaperPlanDate).IsModified = true;
                        entryEventDb.Property(e => e.PaperPlanAgentId).IsModified = true;
                        entryEventDb.Property(e => e.LastChangeUserId).IsModified = true;
                        entryEventDb.Property(e => e.LastChangeDate).IsModified = true;
                        dbContext.SaveChanges();
                        paper.LastPaperEvent = null;
                        var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                        paperDb.LastPaperEventId = paperEventDb.Id;
                        dbContext.SafeAttach(paperDb);
                        var entryPaper = dbContext.Entry(paperDb);
                        entryPaper.Property(e => e.LastPaperEventId).IsModified = true;
                        entryPaper.Property(e => e.LastChangeUserId).IsModified = true;
                        entryPaper.Property(e => e.LastChangeDate).IsModified = true;
                        dbContext.SaveChanges();
                    }

                }

                //var positions = document.Events.SelectMany(x => x.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                //positions = positions.Concat(document.Waits.SelectMany(x => x.OnEvent.Accesses).Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value)).ToList();

                CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id, recalcPositions);
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void ModifyDocumentTags(IContext context, InternalDocumentTag model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qryDictionaryTags = dbContext.DictionaryTagsSet.Where(x => x.ClientId == context.Client.Id).AsQueryable();
                {
                    var filterContains = PredicateBuilder.New<DBModel.Dictionary.DictionaryTags>(false);
                    filterContains = model.Tags.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qryDictionaryTags = qryDictionaryTags.Where(filterContains);
                }

                if (!context.IsAdmin)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Dictionary.DictionaryTags>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value || !e.PositionId.HasValue).Expand());

                    qryDictionaryTags = qryDictionaryTags.Where(filterContains);
                }

                var dictionaryTags = qryDictionaryTags
                    .Select(x => x.Id)
                    .ToList();

                var qryDocumentTags = dbContext.DocumentTagsSet.Where(x => x.ClientId == context.Client.Id)
                                        .Where(x => x.DocumentId == model.DocumentId).AsQueryable();

                if (!context.IsAdmin)
                {
                    var filterContains = PredicateBuilder.New<DocumentTags>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Tag.PositionId == value || !e.Tag.PositionId.HasValue).Expand());

                    qryDocumentTags = qryDocumentTags.Where(filterContains);
                }

                var documentTags = qryDocumentTags
                    .Select(x => x.TagId)
                    .ToList();

                {
                    var filterContains = PredicateBuilder.New<DocumentTags>(false);
                    filterContains = documentTags.Where(y => !dictionaryTags.Contains(y)).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TagId == value).Expand());

                    //Удаляем теги которые не присутствуют в списке
                    var rngToDelete = dbContext.DocumentTagsSet.Where(
                        x => x.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == model.DocumentId)
                        .Where(filterContains);
                    CommonQueries.AddFullTextCacheInfo(context, rngToDelete.Select(x => x.Id).ToList(), EnumObjects.DocumentTags, EnumOperationType.Delete);
                    dbContext.DocumentTagsSet.RemoveRange(rngToDelete);
                }

                var newDictionaryTags = dictionaryTags
                    .Where(x => !documentTags.Contains(x))
                    .Select(x => new DocumentTags
                    {
                        ClientId = model.ClientId,
                        EntityTypeId = model.EntityTypeId,
                        DocumentId = model.DocumentId,
                        TagId = x,
                        LastChangeUserId = model.LastChangeUserId,
                        LastChangeDate = model.LastChangeDate
                    }).ToList();

                dbContext.DocumentTagsSet.AddRange(newDictionaryTags);

                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, newDictionaryTags.Where(x => x.Id != 0).Select(x => x.Id).ToList(), EnumObjects.DocumentTags, EnumOperationType.AddNew);

                transaction.Complete();
            }
        }

        public InternalDocument AddNoteDocumentPrepare(IContext context, AddNote model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        AccessesCount = x.Accesses.Count()
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => !string.IsNullOrEmpty(model.Task) && x.DocumentId == model.DocumentId && x.Task == model.Task)
                    .Select(x => new List<InternalDocumentTask>
                    {
                                        new InternalDocumentTask
                                        {
                                                Id = x.Id,
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                        }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { sendList.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsLaunchPlan = x.IsLaunchPlan,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (sendList.IsInitial)
                {
                    CommonQueries.SetSuccessfulSubscriptions(context, doc);
                }
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument SendForInformationDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { sendList.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsLaunchPlan = x.IsLaunchPlan,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (sendList.IsInitial)
                {
                    CommonQueries.SetSuccessfulSubscriptions(context, doc);
                }
                transaction.Complete();
                return doc;
            }
        }

        public void SetRestrictedSendListsPrepare(IContext context, InternalDocument document)
        {
            if (document == null) return;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                document.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == document.Id)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        PositionId = x.PositionId,
                        DocumentId = x.DocumentId,
                        AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                    }).ToList();
                document.IsRestrictedSendListsLoaded = true;
                transaction.Complete();
            }
        }
        public void SetParentEventAccessesPrepare(IContext context, InternalDocument document, int? eventId)
        {
            if (document == null || !eventId.HasValue) return;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                document.ParentEventAccesses = dbContext.DocumentEventAccessesSet
                    .Where(x => x.ClientId == context.Client.Id).Where(x => x.EventId == eventId)
                    .Select(x => new InternalDocumentEventAccess
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        EventId = x.EventId,
                        PositionId = x.PositionId,
                    }).ToList();
                transaction.Complete();
            }
        }
        #endregion DocumentMainLogic 

        #region DocumentLink    
        public InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        //ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                        LinkTypeId = model.LinkTypeId,
                    }).FirstOrDefault();

                if (doc == null) return null;

                var par = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.ParentDocumentId } })
                    .Select(x => new { x.Id, x.LinkId }).FirstOrDefault();

                if (par == null) return null;

                doc.ParentDocumentId = par.Id;
                doc.ParentDocumentLinkId = par.LinkId;
                transaction.Complete();
                return doc;
            }
        }
        public InternalDocument DeleteDocumentLinkPrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentLinksSet
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Document.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        LinkId = x.Document.LinkId,
                        Links = new List<InternalDocumentLink> { new InternalDocumentLink
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            ParentDocumentId = x.ParentDocumentId,
                            ExecutorPositionId = x.ExecutorPositionId,
                        }}
                    }).FirstOrDefault();

                if (doc?.LinkId == null) return null;
                doc.OldLinks = dbContext.DocumentLinksSet
                    .Where(x => x.Document.LinkId == doc.LinkId.Value)
                    .Select(x => new InternalDocumentLink
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        ParentDocumentId = x.ParentDocumentId,
                    }).ToList();
                //var calc = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.Client.Id)
                //    .Where(x => x.LinkId == doc.LinkId && x.Id != doc.Id).GroupBy(x => true)
                //    .Select(x => new { Count = x.Count(), MinId = x.Min(y => y.Id) }).First();
                //doc.LinkedDocumentsCount = calc.Count;
                //doc.NewLinkId = calc.MinId;
                transaction.Complete();
                return doc;
            }
        }
        public void AddDocumentLink(IContext context, InternalDocument model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (model.Events != null && model.Events.Any(x => x.Id == 0))
                {
                    var eventsDb = ModelConverter.GetDbDocumentEvents(model.Events.Where(x => x.Id == 0).ToList());
                    dbContext.DocumentEventsSet.AddRange(eventsDb);
                }
                var link = new DocumentLinks
                {
                    DocumentId = model.Id,
                    EntityTypeId = model.EntityTypeId,
                    ClientId = model.ClientId,
                    ParentDocumentId = model.ParentDocumentId,
                    LinkTypeId = model.LinkTypeId,
                    ExecutorPositionId = model.ExecutorPositionId,
                    ExecutorPositionExecutorAgentId = model.ExecutorPositionExecutorAgentId,
                    ExecutorPositionExecutorTypeId = model.ExecutorPositionExecutorTypeId,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate,
                };
                dbContext.DocumentLinksSet.Add(link);
                if (!model.ParentDocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                        .Where(x => x.Id == model.ParentDocumentId).Update(x => new DBModel.Document.Documents { LinkId = model.ParentDocumentId, });
                }
                if (!model.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                        .Where(x => x.Id == model.Id).Update(x => new DBModel.Document.Documents { LinkId = model.ParentDocumentLinkId ?? model.ParentDocumentId, });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                        .Where(x => x.LinkId == model.LinkId).Update(x => new DBModel.Document.Documents { LinkId = model.ParentDocumentLinkId ?? model.ParentDocumentId, });
                }
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteDocumentLink(IContext context, InternalDocument model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var id = model.Links.Select(y => y.Id).First();
                dbContext.DocumentLinksSet.RemoveRange(dbContext.DocumentLinksSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id));

                if ((model.OldLinkSet?.Any() ?? false) && model.LinkId != model.OldLinkId)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = model.OldLinkSet.Aggregate(filterContains, (current, value) => current.Or(e => e.Id == value).Expand());
                    dbContext.DocumentsSet.Where(filterContains) //Without security restrictions
                        .Update(u => new DBModel.Document.Documents { LinkId = model.OldLinkId });
                }
                if ((model.NewLinkSet?.Any() ?? false) && model.LinkId != model.NewLinkId)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = model.NewLinkSet.Aggregate(filterContains, (current, value) => current.Or(e => e.Id == value).Expand());
                    dbContext.DocumentsSet.Where(filterContains) //Without security restrictions
                        .Update(u => new DBModel.Document.Documents { LinkId = model.NewLinkId });
                }
                if (model.Events != null && model.Events.Any(x => x.Id == 0))
                {
                    var eventsDb = ModelConverter.GetDbDocumentEvents(model.Events.Where(x => x.Id == 0).ToList());
                    dbContext.DocumentEventsSet.AddRange(eventsDb);
                }
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        #endregion DocumentLink     

        #region DocumentSendList    
        public InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId, string task = null, int id = 0)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        TemplateId = x.TemplateDocument.Id,
                        IsHard = x.TemplateDocument.IsHard,
                        IsLaunchPlan = x.IsLaunchPlan
                    }).FirstOrDefault();

                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => !string.IsNullOrEmpty(task) && x.DocumentId == documentId && x.Task == task)
                        .Select(x => new InternalDocumentTask
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                        }).ToList();
                SetRestrictedSendListsPrepare(context, doc);
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == doc.Id)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        //TargetPositionId = x.TargetPositionId,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        StageType = (EnumStageTypes?)x.StageTypeId,
                        Stage = x.Stage,
                        SourcePositionId = x.AccessGroups.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                        //SourceAgentId = x.AccessGroups.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).AgentId ?? 0,
                        //TargetAgentId = x.TargetAgentId

                    }).ToList();
                CommonQueries.SetSuccessfulSubscriptions(context, doc);
                if (doc.IsHard)
                {
                    doc.Template = new InternalTemplate();

                    doc.Template.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == doc.TemplateId)
                        .Select(x => new InternalTemplateRestrictedSendList
                        {
                            Id = x.Id,
                            PositionId = x.PositionId
                        }).ToList();

                    //doc.TemplateDocument.SendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id)
                    //    .Where(x => x.DocumentId == doc.TemplateDocumentId)
                    //    .Select(x => new InternalTemplateDocumentSendList
                    //    {
                    //        Id = x.Id,
                    //        TargetPositionId = x.TargetPositionId,
                    //        SendType = (EnumSendTypes)x.SendTypeId
                    //    }).ToList();
                }

                if (id != 0)
                {
                    doc.PaperEvents = CommonQueries.GetDocumentEventQuery(context, null)
                        .Where(x => x.SendListId == id)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PaperId = x.PaperId,
                            SourcePositionId = x.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                            TargetPositionId = x.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                            Description = x.Description,
                        }).ToList();
                }
                transaction.Complete();
                return doc;
            }
        }
        public IEnumerable<int> AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model)
        {
            List<int> res;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var items = ModelConverter.GetDbDocumentRestrictedSendLists(model).ToList();
                dbContext.DocumentRestrictedSendListsSet.AddRange(items);
                dbContext.SaveChanges();
                res = items.Select(x => x.Id).ToList();
                transaction.Complete();
            }
            return res;
        }
        public IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {

            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.Client.Id).Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     //TODO DELETE Method?
                     //ClientId = x.ClientId,
                     //EntityTypeId = x.EntityTypeId,
                     DocumentId = model.DocumentId,
                     PositionId = x.TargetPositionId,
                     AccessLevel = (EnumAccessLevels)(x.AccessLevelId ?? (int)EnumAccessLevels.PersonallyAndIOAndReferents)
                 }).ToList();
                transaction.Complete();
                return items;
            }
        }
        public InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == restSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     Id = x.Id,
                     ClientId = x.ClientId,
                     EntityTypeId = x.EntityTypeId,
                     DocumentId = x.DocumentId
                 }).FirstOrDefault();
                transaction.Complete();
                return item;
            }
        }
        public void DeleteDocumentRestrictedSendList(IContext context, int restSendListId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == restSendListId);
                if (item != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }
        public IEnumerable<int> AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> sendList, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> paperEvents = null)
        {
            List<int> res = null;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (task?.Any(x => x.Id == 0) ?? false)
                {
                    var taskDb = ModelConverter.GetDbDocumentTask(task.First(x => x.Id == 0));
                    dbContext.DocumentTasksSet.Add(taskDb);
                    dbContext.SaveChanges();
                    ((List<InternalDocumentSendList>)sendList).ForEach(x => x.TaskId = taskDb.Id);
                }

                if (sendList?.Any(x => x.Stage.HasValue) ?? false)
                {
                    var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendList.Where((x => x.Stage.HasValue)), true).ToList();
                    dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                    dbContext.SaveChanges();
                    res = sendListsDb.Select(x => x.Id).ToList();
                    CommonQueries.AddFullTextCacheInfo(context, res, EnumObjects.DocumentSendLists, EnumOperationType.AddNew);
                }
                if (paperEvents?.Any() ?? false)
                {
                    var listPaperEvent = paperEvents.ToList();
                    listPaperEvent.ForEach(x => { x.SendListId = res.FirstOrDefault(); });
                    var paperEventsDb = ModelConverter.GetDbDocumentEvents(listPaperEvent).ToList();
                    dbContext.DocumentEventsSet.AddRange(paperEventsDb);
                    dbContext.SaveChanges();
                    CommonQueries.AddFullTextCacheInfo(context, paperEventsDb.Select(x => x.Id).ToList(), EnumObjects.DocumentEvents, EnumOperationType.Update);

                }

                transaction.Complete();

            }
            return res;
        }
        public void ModifyDocumentSendListAddDescription(IContext context, InternalDocumentSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var sendListDb = ModelConverter.GetDbDocumentSendList(sendList, true);
                dbContext.SafeAttach(sendListDb);
                var entry = dbContext.Entry(sendListDb);
                entry.Property(e => e.AddDescription).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void ModifyDocumentSendList(IContext context, InternalDocumentSendList sendList, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> addPaperEvents = null, IEnumerable<int?> delPaperEvents = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (task?.Any(x => x.Id == 0) ?? false)
                {
                    var taskDb = ModelConverter.GetDbDocumentTask(task.First(x => x.Id == 0));
                    dbContext.DocumentTasksSet.Add(taskDb);
                    dbContext.SaveChanges();
                    sendList.TaskId = taskDb.Id;
                }
                //TODO MayBe divide delete and add?
                dbContext.DocumentSendListAccessGroupsSet.RemoveRange(
                    dbContext.DocumentSendListAccessGroupsSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.SendListId == sendList.Id));
                dbContext.SaveChanges();
                var sendListDb = ModelConverter.GetDbDocumentSendList(sendList, false);
                dbContext.SafeAttach(sendListDb);
                var entry = dbContext.Entry(sendListDb);
                entry.Property(e => e.Stage).IsModified = true;
                entry.Property(e => e.StageTypeId).IsModified = true;
                entry.Property(e => e.SendTypeId).IsModified = true;
                entry.Property(e => e.TaskId).IsModified = true;
                entry.Property(e => e.IsAddControl).IsModified = true;
                entry.Property(e => e.SelfDescription).IsModified = true;
                entry.Property(e => e.SelfDueDate).IsModified = true;
                entry.Property(e => e.SelfDueDay).IsModified = true;
                entry.Property(e => e.SelfAttentionDate).IsModified = true;
                entry.Property(e => e.SelfAttentionDay).IsModified = true;
                entry.Property(e => e.IsInitial).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.AddDescription).IsModified = true;
                entry.Property(e => e.DueDate).IsModified = true;
                entry.Property(e => e.DueDay).IsModified = true;
                entry.Property(e => e.AccessLevelId).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                if (sendList.AccessGroups?.Any() ?? false)
                {
                    var accessesDb = ModelConverter.GetDbDocumentSendListAccessGroups(sendList.AccessGroups);
                    dbContext.DocumentSendListAccessGroupsSet.AddRange(accessesDb);
                    dbContext.SaveChanges();
                }

                if (delPaperEvents?.Any() ?? false)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = delPaperEvents.Aggregate(filterContains, (current, value) => current.Or(e => e.PaperId == value).Expand());
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(filterContains).Where(x => x.SendListId == sendList.Id));
                    dbContext.SaveChanges();
                }
                if (addPaperEvents?.Any() ?? false)
                {
                    var paperEventsDb = ModelConverter.GetDbDocumentEvents(addPaperEvents).ToList();
                    dbContext.DocumentEventsSet.AddRange(paperEventsDb);
                    dbContext.SaveChanges();
                }

                CommonQueries.AddFullTextCacheInfo(context, sendList.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }
        public InternalDocument DeleteDocumentSendListPrepare(IContext context, int sendListId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
                            .Where(x => x.Id == sendListId)
                            .Select(x => new InternalDocument
                            {
                                Id = x.DocumentId,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                SendLists = new List<InternalDocumentSendList>
                                {
                                        new InternalDocumentSendList
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            SourcePositionId = x.AccessGroups.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                }
                            }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }
        public void DeleteDocumentSendList(IContext context, InternalDocumentSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == sendList.Id);
                if (item != null)
                {
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.SendListId == sendList.Id && x.PaperPlanDate == null));
                    dbContext.DocumentSendListAccessGroupsSet.RemoveRange(dbContext.DocumentSendListAccessGroupsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.SendListId == sendList.Id));
                    dbContext.DocumentSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
                CommonQueries.AddFullTextCacheInfo(context, sendList.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }
        public InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == doc.Id)
                    .Select(y => new InternalDocumentSendList
                    {
                        Id = y.Id,
                        ClientId = y.ClientId,
                        EntityTypeId = y.EntityTypeId,
                        Stage = y.Stage
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }
        public void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var sl in model)
                {
                    var item = new DocumentSendLists
                    {
                        Id = sl.Id,
                        Stage = sl.Stage.Value,
                        LastChangeUserId = sl.LastChangeUserId,
                        LastChangeDate = sl.LastChangeDate
                    };
                    dbContext.SafeAttach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(e => e.Stage).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                }

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public InternalDocument LaunchDocumentSendListItemPrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        SendLists = new List<InternalDocumentSendList>
                                    {
                                        new InternalDocumentSendList
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            Stage = x.Stage,
                                            StageType = (EnumStageTypes?)x.StageTypeId,
                                            SendType = (EnumSendTypes)x.SendTypeId,
                                            TaskId = x.TaskId,
                                            IsAddControl = x.IsAddControl,
                                            SelfDescription = x.SelfDescription,
                                            SelfDueDate = x.SelfDueDate,
                                            SelfDueDay = x.SelfDueDay,
                                            SelfAttentionDate = x.SelfAttentionDate,
                                            SelfAttentionDay = x.SelfAttentionDay,
                                            IsInitial = x.IsInitial,
                                            Description = x.Description,
                                            DueDay = x.DueDay,
                                            DueDate = x.DueDate,
                                            AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                    }
                    }).FirstOrDefault();
                if (doc?.SendLists?.Any() ?? false)
                {
                    CommonQueries.SetAccessGroups(context, doc.SendLists.ToList());
                }
                transaction.Complete();
                return doc;

            }
        }
        #endregion DocumentSendList     

        #region DocumentSavedFilter

        public List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var items = model.Select(x => new DocumentSavedFilters
                {
                    ClientId = context.Client.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                dbContext.DocumentSavedFiltersSet.AddRange(items);
                dbContext.SaveChanges();
                transaction.Complete();
                return items.Select(x => x.Id).ToList();
            }
        }

        public void ModifySavedFilter(IContext context, InternalDocumentSavedFilter model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var item = new DocumentSavedFilters
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    Name = model.Name,
                    Icon = model.Icon,
                    Filter = model.Filter,
                    IsCommon = model.IsCommon,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate
                };
                dbContext.SafeAttach(item);

                var entry = dbContext.Entry(item);
                entry.Property(e => e.UserId).IsModified = true;
                entry.Property(e => e.Name).IsModified = true;
                entry.Property(e => e.Icon).IsModified = true;
                entry.Property(e => e.Filter).IsModified = true;
                entry.Property(e => e.IsCommon).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteSavedFilter(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }

        #endregion DocumentSavedFilter

        #region DocumentTasks
        public IEnumerable<int> AddDocumentTasks(IContext context, InternalDocument document)
        {
            List<int> res = null;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var tasksDb = document.Tasks.Select(ModelConverter.GetDbDocumentTask).ToList();
                dbContext.DocumentTasksSet.AddRange(tasksDb);
                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
                transaction.Complete();
                res = tasksDb.Select(x => x.Id).ToList();
            }
            return res;
        }
        public InternalDocument DeleteDocumentTaskPrepare(IContext context, int taskId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == taskId)
                        .Select(x => new InternalDocument
                        {
                            Id = x.Document.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            Tasks = new List<InternalDocumentTask>
                                    {
                                                        new InternalDocumentTask
                                                        {
                                                            Id = x.Id,
                                                            ClientId = x.ClientId,
                                                            EntityTypeId = x.EntityTypeId,
                                                            PositionId = x.PositionId,
                                                            CountEvents = x.Events.Count(y=>y.EventTypeId!=(int)EnumEventTypes.TaskFormulation),
                                                            CountSendLists = x.SendLists.Count,
                                                        }
                                    }
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }
        public void ModifyDocumentTask(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First());
                dbContext.SafeAttach(taskDb);
                var entry = dbContext.Entry(taskDb);
                entry.Property(e => e.Task).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteDocumentTask(IContext context, int itemId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == itemId);
                if (item != null)
                {
                    dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == itemId));
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == itemId && x.EventTypeId != (int)EnumEventTypes.TaskFormulation));
                    dbContext.DocumentTasksSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }
        public InternalDocument ModifyDocumentTaskPrepare(IContext context, int? id, BaseModifyDocumentTask model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id).Where(x => (x.Task == model.Name || x.Id == id) && x.DocumentId == model.DocumentId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        PositionId = x.PositionId,
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }
        #endregion DocumentTasks

        #region DocumentPapers
        public InternalDocument DeleteDocumentPaperPrepare(IContext context, int paperId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == paperId)
                        .Select(x => new InternalDocument
                        {
                            Id = x.Document.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            ExecutorPositionId = x.Document.ExecutorPositionId,
                            Papers = new List<InternalDocumentPaper>
                                    {
                                        new InternalDocumentPaper
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            DocumentId = x.DocumentId,
                                            OrderNumber = x.OrderNumber,
                                            IsInWork = x.IsInWork,
                                            LastPaperEvent = new InternalDocumentEvent
                                            {
                                                ClientId = x.ClientId,
                                                EntityTypeId = x.EntityTypeId,
                                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                                TargetPositionId = x.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                                            }
                                        }
                                    }
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }
        public InternalDocument EventDocumentPaperPrepare(IContext context, PaperList filters, bool isCalcPreLastPaperEvent = false)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Select(x => x);

                if (filters.PaperId != null && filters.PaperId.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentPapers>(false);
                    filterContains = filters.PaperId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.PaperListId.HasValue)
                {
                    qry = qry.Where(x => x.LastPaperEvent.PaperListId == filters.PaperListId);
                }

                var doc = new InternalDocument
                {
                    Papers = qry.Select(x => new InternalDocumentPaper
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsInWork = x.IsInWork,
                        DocumentId = x.DocumentId,
                        LastPaperEvent = !x.LastPaperEventId.HasValue
                                ? null
                                : new InternalDocumentEvent
                                {
                                    Id = x.LastPaperEvent.Id,
                                    ClientId = x.ClientId,
                                    EntityTypeId = x.EntityTypeId,
                                    SourcePositionId = x.LastPaperEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                    TargetPositionId = x.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                    PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                                    PaperSendDate = x.LastPaperEvent.PaperSendDate,
                                    PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                                },
                    }
                        ).ToList()
                };
                if (!doc.Papers.Any()) return null;
                doc.Id = doc.Papers.First().DocumentId;
                if (isCalcPreLastPaperEvent)
                {
                    ((List<InternalDocumentPaper>)doc.Papers).ForEach(x =>
                    {
                        x.PreLastPaperEventId = dbContext.DocumentEventsSet.Where(y => y.ClientId == context.Client.Id)
                                .Where(y => y.PaperId == x.Id && y.Id != x.LastPaperEvent.Id && y.PaperRecieveDate.HasValue &&
                                            y.Accesses.Where(z => z.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(z => z.AccessTypeId).FirstOrDefault().PositionId == x.LastPaperEvent.SourcePositionId)
                                .OrderByDescending(y => y.PaperRecieveDate)
                                .Select(y => y.Id)
                                .FirstOrDefault();
                    });
                }
                transaction.Complete();
                return doc;
            }
        }
        public InternalDocument ModifyDocumentPaperPrepare(IContext context, int? id, BaseModifyDocumentPaper model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();

                if (doc == null) return null;
                if (!id.HasValue)
                {
                    doc.MaxPaperOrderNumber = dbContext.DocumentPapersSet
                        .Where(
                            x =>
                                x.DocumentId == model.DocumentId && x.Name == model.Name && x.IsMain == model.IsMain &&
                                x.IsCopy == model.IsCopy && x.IsOriginal == model.IsOriginal)
                        .OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault();
                }
                else
                {
                    doc.Papers = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Where(x => (x.Id == id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
                        .Select(x => new InternalDocumentPaper
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            IsCopy = x.IsCopy,
                            IsInWork = x.IsInWork,
                            LastPaperEvent = new InternalDocumentEvent
                            {
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                TargetPositionId = x.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                            }
                        }).ToList();
                }
                transaction.Complete();
                return doc;
            }
        }
        public IEnumerable<int> AddDocumentPapers(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            List<int> res = new List<int>();
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (papers != null && papers.Any())
                {
                    foreach (var paper in papers)
                    {
                        var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                        if (paper.Events != null && paper.Events.Any())
                        {
                            paperDb.Events = ModelConverter.GetDbDocumentEvents(paper.Events).ToList();
                        }
                        dbContext.DocumentPapersSet.Add(paperDb);
                        dbContext.SaveChanges();
                        res.Add(paperDb.Id);
                        paperDb.LastPaperEventId = paperDb.Events.First().Id;
                        dbContext.SafeAttach(paperDb);
                        var entry = dbContext.Entry(paperDb);
                        entry.Property(x => x.LastPaperEventId).IsModified = true;
                        dbContext.SaveChanges();
                    }
                    CommonQueries.AddFullTextCacheInfo(context, papers.First().DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                }
                transaction.Complete();
            }
            return res;
        }
        public void ModifyDocumentPaper(IContext context, InternalDocumentPaper item)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbDocumentPaper(item);
                dbContext.SafeAttach(itemDb);
                var entry = dbContext.Entry(itemDb);
                entry.Property(e => e.Name).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.IsMain).IsModified = true;
                entry.Property(e => e.IsOriginal).IsModified = true;
                entry.Property(e => e.IsCopy).IsModified = true;
                entry.Property(e => e.PageQuantity).IsModified = true;
                //entry.Property(e => e.OrderNumber).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, item.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }
        public void DeleteDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var paperDb = new DocumentPapers { Id = paper.Id };
                dbContext.SafeAttach(paperDb);
                var entry = dbContext.Entry(paperDb);
                entry.Property(e => e.LastPaperEventId).IsModified = true;
                dbContext.SaveChanges();
                dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.PaperId == paper.Id && x.EventTypeId == (int)EnumEventTypes.AddNewPaper));
                dbContext.DocumentPapersSet.RemoveRange(dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == paper.Id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, paper.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }
        public void MarkOwnerDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                dbContext.DocumentEventsSet.Add(paperEventDb);
                dbContext.SaveChanges();
                paper.LastPaperEventId = paperEventDb.Id;
                var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                dbContext.SafeAttach(paperDb);
                var entry = dbContext.Entry(paperDb);
                entry.Property(e => e.LastPaperEventId).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }

        }
        public void MarkСorruptionDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                dbContext.DocumentEventsSet.Add(paperEventDb);
                dbContext.SaveChanges();
                paper.LastPaperEventId = paperEventDb.Id;
                var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                dbContext.SafeAttach(paperDb);
                var entry = dbContext.Entry(paperDb);
                entry.Property(e => e.IsInWork).IsModified = true;
                entry.Property(e => e.LastPaperEventId).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();

            }
        }
        public void SendDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var paper in papers)
                {
                    var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                    dbContext.SafeAttach(paperEventDb);
                    var entry = dbContext.Entry(paperEventDb);
                    entry.Property(e => e.Date).IsModified = true;
                    entry.Property(e => e.PaperSendDate).IsModified = true;
                    entry.Property(e => e.PaperSendAgentId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }
        public void RecieveDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var paper in papers)
                {
                    var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                    dbContext.SafeAttach(paperEventDb);
                    var entry = dbContext.Entry(paperEventDb);
                    entry.Property(e => e.Date).IsModified = true;
                    entry.Property(e => e.PaperRecieveDate).IsModified = true;
                    entry.Property(e => e.PaperRecieveAgentId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                }
                transaction.Complete();

            }
        }
        public void CancelPlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var paper in papers)
                {
                    var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                    dbContext.SafeAttach(paperEventDb);
                    var entry = dbContext.Entry(paperEventDb);
                    entry.Property(e => e.ParentEventId).IsModified = true;
                    entry.Property(e => e.SendListId).IsModified = true;
                    entry.Property(e => e.Date).IsModified = true;
                    entry.Property(e => e.PaperPlanDate).IsModified = true;
                    entry.Property(e => e.PaperPlanAgentId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                    var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                    dbContext.SafeAttach(paperDb);
                    var entryP = dbContext.Entry(paperDb);
                    entryP.Property(e => e.LastPaperEventId).IsModified = true;
                    entryP.Property(e => e.LastChangeUserId).IsModified = true;
                    entryP.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }
        public IEnumerable<InternalDocumentPaper> PlanDocumentPaperFromSendListPrepare(IContext context, int idSendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.SendListId == idSendList && x.PaperPlanDate == null && x.PaperSendDate == null && x.PaperRecieveDate == null)
                    .Select(x => new InternalDocumentPaper
                    {
                        Id = x.Paper.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.Paper.DocumentId,
                        IsInWork = x.Paper.IsInWork,
                        LastPaperEventId = x.Paper.LastPaperEventId,
                        LastPaperEvent = !x.Paper.LastPaperEventId.HasValue
                            ? null
                            : new InternalDocumentEvent
                            {
                                Id = x.Paper.LastPaperEvent.Id,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                SourcePositionId = x.Paper.LastPaperEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                TargetPositionId = x.Paper.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                PaperPlanDate = x.Paper.LastPaperEvent.PaperPlanDate,
                                PaperSendDate = x.Paper.LastPaperEvent.PaperSendDate,
                                PaperRecieveDate = x.Paper.LastPaperEvent.PaperRecieveDate,
                            },
                        NextPaperEventId = x.Id,
                    }).ToList();
                transaction.Complete();
                return res;

            }
        }
        public InternalDocument PlanDocumentPaperEventPrepare(IContext context, List<int> paperIds)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = new InternalDocument();

                var filterContains = PredicateBuilder.New<DocumentPapers>(false);
                filterContains = paperIds.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                doc.Papers = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentPaper
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        IsInWork = x.IsInWork,
                        LastPaperEventId = x.LastPaperEventId,
                        LastPaperEvent = !x.LastPaperEventId.HasValue
                            ? null
                            : new InternalDocumentEvent
                            {
                                Id = x.LastPaperEvent.Id,
                                ClientId = x.ClientId,
                                EntityTypeId = x.EntityTypeId,
                                SourcePositionId = x.LastPaperEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                TargetPositionId = x.LastPaperEvent.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                            }
                    }).ToList();
                if (!doc.Papers.Any()) return null;
                doc.Id = doc.Papers.First().Id;
                transaction.Complete();
                return doc;
            }
        }
        public void PlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (papers != null && papers.Any(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null))
                {
                    foreach (var paper in papers.Where(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null).ToList())
                    {
                        var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                        dbContext.DocumentEventsSet.Add(paperEventDb);
                        dbContext.SaveChanges();
                        paper.LastPaperEventId = paperEventDb.Id;
                        var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                        dbContext.SafeAttach(paperDb);
                        var entry = dbContext.Entry(paperDb);
                        entry.Property(e => e.LastPaperEventId).IsModified = true;
                        entry.Property(e => e.LastChangeUserId).IsModified = true;
                        entry.Property(e => e.LastChangeDate).IsModified = true;
                        dbContext.SaveChanges();
                        var positions = paper.LastPaperEvent.Accesses.Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList();
                        CommonQueries.ModifyDocumentAccessesStatistics(context, paper.DocumentId, positions);
                    }
                }
                transaction.Complete();

            }
        }

        public InternalDocumentPaperList AddDocumentPaperListsPrepare(IContext context, AddDocumentPaperList model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var sourcePositions = (model.SourcePositionIds == null || !model.SourcePositionIds.Any())
                    ? context.CurrentPositionsIdList
                    : context.CurrentPositionsIdList.Where(x => model.SourcePositionIds.Contains(x)).ToList();

                var qry = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                            .Where(x =>
                                x.PaperPlanDate.HasValue
                                && !x.PaperSendDate.HasValue
                                && !x.PaperRecieveDate.HasValue
                                && !x.PaperListId.HasValue
                                && x.EventTypeId == (int)EnumEventTypes.MoveDocumentPaper
                        ).AsQueryable();

                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = sourcePositions.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Accesses.Any(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source && y.PositionId == value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (model.TargetPositionIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = model.TargetPositionIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (model.TargetAgentIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = model.TargetAgentIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                                .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().AgentId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                var list = new InternalDocumentPaperList
                {
                    Events = qry.Select(x => new InternalDocumentEvent
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        SourcePositionId = x.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,   //TODO multitarget
                        TargetPositionId = x.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().PositionId,
                        TargetAgentId = x.Accesses.Where(y => y.AccessTypeId <= (int)EnumEventAccessTypes.Target)
                                                     .OrderByDescending(y => y.AccessTypeId).FirstOrDefault().AgentId,
                    }
                        )
                        .ToList()
                };
                transaction.Complete();
                return list;
            }
        }

        public List<int> AddDocumentPaperLists(IContext context, IEnumerable<InternalDocumentPaperList> items)
        {
            List<int> res = new List<int>();
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var item in items)
                {
                    var itemDb = ModelConverter.GetDbDocumentPaperList(item);
                    itemDb.ClientId = context.Client.Id;
                    dbContext.DocumentPaperListsSet.Add(itemDb);
                    dbContext.SaveChanges();
                    res.Add(itemDb.Id);
                    var eventList = item.Events.Select(x => x.Id).ToList();

                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = item.Events.Select(x => x.Id).ToList().Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(filterContains)
                        .ToList()
                        .ForEach(x =>
                        {
                            x.PaperListId = itemDb.Id;
                            x.LastChangeUserId = itemDb.LastChangeUserId;
                            x.LastChangeDate = itemDb.LastChangeDate;
                        });
                    dbContext.SaveChanges();
                }
                transaction.Complete();

            }
            return res;
        }

        public InternalDocumentPaperList DeleteDocumentPaperListPrepare(IContext context, int itemId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Accesses.Any(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source && y.PositionId == value)).Expand());

                var res = new InternalDocumentPaperList
                {
                    Id = itemId,
                    Events = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                                .Where(x =>
                                    x.PaperListId.HasValue
                                    && itemId == x.PaperListId.Value
                                    && x.EventTypeId == (int)EnumEventTypes.MoveDocumentPaper
                                    )
                                .Where(filterContains)
                                .Select(x => new InternalDocumentEvent
                                {
                                    Id = x.Id,
                                    ClientId = x.ClientId,
                                    EntityTypeId = x.EntityTypeId,
                                    PaperListId = x.PaperListId,
                                    SourcePositionId = x.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                                    PaperPlanDate = x.PaperPlanDate,
                                    PaperSendDate = x.PaperSendDate,
                                    PaperRecieveDate = x.PaperRecieveDate,
                                }).ToList()
                };
                transaction.Complete();
                return res;
            }
        }

        public InternalDocumentPaperList ModifyDocumentPaperListPrepare(IContext context, int itemId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var list = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == itemId)
                    .Select(x => new InternalDocumentPaperList
                    {
                        Id = x.Id,
                    }
                    ).FirstOrDefault();
                if (list == null) return null;
                list.SourcePositionId = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                    .FirstOrDefault(x => x.PaperListId.HasValue && itemId == x.PaperListId.Value)
                    .Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId;
                transaction.Complete();
                return list;
            }
        }

        public void ModifyDocumentPaperList(IContext context, InternalDocumentPaperList item)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbDocumentPaperList(item);
                dbContext.SafeAttach(itemDb);
                var entry = dbContext.Entry(itemDb);
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteDocumentPaperList(IContext context, InternalDocumentPaperList item)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(x => item.Id == x.PaperListId).ToList()
                    .ForEach(x =>
                    {
                        x.PaperListId = null;
                        x.LastChangeUserId = item.LastChangeUserId;
                        x.LastChangeDate = item.LastChangeDate;
                    });
                dbContext.DocumentPaperListsSet.RemoveRange(dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == item.Id));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion DocumentPapers
    }
}