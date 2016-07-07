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
using System.Transactions;
using BL.Model.AdminCore;
using BL.Model.SystemCore;
using DocumentAccesses = BL.Database.DBModel.Document.DocumentAccesses;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Exception;
using LinqKit;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        #region DocumentAction

        public DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();
                res.Document = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        IsLaunchPlan = x.IsLaunchPlan,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = CommonQueries.GetDocumentAccessesesQry(dbContext, res.Document.Id, context)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                            IsFavourite = x.IsFavourite,
                        }
                        ).ToList();
                    res.Document.IsInWork = res.Document.Accesses.Any(x => x.IsInWork);
                    res.Document.IsFavourite = res.Document.Accesses.Any(x => x.IsFavourite);


                    res.Document.Events = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                        }
                        ).ToList();

                    res.Document.Waits = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentWait
                        {
                            Id = x.Id,
                            OffEventId = x.OffEventId,
                            ParentId = x.ParentId,
                            OnEvent = new InternalDocumentEvent
                            {
                                Id = x.OnEvent.Id,
                                TaskId = x.OnEvent.TaskId,
                                IsAvailableWithinTask = x.OnEvent.IsAvailableWithinTask,
                                SourcePositionId = x.OnEvent.SourcePositionId,
                                TargetPositionId = x.OnEvent.TargetPositionId,
                                //SourcePositionName = x.OnEvent.SourcePosition.Name,
                                //TargetPositionName = x.OnEvent.TargetPosition.Name,
                                //SourcePositionExecutorAgentName = x.OnEvent.SourcePosition.ExecutorAgent.Name,
                                //TargetPositionExecutorAgentName = x.OnEvent.TargetPosition.ExecutorAgent.Name,
                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                //GeneralInfo = $"от {x.OnEvent.SourcePosition.ExecutorAgent.Name} к {x.OnEvent.TargetPosition.ExecutorAgent.Name} {x.OnEvent.TaskName}"
                            }

                        }
                        ).ToList();

                    res.Document.Subscriptions =
                        dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                            .Select(x => new InternalDocumentSubscription
                            {
                                Id = x.Id,
                            }
                            ).ToList();
                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentSendList
                        {
                            Id = x.Id,
                        }
                        ).ToList();
                    res.Document.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentTask
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                        }
                        ).ToList();



                    var positionAccesses = res.Document?.Accesses.Select(y => y.PositionId).ToList();

                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = CommonQueries.GetPositionWithActions(context, dbContext, positionAccesses);
                        res.ActionsList = CommonQueries.GetActionsListForCurrentPositionsList(context, dbContext, new List<EnumObjects> { EnumObjects.Documents, EnumObjects.DocumentEvents, EnumObjects.DocumentWaits, EnumObjects.DocumentSubscriptions }, positionAccesses);
                    }
                }
                return res;
            }
        }

        public DocumentActionsModel GetDocumentSendListActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();

                res.Document = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                        }
                        ).ToList();

                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                         .Select(x => new InternalDocumentSendList
                         {
                             Id = x.Id,
                             SourcePositionId = x.SourcePositionId,
                             StartEventId = x.StartEventId,
                             CloseEventId = x.CloseEventId,
                         }
                         ).ToList();


                    var positionAccesses = res.Document?.Accesses.Select(y => y.PositionId).ToList();

                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = CommonQueries.GetPositionWithActions(context, dbContext, positionAccesses);
                        res.ActionsList = CommonQueries.GetActionsListForCurrentPositionsList(context, dbContext, new List<EnumObjects> { EnumObjects.DocumentSendLists, EnumObjects.DocumentSendListStages }, positionAccesses);
                    }
                }
                return res;
            }
        }

        public DocumentActionsModel GetDocumentFileActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();

                res.Document = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                        }
                        ).ToList();

                    res.Document.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.Id,
                            ExecutorPositionId = x.ExecutorPositionId,
                            IsAdditional = x.IsAdditional,
                            IsWorkedOut = x.IsWorkedOut,
                            IsMainVersion = x.IsMainVersion,
                            IsDeleted = x.IsDeleted,
                        }).ToList();


                    var positionAccesses = res.Document?.Accesses.Select(y => y.PositionId).ToList();

                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = CommonQueries.GetPositionWithActions(context, dbContext, positionAccesses);
                        res.ActionsList = CommonQueries.GetActionsListForCurrentPositionsList(context, dbContext, new List<EnumObjects> { EnumObjects.DocumentFiles }, positionAccesses);
                    }
                }
                return res;
            }
        }

        public DocumentActionsModel GetDocumentPaperActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();

                res.Document = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        //IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        //LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {
                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                        }
                        ).ToList();

                    res.Document.Papers = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId)
                         .Select(x => new InternalDocumentPaper
                         {
                             Id = x.Id,
                             IsInWork = x.IsInWork,
                             LastPaperEvent = !x.LastPaperEventId.HasValue ? null :
                                            new InternalDocumentEvent
                                            {
                                                Id = x.LastPaperEvent.Id,
                                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                                SourcePositionId = x.LastPaperEvent.SourcePositionId,
                                                TargetPositionId = x.LastPaperEvent.TargetPositionId,
                                                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                                                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,

                                            }
                         }
                         ).ToList();

                    var positionAccesses = res.Document?.Accesses.Select(y => y.PositionId).ToList();
                    if (positionAccesses.Any())
                    {
                        res.PositionWithActions = CommonQueries.GetPositionWithActions(context, dbContext, positionAccesses);
                        res.ActionsList = CommonQueries.GetActionsListForCurrentPositionsList(context, dbContext, new List<EnumObjects> { EnumObjects.DocumentPapers, EnumObjects.DocumentPaperEvents }, positionAccesses);
                    }
                }

                return res;
            }
        }

        #endregion DocumentAction   

        #region DocumentMainLogic  

        public void AddDocumentWaits(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                    {
                        var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First(x => x.Id == 0));
                        dbContext.DocumentTasksSet.Add(taskDb);
                        dbContext.SaveChanges();
                        ((List<InternalDocumentWait>)document.Waits).ForEach(x => x.OnEvent.TaskId = taskDb.Id);
                    }

                    if (document.Waits?.Any() ?? false)
                    {
                        dbContext.DocumentWaitsSet.AddRange(ModelConverter.GetDbDocumentWaits(document.Waits));
                        dbContext.SaveChanges();
                    }
                    CommonQueries.ModifyDocumentTaskAccesses(dbContext, ctx, document.Id);
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void ChangeDocumentWait(IContext ctx, InternalDocumentWait wait)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    var waitParentDb = ModelConverter.GetDbDocumentWait(wait.ParentWait);
                    dbContext.DocumentWaitsSet.Add(waitParentDb);
                    dbContext.SaveChanges();


                    var eventDb = ModelConverter.GetDbDocumentEvent(wait.OnEvent);
                    eventDb.Id = wait.OnEvent.Id;
                    dbContext.DocumentEventsSet.Attach(eventDb);
                    dbContext.Entry(eventDb).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    wait.OnEvent = null;

                    var waitDb = ModelConverter.GetDbDocumentWait(wait);
                    waitDb.Id = wait.Id;
                    waitDb.ParentId = waitParentDb.Id;
                    waitDb.ParentWait = null;
                    dbContext.DocumentWaitsSet.Attach(waitDb);
                    dbContext.Entry(waitDb).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void ChangeTargetDocumentWait(IContext ctx, InternalDocumentWait wait, InternalDocumentEvent newEvent)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var eventDb = ModelConverter.GetDbDocumentEvent(newEvent);
                    dbContext.DocumentEventsSet.Add(eventDb);
                    dbContext.SaveChanges();

                    var waitDb = ModelConverter.GetDbDocumentWait(wait);
                    dbContext.DocumentWaitsSet.Attach(waitDb);
                    var entry = dbContext.Entry(waitDb);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.TargetDescription).IsModified = true;
                    entry.Property(x => x.AttentionDate).IsModified = true;
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void CloseDocumentWait(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var offEvent = ModelConverter.GetDbDocumentEvent(document.Waits.First().OffEvent);
                foreach (var docWait in document.Waits)
                {
                    var wait = new DocumentWaits
                    {
                        Id = docWait.Id,
                        ResultTypeId = docWait.ResultTypeId,
                        LastChangeDate = docWait.LastChangeDate,
                        LastChangeUserId = docWait.LastChangeUserId
                    };
                    dbContext.DocumentWaitsSet.Attach(wait);
                    wait.OffEvent = offEvent;
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
                    dbContext.DocumentSendListsSet.Attach(sendListDb);
                    var entry = dbContext.Entry(sendListDb);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    if (sendList.StartEventId != null)
                    {
                        sendListDb.CloseEvent = offEvent;
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
                    var docHash = CommonQueries.GetDocumentHash(dbContext, ctx, document.Id,
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
                        LastChangeDate = subscription.LastChangeDate,
                        LastChangeUserId = subscription.LastChangeUserId
                    };
                    dbContext.DocumentSubscriptionsSet.Attach(subscriptionDb);
                    if (subscription.DoneEvent != null)
                    {
                        subscriptionDb.DoneEvent = offEvent;
                    }
                    var entry = dbContext.Entry(subscriptionDb);
                    entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.Description).IsModified = true;
                    entry.Property(x => x.SubscriptionStateId).IsModified = true;
                    entry.Property(x => x.Hash).IsModified = true;
                    entry.Property(x => x.FullHash).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ControlChangeDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var maxDateTime = DateTime.Now.AddYears(50);

                var doc = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.OnEventId == eventId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            ParentId = x.ParentId,
                                            OnEventId = x.OnEventId,
                                            OffEventId = x.OffEventId,
                                            DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                                            AttentionDate = x.AttentionDate,
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                DocumentId = x.OnEvent.DocumentId,
                                                SourcePositionId = x.OnEvent.SourcePositionId,
                                                SourcePositionExecutorAgentId = x.OnEvent.SourcePositionExecutorAgentId,
                                                TargetPositionId = x.OnEvent.TargetPositionId,
                                                TargetPositionExecutorAgentId = x.OnEvent.TargetPositionExecutorAgentId,
                                                SourceAgentId = x.OnEvent.SourceAgentId,
                                                TargetAgentId = x.OnEvent.TargetAgentId,
                                                TaskId = x.OnEvent.TaskId,
                                                IsAvailableWithinTask = x.OnEvent.IsAvailableWithinTask,
                                                Description = x.OnEvent.Description,
                                                AddDescription = x.OnEvent.AddDescription,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                                CreateDate = x.OnEvent.CreateDate,
                                                Date = x.OnEvent.Date,
                                                LastChangeUserId = x.OnEvent.LastChangeUserId,
                                                LastChangeDate = x.OnEvent.LastChangeDate,
                                                SendDate = x.OnEvent.SendDate,
                                                ReadDate = x.OnEvent.ReadDate,
                                                ReadAgentId = x.OnEvent.ReadAgentId,

                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                return doc;

            }
        }

        public InternalDocument ControlTargetChangeDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.OnEventId == eventId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            OnEventId = x.OnEventId,
                                            OffEventId = x.OffEventId,
                                            TargetDescription = x.TargetDescription,
                                            AttentionDate = x.AttentionDate,
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                TargetPositionId = x.OnEvent.TargetPositionId,
                                                TaskId = x.OnEvent.TaskId,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                return doc;

            }
        }

        public InternalDocument ControlOffDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.OnEventId == eventId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            OffEventId = x.OffEventId,
                                            OnEventId = x.OnEventId,
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                DocumentId = x.OnEvent.DocumentId,
                                                EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                                                SourcePositionId = x.OnEvent.SourcePositionId,
                                                TargetPositionId = x.OnEvent.TargetPositionId,
                                                TaskId = x.OnEvent.TaskId,
                                                IsAvailableWithinTask = x.OnEvent.IsAvailableWithinTask,
                                            }
                                        }
                                    }
                    }).FirstOrDefault();

                return doc;

            }
        }

        public void ControlOffSendListPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context))
            {
                var filterContains = PredicateBuilder.False<DocumentSendLists>();
                filterContains = document.Waits.Select(x => x.OnEventId).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.StartEventId == value && !e.CloseEventId.HasValue).Expand());

                document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                    }
                    ).ToList();
            }
        }

        public void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context))
            {
                var filterContains = PredicateBuilder.False<DocumentWaits>();
                filterContains = document.Waits.Select(x => x.Id).ToList().Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ParentId == value).Expand());

                var waitRes = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(filterContains)
                    .Where(x => !x.OffEventId.HasValue && x.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution)
                    .Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        DocumentId = x.OnEvent.DocumentId,
                        OnEvent = new InternalDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            DocumentId = x.OnEvent.DocumentId,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                            SourcePositionId = x.OnEvent.SourcePositionId,
                            TargetPositionId = x.OnEvent.TargetPositionId,
                            TaskId = x.OnEvent.TaskId,
                            IsAvailableWithinTask = x.OnEvent.IsAvailableWithinTask,
                        }
                    }
                    ).ToList();
                ((List<InternalDocumentWait>)document.Waits).AddRange(waitRes);
            }
        }

        public void ControlOffSubscriptionPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context))
            {
                var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                filterContains = document.Waits.Select(x => x.OnEventId).ToList().Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SendEventId == value).Expand());

                document.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => !x.DoneEventId.HasValue)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentSubscription
                    {
                        Id = x.Id,
                    }
                    ).ToList();
            }
        }

        public void AddDocumentEvents(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                    {
                        var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First(x => x.Id == 0));
                        dbContext.DocumentTasksSet.Add(taskDb);
                        dbContext.SaveChanges();
                        ((List<InternalDocumentEvent>)document.Events).ForEach(x => x.TaskId = taskDb.Id);
                    }

                    if (document.Events?.Any() ?? false)
                    {
                        var eventsDb = ModelConverter.GetDbDocumentEvents(document.Events);
                        dbContext.DocumentEventsSet.AddRange(eventsDb);
                        dbContext.SaveChanges();
                    }
                    CommonQueries.ModifyDocumentTaskAccesses(dbContext, ctx, document.Id);
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentEventQuery(ctx, dbContext, new FilterDocumentEvent { EventId = new List<int> { eventId } })
                    .Select(x => new FrontDocumentEvent
                    {
                        Id = x.Id,
                        DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                        DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                        DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,
                        ReadAgentName = x.ReadAgent.Name,
                        ReadDate = x.ReadDate,
                        SourceAgentName = x.SourceAgent.Name,
                        TargetAgentName = x.TargetAgent.Name,
                        SourcePositionName = x.SourcePosition.Name,
                        TargetPositionName = x.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = x.TargetPosition.ExecutorAgent.Name,
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        IsAvailableWithinTask = x.IsAvailableWithinTask,
                        PaperPlanAgentName = x.PaperPlanAgent.Name,
                        PaperSendAgentName = x.PaperSendAgent.Name,
                        PaperRecieveAgentName = x.PaperRecieveAgent.Name,
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterBase filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentEvents(ctx, dbContext, filter, paging).ToList();
            }
        }

        public IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterBase filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentWaits(dbContext, filter, ctx, paging).ToList();
            }
        }

        public IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentSubscriptions(dbContext, filter, ctx, paging).ToList();
            }
        }

        public IEnumerable<InternalDocumentEvent> MarkDocumentEventsAsReadPrepare(IContext ctx, MarkDocumentEventAsRead model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                if (model.EventIds == null)
                    return new List<InternalDocumentEvent>();

                var qry = CommonQueries.GetDocumentEventQuery(ctx, dbContext, new FilterDocumentEvent { IsNew = true, EventId = model.EventIds })
                    .Where(x=> x.TargetPositionId.HasValue && !(x.TargetPositionId == x.SourcePositionId));

                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => new InternalDocumentEvent
                {
                    Id = x.Id
                }).ToList();

                return res;
            }
        }

        public void MarkDocumentEventAsRead(IContext ctx, IEnumerable<InternalDocumentEvent> eventList)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                foreach (var bdev in eventList.Select(evt => new DocumentEvents
                {
                    Id = evt.Id,
                    ReadAgentId = evt.ReadAgentId,
                    ReadDate = evt.ReadDate,
                    LastChangeDate = evt.LastChangeDate,
                    LastChangeUserId = evt.LastChangeUserId
                }))
                {
                    dbContext.DocumentEventsSet.Attach(bdev);
                    var entry = dbContext.Entry(bdev);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.ReadAgentId).IsModified = true;
                    entry.Property(x => x.ReadDate).IsModified = true;
                }
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetInternalDocumentAccesses(dbContext, ctx, documentId);
            }
        }

        public IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetInternalPositionsInfo(dbContext, ctx, positionIds);
            }
        }

        public void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess)
        {
            using (var dbContext = new DmsContext(context))
            {
                var acc = new DocumentAccesses
                {
                    Id = docAccess.Id,
                    IsFavourite = docAccess.IsFavourite,
                    LastChangeDate = docAccess.LastChangeDate,
                    LastChangeUserId = docAccess.LastChangeUserId
                };
                dbContext.DocumentAccessesSet.Attach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsFavourite).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsFavourite = x.IsFavourite,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            IsFavourite = x.IsFavourite,
                                            PositionId = x.PositionId,
                                        }
                                    }

                    }).FirstOrDefault();
                return doc;

            }
        }

        public void ChangeIsInWorkAccess(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var docAccess = document.Accesses.FirstOrDefault();
                if (docAccess == null)
                {
                    throw new WrongParameterValueError();
                }
                var acc = new DocumentAccesses
                {
                    Id = docAccess.Id,
                    IsInWork = docAccess.IsInWork,
                    LastChangeDate = docAccess.LastChangeDate,
                    LastChangeUserId = docAccess.LastChangeUserId
                };
                dbContext.DocumentAccessesSet.Attach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsInWork).IsModified = true;
                dbContext.DocumentEventsSet.Add(ModelConverter.GetDbDocumentEvent(document.Events.FirstOrDefault()));
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var acc = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            IsInWork = x.IsInWork,
                                        }
                                    }

                    }).FirstOrDefault();
                return acc;

            }
        }

        public void SendBySendList(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var sendList = document.SendLists.First();
                    var sendListDb = new DocumentSendLists
                    {
                        Id = sendList.Id,
                        LastChangeDate = sendList.LastChangeDate,
                        LastChangeUserId = sendList.LastChangeUserId
                    };
                    dbContext.DocumentSendListsSet.Attach(sendListDb);
                    sendListDb.StartEvent = ModelConverter.GetDbDocumentEvent(sendList.StartEvent);
                    if (sendList.CloseEvent != null)
                    {
                        sendListDb.CloseEvent = sendListDb.StartEvent;
                    }
                    var entry = dbContext.Entry(sendListDb);
                    //entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    dbContext.SaveChanges();

                    if (document.Accesses?.Any() ?? false)
                    {
                        dbContext.DocumentAccessesSet.AddRange(
                            CommonQueries.GetDbDocumentAccesses(dbContext, ctx, document.Accesses, document.Id).ToList());
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
                        //if (eventsDb != null && eventsDb.Any())
                        //{
                        //    dbContext.DocumentEventsSet.Attach(eventsDb.First());
                        //    dbContext.Entry(eventsDb.First()).State = EntityState.Added;
                        //}
                        dbContext.DocumentEventsSet.Add(eventDb);
                        dbContext.SaveChanges();
                    }

                    if (document.Papers?.Any() ?? false)
                    {
                        foreach (var paper in document.Papers.ToList())
                        {
                            var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                            paperEventDb.ParentEventId = sendListDb.StartEventId.Value;
                            dbContext.DocumentEventsSet.Attach(paperEventDb);
                            var entryEventDb = dbContext.Entry(paperEventDb);
                            entryEventDb.Property(e => e.SourcePositionExecutorAgentId).IsModified = true;
                            entryEventDb.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                            entryEventDb.Property(e => e.SourceAgentId).IsModified = true;
                            entryEventDb.Property(e => e.TargetAgentId).IsModified = true;
                            entryEventDb.Property(e => e.ParentEventId).IsModified = true;
                            entryEventDb.Property(e => e.PaperPlanDate).IsModified = true;
                            entryEventDb.Property(e => e.PaperPlanAgentId).IsModified = true;
                            entryEventDb.Property(e => e.LastChangeUserId).IsModified = true;
                            entryEventDb.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                            paper.LastPaperEvent = null;
                            var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                            paperDb.LastPaperEventId = paperEventDb.Id;
                            dbContext.DocumentPapersSet.Attach(paperDb);
                            var entryPaper = dbContext.Entry(paperDb);
                            entryPaper.Property(e => e.LastPaperEventId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeUserId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }

                    }

                    CommonQueries.ModifyDocumentTaskAccesses(dbContext, ctx, document.Id);
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void ModifyDocumentTags(IContext ctx, InternalDocumentTag model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qryDictionaryTags = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                                            .AsQueryable();

                {
                    var filterContains = PredicateBuilder.False<DBModel.Dictionary.DictionaryTags>();
                    filterContains = model.Tags.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qryDictionaryTags = qryDictionaryTags.Where(filterContains);
                }

                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Dictionary.DictionaryTags>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value || !e.PositionId.HasValue).Expand());

                    qryDictionaryTags = qryDictionaryTags.Where(filterContains);
                }

                var dictionaryTags = qryDictionaryTags
                    .Select(x => x.Id)
                    .ToList();

                var qryDocumentTags = dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                        .Where(x => x.DocumentId == model.DocumentId).AsQueryable();

                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Tag.PositionId == value || !e.Tag.PositionId.HasValue).Expand());

                    qryDocumentTags = qryDocumentTags.Where(filterContains);
                }

                var documentTags = qryDocumentTags
                    .Select(x => x.TagId)
                    .ToList();

                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = documentTags.Where(y => !dictionaryTags.Contains(y)).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TagId == value).Expand());

                    //Удаляем теги которые не присутствуют в списке
                    dbContext.DocumentTagsSet
                        .RemoveRange(dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Where(x => x.DocumentId == model.DocumentId)
                            .Where(filterContains));
                }

                var newDictionaryTags = dictionaryTags
                    .Where(x => !documentTags.Contains(x))
                    .Select(x => new DocumentTags
                    {
                        DocumentId = model.DocumentId,
                        TagId = x,
                        LastChangeUserId = model.LastChangeUserId,
                        LastChangeDate = model.LastChangeDate
                    });

                dbContext.DocumentTagsSet.AddRange(newDictionaryTags);

                dbContext.SaveChanges();
            }
        }

        public InternalDocument AddNoteDocumentPrepare(IContext ctx, AddNote model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => !string.IsNullOrEmpty(model.Task) && x.DocumentId == model.DocumentId && x.Task == model.Task)
                    .Select(x => new List<InternalDocumentTask>
                    {
                                        new InternalDocumentTask
                                        {
                                                Id = x.Id,
                                        }
                    }).FirstOrDefault();

                return doc;
            }
        }

        public InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (sendList.SendType == EnumSendTypes.SendForResponsibleExecution || sendList.SendType == EnumSendTypes.SendForControl || sendList.IsWorkGroup)
                {
                    doc.Waits = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.Task.Id == sendList.TaskId && !x.OffEventId.HasValue
                                    && x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution
                                    && x.OnEvent.SourcePositionId == sendList.SourcePositionId)
                        .Select(x => new InternalDocumentWait
                        {
                            Id = x.Id,
                            OnEvent = new InternalDocumentEvent
                            {
                                TargetPositionId = x.OnEvent.TargetPositionId,
                                TargetPositionExecutorAgentId = x.OnEvent.TargetPositionExecutorAgentId,
                            }
                        }
                        ).ToList();
                }
                if (sendList.SendType == EnumSendTypes.SendForResponsibleExecution || sendList.SendType == EnumSendTypes.SendForControl)
                {
                    doc.Events = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.Task.Id == sendList.TaskId
                                    && x.EventTypeId == (int)EnumEventTypes.SendForControl
                                    && x.SourcePositionId == sendList.SourcePositionId)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                            TargetPositionId = x.TargetPositionId,
                            TargetPositionExecutorAgentId = x.TargetPositionExecutorAgentId,
                        }
                        ).ToList();
                }
                return doc;
            }
        }

        public InternalDocument SendForSigningDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id
                    }).FirstOrDefault();
                if (doc == null) return null;

                return doc;

            }
        }

        #endregion DocumentMainLogic 

        #region DocumentLink    

        public InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,
                        LinkTypeId = model.LinkTypeId,
                    }).FirstOrDefault();

                if (doc == null) return null;

                var par = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.ParentDocumentId)
                    .Select(x => new { Id = x.Id, LinkId = x.LinkId }).FirstOrDefault();

                if (par == null) return null;

                doc.ParentDocumentId = par.Id;
                doc.ParentDocumentLinkId = par.LinkId;

                return doc;
            }
        }

        public InternalDocument DeleteDocumentLinkPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        LinkId = x.LinkId,

                    }).FirstOrDefault();

                if (doc?.LinkId == null) return null;

                var calc = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.LinkId == doc.LinkId && x.Id != doc.Id).GroupBy(x => true)
                    .Select(x => new { Count = x.Count(), MinId = x.Min(y => y.Id) }).First();
                doc.LinkedDocumentsCount = calc.Count;
                doc.NewLinkId = calc.MinId;

                return doc;
            }
        }



        public void AddDocumentLink(IContext context, InternalDocument model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var link = new DocumentLinks
                {
                    DocumentId = model.Id,
                    ParentDocumentId = model.ParentDocumentId,
                    LinkTypeId = model.LinkTypeId,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate,
                };
                dbContext.DocumentLinksSet.Add(link);
                if (!model.ParentDocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.Id == model.ParentDocumentId).ToList()  //TODO OPTIMIZE
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                if (!model.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.Id == model.Id).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentLinkId ?? model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.LinkId == model.LinkId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentLinkId ?? model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                dbContext.SaveChanges();
            }
        }

        public void DeleteDocumentLink(IContext context, InternalDocument model)
        {
            using (var dbContext = new DmsContext(context))
            {
                dbContext.DocumentLinksSet.RemoveRange(dbContext.DocumentLinksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == model.Id || x.ParentDocumentId == model.Id));
                if (model.LinkId == model.Id || model.LinkedDocumentsCount < 2)
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.LinkId == model.LinkId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = (x.Id == model.Id ? null : model.NewLinkId);
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.Id == model.Id).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = null;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentLink     

        #region DocumentSendList    
        public InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId, string task = null, int id = 0)
        {
            using (var dbContext = new DmsContext(context))
            {
                var docDb = from doc in dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == documentId)
                            join tmp in dbContext.TemplateDocumentsSet on doc.TemplateDocumentId equals tmp.Id
                            where tmp.ClientId == context.CurrentClientId
                            select new { doc, tmp };

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,
                    TemplateDocumentId = x.tmp.Id,
                    IsHard = x.tmp.IsHard,
                    IsLaunchPlan = x.doc.IsLaunchPlan
                }).FirstOrDefault();

                if (docRes == null) return null;
                docRes.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => !string.IsNullOrEmpty(task) && x.DocumentId == documentId && x.Task == task)
                        .Select(x => new List<InternalDocumentTask>
                        {
                                                            new InternalDocumentTask
                                                            {
                                                                    Id = x.Id,
                                                            }
                        }).FirstOrDefault();
                docRes.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == docRes.Id)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId
                    }).ToList();


                docRes.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == docRes.Id)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        TargetPositionId = x.TargetPositionId,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        Stage = x.Stage,
                        SourcePositionId = x.SourcePositionId,
                        SourceAgentId = x.SourceAgentId,
                        TargetAgentId = x.TargetAgentId

                    }).ToList();

                if (docRes.IsHard)
                {
                    docRes.TemplateDocument = new InternalTemplateDocument();

                    docRes.TemplateDocument.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                        .Select(x => new InternalTemplateDocumentRestrictedSendList
                        {
                            Id = x.Id,
                            PositionId = x.PositionId
                        }).ToList();

                    docRes.TemplateDocument.SendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                        .Select(x => new InternalTemplateDocumentSendList
                        {
                            Id = x.Id,
                            TargetPositionId = x.TargetPositionId,
                            SendType = (EnumSendTypes)x.SendTypeId
                        }).ToList();
                }

                if (id != 0)
                {
                    docRes.PaperEvents = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.SendListId == id)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                            PaperId = x.PaperId,
                            SourcePositionId = x.SourcePositionId,
                            TargetPositionId = x.TargetPositionId,
                            Description = x.Description,
                        }).ToList();
                }

                return docRes;
            }
        }

        public IEnumerable<int> AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model)
        {
            List<int> res;
            using (var dbContext = new DmsContext(context))
            {
                var items = model.Select(x => new DocumentRestrictedSendLists
                {
                    AccessLevelId = (int)x.AccessLevel,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                dbContext.DocumentRestrictedSendListsSet.AddRange(items);
                dbContext.SaveChanges();
                res = items.Select(x => x.Id).ToList();
            }
            return res;
        }

        public IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     DocumentId = model.DocumentId,
                     PositionId = x.TargetPositionId,
                     AccessLevel = (EnumDocumentAccesses)(x.AccessLevelId ?? (int)EnumDocumentAccesses.PersonalRefIO)
                 }).ToList();

                return items;
            }
        }

        public InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(context))
            {

                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == restSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     Id = x.Id,
                     DocumentId = x.DocumentId
                 }).FirstOrDefault();

                return item;
            }
        }

        public void DeleteDocumentRestrictedSendList(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == restSendListId);
                if (item != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<int> AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> sendList, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> paperEvents = null)
        {
            List<int> res = null;
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (task?.Any(x => x.Id == 0) ?? false)
                    {
                        var taskDb = ModelConverter.GetDbDocumentTask(task.First(x => x.Id == 0));
                        dbContext.DocumentTasksSet.Add(taskDb);
                        dbContext.SaveChanges();
                        ((List<InternalDocumentSendList>)sendList).ForEach(x => x.TaskId = taskDb.Id);
                    }

                    if (sendList?.Any() ?? false)
                    {
                        var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendList).ToList();
                        dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                        dbContext.SaveChanges();
                        res = sendListsDb.Select(x => x.Id).ToList();
                    }
                    if (paperEvents?.Any() ?? false)
                    {
                        var listPaperEvent = paperEvents.ToList();
                        listPaperEvent.ForEach(x => { x.SendListId = res.FirstOrDefault(); });
                        var paperEventsDb = ModelConverter.GetDbDocumentEvents(listPaperEvent).ToList();
                        dbContext.DocumentEventsSet.AddRange(paperEventsDb);
                        dbContext.SaveChanges();
                    }

                    transaction.Complete();
                }
            }
            return res;
        }

        public IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            //TODO DELETE!!!!
            using (var dbContext = new DmsContext(context))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentSendList
                 {
                     DocumentId = model.DocumentId,
                     Stage = x.Stage,
                     SendType = (EnumSendTypes)x.SendTypeId,
                     SourcePositionId = context.CurrentPositionId,
                     SourceAgentId = context.CurrentAgentId,
                     TargetPositionId = x.TargetPositionId,
                     Description = x.Description,
                     DueDate = x.DueDate,
                     DueDay = x.DueDay,
                     IsInitial = model.IsInitial,
                     AccessLevel = (EnumDocumentAccesses)(x.AccessLevelId ?? (int)EnumDocumentAccesses.PersonalRefIO),
                     LastChangeUserId = context.CurrentAgentId,
                     LastChangeDate = DateTime.Now,
                 }).ToList();

                return items;
            }
        }

        public void ModifyDocumentSendList(IContext context, InternalDocumentSendList sendList, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> addPaperEvents = null, IEnumerable<int?> delPaperEvents = null)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (task?.Any(x => x.Id == 0) ?? false)
                    {
                        var taskDb = ModelConverter.GetDbDocumentTask(task.First(x => x.Id == 0));
                        dbContext.DocumentTasksSet.Add(taskDb);
                        dbContext.SaveChanges();
                        sendList.TaskId = taskDb.Id;
                    }
                    var sendListDb = ModelConverter.GetDbDocumentSendList(sendList);

                    dbContext.DocumentSendListsSet.Attach(sendListDb);
                    var entry = dbContext.Entry(sendListDb);
                    entry.Property(e => e.Stage).IsModified = true;
                    entry.Property(e => e.SendTypeId).IsModified = true;
                    entry.Property(e => e.TargetPositionId).IsModified = true;
                    entry.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                    entry.Property(e => e.TargetAgentId).IsModified = true;
                    entry.Property(e => e.TaskId).IsModified = true;
                    entry.Property(e => e.IsAvailableWithinTask).IsModified = true;
                    entry.Property(e => e.IsWorkGroup).IsModified = true;
                    entry.Property(e => e.IsAddControl).IsModified = true;
                    entry.Property(e => e.SelfDueDate).IsModified = true;
                    entry.Property(e => e.SelfDueDay).IsModified = true;
                    entry.Property(e => e.SelfAttentionDate).IsModified = true;
                    entry.Property(e => e.IsInitial).IsModified = true;
                    entry.Property(e => e.Description).IsModified = true;
                    entry.Property(e => e.DueDate).IsModified = true;
                    entry.Property(e => e.DueDay).IsModified = true;
                    entry.Property(e => e.AccessLevelId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();

                    if (delPaperEvents?.Any() ?? false)
                    {
                        var filterContains = PredicateBuilder.False<DocumentEvents>();
                        filterContains = delPaperEvents.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.PaperId == value).Expand());

                        dbContext.DocumentEventsSet.RemoveRange(
                            dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                    .Where(filterContains)
                                    .Where(x => x.SendListId == sendList.Id));
                        dbContext.SaveChanges();
                    }
                    if (addPaperEvents?.Any() ?? false)
                    {
                        var paperEventsDb = ModelConverter.GetDbDocumentEvents(addPaperEvents).ToList();
                        dbContext.DocumentEventsSet.AddRange(paperEventsDb);
                        dbContext.SaveChanges();
                    }


                    transaction.Complete();
                }
            }
        }

        public InternalDocument DeleteDocumentSendListPrepare(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                            .Where(x => x.Id == sendListId)
                            .Select(x => new InternalDocument
                            {
                                Id = x.DocumentId,
                                SendLists = new List<InternalDocumentSendList>
                                {
                                        new InternalDocumentSendList
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            SourcePositionId = x.SourcePositionId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                }
                            }).FirstOrDefault();
                return doc;
            }
        }

        public void DeleteDocumentSendList(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == sendListId);
                if (item != null)
                {
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.SendListId == sendListId && x.PaperPlanDate == null));
                    dbContext.DocumentSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var docDb = (from doc in dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == documentId)
                             select new { doc })
                             .GroupJoin(dbContext.DocumentSendListsSet, x => x.doc.Id, y => y.DocumentId, (x, y) => new { x.doc, sls = y });

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,

                    SendLists = x.sls.Select(y => new InternalDocumentSendList
                    {
                        Id = y.Id,
                        Stage = y.Stage
                    }),
                }).FirstOrDefault();

                return docRes;
            }
        }

        public void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model)
        {
            using (var dbContext = new DmsContext(context))
            {
                foreach (var sl in model)
                {
                    var item = new DocumentSendLists
                    {
                        Id = sl.Id,
                        Stage = sl.Stage,
                        LastChangeUserId = sl.LastChangeUserId,
                        LastChangeDate = sl.LastChangeDate
                    };
                    dbContext.DocumentSendListsSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(e => e.Stage).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                }

                dbContext.SaveChanges();
            }
        }

        public InternalDocument LaunchDocumentSendListItemPrepare(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        SendLists = new List<InternalDocumentSendList>
                                    {
                                        new InternalDocumentSendList
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            Stage = x.Stage,
                                            SendType = (EnumSendTypes)x.SendTypeId,
                                            SourcePositionId = x.SourcePositionId,
                                            SourceAgentId = x.SourceAgentId,
                                            TargetPositionId = x.TargetPositionId,
                                            TargetAgentId = x.TargetAgentId,
                                            TaskId = x.TaskId,
                                            IsAvailableWithinTask = x.IsAvailableWithinTask,
                                            IsWorkGroup = x.IsWorkGroup,
                                            IsAddControl = x.IsAddControl,
                                            SelfDueDate = x.SelfDueDate,
                                            SelfDueDay = x.SelfDueDay,
                                            SelfAttentionDate = x.SelfAttentionDate,
                                            IsInitial = x.IsInitial,
                                            Description = x.Description,
                                            DueDay = x.DueDay,
                                            DueDate = x.DueDate,
                                            AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                    }
                    }).FirstOrDefault();
                return doc;

            }
        }


        #endregion DocumentSendList     

        #region DocumentSavedFilter

        public List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var items = model.Select(x => new DocumentSavedFilters
                {
                    ClientId = context.CurrentClientId,
                    PositionId = x.PositionId,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                dbContext.DocumentSavedFiltersSet.AddRange(items);
                dbContext.SaveChanges();

                return items.Select(x => x.Id).ToList();
            }
        }

        public void ModifySavedFilter(IContext context, InternalDocumentSavedFilter model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = new DocumentSavedFilters
                {
                    Id = model.Id,
                    PositionId = model.PositionId,
                    Icon = model.Icon,
                    Filter = model.Filter,
                    IsCommon = model.IsCommon,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate
                };
                dbContext.DocumentSavedFiltersSet.Attach(item);

                var entry = dbContext.Entry(item);
                entry.Property(e => e.PositionId).IsModified = true;
                entry.Property(e => e.Icon).IsModified = true;
                entry.Property(e => e.Filter).IsModified = true;
                entry.Property(e => e.IsCommon).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
            }
        }

        public void DeleteSavedFilter(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion DocumentSavedFilter

        #region DocumentTasks
        public IEnumerable<int> AddDocumentTasks(IContext context, InternalDocument document)
        {
            List<int> res = null;
            using (var dbContext = new DmsContext(context))
            {
                var tasksDb = document.Tasks.Select(ModelConverter.GetDbDocumentTask).ToList();
                dbContext.DocumentTasksSet.AddRange(tasksDb);
                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
                res = tasksDb.Select(x => x.Id).ToList();
            }
            return res;
        }
        public InternalDocument DeleteDocumentTaskPrepare(IContext context, int taskId)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == taskId)
                        .Select(x => new InternalDocument
                        {
                            Id = x.Document.Id,
                            Tasks = new List<InternalDocumentTask>
                                    {
                                                        new InternalDocumentTask
                                                        {
                                                            Id = x.Id,
                                                            PositionId = x.PositionId,
                                                            CountEvents = x.Events.Count(y=>y.EventTypeId!=(int)EnumEventTypes.TaskFormulation),
                                                            CountSendLists = x.SendLists.Count,
                                                        }
                                    }
                        }).FirstOrDefault();
            }
        }

        public void ModifyDocumentTask(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context))
            {
                var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First());
                dbContext.DocumentTasksSet.Attach(taskDb);
                var entry = dbContext.Entry(taskDb);
                entry.Property(e => e.Task).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDocumentTask(IContext context, int itemId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == itemId);
                if (item != null)
                {
                    dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == itemId));
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == itemId && x.EventTypeId != (int)EnumEventTypes.TaskFormulation));
                    dbContext.DocumentTasksSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDocument ModifyDocumentTaskPrepare(IContext context, ModifyDocumentTasks model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => (x.Task == model.Name || x.Id == model.Id) && x.DocumentId == model.DocumentId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                        PositionId = x.PositionId,
                    }).ToList();

                return doc;
            }
        }
        #endregion DocumentTasks

        #region DocumentPapers

        public InternalDocument DeleteDocumentPaperPrepare(IContext context, int paperId)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == paperId)
                        .Select(x => new InternalDocument
                        {
                            Id = x.Document.Id,
                            ExecutorPositionId = x.Document.ExecutorPositionId,
                            Papers = new List<InternalDocumentPaper>
                                    {
                                        new InternalDocumentPaper
                                        {
                                            Id = x.Id,
                                            OrderNumber = x.OrderNumber,
                                            IsInWork = x.IsInWork,
                                            LastPaperEvent = new InternalDocumentEvent
                                            {
                                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                                TargetPositionId = x.LastPaperEvent.TargetPositionId,
                                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                                            }
                                        }
                                    }
                        }).FirstOrDefault();
            }
        }

        public InternalDocument EventDocumentPaperPrepare(IContext context, PaperList filters, bool isCalcPreLastPaperEvent = false)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Select(x => x);

                if (filters.PaperId != null && filters.PaperId.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPapers>();
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
                        IsInWork = x.IsInWork,
                        DocumentId = x.DocumentId,
                        LastPaperEvent = !x.LastPaperEventId.HasValue
                                ? null
                                : new InternalDocumentEvent
                                {
                                    Id = x.LastPaperEvent.Id,
                                    SourcePositionId = x.LastPaperEvent.SourcePositionId,
                                    TargetPositionId = x.LastPaperEvent.TargetPositionId,
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
                        x.PreLastPaperEventId = dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => y.PaperId == x.Id && y.Id != x.LastPaperEvent.Id && y.PaperRecieveDate != null &&
                                            y.TargetPositionId == x.LastPaperEvent.SourcePositionId)
                                .OrderByDescending(y => y.PaperRecieveDate)
                                .Select(y => y.Id)
                                .FirstOrDefault();
                    });
                }
                return doc;
            }
        }

        public InternalDocument ModifyDocumentPaperPrepare(IContext context, ModifyDocumentPapers model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();

                if (doc == null) return null;
                if (model.Id == 0)
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
                    doc.Papers = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => (x.Id == model.Id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
                        .Select(x => new InternalDocumentPaper
                        {
                            Id = x.Id,
                            IsCopy = x.IsCopy,
                            IsInWork = x.IsInWork,
                            LastPaperEvent = new InternalDocumentEvent
                            {
                                EventType = (EnumEventTypes)x.LastPaperEvent.EventTypeId,
                                TargetPositionId = x.LastPaperEvent.TargetPositionId,
                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                            }
                        }).ToList();
                }

                return doc;
            }
        }

        public IEnumerable<int> AddDocumentPapers(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            List<int> res = new List<int>();
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
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
                            dbContext.DocumentPapersSet.Attach(paperDb);
                            var entry = dbContext.Entry(paperDb);
                            entry.Property(x => x.LastPaperEventId).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    transaction.Complete();
                }
            }
            return res;
        }

        public void ModifyDocumentPaper(IContext context, InternalDocumentPaper item)
        {
            using (var dbContext = new DmsContext(context))
            {
                var itemDb = ModelConverter.GetDbDocumentPaper(item);
                dbContext.DocumentPapersSet.Attach(itemDb);
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
            }
        }

        public void DeleteDocumentPaper(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var paper = new DocumentPapers { Id = id };
                    dbContext.DocumentPapersSet.Attach(paper);
                    var entry = dbContext.Entry(paper);
                    entry.Property(e => e.LastPaperEventId).IsModified = true;
                    dbContext.SaveChanges();
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.PaperId == id && x.EventTypeId == (int)EnumEventTypes.AddNewPaper));
                    dbContext.DocumentPapersSet.RemoveRange(dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == id));
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void MarkOwnerDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                    dbContext.DocumentEventsSet.Add(paperEventDb);
                    dbContext.SaveChanges();
                    paper.LastPaperEventId = paperEventDb.Id;
                    var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                    dbContext.DocumentPapersSet.Attach(paperDb);
                    var entry = dbContext.Entry(paperDb);
                    entry.Property(e => e.LastPaperEventId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public void MarkСorruptionDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                    dbContext.DocumentEventsSet.Add(paperEventDb);
                    dbContext.SaveChanges();
                    paper.LastPaperEventId = paperEventDb.Id;
                    var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                    dbContext.DocumentPapersSet.Attach(paperDb);
                    var entry = dbContext.Entry(paperDb);
                    entry.Property(e => e.IsInWork).IsModified = true;
                    entry.Property(e => e.LastPaperEventId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public void SendDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var paper in papers)
                    {
                        var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                        dbContext.DocumentEventsSet.Attach(paperEventDb);
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
        }

        public void RecieveDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var paper in papers)
                    {
                        var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                        dbContext.DocumentEventsSet.Attach(paperEventDb);
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
        }

        public void CancelPlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var paper in papers)
                    {
                        var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                        dbContext.DocumentEventsSet.Attach(paperEventDb);
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
                        dbContext.DocumentPapersSet.Attach(paperDb);
                        var entryP = dbContext.Entry(paperDb);
                        entryP.Property(e => e.LastPaperEventId).IsModified = true;
                        entryP.Property(e => e.LastChangeUserId).IsModified = true;
                        entryP.Property(e => e.LastChangeDate).IsModified = true;
                        dbContext.SaveChanges();
                    }
                    transaction.Complete();
                }
            }
        }

        public IEnumerable<InternalDocumentPaper> PlanDocumentPaperFromSendListPrepare(IContext context, int idSendList)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.SendListId == idSendList && x.PaperPlanDate == null && x.PaperSendDate == null && x.PaperRecieveDate == null)
                    .Select(x => new InternalDocumentPaper
                    {
                        Id = x.Paper.Id,
                        DocumentId = x.Paper.DocumentId,
                        IsInWork = x.Paper.IsInWork,
                        LastPaperEventId = x.Paper.LastPaperEventId,
                        LastPaperEvent = !x.Paper.LastPaperEventId.HasValue
                            ? null
                            : new InternalDocumentEvent
                            {
                                Id = x.Paper.LastPaperEvent.Id,
                                SourcePositionId = x.Paper.LastPaperEvent.SourcePositionId,
                                TargetPositionId = x.Paper.LastPaperEvent.TargetPositionId,
                                PaperPlanDate = x.Paper.LastPaperEvent.PaperPlanDate,
                                PaperSendDate = x.Paper.LastPaperEvent.PaperSendDate,
                                PaperRecieveDate = x.Paper.LastPaperEvent.PaperRecieveDate,
                            },
                        NextPaperEventId = x.Id,
                    }).ToList();

            }
        }

        public InternalDocument PlanDocumentPaperEventPrepare(IContext context, List<int> paperIds)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = new InternalDocument();

                var filterContains = PredicateBuilder.False<DocumentPapers>();
                filterContains = paperIds.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                doc.Papers = dbContext.DocumentPapersSet
                    .Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(filterContains)
                    .Select(x => new InternalDocumentPaper
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        IsInWork = x.IsInWork,
                        LastPaperEventId = x.LastPaperEventId,
                        LastPaperEvent = !x.LastPaperEventId.HasValue
                            ? null
                            : new InternalDocumentEvent
                            {
                                Id = x.LastPaperEvent.Id,
                                SourcePositionId = x.LastPaperEvent.SourcePositionId,
                                TargetPositionId = x.LastPaperEvent.TargetPositionId,
                                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,
                            }
                    }).ToList();
                if (!doc.Papers.Any()) return null;
                doc.Id = doc.Papers.First().Id;
                return doc;
            }
        }

        public void PlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
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
                            dbContext.DocumentPapersSet.Attach(paperDb);
                            var entry = dbContext.Entry(paperDb);
                            entry.Property(e => e.LastPaperEventId).IsModified = true;
                            entry.Property(e => e.LastChangeUserId).IsModified = true;
                            entry.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    transaction.Complete();
                }
            }
        }

        public InternalDocumentPaperList AddDocumentPaperListsPrepare(IContext context, AddDocumentPaperLists model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var sourcePositions = (model.SourcePositionIds == null || !model.SourcePositionIds.Any())
                    ? context.CurrentPositionsIdList
                    : context.CurrentPositionsIdList.Where(x => model.SourcePositionIds.Contains(x)).ToList();

                var qry = dbContext.DocumentEventsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                            .Where(x =>
                                x.PaperPlanDate.HasValue
                                && !x.PaperSendDate.HasValue
                                && !x.PaperRecieveDate.HasValue
                                && !x.PaperListId.HasValue
                                && x.EventTypeId == (int)EnumEventTypes.MoveDocumentPaper
                        ).AsQueryable();

                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = sourcePositions.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (model.TargetPositionIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = model.TargetPositionIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (model.TargetAgentIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = model.TargetAgentIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                var list = new InternalDocumentPaperList
                {
                    Events = qry.Select(x => new InternalDocumentEvent
                    {
                        Id = x.Id,
                        SourcePositionId = x.SourcePositionId,
                        TargetPositionId = x.TargetPositionId,
                        TargetAgentId = x.TargetAgentId,
                    }
                        )
                        .ToList()
                };
                return list;
            }
        }

        public List<int> AddDocumentPaperLists(IContext context, IEnumerable<InternalDocumentPaperList> items)
        {
            List<int> res = new List<int>();
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var item in items)
                    {
                        var itemDb = ModelConverter.GetDbDocumentPaperList(item);
                        itemDb.ClientId = context.CurrentClientId;
                        dbContext.DocumentPaperListsSet.Add(itemDb);
                        dbContext.SaveChanges();
                        res.Add(itemDb.Id);
                        var eventList = item.Events.Select(x => x.Id).ToList();

                        var filterContains = PredicateBuilder.False<DocumentEvents>();
                        filterContains = item.Events.Select(x => x.Id).ToList().Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        dbContext.DocumentEventsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
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
            }
            return res;
        }

        public InternalDocumentPaperList DeleteDocumentPaperListPrepare(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                return new InternalDocumentPaperList
                {
                    Id = itemId,
                    Events = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Where(x =>
                                    x.PaperListId.HasValue
                                    && itemId == x.PaperListId.Value
                                    && x.EventTypeId == (int)EnumEventTypes.MoveDocumentPaper
                                    )
                                .Where(filterContains)
                                .Select(x => new InternalDocumentEvent
                                {
                                    Id = x.Id,
                                    PaperListId = x.PaperListId,
                                    SourcePositionId = x.SourcePositionId,
                                    PaperPlanDate = x.PaperPlanDate,
                                    PaperSendDate = x.PaperSendDate,
                                    PaperRecieveDate = x.PaperRecieveDate,
                                }).ToList()
                };
            }
        }

        public InternalDocumentPaperList ModifyDocumentPaperListPrepare(IContext context, int itemId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var list = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == itemId)
                    .Select(x => new InternalDocumentPaperList
                    {
                        Id = x.Id,
                    }
                    ).FirstOrDefault();
                if (list == null) return null;
                list.SourcePositionId = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.PaperListId.HasValue && itemId == x.PaperListId.Value).SourcePositionId;
                return list;
            }
        }

        public void ModifyDocumentPaperList(IContext context, InternalDocumentPaperList item)
        {
            using (var dbContext = new DmsContext(context))
            {
                var itemDb = ModelConverter.GetDbDocumentPaperList(item);
                dbContext.DocumentPaperListsSet.Attach(itemDb);
                var entry = dbContext.Entry(itemDb);
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDocumentPaperList(IContext context, InternalDocumentPaperList item)
        {
            using (var dbContext = new DmsContext(context))
            {
                dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => item.Id == x.PaperListId).ToList()
                    .ForEach(x =>
                    {
                        x.PaperListId = null;
                        x.LastChangeUserId = item.LastChangeUserId;
                        x.LastChangeDate = item.LastChangeDate;
                    });
                dbContext.DocumentPaperListsSet.RemoveRange(dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == item.Id));
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentPapers
    }
}