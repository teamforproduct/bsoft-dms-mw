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
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.SystemCore;
using DocumentAccesses = BL.Database.DBModel.Document.DocumentAccesses;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        public DocumentOperationsDbProcess()
        {
        }

        public DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();

                res.Document = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        IsRegistered = x.Doc.IsRegistered,
                        IsLaunchPlan = x.Doc.IsLaunchPlan,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = CommonQueries.GetDocumentAccessesesQry(dbContext, res.Document.Id)
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


                    res.Document.Events = dbContext.DocumentEventsSet.Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                        }
                        ).ToList();

                    res.Document.Waits = dbContext.DocumentWaitsSet.Where(x => x.DocumentId == documentId)
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
                        dbContext.DocumentSubscriptionsSet.Where(x => x.DocumentId == documentId)
                            .Select(x => new InternalDocumentSubscription
                            {
                                Id = x.Id,
                            }
                            ).ToList();
                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentSendList
                        {
                            Id = x.Id,
                        }
                        ).ToList();



                    var posAcc = res.Document?.Accesses.Select(y => y.PositionId).ToList();

                    if (posAcc.Any())
                    {
                        res.PositionWithActions = dbContext.DictionaryPositionsSet
                            .Where(
                                x =>
                                    context.CurrentPositionsIdList.Contains(x.Id) &&
                                    posAcc.Contains(x.Id))
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

                }
                foreach (int posId in context.CurrentPositionsIdList)
                {
                    var qry = dbContext.SystemActionsSet.Where(x => x.ObjectId == (int)EnumObjects.Documents
                    && x.IsVisible &&
                    (!x.IsGrantable ||
                        x.RoleActions.Any(y => y.Role.PositionRoles.Any(pr => pr.PositionId == posId) && y.Role.UserRoles.Any(z => z.UserId == context.CurrentAgentId)))
                    );

                    var actLst = qry.Select(a => new InternalSystemAction
                    {
                        DocumentAction = (EnumDocumentActions)a.Id,
                        Object = (EnumObjects)a.ObjectId,
                        ActionCode = a.Code,
                        ObjectCode = a.Object.Code,
                        API = a.API,
                        Description = a.Description,
                    }).ToList();
                    res.ActionsList.Add(posId, actLst);

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
                    .Where(x => x.Doc.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();

                if (res.Document != null)
                {

                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                        }
                        ).ToList();

                    res.Document.SendLists = dbContext.DocumentSendListsSet.Where(x => x.DocumentId == documentId)
                         .Select(x => new InternalDocumentSendList
                         {
                             Id = x.Id,
                             SourcePositionId = x.SourcePositionId,
                             StartEventId = x.StartEventId,
                             CloseEventId = x.CloseEventId,
                         }
                         ).ToList();


                    var posAcc = res.Document?.Accesses.Select(y => y.PositionId).ToList();

                    if (posAcc.Any())
                    {
                        res.PositionWithActions = dbContext.DictionaryPositionsSet
                            .Where(
                                x =>
                                    context.CurrentPositionsIdList.Contains(x.Id) &&
                                    posAcc.Contains(x.Id))
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
                }

                foreach (int posId in context.CurrentPositionsIdList)
                {
                    var qry = dbContext.SystemActionsSet.Where(x => x.ObjectId == (int)EnumObjects.DocumentSendLists
                    && x.IsVisible &&
                    (!x.IsGrantable ||
                        x.RoleActions.Any(y => y.Role.PositionRoles.Any(pr => pr.PositionId == posId) && y.Role.UserRoles.Any(z => z.UserId == context.CurrentAgentId)))
                    );

                    var actLst = qry.Select(a => new InternalSystemAction
                    {
                        DocumentAction = (EnumDocumentActions)a.Id,
                        Object = (EnumObjects)a.ObjectId,
                        ActionCode = a.Code,
                        ObjectCode = a.Object.Code,
                        API = a.API,
                        Description = a.Description,
                    }).ToList();
                    res.ActionsList.Add(posId, actLst);

                }
                return res;
            }
        }


        #region DocumentLink    
        public InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                        LinkTypeId = model.LinkTypeId,
                    }).FirstOrDefault();

                if (doc == null) return null;

                var par = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == model.ParentDocumentId)
                    .Select(x => new { Id = x.Doc.Id, LinkId = x.Doc.LinkId }).FirstOrDefault();

                if (par == null) return null;

                doc.ParentDocumentId = par.Id;
                doc.ParentDocumentLinkId = par.LinkId;

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
                    dbContext.DocumentsSet.Where(x => x.Id == model.ParentDocumentId).ToList()  //TODO OPTIMIZE
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                if (!model.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.Id).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.LinkId == model.LinkId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentLink         

        public void AddDocumentWaits(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (document.Tasks?.Any() ?? false)
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
                    sendListDb.CloseEvent = offEvent;
                    var entry = dbContext.Entry(sendListDb);
                    entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }

                var subscription = document.Subscriptions?.FirstOrDefault();
                if (subscription != null)
                {
                    var subscriptionDb = new DocumentSubscriptions
                    {
                        Id = subscription.Id,
                        Description = subscription.Description,
                        SubscriptionStateId = (int)subscription.SubscriptionStates,
                        Hash = subscription.Hash,
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
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ControlChangeDocumentPrepare(IContext context, int eventId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentWaitsSet
                    .Where(x => x.OnEventId == eventId && context.CurrentPositionsIdList.Contains(x.OnEvent.SourcePositionId.Value))
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
                                            DueDate = x.DueDate,
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

        public InternalDocument ControlOffDocumentPrepare(IContext context, int eventId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentWaitsSet
                    .Where(x => x.OnEventId == eventId && context.CurrentPositionsIdList.Contains(x.OnEvent.SourcePositionId.Value))
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
                var eventsId = document.Waits.Select(x => x.OnEventId).ToList();

                document.SendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.StartEventId.HasValue && !x.CloseEventId.HasValue && eventsId.Contains(x.StartEventId.Value))
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                    }
                    ).ToList();
            }
        }

        public void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context))
            {
                var waitsId = document.Waits.Select(x => x.Id).ToList();

                var waitRes = dbContext.DocumentWaitsSet
                    .Where(x => x.ParentId.HasValue && !x.OffEventId.HasValue && waitsId.Contains(x.ParentId.Value) && x.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution)
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
                var eventsId = document.Waits.Select(x => x.OnEventId).ToList();

                document.Subscriptions = dbContext.DocumentSubscriptionsSet
                    .Where(x => !x.DoneEventId.HasValue && eventsId.Contains(x.SendEventId))
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
                    transaction.Complete();
                }
            }
        }

        public FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentEventsQuery(ctx, dbContext).Where(x => x.Id == eventId)
                    .Select(x => new FrontDocumentEvent
                    {
                        Id = x.Id,
                        DocumentDescription = x.Document.Description,
                        DocumentTypeName = x.Document.TemplateDocument.DocumentType.Name,
                        DocumentDirectionName = x.Document.TemplateDocument.DocumentDirection.Name,
                        ReadAgentName = x.ReadAgent.Name,
                        ReadDate = x.ReadDate,
                        SourceAgentName = x.SourceAgent.Name,
                        SourcePositionName = x.SourcePosition.Name,
                        TargetPositionName = x.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.SourcePosition.ExecutorAgent.Name,
                        TargetPositionExecutorNowAgentName = x.TargetPosition.ExecutorAgent.Name,
                        SourcePositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "TargetPositionAgentPhoneNumber", //TODO 
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetDocumentEventsQuery(ctx, dbContext);

                #region filters
                if (filter != null)
                {
                    if (filter.EventId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.EventId.Contains(x.Id));
                    }

                    if (filter.DocumentId.HasValue)
                    {
                        qry = qry.Where(x => filter.DocumentId.Value == x.DocumentId);
                    }

                    if (filter.ListDocumentId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.ListDocumentId.Contains(x.DocumentId));
                    }

                    if (!String.IsNullOrEmpty(filter.Description))
                    {
                        qry = qry.Where(x => x.Description.Contains(filter.Description));
                    }

                    if (filter.EventType?.Count > 0)
                    {
                        qry = qry.Where(x => filter.EventType.Cast<int>().Contains(x.EventTypeId));
                    }

                    if (filter.Importance?.Count > 0)
                    {
                        qry = qry.Where(x => filter.Importance.Cast<int>().Contains(x.EventType.ImportanceEventTypeId));
                    }

                    if (filter.AgentId?.Count > 0)
                    {
                        qry =
                            qry.Where(
                                x =>
                                    (x.TargetAgentId.HasValue && filter.AgentId.Contains(x.TargetAgentId.Value)) || filter.AgentId.Contains(x.SourceAgentId) ||
                                    (x.ReadAgentId.HasValue && filter.AgentId.Contains(x.ReadAgentId.Value))
                                    || (x.SourcePositionExecutorAgentId.HasValue && filter.AgentId.Contains(x.SourcePositionExecutorAgentId.Value))
                                    || (x.TargetPositionExecutorAgentId.HasValue && filter.AgentId.Contains(x.TargetPositionExecutorAgentId.Value)));
                    }

                    if (filter.PositionId?.Count > 0)
                    {
                        qry =
                            qry.Where(
                                x =>
                                    (x.SourcePositionId.HasValue && filter.PositionId.Contains(x.SourcePositionId.Value)) ||
                                    (x.TargetPositionId.HasValue && filter.PositionId.Contains(x.TargetPositionId.Value)));
                    }
                }
                #endregion

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderByDescending(x => x.Id)
                            .Skip(paging.PageSize * (paging.CurrentPage - 1))
                            .Take(paging.PageSize);
                }

                var ddate = dbContext.DocumentWaitsSet.Join(qry, w => w.OnEventId, e => e.Id, (w, e) => new { wt = w })
                    .Where(x => !x.wt.OffEventId.HasValue)
                    .Select(x => new { evtId = x.wt.OnEventId, x.wt.DueDate }).ToList();

                var res = qry.Select(x => new FrontDocumentEvent
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    EventTypeName = x.EventType.Name,
                    Date = x.Date,
                    Task = x.Task.Task,
                    Description = x.Description,

                    SourcePositionExecutorAgentName = x.SourcePositionExecutorAgent.Name,
                    TargetPositionExecutorAgentName = x.TargetPositionExecutorAgent.Name ?? x.TargetAgent.Name,
                    DocumentDate = x.Document.RegistrationDate ?? x.Document.CreateDate,
                    RegistrationFullNumber = (x.Document.RegistrationNumber != null
                                          ? x.Document.RegistrationNumberPrefix + x.Document.RegistrationNumber +
                                            x.Document.RegistrationNumberSuffix
                                          : "#" + x.Document.Id),
                    DueDate = null,
                }).ToList();

                foreach (var el in res.Join(ddate, r => r.Id, d => d.evtId, (r, d) => new { r, d }))
                {
                    el.r.DueDate = el.d.DueDate;
                }

                return res;
            }
        }

        public InternalDocument MarkDocumentEventsAsReadPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var res = new InternalDocument { Id = documentId };
                var qry = CommonQueries.GetDocumentEventsQuery(ctx, dbContext).Where(x => x.DocumentId == documentId
                && !x.ReadDate.HasValue
                && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId
                && ctx.CurrentPositionsIdList.Contains(x.TargetPositionId.Value));

                res.Events = qry.Select(x => new InternalDocumentEvent
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
                return CommonQueries.GetInternalDocumentAccesses(dbContext, documentId);
            }
        }

        public IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetInternalPositionsInfo(dbContext, positionIds);
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
                var doc = dbContext.DocumentAccessesSet
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
                var acc = dbContext.DocumentAccessesSet
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
                    entry.Property(x => x.Id).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;

                    dbContext.DocumentAccessesSet.AddRange(
                        CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, document.Id).ToList());
                    dbContext.SaveChanges();

                    if (document.Waits?.Any() ?? false)
                    {
                        var wait = document.Waits.First();
                        var waitDb = ModelConverter.GetDbDocumentWait(wait);
                        if (wait.OnEvent == sendList.StartEvent)
                        {
                            waitDb.OnEventId = sendListDb.StartEventId.Value;
                            waitDb.OnEvent = null;
                        }
                        dbContext.DocumentWaitsSet.Add(waitDb);
                    }
                    dbContext.SaveChanges();

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
                    }
                    dbContext.SaveChanges();

                    var eventsDb = ModelConverter.GetDbDocumentEvents(document.Events);
                    if (document.Events?.Any() ?? false)
                    {
                        //dbContext.DocumentEventsSet.AddRange(eventsDb);
                        dbContext.DocumentEventsSet.Attach(eventsDb.First());
                        dbContext.Entry(eventsDb.First()).State = EntityState.Added;
                    }
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public void ModifyDocumentTags(IContext context, InternalDocumentTag model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dictionaryTags = dbContext.DictionaryTagsSet
                    .Where(x => !x.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.PositionId ?? 0))
                    .Where(x => model.Tags.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToList();

                var documentTags = dbContext.DocumentTagsSet
                    .Where(x => x.DocumentId == model.DocumentId)
                    .Where(x => !x.Tag.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
                    .Select(x => x.TagId)
                    .ToList();

                //Удаляем теги которые не присутствуют в списке
                dbContext.DocumentTagsSet
                    .RemoveRange(dbContext.DocumentTagsSet
                        .Where(x => x.DocumentId == model.DocumentId
                            && documentTags.Where(y => !dictionaryTags.Contains(y)).Contains(x.TagId)));

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

        public InternalDocument AddNoteDocumentPrepare(IContext context, AddNote model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == model.DocumentId /*&& context.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId)*/)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet
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
                    .Where(x => x.Doc.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Waits = dbContext.DocumentWaitsSet
                    .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.Task.Id == sendList.TaskId && x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                    .Select(x => new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                                Id = x.Id,
                                                OnEvent = new InternalDocumentEvent
                                                {
                                                    TargetPositionId = x.OnEvent.TargetPositionId
                                                }
                                        }
                                    }
                    ).FirstOrDefault();
                return doc;

            }
        }

        public InternalDocument SendForSigningDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id
                    }).FirstOrDefault();
                if (doc == null) return null;
                /*
                doc.Waits = dbContext.DocumentWaitsSet
                    .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.TaskName == sendList.TaskName && x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                    .Select(x => new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                                Id = x.Id,
                                                OnEvent = new InternalDocumentEvent
                                                {
                                                    TargetPositionId = x.OnEvent.TargetPositionId
                                                }
                                        }
                                    }
                    ).FirstOrDefault();
                    */
                return doc;

            }
        }

        #region DocumentSendList    
        public InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId, string task = null)
        {
            using (var dbContext = new DmsContext(context))
            {
                var docDb = from doc in dbContext.DocumentsSet.Where(x => x.Id == documentId)
                            join tmp in dbContext.TemplateDocumentsSet on doc.TemplateDocumentId equals tmp.Id
                            select new { doc, tmp };

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,
                    TemplateDocumentId = x.tmp.Id,
                    IsHard = x.tmp.IsHard,
                    IsLaunchPlan = x.doc.IsLaunchPlan
                }).FirstOrDefault();

                if (docRes == null) return docRes;
                docRes.Tasks = dbContext.DocumentTasksSet
                        .Where(x => !string.IsNullOrEmpty(task) && x.DocumentId == documentId && x.Task == task)
                        .Select(x => new List<InternalDocumentTask>
                        {
                                                            new InternalDocumentTask
                                                            {
                                                                    Id = x.Id,
                                                            }
                        }).FirstOrDefault();
                docRes.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == docRes.Id)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId
                    }).ToList();


                docRes.SendLists = dbContext.DocumentSendListsSet.Where(x => x.DocumentId == docRes.Id)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        TargetPositionId = x.TargetPositionId,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        Stage = x.Stage,
                        SourcePositionId = x.SourcePositionId

                    }).ToList();

                if (docRes.IsHard)
                {
                    docRes.TemplateDocument = new InternalTemplateDocument();

                    docRes.TemplateDocument.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet
                        .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                        .Select(x => new InternalTemplateDocumentRestrictedSendList
                        {
                            Id = x.Id,
                            PositionId = x.PositionId
                        }).ToList();

                    docRes.TemplateDocument.SendLists = dbContext.TemplateDocumentSendListsSet
                        .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                        .Select(x => new InternalTemplateDocumentSendList
                        {
                            Id = x.Id,
                            TargetPositionId = x.TargetPositionId,
                            SendType = (EnumSendTypes)x.SendTypeId
                        }).ToList();
                }

                return docRes;
            }
        }

        public void AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model)
        {
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
            }
        }

        public IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendListId == model.StandartSendListId)
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

                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Id == restSendListId)
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
                var item = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restSendListId);
                if (item != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public void AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> sendList, IEnumerable<InternalDocumentTask> task = null)
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
                        ((List<InternalDocumentSendList>)sendList).ForEach(x => x.TaskId = taskDb.Id);
                    }

                    if (sendList?.Any() ?? false)
                    {
                        var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendList);
                        dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                        dbContext.SaveChanges();
                    }

                    transaction.Complete();
                }
            }
        }

        public IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            //TODO DELETE!!!!
            using (var dbContext = new DmsContext(context))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendListId == model.StandartSendListId)
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

        public void ModifyDocumentSendList(IContext context, InternalDocumentSendList sendList, IEnumerable<InternalDocumentTask> task = null)
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
                    var item = new DocumentSendLists
                    {
                        Id = sendList.Id,
                        Stage = sendList.Stage,
                        SendTypeId = (int)sendList.SendType,
                        TargetPositionId = sendList.TargetPositionId,
                        TargetPositionExecutorAgentId = sendList.TargetPositionExecutorAgentId,
                        TargetAgentId = sendList.TargetAgentId,
                        TaskId = sendList.TaskId,
                        IsAvailableWithinTask = sendList.IsAvailableWithinTask,
                        IsAddControl = sendList.IsAddControl,
                        IsInitial = sendList.IsInitial,
                        Description = sendList.Description,
                        DueDate = sendList.DueDate,
                        DueDay = sendList.DueDay,
                        AccessLevelId = (int)sendList.AccessLevel,
                        LastChangeUserId = sendList.LastChangeUserId,
                        LastChangeDate = sendList.LastChangeDate
                    };
                    dbContext.DocumentSendListsSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(e => e.Stage).IsModified = true;
                    entry.Property(e => e.SendTypeId).IsModified = true;
                    entry.Property(e => e.TargetPositionId).IsModified = true;
                    entry.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                    entry.Property(e => e.TargetAgentId).IsModified = true;
                    entry.Property(e => e.TaskId).IsModified = true;
                    entry.Property(e => e.IsAvailableWithinTask).IsModified = true;
                    entry.Property(e => e.IsAddControl).IsModified = true;
                    entry.Property(e => e.IsInitial).IsModified = true;
                    entry.Property(e => e.Description).IsModified = true;
                    entry.Property(e => e.DueDate).IsModified = true;
                    entry.Property(e => e.DueDay).IsModified = true;
                    entry.Property(e => e.AccessLevelId).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public InternalDocument DeleteDocumentSendListPrepare(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = dbContext.DocumentSendListsSet
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
                var item = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendListId);
                if (item != null)
                {
                    dbContext.DocumentSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var docDb = (from doc in dbContext.DocumentsSet.Where(x => x.Id == documentId)
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
                var doc = dbContext.DocumentSendListsSet
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
                                            IsAddControl = x.IsAddControl,
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
                var item = dbContext.DocumentSavedFiltersSet.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion DocumentSavedFilter
    }
}