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
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.Helpers;
using EntityFramework.Extensions;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        #region DocumentAction

        public DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int? documentId, int? id = null)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();
                var qry = CommonQueries.GetDocumentQuery(dbContext, context);
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
                    IsRegistered = x.IsRegistered,
                    IsLaunchPlan = x.IsLaunchPlan,
                    ExecutorPositionId = x.ExecutorPositionId,
                    LinkId = x.LinkId,
                    AccessesCount = x.Accesses.Count(),
                }).FirstOrDefault();

                if (res.Document != null)
                {
                    documentId = res.Document.Id;
                    res.Document.Accesses = CommonQueries.GetDocumentAccessesesQry(dbContext, res.Document.Id, context, true)
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
                    var qryEvents = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId);
                    if (id.HasValue)
                    {
                        qryEvents = qryEvents.Where(x => x.Id == id);
                    }
                    else
                    {
                        qryEvents = qryEvents.Where(x => x.DocumentId == documentId);
                    }

                    res.Document.Events = qryEvents.Select(x => new InternalDocumentEvent
                    {
                        Id = x.Id,
                    }
                        ).ToList();

                    var qryWaits = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId);
                    if (id.HasValue)
                    {
                        qryWaits = qryWaits.Where(x => x.OnEventId == id);
                    }
                    else
                    {
                        qryWaits = qryWaits.Where(x => x.DocumentId == documentId);
                    }
                    res.Document.Waits = qryWaits.Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        OffEventId = x.OffEventId,
                        ParentId = x.ParentId,
                        IsHasMarkExecution = x.ChildWaits.Any(y => y.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution && !y.OffEventId.HasValue),
                        IsHasAskPostponeDueDate = x.ChildWaits.Any(y => y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate && !y.OffEventId.HasValue),
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
                                SubscriptionStatesId = x.SubscriptionStateId,
                                SubscriptionStatesIsSuccess = x.SubscriptionState.IsSuccess,
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
                transaction.Complete();
                return res;
            }
        }

        public DocumentActionsModel GetDocumentSendListActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

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

                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
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
                transaction.Complete();
                return res;
            }
        }

        public DocumentActionsModel GetDocumentFileActionsModelPrepare(IContext context, int? documentId, int? id = null)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

                var qry = CommonQueries.GetDocumentQuery(dbContext, context);

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
                    IsRegistered = x.IsRegistered,
                    ExecutorPositionId = x.ExecutorPositionId,
                    LinkId = x.LinkId,
                }).FirstOrDefault();

                if (res.Document != null)
                {
                    documentId = res.Document.Id;
                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
                        }
                        ).ToList();
                    var qryFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId);
                    if (id.HasValue)
                    {
                        qryFiles = qryFiles.Where(x => x.Id == id);
                    }
                    else
                    {
                        qryFiles = qryFiles.Where(x => x.DocumentId == documentId);
                    }
                    res.Document.DocumentFiles = qryFiles.Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        Type = (EnumFileTypes)x.TypeId,
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
                transaction.Complete();
                return res;
            }
        }

        public DocumentActionsModel GetDocumentPaperActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemActionForDocument>>();

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
                    res.Document.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.IsInWork)
                        .Select(x => new InternalDocumentAccess
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            IsInWork = x.IsInWork,
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
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentAction   

        #region DocumentMainLogic  

        public void AddDocumentWaits(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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

        public void ChangeDocumentWait(IContext ctx, InternalDocumentWait wait)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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

                if (wait.AskPostponeDueDateWait != null)
                {
                    var askWaitDb = ModelConverter.GetDbDocumentWait(wait.AskPostponeDueDateWait);
                    askWaitDb.ParentId = waitParentDb.Id;
                    askWaitDb.OnEvent = null;
                    dbContext.DocumentWaitsSet.Attach(askWaitDb);
                    var entry = dbContext.Entry(askWaitDb);
                    entry.Property(x => x.ResultTypeId).IsModified = true;
                    entry.Property(x => x.ParentId).IsModified = true;
                    entry.Property(x => x.OffEventId).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    dbContext.SaveChanges();
                }

                transaction.Complete();

            }
        }

        public void ChangeTargetDocumentWait(IContext ctx, InternalDocumentWait wait, InternalDocumentEvent newEvent)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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

        public void CloseDocumentWait(IContext ctx, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
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
                                                                    isUseInternalSign, isUseCertificateSign, subscription,
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
                    transaction.Complete();
                }
            }
        }

        public void VerifySigningDocument(IContext ctx, int documentId, bool isUseInternalSign, bool isUseCertificateSign)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var docHash = CommonQueries.GetDocumentHash(dbContext, ctx, documentId, isUseInternalSign, isUseCertificateSign, null, false, true);
                transaction.Complete();
            }
        }

        public void SelfAffixSigningDocument(IContext ctx, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var eventDb = ModelConverter.GetDbDocumentEvent(document.Events.First());

                var subscription = document.Subscriptions.First();

                var docHash = CommonQueries.GetDocumentHash(dbContext, ctx, document.Id,
                                                            isUseInternalSign, isUseCertificateSign, subscription,
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

                transaction.Complete();

            }
        }

        public InternalDocument ControlChangeDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var maxDateTime = DateTime.UtcNow.AddYears(50);

                var doc = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.OnEventId == eventId && x.OnEvent.EventTypeId != (int)EnumEventTypes.AskPostponeDueDate)
                    .Concat(dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).
                            Where(x => x.Id == dbContext.DocumentWaitsSet.Where(y => y.OnEventId == eventId && y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate).Select(y => y.ParentId).FirstOrDefault())
                            )
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
                                            IsHasAskPostponeDueDate = x.ChildWaits.Any(y=>y.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate && !y.OffEventId.HasValue),
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                DocumentId = x.OnEvent.DocumentId,
                                                SourcePositionId = x.OnEvent.SourcePositionId,
                                                SourcePositionExecutorAgentId = x.OnEvent.SourcePositionExecutorAgentId,
                                                SourcePositionExecutorTypeId = x.OnEvent.SourcePositionExecutorTypeId,
                                                TargetPositionId = x.OnEvent.TargetPositionId,
                                                TargetPositionExecutorAgentId = x.OnEvent.TargetPositionExecutorAgentId,
                                                TargetPositionExecutorTypeId = x.OnEvent.TargetPositionExecutorTypeId,
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
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument ControlTargetChangeDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument ControlOffDocumentPrepare(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.OnEventId == eventId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        IsLaunchPlan = x.Document.IsLaunchPlan,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
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
                transaction.Complete();
                return doc;

            }
        }

        public InternalDocument SelfAffixSigningDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx, null, false, true, true);
                var doc = qry.Where(x => x.Id == documentId)
                            .Select(x => new InternalDocument
                            {
                                Id = x.Id,
                                ExecutorPositionId = x.ExecutorPositionId
                            }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public void ControlOffSendListPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }


        public void ControlOffAskPostponeDueDateWaitPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var filterContains = PredicateBuilder.False<DocumentWaits>();
                filterContains = document.Waits.Select(x => x.Id).ToList().Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ParentId == value).Expand());

                var waitRes = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(filterContains)
                    .Where(x => !x.OffEventId.HasValue && x.OnEvent.EventTypeId == (int)EnumEventTypes.AskPostponeDueDate)
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
                transaction.Complete();
            }
        }
        public void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void ControlOffSubscriptionPrepare(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void AddDocumentEvents(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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

        public FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentEventQuery(ctx, dbContext, new FilterDocumentEvent { EventId = new List<int> { eventId } })
                    .Select(x => new FrontDocumentEvent
                    {
                        Id = x.Id,
                        DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                        DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                        DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,
                        ReadAgentId = x.ReadAgentId,
                        ReadAgentName = x.ReadAgent.Name,
                        ReadDate = x.ReadDate,
                        SourceAgentId = x.SourceAgentId,
                        SourceAgentName = x.SourceAgent.Name,
                        TargetAgentId = x.TargetAgentId,
                        TargetAgentName = x.TargetAgent.Name,
                        TargetPositionId = x.TargetPositionId,
                        SourcePositionId = x.SourcePositionId,
                        SourcePositionExecutorAgentId = x.SourcePositionExecutorAgentId,
                        TargetPositionExecutorAgentId = x.TargetPositionExecutorAgentId,
                        SourcePositionName = x.SourcePosition.Name,
                        TargetPositionName = x.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.SourcePosition.ExecutorAgent.Name + (x.SourcePosition.ExecutorType.Suffix != null ? " (" + x.SourcePosition.ExecutorType.Suffix + ")" : null),
                        TargetPositionExecutorNowAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        IsAvailableWithinTask = x.IsAvailableWithinTask,
                        PaperPlanAgentName = x.PaperPlanAgent.Name,
                        PaperSendAgentName = x.PaperSendAgent.Name,
                        PaperRecieveAgentName = x.PaperRecieveAgent.Name,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterBase filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentEvents(ctx, dbContext, filter, paging).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterBase filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentWaits(dbContext, filter, ctx, paging).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentSubscriptions(dbContext, filter, ctx, paging).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(IContext ctx, FilterDictionaryPosition filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentWorkGroup(dbContext, ctx, filter).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDocumentEvent> MarkDocumentEventsAsReadPrepare(IContext ctx, MarkDocumentEventAsRead model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                if (model.EventIds == null)
                    return new List<InternalDocumentEvent>();

                var qry = CommonQueries.GetDocumentEventQuery(ctx, dbContext, new FilterDocumentEvent { IsNew = true, EventId = model.EventIds });

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
                transaction.Complete();
                return res;
            }
        }

        public void MarkDocumentEventAsRead(IContext ctx, IEnumerable<InternalDocumentEvent> eventList)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetInternalDocumentAccesses(dbContext, ctx, documentId);
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetInternalPositionsInfo(dbContext, ctx, positionIds);
                transaction.Complete();
                return res;
            }
        }

        public void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                                            IsInWork = x.IsInWork,
                                        }
                                    }

                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }

        public void ChangeIsInWorkAccess(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return acc;

            }
        }

        public void SendBySendList(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var sendList = document.SendLists.First();
                var sendListDb = new DocumentSendLists
                {
                    Id = sendList.Id,
                    LastChangeDate = sendList.LastChangeDate,
                    LastChangeUserId = sendList.LastChangeUserId
                };
                var startEventDb = ModelConverter.GetDbDocumentEvent(sendList.StartEvent);

                if (sendList.Stage.HasValue)
                {
                    dbContext.DocumentSendListsSet.Attach(sendListDb);
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
                        entryEventDb.Property(e => e.SourcePositionExecutorTypeId).IsModified = true;
                        entryEventDb.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                        entryEventDb.Property(e => e.TargetPositionExecutorTypeId).IsModified = true;
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

        public void ModifyDocumentTags(IContext ctx, InternalDocumentTag model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public InternalDocument AddNoteDocumentPrepare(IContext ctx, AddNote model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, false, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        AccessesCount = x.Accesses.Count()
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
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, false, true, true)
                    .Where(x => x.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsLaunchPlan = x.IsLaunchPlan,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (sendList.SendType == EnumSendTypes.SendForResponsibleExecution || sendList.SendType == EnumSendTypes.SendForControl || sendList.IsWorkGroup)
                {
                    var initiatorInfo = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId)
                        .Where(x => x.Id == sendList.SourcePositionId)
                        .Select(x => new InternalDictionaryPositionWithActions
                                        {
                                            //Id = x.Id,
                                            Name = x.Name,
                                            //DepartmentId = x.DepartmentId,
                                            //ExecutorAgentId = x.ExecutorAgentId,
                                            //DepartmentName = x.Department.Name,
                                            ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                                        }).FirstOrDefault();
                    if (initiatorInfo == null) return null;
                    sendList.InitiatorPositionName = initiatorInfo.Name;
                    sendList.InitiatorPositionExecutorAgentName = initiatorInfo.ExecutorAgentName;
                    doc.Events = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.Task.Id == sendList.TaskId
                                    && x.EventTypeId == (int)EnumEventTypes.SendForControl
                                    && x.SourcePositionId == sendList.SourcePositionId)
                        .Select(x => new InternalDocumentEvent
                        {
                            Id = x.Id,
                            //ParentEventId = x.ParentEventId,
                            SourcePositionId = x.SourcePositionId,
                            //SourcePositionName = x.SourcePosition.Name,
                            //SourcePositionExecutorAgentName = x.SourcePosition.ExecutorAgent.Name + (x.SourcePosition.ExecutorType.Suffix != null ? " (" + x.SourcePosition.ExecutorType.Suffix + ")" : null),
                            TargetPositionId = x.TargetPositionId,
                            TargetPositionName = x.TargetPosition.Name,
                            TargetPositionExecutorAgentId = x.TargetPosition.ExecutorAgentId,
                            TargetPositionExecutorTypeId = x.TargetPosition.PositionExecutorTypeId,
                            TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                        }).ToList();

                    var qryWaits = doc.Waits = dbContext.DocumentWaitsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.Task.Id == sendList.TaskId && !x.OffEventId.HasValue
                                    && (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecutionChange))
                        .Select(x => new
                        {
                            waitRE = x,
                            eventC = x.OnEvent.ChildEvents.Where(y => y.ParentEventId == x.OnEvent.Id && y.EventTypeId == (int)EnumEventTypes.InfoSendForResponsibleExecutionReportingControler).FirstOrDefault()
                        }).Where(x => x.eventC == null && x.waitRE.OnEvent.SourcePositionId == sendList.SourcePositionId || x.eventC != null && x.eventC.SourcePositionId == sendList.SourcePositionId)
                        .Select(x => new InternalDocumentWait
                        {
                            Id = x.waitRE.Id,
                            OnEvent = new InternalDocumentEvent
                            {
                                SourcePositionId = x.eventC == null ? x.waitRE.OnEvent.SourcePositionId : x.eventC.SourcePositionId,
                                //SourcePositionName = x.eventC == null ? x.waitRE.OnEvent.SourcePosition.Name : x.eventC.SourcePosition.Name,
                                //SourcePositionExecutorAgentName = x.eventC == null
                                //    ? x.waitRE.OnEvent.SourcePosition.ExecutorAgent.Name + (x.waitRE.OnEvent.SourcePosition.ExecutorType.Suffix != null ? " (" + x.waitRE.OnEvent.SourcePosition.ExecutorType.Suffix + ")" : null)
                                //    : x.eventC.SourcePosition.ExecutorAgent.Name + (x.eventC.SourcePosition.ExecutorType.Suffix != null ? " (" + x.eventC.SourcePosition.ExecutorType.Suffix + ")" : null),
                                TargetPositionId = x.waitRE.OnEvent.TargetPositionId,
                                TargetPositionName = x.waitRE.OnEvent.TargetPosition.Name,
                                TargetPositionExecutorAgentId = x.waitRE.OnEvent.TargetPosition.ExecutorAgentId,
                                TargetPositionExecutorTypeId = x.waitRE.OnEvent.TargetPosition.PositionExecutorTypeId,
                                TargetPositionExecutorAgentName = x.waitRE.OnEvent.TargetPosition.ExecutorAgent.Name + (x.waitRE.OnEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.waitRE.OnEvent.TargetPosition.ExecutorType.Suffix + ")" : null),
                            }
                        });
                    doc.Waits = qryWaits.ToList();
                }
                if (sendList.IsInitial)
                {
                    doc.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.SubscriptionState.IsSuccess)
                        .Select(x => new InternalDocumentSubscription
                        {
                            Id = x.Id,
                            DoneEvent = new InternalDocumentEvent
                            {
                                SourcePositionId = x.DoneEvent.SourcePositionId,
                            }
                        }).ToList();
                }
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == sendList.DocumentId)
                    .GroupBy(x => x.PositionId)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        PositionId = x.Key
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument SendForInformationDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, false, true, true)
                    .Where(x => x.Id == sendList.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsLaunchPlan = x.IsLaunchPlan,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (sendList.IsInitial)
                {
                    doc.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.DocumentId == sendList.DocumentId && x.SubscriptionState.IsSuccess)
                        .Select(x => new InternalDocumentSubscription
                        {
                            Id = x.Id,
                            DoneEvent = new InternalDocumentEvent
                            {
                                SourcePositionId = x.DoneEvent.SourcePositionId,
                            }
                        }).ToList();
                }
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.DocumentId == sendList.DocumentId)
                    .GroupBy(x => x.PositionId)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        PositionId = x.Key
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        #endregion DocumentMainLogic 

        #region DocumentLink    

        public InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, false, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        //ExecutorPositionId = x.ExecutorPositionId,
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
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument DeleteDocumentLinkPrepare(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentLinksSet
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Document.Id,
                        LinkId = x.Document.LinkId,
                        Links = new List<InternalDocumentLink> { new InternalDocumentLink
                        {
                            Id = x.Id,
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
                        DocumentId = x.DocumentId,
                        ParentDocumentId = x.ParentDocumentId,
                    }).ToList();
                //var calc = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                if (model.Events != null && model.Events.Any(x => x.Id == 0))
                {
                    var eventsDb = ModelConverter.GetDbDocumentEvents(model.Events.Where(x => x.Id == 0).ToList());
                    dbContext.DocumentEventsSet.AddRange(eventsDb);
                }
                var link = new DocumentLinks
                {
                    DocumentId = model.Id,
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
                transaction.Complete();
            }
        }

        public void DeleteDocumentLink(IContext context, InternalDocument model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var id = model.Links.Select(y => y.Id).First();
                dbContext.DocumentLinksSet.RemoveRange(dbContext.DocumentLinksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id));

                if ((model.OldLinkSet?.Any() ?? false) && model.LinkId != model.OldLinkId)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = model.OldLinkSet.Aggregate(filterContains, (current, value) => current.Or(e => e.Id == value).Expand());
                    dbContext.DocumentsSet.Where(filterContains).Update(u => new DBModel.Document.Documents { LinkId = model.OldLinkId });
                }
                if ((model.NewLinkSet?.Any() ?? false) && model.LinkId != model.NewLinkId)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = model.NewLinkSet.Aggregate(filterContains, (current, value) => current.Or(e => e.Id == value).Expand());
                    dbContext.DocumentsSet.Where(filterContains).Update(u => new DBModel.Document.Documents { LinkId = model.NewLinkId });
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                        StageType = (EnumStageTypes?)x.StageTypeId,
                        Stage = x.Stage,
                        SourcePositionId = x.SourcePositionId,
                        SourceAgentId = x.SourceAgentId,
                        TargetAgentId = x.TargetAgentId

                    }).ToList();
                docRes.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == docRes.Id && x.SubscriptionState.IsSuccess)
                    .Select(x => new InternalDocumentSubscription
                    {
                        Id = x.Id,
                        SubscriptionStatesId = x.SubscriptionStateId,
                        SubscriptionStatesIsSuccess = x.SubscriptionState.IsSuccess,
                        DoneEvent = new InternalDocumentEvent
                        {
                            SourcePositionId = x.DoneEvent.SourcePositionId,
                        }
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
                transaction.Complete();
                return docRes;
            }
        }

        public IEnumerable<int> AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model)
        {
            List<int> res;
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
            return res;
        }

        public IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     DocumentId = model.DocumentId,
                     PositionId = x.TargetPositionId,
                     AccessLevel = (EnumDocumentAccesses)(x.AccessLevelId ?? (int)EnumDocumentAccesses.PersonalRefIO)
                 }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == restSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     Id = x.Id,
                     DocumentId = x.DocumentId
                 }).FirstOrDefault();
                transaction.Complete();
                return item;
            }
        }

        public void DeleteDocumentRestrictedSendList(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == restSendListId);
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                    var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendList.Where((x => x.Stage.HasValue))).ToList();
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
            return res;
        }

        public IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            //TODO DELETE!!!!
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentSendList
                 {
                     DocumentId = model.DocumentId,
                     Stage = x.Stage,
                     //StageType = (EnumStageTypes)x.StageTypeId,
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
                     LastChangeDate = DateTime.UtcNow,
                 }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public void ModifyDocumentSendListAddDescription(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var sendListDb = ModelConverter.GetDbDocumentSendList(sendList);
                dbContext.DocumentSendListsSet.Attach(sendListDb);
                var entry = dbContext.Entry(sendListDb);
                entry.Property(e => e.AddDescription).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void ModifyDocumentSendList(IContext context, InternalDocumentSendList sendList, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> addPaperEvents = null, IEnumerable<int?> delPaperEvents = null)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                entry.Property(e => e.StageTypeId).IsModified = true;
                entry.Property(e => e.SendTypeId).IsModified = true;
                entry.Property(e => e.SourcePositionExecutorAgentId).IsModified = true;
                entry.Property(e => e.SourcePositionExecutorTypeId).IsModified = true;
                entry.Property(e => e.TargetPositionId).IsModified = true;
                entry.Property(e => e.TargetPositionExecutorAgentId).IsModified = true;
                entry.Property(e => e.TargetPositionExecutorTypeId).IsModified = true;
                entry.Property(e => e.TargetAgentId).IsModified = true;
                entry.Property(e => e.TaskId).IsModified = true;
                entry.Property(e => e.IsAvailableWithinTask).IsModified = true;
                entry.Property(e => e.IsWorkGroup).IsModified = true;
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

        public InternalDocument DeleteDocumentSendListPrepare(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return doc;
            }
        }

        public void DeleteDocumentSendList(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == sendListId);
                if (item != null)
                {
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.SendListId == sendListId && x.PaperPlanDate == null));
                    dbContext.DocumentSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }

        public InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return docRes;
            }
        }

        public void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                    dbContext.DocumentSendListsSet.Attach(item);

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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                                            StageType = (EnumStageTypes?)x.StageTypeId,
                                            SendType = (EnumSendTypes)x.SendTypeId,
                                            SourcePositionId = x.SourcePositionId,
                                            SourceAgentId = x.SourceAgentId,
                                            TargetPositionId = x.TargetPositionId,
                                            TargetAgentId = x.TargetAgentId,
                                            TaskId = x.TaskId,
                                            IsAvailableWithinTask = x.IsAvailableWithinTask,
                                            IsWorkGroup = x.IsWorkGroup,
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
                                            AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                    }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;

            }
        }


        #endregion DocumentSendList     

        #region DocumentSavedFilter

        public List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var items = model.Select(x => new DocumentSavedFilters
                {
                    ClientId = context.CurrentClientId,
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                dbContext.DocumentSavedFiltersSet.Attach(item);

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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == taskId)
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
                transaction.Complete();
                return res;
            }
        }

        public void ModifyDocumentTask(IContext context, InternalDocument document)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var taskDb = ModelConverter.GetDbDocumentTask(document.Tasks.First());
                dbContext.DocumentTasksSet.Attach(taskDb);
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == itemId);
                if (item != null)
                {
                    dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == itemId));
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == itemId && x.EventTypeId != (int)EnumEventTypes.TaskFormulation));
                    dbContext.DocumentTasksSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }

        public InternalDocument ModifyDocumentTaskPrepare(IContext context, int? id, BaseModifyDocumentTasks model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, false, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => (x.Task == model.Name || x.Id == id) && x.DocumentId == model.DocumentId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => x.Id == paperId)
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
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument EventDocumentPaperPrepare(IContext context, PaperList filters, bool isCalcPreLastPaperEvent = false)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                                .Where(y => y.PaperId == x.Id && y.Id != x.LastPaperEvent.Id && y.PaperRecieveDate.HasValue &&
                                            y.TargetPositionId == x.LastPaperEvent.SourcePositionId)
                                .OrderByDescending(y => y.PaperRecieveDate)
                                .Select(y => y.Id)
                                .FirstOrDefault();
                    });
                }
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument ModifyDocumentPaperPrepare(IContext context, int? id, BaseModifyDocumentPapers model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context, null, false, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
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
                    doc.Papers = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).Where(x => (x.Id == id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
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
                transaction.Complete();
                return doc;
            }
        }

        public IEnumerable<int> AddDocumentPapers(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            List<int> res = new List<int>();
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
            return res;
        }

        public void ModifyDocumentPaper(IContext context, InternalDocumentPaper item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeleteDocumentPaper(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public void MarkOwnerDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public void MarkСorruptionDocumentPaper(IContext context, InternalDocumentPaper paper)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public void SendDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public void RecieveDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public void CancelPlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public IEnumerable<InternalDocumentPaper> PlanDocumentPaperFromSendListPrepare(IContext context, int idSendList)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
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
                transaction.Complete();
                return res;

            }
        }

        public InternalDocument PlanDocumentPaperEventPrepare(IContext context, List<int> paperIds)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return doc;
            }
        }

        public void PlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> papers)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public InternalDocumentPaperList AddDocumentPaperListsPrepare(IContext context, AddDocumentPaperLists model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return list;
            }
        }

        public List<int> AddDocumentPaperLists(IContext context, IEnumerable<InternalDocumentPaperList> items)
        {
            List<int> res = new List<int>();
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
            return res;
        }

        public InternalDocumentPaperList DeleteDocumentPaperListPrepare(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                var res = new InternalDocumentPaperList
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
                transaction.Complete();
                return res;
            }
        }

        public InternalDocumentPaperList ModifyDocumentPaperListPrepare(IContext context, int itemId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var list = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == itemId)
                    .Select(x => new InternalDocumentPaperList
                    {
                        Id = x.Id,
                    }
                    ).FirstOrDefault();
                if (list == null) return null;
                list.SourcePositionId = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).FirstOrDefault(x => x.PaperListId.HasValue && itemId == x.PaperListId.Value).SourcePositionId;
                transaction.Complete();
                return list;
            }
        }

        public void ModifyDocumentPaperList(IContext context, InternalDocumentPaperList item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbDocumentPaperList(item);
                dbContext.DocumentPaperListsSet.Attach(itemDb);
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        #endregion DocumentPapers
    }
}