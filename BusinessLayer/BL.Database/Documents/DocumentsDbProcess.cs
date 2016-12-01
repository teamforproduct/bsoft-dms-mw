using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Database.DBModel.Document;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.InternalModel;
using LinqKit;
using System.Data.Entity;
using BL.Model.Reports.FrontModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Admin;

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        public DocumentsDbProcess()
        {
        }

        public void GetCountDocuments(IContext ctx, LicenceInfo licence)
        {
            if (licence == null)
            {
                throw new LicenceError();
            }

            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

                var count = qry.Count();

                licence.CountDocument += count;

                if (count > 0)
                {
                    var dateFirstDocument = qry.OrderBy(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();
                    if (licence.DateFirstDocument == null || dateFirstDocument < licence.DateFirstDocument)
                    {
                        licence.DateFirstDocument = dateFirstDocument;
                    }

                }
            }
        }

        public int GetDocumentIdBySendListId(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return dbContext.DocumentSendListsSet.Where(x => x.Id == id).Select(x => x.DocumentId).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterBase filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetDocumentQuery(ctx, dbContext, filter?.Document, true);

                if (filter?.File != null)
                {
                    var files = CommonQueries.GetDocumentFileQuery(ctx, dbContext, filter?.File);
                    qry = qry.Where(x => files.Select(y => y.DocumentId).Contains(x.Id));
                }

                if (filter?.Event != null)
                {
                    var events = CommonQueries.GetDocumentEventQuery(ctx, dbContext, filter?.Event);
                    qry = qry.Where(x => events.Select(y => y.DocumentId).Contains(x.Id));
                }

                if (filter?.Wait != null)
                {
                    var waits = CommonQueries.GetDocumentWaitQuery(ctx, dbContext, filter?.Wait);
                    qry = qry.Where(x => waits.Select(y => y.DocumentId).Contains(x.Id));
                }


                //TODO Sort
                //TODO After ToList
                {
                    qry = qry.OrderByDescending(x => x.CreateDate).ThenByDescending(x=>x.Id);
                }

                #region Paging
                if (paging != null && paging.Sort == EnumSort.IncomingIds && filter?.Document?.DocumentId?.Count() > 0)
                {
                    var sortDocIds = filter.Document.DocumentId.Select((x, i) => new { DocId = x, Index = i }).ToList();
                    var docIds = qry.Select(x => x.Id).ToList();

                    docIds = docIds.Join(sortDocIds, o => o, i => i.DocId, (o, i) => i)
                        .OrderBy(x => x.Index).Select(x => x.DocId).ToList();

                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = docIds.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDocument>();
                    }

                    if (!paging.IsAll)
                    {
                        docIds = docIds.Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                    }

                    if (docIds.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                        filterContains = docIds.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = dbContext.DocumentsSet.Where(filterContains);
                    }
                    else
                    {
                        qry = dbContext.DocumentsSet.Where(x => false);
                    }
                }
                else if (filter?.Document?.FullTextSearchDocumentId != null)
                {
                    var sortDocIds = filter.Document.FullTextSearchDocumentId.Select((x, i) => new { DocId = x, Index = i }).ToList();
                    var docIds = qry.Select(x => x.Id).ToList();

                    docIds = docIds.Join(sortDocIds, o => o, i => i.DocId, (o, i) => i)
                        .OrderBy(x => x.Index).Select(x => x.DocId).ToList();

                    if (paging != null)
                    {
                        if (paging.IsOnlyCounter ?? true)
                        {
                            paging.TotalItemsCount = docIds.Count();
                        }

                        if (paging.IsOnlyCounter ?? false)
                        {
                            //var filterContainsPosition = PredicateBuilder.False<DictionaryTags>();
                            //filterContainsPosition = ctx.CurrentPositionsIdList.Aggregate(filterContainsPosition,
                            //    (current, value) => current.Or(e => !e.PositionId.HasValue || e.PositionId == value).Expand());

                            var qryTagCounters = dbContext.DictionaryTagsSet//.Where(filterContainsPosition)
                                    .Select(x => new FrontDocumentTag
                                    {
                                        TagId = x.Id,
                                        PositionId = x.PositionId,
                                        PositionName = x.Position.Name,
                                        Color = x.Color,
                                        Name = x.Name,
                                        IsSystem = !x.PositionId.HasValue,
                                        DocCount = x.Documents.Count(y => qry.Select(z => z.Id).Contains(y.DocumentId))
                                    })
                                    .Where(x => x.DocCount > 0);
                            var tagCounters = qryTagCounters.ToList();
                            return new List<FrontDocument> { new FrontDocument { DocumentTags = tagCounters } };
                        }

                        if (!paging.IsAll)
                        {
                            docIds = docIds.Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                        }
                    }

                    if (docIds.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                        filterContains = docIds.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = dbContext.DocumentsSet.Where(filterContains);
                    }
                    else
                    {
                        qry = dbContext.DocumentsSet.Where(x => false);
                    }
                }
                else if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        //var filterContainsPosition = PredicateBuilder.False<DictionaryTags>();
                        //filterContainsPosition = ctx.CurrentPositionsIdList.Aggregate(filterContainsPosition,
                        //    (current, value) => current.Or(e => !e.PositionId.HasValue || e.PositionId == value).Expand());

                        var qryTagCounters = dbContext.DictionaryTagsSet//.Where(filterContainsPosition)
                            .Select(x => new FrontDocumentTag
                            {
                                TagId = x.Id,
                                PositionId = x.PositionId,
                                PositionName = x.Position.Name,
                                Color = x.Color,
                                Name = x.Name,
                                IsSystem = !x.PositionId.HasValue,
                                DocCount = x.Documents.Count(y => qry.Select(z => z.Id).Contains(y.DocumentId))
                            })
                                .Where(x => x.DocCount > 0);
                        var tagCounters = qryTagCounters.ToList();
                        return new List<FrontDocument> { new FrontDocument { DocumentTags = tagCounters } };
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }
                if ((paging?.IsAll ?? true) && (filter == null || filter.Document == null || ((filter.Document.DocumentId?.Count ?? 0) == 0 )))
                {
                    throw new WrongAPIParameters();
                }
                #endregion Paging

                #region Counter
                var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
                filterOnEventPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventPositionsContains, (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value).Expand());

                var filterOnEventTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterOnEventTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains, (current, value) => current.Or(e => e.PositionId == value).Expand());

                var filterNewEventContains = PredicateBuilder.False<DocumentEvents>();
                filterNewEventContains = ctx.CurrentPositionsIdList.Aggregate(filterNewEventContains, (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                #endregion Counter

                //var t = dbContext.DictionaryPositionExecutorsSet.Where(x=>true).Select(x=> new
                //{
                //    x.AgentId,
                //    x.Description,
                //    x.
                //}
                //)

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Id,
                    DocumentDirectionName = doc.TemplateDocument.DocumentDirection.Name,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,

                    RegistrationNumber = doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.RegistrationNumberSuffix,

                    DocumentDate = doc.RegistrationDate ?? doc.CreateDate,
                    IsRegistered = doc.IsRegistered,
                    IsLaunchPlan = doc.IsLaunchPlan,
                    Description = doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgent.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,

                    WaitCount = doc.Waits.AsQueryable().Where(x => !x.OffEventId.HasValue && !x.OnEvent.IsAvailableWithinTask).Where(filterOnEventPositionsContains)
                        .Concat(doc.Waits.AsQueryable().Where(x => !x.OffEventId.HasValue && x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue &&
                            x.OnEvent.Task.TaskAccesses.AsQueryable().Any(filterOnEventTaskAccessesContains)))
                            .GroupBy(x => x.DocumentId)
                            .Select(x => new UICounters { Counter1 = x.Count(), Counter2 = x.Count(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow) })
                            .FirstOrDefault(),

                    NewEventCount = doc.Events.AsQueryable().Where(filterNewEventContains).Count(x => !x.ReadDate.HasValue && x.TargetPositionId != x.SourcePositionId),

                    AttachedFilesCount = doc.Files.Where(fl => fl.IsMainVersion && !fl.IsDeleted).Count(),

                    LinkId = doc.LinkId,
                    //LinkedDocumentsCount = doc.Links
                    //.GroupBy(x => x.LinkId)
                    //.Select(x => x.Count())
                    //.Select(x => x < 2 ? 0 : x - 1).FirstOrDefault(),
                });

                var docs = res.ToList();

                //TODO Sort
                if (paging != null && paging.Sort == EnumSort.IncomingIds && filter?.Document?.DocumentId?.Count() > 0)
                {
                    docs = docs.OrderBy(x => filter.Document.DocumentId.IndexOf(x.Id)).ToList();
                }
                else if (filter?.Document?.FullTextSearchDocumentId?.Count() > 0)
                {
                    docs = docs.OrderBy(x => filter.Document.FullTextSearchDocumentId.IndexOf(x.Id)).ToList();
                }

                if (docs.Any(x => x.LinkId.HasValue))
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = docs.GroupBy(x => x.LinkId).Where(x => x.Key.HasValue).Select(x => x.Key).Aggregate(filterContains, (current, value) => current.Or(e => e.LinkId == value).Expand());

                    var links = CommonQueries.GetDocumentQuery(dbContext, ctx, null, false, true)
                        .Where(filterContains)
                        .GroupBy(x => x.LinkId.Value)
                        .Select(x => new { LinkId = x.Key, Count = x.Count() })
                        .ToList();
                    //.Select(x => x < 2 ? 0 : x - 1)

                    docs.ForEach(x =>
                    {
                        x.LinkedDocumentsCount = links.FirstOrDefault(y => y.LinkId == x.LinkId)?.Count ?? 0;
                        x.LinkedDocumentsCount = x.LinkedDocumentsCount < 2 ? 0 : x.LinkedDocumentsCount - 1;
                    });
                }

                docs.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

                {
                    var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                    filterContains = docs.Select(x => x.Id).Aggregate(filterContains, (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    var acc = CommonQueries.GetDocumentAccesses(ctx, dbContext)
                                        .Where(filterContains).GroupBy(x => x.DocumentId)
                                        .Select(x => new
                                        {
                                            DocumentId = x.Key,
                                            IsFavourite = x.Any(y => y.IsFavourite),
                                            IsInWork = x.Any(y => y.IsInWork)
                                        }).ToList();

                    foreach (var doc in docs)
                    {
                        var docAccs = acc.FirstOrDefault(x => x.DocumentId == doc.Id);
                        if (docAccs == null) continue;
                        doc.IsFavourite = docAccs.IsFavourite;
                        doc.IsInWork = docAccs.IsInWork;
                    }
                }

                docs = docs.GroupJoin(CommonQueries.GetDocumentTags(dbContext, ctx, new FilterDocumentTag { DocumentId = docs.Select(x => x.Id).ToList(), CurrentPositionsId = ctx.CurrentPositionsIdList }),
                    d => d.Id,
                    t => t.DocumentId,
                    (d, t) => { d.DocumentTags = t.ToList(); return d; }).ToList();
                transaction.Complete();
                return docs;
            }
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var accs = CommonQueries.GetDocumentAccesses(ctx, dbContext).Where(x => x.DocumentId == documentId).ToList();

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Id,
                    DocumentDirection = (EnumDocumentDirections)doc.TemplateDocument.DocumentDirectionId,
                    DocumentDirectionName = doc.TemplateDocument.DocumentDirection.Name,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,
                    DocumentDate = doc.RegistrationDate ?? doc.CreateDate,
                    IsRegistered = doc.IsRegistered,
                    Description = doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgent.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    LinkedDocumentsCount = 0, //TODO

                    TemplateDocumentId = doc.TemplateDocumentId,
                    DocumentSubjectId = doc.DocumentSubjectId,
                    DocumentSubjectName = doc.DocumentSubject.Name,

                    RegistrationJournalId = doc.RegistrationJournalId,
                    RegistrationJournalName = doc.RegistrationJournal.Name,
                    RegistrationNumber = doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.RegistrationNumberSuffix,
                    RegistrationDate = doc.RegistrationDate,

                    ExecutorPositionId = doc.ExecutorPositionId,
                    ExecutorPositionExecutorNowAgentName = doc.ExecutorPosition.ExecutorAgent.Name,
                    ExecutorPositionAgentPhoneNumber = "(888)888-88-88", //TODO

                    SenderAgentId = doc.SenderAgentId,
                    SenderAgentName = doc.SenderAgent.Name,
                    SenderAgentPersonId = doc.SenderAgentPersonId,
                    SenderAgentPersonName = doc.SenderAgentPerson.Agent.Name,
                    SenderNumber = doc.SenderNumber,
                    SenderDate = doc.SenderDate,
                    Addressee = doc.Addressee,

                    IsLaunchPlan = doc.IsLaunchPlan,
                    TemplateDocumentName = doc.TemplateDocument.Name,
                    IsHard = doc.TemplateDocument.IsHard,
                    LinkId = doc.LinkId,

                }).FirstOrDefault();

                if (res == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                var accByExecutorPosition = accs.FirstOrDefault(x => x.PositionId == res.ExecutorPositionId);
                if (accByExecutorPosition != null)
                {
                    res.AccessLevelId = accByExecutorPosition.AccessLevelId;
                    res.AccessLevelName = accByExecutorPosition.AccessLevelName;
                }
                res.IsFavourite = accs.Any(x => x.IsFavourite);
                res.IsInWork = accs.Any(x => x.IsInWork);
                res.Accesses = accs;

                CommonQueries.ChangeRegistrationFullNumber(res, false);

                var docIds = new List<int> { res.Id };

                if (res.LinkId.HasValue)
                {
                    res.LinkedDocuments = CommonQueries.GetLinkedDocuments(ctx, dbContext, res.LinkId.Value);
                    var linkedDocumentsCount = res.LinkedDocuments.Count();
                    if (linkedDocumentsCount > 1)
                    {
                        res.LinkedDocuments = res.LinkedDocuments.OrderBy(x => x.Id == documentId ? 0 : 1).ThenBy(x=>x.DocumentDate);
                    }
                    res.LinkedDocumentsCount = linkedDocumentsCount < 2 ? 0 : linkedDocumentsCount - 1;
                    if (filter?.DocumentsIdForAIP?.Count() > 0)
                    {
                        docIds = filter.DocumentsIdForAIP;
                    }
                    else
                    {
                        docIds = res.LinkedDocuments.Select(x => x.Id).ToList();
                    }
                }

                res.SendLists = CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

                res.SendListStageMax = (res.SendLists == null) || !res.SendLists.Any() ? 0 : res.SendLists.Max(x => x.Stage);

                res.RestrictedSendLists = CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });

                res.DocumentTags = CommonQueries.GetDocumentTags(dbContext, ctx, new FilterDocumentTag { DocumentId = docIds, CurrentPositionsId = ctx.CurrentPositionsIdList });

                res.DocumentWorkGroup = CommonQueries.GetDocumentWorkGroup(dbContext, ctx, new FilterDictionaryPosition { DocumentIDs = docIds });

                res.Properties = CommonQueries.GetPropertyValues(dbContext, ctx, new FilterPropertyValue { RecordId = new List<int> { documentId }, Object = new List<EnumObjects> { EnumObjects.Documents } });

                return res;
            }
        }

        public IEnumerable<int> GetLinkedDocumentIds(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return CommonQueries.GetLinkedDocumentIds(ctx, dbContext, documentId);
            }
        }

        public InternalDocument ReportDocumentForDigitalSignaturePrepare(IContext ctx, DigitalSignatureDocumentPdf model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentDigitalSignaturePrepare(dbContext, ctx, model.DocumentId, new List<EnumSubscriptionStates> {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                        });

                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                return doc;
            }
        }

        public FrontReport ReportDocumentForDigitalSignature(IContext ctx, DigitalSignatureDocumentPdf model, bool isUseInternalSign, bool isUseCertificateSign)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                CommonQueries.GetDocumentHash(dbContext, ctx, model.DocumentId, isUseInternalSign, isUseCertificateSign, null, false, false, false);

                var subscriptionStates = new List<EnumSubscriptionStates> {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                        };

                InternalDocument doc = CommonQueries.GetDocumentDigitalSignaturePrepare(dbContext, ctx, model.DocumentId, subscriptionStates);

                if (model.IsAddSubscription)
                {
                    var subscriptions = doc.Subscriptions.ToList();
                    subscriptions.Add(dbContext.DictionaryPositionsSet.Where(x => x.Id == ctx.CurrentPositionId).Select(x => new InternalDocumentSubscription { Id = 0, DocumentId = model.DocumentId, DoneEventSourcePositionName = x.Name, DoneEventSourcePositionExecutorAgentName = x.ExecutorAgent.Name }).FirstOrDefault());
                    doc.Subscriptions = subscriptions;
                }

                var pdf = CommonQueries.GetDocumentCertificateSignPdf(dbContext, ctx, doc);

                return pdf;
            }
        }

        public InternalDocument ReportRegistrationCardDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var doc = qry.Select(x => new InternalDocument
                {
                    Id = x.Id,
                    DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                    IsRegistered = x.IsRegistered,
                    ExecutorPositionId = x.ExecutorPositionId,
                }).FirstOrDefault();

                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                return doc;
            }
        }

        public InternalDocument ReportRegistrationCardDocument(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var doc = qry.FirstOrDefault();
                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                //var accs = CommonQueries.GetDocumentAccesses(ctx, dbContext).Where(x => x.DocumentId == doc.Id).ToList();

                var res = new InternalDocument
                {
                    Id = doc.Id,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    Addressee = doc.Addressee,
                    Description = doc.Description,
                    SenderAgentName = doc.SenderAgent?.Name,
                    SenderAgentPersonName = doc.SenderAgentPerson?.Agent.Name,
                };

                var docIds = new List<int> { res.Id };

                var maxDateTime = DateTime.UtcNow.AddYears(50);

                res.Waits = CommonQueries.GetDocumentWaitQuery(ctx, dbContext, new FilterDocumentWait { DocumentId = new List<int> { res.Id } })
                    .Select(x => new InternalDocumentWait
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        CreateDate = x.OnEvent.Date,
                        TargetPositionName = x.OnEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.OnEvent.TargetPositionExecutorAgent.Name,
                        SourcePositionName = x.OnEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.OnEvent.SourcePositionExecutorAgent.Name,
                        DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                        IsClosed = x.OffEventId != null,
                        ResultTypeName = x.ResultType.Name,
                        AttentionDate = x.AttentionDate,
                        OnEventTypeName = x.OnEvent.EventType.Name,
                        OffEventDate = x.OffEventId.HasValue ? x.OffEvent.CreateDate : (DateTime?)null
                    }).ToList();

                res.Subscriptions = CommonQueries.GetDocumentSubscriptionsQuery(dbContext, new FilterDocumentSubscription { DocumentId = new List<int> { res.Id }, SubscriptionStates = new List<EnumSubscriptionStates> { EnumSubscriptionStates.Sign } }, ctx)
                    .Select(x => new InternalDocumentSubscription
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        SubscriptionStatesName = x.SubscriptionState.Name,
                        DoneEventSourcePositionName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePosition.Name : string.Empty,
                        DoneEventSourcePositionExecutorAgentName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePositionExecutorAgent.Name : string.Empty
                    }).ToList();

                return res;
            }
        }

        //public InternalDocument ReportTransmissionDocumentPaperEventPrepare(IContext ctx, int documentId)
        //{
        //    using (var dbContext = new DmsContext(ctx))
        //    {
        //        var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Doc.Id == documentId);

        //        var doc = qry.Select(x => new InternalDocument
        //        {
        //            Id = x.Doc.Id,
        //            ExecutorPositionId = x.Doc.ExecutorPositionId,
        //        }).FirstOrDefault();

        //        if (doc == null)
        //        {
        //            throw new DocumentNotFoundOrUserHasNoAccess();
        //        }

        //        return doc;
        //    }
        //}

        public List<InternalDocument> ReportRegisterTransmissionDocuments(IContext ctx, int paperListId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                var qry = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.PaperListId == paperListId);

                var res = qry.GroupBy(x => x.Document)
                    .Select(x => x.Key)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        RegistrationNumber = x.RegistrationNumber,
                        RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                        Description = x.Description
                        //DocumentTypeName = x.DocTypeName,
                        //ExecutorPositionName = x.ExecutorPosName,
                        //Addressee = x.Doc.Addressee,
                        //SenderAgentName = doc.SenderAgentname,
                        //SenderAgentPersonName = doc.SenderPersonName,
                    }).ToList();

                res.ForEach(x => x.RegistrationFullNumber = CommonQueries.GetRegistrationFullNumber(x));

                var events = qry.Select(x => new InternalDocumentEvent
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SourcePositionName = x.SourcePosition.Name,
                    SourcePositionExecutorAgentName = x.SourcePositionExecutorAgent.Name,
                    TargetPositionName = x.TargetPosition.Name,
                    TargetPositionExecutorAgentName = x.TargetPositionExecutorAgent.Name,
                    PaperId = x.PaperId,
                    Paper = !x.PaperId.HasValue
                        ? null
                        : new InternalDocumentPaper
                        {
                            Id = x.Paper.Id,
                            DocumentId = x.Paper.DocumentId,
                            Name = x.Paper.Name,
                            Description = x.Paper.Description
                        }
                }).ToList();

                res = res.GroupJoin(events, o => o.Id, i => i.DocumentId, (o, i) => { o.Events = i.ToList(); return o; }).ToList();

                return res;
            }
        }

        public InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == templateDocumentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.Id,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                        RegistrationJournalId = x.RegistrationJournalId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.Tasks = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentTask()
                    {
                        Name = y.Task,
                        Description = y.Description,
                        PositionId = y.PositionId ?? 0,
                    }).ToList();

                doc.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentRestrictedSendList()
                    {
                        PositionId = y.PositionId,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId
                    }).ToList();

                doc.SendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentSendList()
                    {
                        SendType = (EnumSendTypes)y.SendTypeId,
                        SourcePositionId = y.SourcePositionId ?? 0,
                        TargetPositionId = y.TargetPositionId,
                        TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
                        IsAvailableWithinTask = y.IsAvailableWithinTask,
                        IsWorkGroup = y.IsWorkGroup,
                        IsAddControl = y.IsAddControl,
                        SelfDueDate = y.SelfDueDate,
                        SelfDueDay = y.SelfDueDay,
                        SelfAttentionDate = y.SelfAttentionDate,
                        Description = y.Description,
                        Stage = y.Stage,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                    }).ToList();

                doc.DocumentFiles = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == templateDocumentId).Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Extension = x.Extention,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    OrderInDocument = x.OrderNumber,
                    Type = (EnumFileTypes)x.TypeId,
                    Hash = x.Hash,
                    Description = x.Description,
                }).ToList();

                doc.Properties = CommonQueries.GetInternalPropertyValues(dbContext, context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { templateDocumentId } }).ToList();

                return doc;
            }
        }

        public InternalDocument CopyDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.TemplateDocumentId,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,

                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.AccessLevel = (EnumDocumentAccesses)CommonQueries.GetDocumentAccessesesQry(dbContext, documentId, ctx).Max(x => x.AccessLevelId);
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentTask
                        {
                            Name = x.Task,
                            Description = x.Description,
                            PositionId = x.PositionId,
                        }
                        ).ToList();
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.IsInitial)
                        .Select(y => new InternalDocumentSendList
                        {
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            SourcePositionId = y.SourcePositionId,
                            TargetPositionId = y.TargetPositionId,
                            TargetAgentId = y.TargetAgentId,
                            TaskName = y.Task.Task,
                            IsAvailableWithinTask = y.IsAvailableWithinTask,
                            IsWorkGroup = y.IsWorkGroup,
                            IsAddControl = y.IsAddControl,
                            SelfDueDate = y.SelfDueDate,
                            SelfDueDay = y.SelfDueDay,
                            SelfAttentionDate = y.SelfAttentionDate,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            IsInitial = y.IsInitial,
                        }).ToList();
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentRestrictedSendList
                        {
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                        }).ToList();
                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId).Where(x => x.Type != EnumFileTypes.SubscribePdf).ToList();

                doc.Properties = CommonQueries.GetInternalPropertyValues(dbContext, ctx, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Documents }, RecordId = new List<int> { documentId } }).ToList();

                return doc;
            }
        }

        public void AddDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var doc = ModelConverter.GetDbDocument(document);

                    if (document.Accesses != null && document.Accesses.Any())
                    {
                        doc.Accesses = ModelConverter.GetDbDocumentAccesses(document.Accesses).ToList();
                    }

                    if (document.Events != null && document.Events.Any())
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events).ToList();
                    }

                    if (document.RestrictedSendLists != null && document.RestrictedSendLists.Any())
                    {
                        doc.RestrictedSendLists = ModelConverter.GetDbDocumentRestrictedSendLists(document.RestrictedSendLists).ToList();
                    }

                    if (document.DocumentFiles != null && document.DocumentFiles.Any())
                    {
                        doc.Files = ModelConverter.GetDbDocumentFiles(document.DocumentFiles).ToList();
                    }

                    if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                    {
                        doc.Tasks = ModelConverter.GetDbDocumentTasks(document.Tasks.Where(x => x.Id == 0)).ToList();
                    }

                    dbContext.DocumentsSet.Add(doc);
                    dbContext.SaveChanges();

                    var sendListsDb = new List<DocumentSendLists>();
                    if (document.SendLists != null && document.SendLists.Any())
                    {
                        var sendLists = document.SendLists.ToList();
                        sendLists.ForEach(x =>
                        {
                            x.DocumentId = doc.Id;
                            var taskId = doc.Tasks.Where(y => y.Task == x.TaskName).Select(y => y.Id).FirstOrDefault();
                            x.TaskId = (taskId == 0 ? null : (int?)taskId);
                        });
                        sendListsDb = ModelConverter.GetDbDocumentSendLists(sendLists).ToList();
                        dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                        dbContext.SaveChanges();

                    }

                    if (document.Properties?.Any() ?? false)
                    {
                        document.Properties.ToList().ForEach(x => { x.RecordId = doc.Id; });
                        var propertyValues = ModelConverter.GetDbPropertyValue(document.Properties).ToList();
                        dbContext.PropertyValuesSet.AddRange(propertyValues);
                        dbContext.SaveChanges();
                    }

                    document.Id = doc.Id;

                    //TODO we schould check if it needed or not? 
                    if (document.DocumentFiles != null)
                        foreach (var fl in document.DocumentFiles)
                        {
                            fl.DocumentId = doc.Id;
                        }
                    dbContext.SaveChanges();
                    //TODO Papers
                    CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.AddNew);
                    CommonQueries.AddFullTextCashInfo(dbContext, doc.Events.Select(x => x.Id).ToList(), EnumObjects.DocumentEvents, EnumOperationType.AddNew);
                    CommonQueries.AddFullTextCashInfo(dbContext, doc.Files.Select(x => x.Id).ToList(), EnumObjects.DocumentFiles, EnumOperationType.AddNew);
                    CommonQueries.AddFullTextCashInfo(dbContext, sendListsDb.Select(x => x.Id).ToList(), EnumObjects.DocumentSendLists, EnumOperationType.AddNew);
                    transaction.Complete();
                }
            }
        }

        public InternalDocument ModifyDocumentPrepare(IContext ctx, ModifyDocument model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.Id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        IsHard = x.TemplateDocument.IsHard,
                        DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                        IsRegistered = x.IsRegistered,
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.Id && x.PositionId == doc.ExecutorPositionId && x.AccessLevelId != model.AccessLevelId)
                    .Select(x => new InternalDocumentAccess
                    {
                        Id = x.Id,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId
                    }).ToList();
                return doc;
            }
        }

        public void ModifyDocument(IContext ctx, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    var doc = new DBModel.Document.Documents
                    {
                        Id = document.Id,
                        DocumentSubjectId = document.DocumentSubjectId,
                        Description = document.Description,
                        SenderAgentId = document.SenderAgentId,
                        SenderAgentPersonId = document.SenderAgentPersonId,
                        SenderNumber = document.SenderNumber,
                        SenderDate = document.SenderDate,
                        Addressee = document.Addressee,
                        LastChangeDate = document.LastChangeDate,
                        LastChangeUserId = document.LastChangeUserId,
                        IsRegistered = document.IsRegistered,
                    };
                    dbContext.DocumentsSet.Attach(doc);
                    var entry = dbContext.Entry(doc);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.DocumentSubjectId).IsModified = true;
                    entry.Property(x => x.Description).IsModified = true;
                    entry.Property(x => x.SenderAgentId).IsModified = true;
                    entry.Property(x => x.SenderAgentPersonId).IsModified = true;
                    entry.Property(x => x.SenderNumber).IsModified = true;
                    entry.Property(x => x.SenderDate).IsModified = true;
                    entry.Property(x => x.Addressee).IsModified = true;
                    entry.Property(x => x.IsRegistered).IsModified = true;

                    var docAccess = document.Accesses.FirstOrDefault();
                    if (docAccess != null)
                    {
                        var acc = new DocumentAccesses
                        {
                            Id = docAccess.Id,
                            AccessLevelId = (int)docAccess.AccessLevel,
                            LastChangeDate = docAccess.LastChangeDate,
                            LastChangeUserId = docAccess.LastChangeUserId
                        };
                        dbContext.DocumentAccessesSet.Attach(acc);
                        var entryAcc = dbContext.Entry(acc);
                        entryAcc.Property(x => x.LastChangeDate).IsModified = true;
                        entryAcc.Property(x => x.LastChangeUserId).IsModified = true;
                        entryAcc.Property(x => x.AccessLevelId).IsModified = true;
                    }

                    if (document.Properties != null)
                    {
                        CommonQueries.ModifyPropertyValues(dbContext, ctx,
                            new InternalPropertyValues
                            {
                                Object = EnumObjects.Documents,
                                RecordId = document.Id,
                                PropertyValues = document.Properties
                            });
                    }
                    dbContext.SaveChanges();

                    CommonQueries.GetDocumentHash(dbContext, ctx, document.Id, isUseInternalSign, isUseCertificateSign, null, false, false);
                    CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.Update);
                    transaction.Complete();
                }
            }
        }

        public InternalDocument DeleteDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        LinkId = x.LinkId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        WaitsCount = x.Waits.Count,
                        SubscriptionsCount = x.Subscriptions.Count,
                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, doc.Id);

                return doc;
            }
        }

        public void DeleteDocument(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    //ADD OTHER TABLES!!!!
                    dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id).ToList()  //TODO OPTIMIZE
                    .ForEach(x =>
                    {
                        x.LastPaperEventId = null;
                    });
                    //TODO придумать с удалением для полнотекста
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.SaveChanges();
                    dbContext.DocumentTagsSet.RemoveRange(dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentFilesSet.RemoveRange(dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentRestrictedSendListsSet.RemoveRange(dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentSendListsSet.RemoveRange(dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentTasksSet.RemoveRange(dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));

                    dbContext.DocumentPapersSet.RemoveRange(dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));

                    CommonQueries.DeletePropertyValues(dbContext, ctx, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Documents }, RecordId = new List<int> { id } });

                    dbContext.DocumentsSet.RemoveRange(dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == id));

                    dbContext.SaveChanges();

                    CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.Documents, EnumOperationType.Delete);
                    transaction.Complete();
                }
            }
        }

        public InternalDocument RegisterDocumentPrepare(IContext context, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        SenderNumber = x.SenderNumber,
                        SenderDate = x.SenderDate,
                        Addressee = x.Addressee,
                        LinkId = x.LinkId,

                        DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }
                var regJournal = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == model.RegistrationJournalId)
                    .Where(x => dbContext.AdminRegistrationJournalPositionsSet
                                            .Where(y => y.PositionId == context.CurrentPositionId && y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                                            .Select(y => y.RegJournalId).Contains(x.Id))
                    .Select(x => new { x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula }).FirstOrDefault();

                if (regJournal != null)
                {
                    doc.RegistrationJournalId = regJournal.Id;
                    doc.NumerationPrefixFormula = regJournal.NumerationPrefixFormula;
                    doc.RegistrationJournalPrefixFormula = regJournal.PrefixFormula;
                    doc.RegistrationJournalSuffixFormula = regJournal.SuffixFormula;
                }
                else
                {
                    doc.RegistrationJournalId = null;
                }
                return doc;
            }
        }

        public InternalDocumnRegistration RegisterModelDocumentPrepare(IContext context, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new
                    {
                        DocumentId = x.Id,
                        LinkId = x.LinkId,
                        SenderAgentId = x.SenderAgentId,
                        ExecutorPositionDepartmentCode = x.ExecutorPosition.Department.Code,
                        SubscriptionsPositionDepartmentCode = x.Subscriptions
                                        .Where(y => y.SubscriptionStateId == (int)EnumSubscriptionStates.Sign)
                                        .OrderBy(y => y.LastChangeDate).Take(1)
                                        .Select(y => y.DoneEvent.SourcePosition.Department.Code).FirstOrDefault(),
                        DocumentSendListLastAgentExternalFirstSymbolName = x.SendLists
                                        .Where(y => y.SendTypeId == (int)EnumSendTypes.SendForInformationExternal)
                                        .OrderByDescending(y => y.LastChangeDate).Take(1)
                                        .Select(y => y.TargetAgent.Name).FirstOrDefault()
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                var res = new InternalDocumnRegistration
                {
                    ExecutorPositionDepartmentCode = doc.ExecutorPositionDepartmentCode,
                    RegistrationDate = model.RegistrationDate,
                    SubscriptionsPositionDepartmentCode = doc.SubscriptionsPositionDepartmentCode,
                    DocumentSendListLastAgentExternalFirstSymbolName = doc.DocumentSendListLastAgentExternalFirstSymbolName
                };

                if (!string.IsNullOrEmpty(res.DocumentSendListLastAgentExternalFirstSymbolName))
                    res.DocumentSendListLastAgentExternalFirstSymbolName = res.DocumentSendListLastAgentExternalFirstSymbolName.Substring(0, 1);

                //TODO ??? если doc.LinkId==null || doc.SenderAgentId ==null
                res.OrdinalNumberDocumentLinkForCorrespondent = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.LinkId == doc.LinkId && x.SenderAgentId == doc.SenderAgentId && x.IsRegistered == true)
                        .Count() + 1;

                var regJournal = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == model.RegistrationJournalId)
                    .Select(x => new { x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula, x.Index, RegistrationJournalDepartmentCode = x.Department.Code }).FirstOrDefault();

                if (regJournal != null)
                {
                    res.RegistrationJournalId = regJournal.Id;
                    res.RegistrationJournalIndex = regJournal.Index;
                    res.RegistrationJournalDepartmentCode = regJournal.RegistrationJournalDepartmentCode;
                }
                else
                {
                    res.RegistrationJournalId = null;
                }

                var initiativeDoc = dbContext.DocumentLinksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == doc.DocumentId)
                    .OrderBy(x => x.LastChangeDate)
                    .Select(x => x.ParentDocument)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        RegistrationNumber = x.RegistrationNumber,
                        RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                        SenderNumber = x.SenderNumber
                    })
                    .FirstOrDefault();

                if (initiativeDoc != null)
                {
                    res.InitiativeRegistrationFullNumber = CommonQueries.GetRegistrationFullNumber(initiativeDoc);
                    res.InitiativeRegistrationNumberPrefix = initiativeDoc.RegistrationNumberPrefix;
                    res.InitiativeRegistrationNumberSuffix = initiativeDoc.RegistrationNumberSuffix;
                    res.InitiativeRegistrationNumber = initiativeDoc.RegistrationNumber;
                    res.InitiativeRegistrationSenderNumber = initiativeDoc.SenderNumber;
                }

                res.CurrentPositionDepartmentCode = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id == model.CurrentPositionId)
                    .Select(x => x.Department.Code).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public void RegisterDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = new DBModel.Document.Documents
                {
                    Id = document.Id,
                    IsRegistered = document.IsRegistered,
                    RegistrationJournalId = document.RegistrationJournalId,
                    NumerationPrefixFormula = document.NumerationPrefixFormula,
                    RegistrationNumber = document.RegistrationNumber,
                    RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                    RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                    RegistrationDate = document.RegistrationDate,
                    LastChangeDate = document.LastChangeDate,
                    LastChangeUserId = document.LastChangeUserId
                };
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsRegistered).IsModified = true;
                entry.Property(x => x.RegistrationJournalId).IsModified = true;
                entry.Property(x => x.NumerationPrefixFormula).IsModified = true;
                entry.Property(x => x.RegistrationNumber).IsModified = true;
                entry.Property(x => x.RegistrationNumberSuffix).IsModified = true;
                entry.Property(x => x.RegistrationNumberPrefix).IsModified = true;
                entry.Property(x => x.RegistrationDate).IsModified = true;

                if (document.IsRegistered ?? false)
                {
                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                        document.Events = null; //евент добавляем один раз
                    }

                }
                else
                {
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == document.Id && x.EventTypeId == (int)EnumEventTypes.Registered));
                }

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                //get next number
                var maxNumber = (from docreg in dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                                 where docreg.RegistrationJournalId == document.RegistrationJournalId
                                       && docreg.NumerationPrefixFormula == document.NumerationPrefixFormula
                                       && docreg.Id != document.Id
                                 select docreg.RegistrationNumber).Max();
                document.RegistrationNumber = (maxNumber ?? 0) + 1;
                transaction.Complete();
            }
        }

        public bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var res = !dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Any(x => x.RegistrationJournalId == document.RegistrationJournalId
                                         && x.NumerationPrefixFormula == document.NumerationPrefixFormula
                                         && x.RegistrationNumber == document.RegistrationNumber
                                         && x.Id != document.Id);
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId && x.TypeId == (int)EnumFileTypes.Main)// !x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                    }).ToList();
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId)
                    .GroupBy(x => x.PositionId)
                    .Select(x => new InternalDocumentRestrictedSendList
                    {
                        PositionId = x.Key
                    }).ToList();
                return doc;
            }
        }

        public InternalDocument ChangePositionDocumentPrepare(IContext ctx, ChangePosition model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId && x.TypeId == (int)EnumFileTypes.Main) //!x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                    }).ToList();
                return doc;
            }
        }

        public void ChangeExecutorDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var doc = new DBModel.Document.Documents
                    {
                        Id = document.Id,
                        ExecutorPositionId = document.ExecutorPositionId,
                        ExecutorPositionExecutorAgentId = document.ExecutorPositionExecutorAgentId,
                        LastChangeDate = document.LastChangeDate,
                        LastChangeUserId = document.LastChangeUserId
                    };
                    dbContext.DocumentsSet.Attach(doc);
                    var entry = dbContext.Entry(doc);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.ExecutorPositionId).IsModified = true;
                    entry.Property(x => x.ExecutorPositionExecutorAgentId).IsModified = true;

                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                    }

                    //TODO При получении документа возвращаеться только один Accesses
                    if (document.Accesses != null && document.Accesses.Any())
                    {
                        //TODO Не сохраняеться через свойства
                        //doc.Accesses = CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, doc.Id).ToList();
                        dbContext.DocumentAccessesSet.AddRange(
                            CommonQueries.GetDbDocumentAccesses(dbContext, ctx, document.Accesses, doc.Id).ToList());
                    }
                    dbContext.SaveChanges();

                    if (document.Papers != null && document.Papers.Any(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null))
                    {
                        foreach (
                            var paper in
                                document.Papers.Where(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null).ToList())
                        {
                            var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                            dbContext.DocumentEventsSet.Add(paperEventDb);
                            dbContext.SaveChanges();
                            paper.LastPaperEventId = paperEventDb.Id;
                            var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                            dbContext.DocumentPapersSet.Attach(paperDb);
                            var entryPaper = dbContext.Entry(paperDb);
                            entryPaper.Property(e => e.LastPaperEventId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeUserId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    if (document.DocumentFiles?.Any() ?? false)
                    {
                        foreach (var fileDb in document.DocumentFiles.Select(ModelConverter.GetDbDocumentFile))
                        {
                            dbContext.DocumentFilesSet.Attach(fileDb);
                            var entryFile = dbContext.Entry(fileDb);
                            entryFile.Property(e => e.ExecutorPositionId).IsModified = true;
                            entryFile.Property(e => e.ExecutorPositionExecutorAgentId).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    if (document.Tasks?.Any() ?? false)
                    {
                        foreach (var taskDb in document.Tasks.Select(ModelConverter.GetDbDocumentTask))
                        {
                            dbContext.DocumentTasksSet.Attach(taskDb);
                            var entryTask = dbContext.Entry(taskDb);
                            entryTask.Property(e => e.PositionId).IsModified = true;
                            entryTask.Property(e => e.PositionExecutorAgentId).IsModified = true;
                            entryTask.Property(e => e.LastChangeUserId).IsModified = true;
                            entryTask.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.Update);
                    CommonQueries.AddFullTextCashInfo(dbContext, doc.Events.Select(x => x.Id).ToList(), EnumObjects.DocumentEvents, EnumOperationType.AddNew);
                    transaction.Complete();
                }
            }
        }

        public void ChangePositionDocument(IContext ctx, ChangePosition model, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == model.DocumentId && x.ExecutorPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.ExecutorPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.ExecutorPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.SourcePositionId = model.NewPositionId;
                        });
                    dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.TargetPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.SourcePositionId = model.NewPositionId;
                        });
                    dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.TargetPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                    dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                    dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });

                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                    }

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsLaunchPlan = x.IsLaunchPlan
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                    .Where(x => x.DocumentId == documentId)
                                    .Select(x => new InternalDocumentSendList
                                    {
                                        Id = x.Id,
                                    }
                                    ).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = new DBModel.Document.Documents
                {
                    Id = document.Id,
                    IsLaunchPlan = document.IsLaunchPlan,
                    LastChangeDate = document.LastChangeDate,
                    LastChangeUserId = document.LastChangeUserId
                };
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsLaunchPlan).IsModified = true;
                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                }
                dbContext.SaveChanges();
                transaction.Complete();

            }
        }

        public InternalDocument GetBlankInternalDocumentById(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();

                return doc;
            }
        }


        #region DocumentPapers

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return CommonQueries.GetDocumentPapers(dbContext, ctx, filter, paging);
            }
        }

        public FrontDocumentPaper GetDocumentPaper(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return CommonQueries.GetDocumentPapers(dbContext, ctx, new FilterDocumentPaper { Id = new List<int> { id } }, null).FirstOrDefault();
            }
        }
        #endregion DocumentPapers   

        #region DocumentPaperLists

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return CommonQueries.GetDocumentPaperLists(dbContext, ctx, filter);
            }
        }

        public FrontDocumentPaperList GetDocumentPaperList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return CommonQueries.GetDocumentPaperLists(dbContext, ctx, new FilterDocumentPaperList { PaperListId = new List<int> { id } }).FirstOrDefault();
            }
        }
        #endregion DocumentPaperLists   

        #region DocumentAccesses

        public IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                var qry = CommonQueries.GetDocumentAccesses(ctx, dbContext);

                if (filter != null)
                {
                    if (filter.DocumentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                        filterContains = filter.DocumentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.AccessLevelId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                        filterContains = filter.AccessLevelId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.AccessLevelId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.IsInWork.HasValue)
                    {
                        qry = qry.Where(x => x.IsInWork == filter.IsInWork);
                    }
                }
                qry = qry.OrderByDescending(x => x.DocumentId);
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDocumentAccess>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }
                if ((paging?.IsAll ?? true) && (filter == null ||  ((filter.DocumentId?.Count ?? 0) == 0 )))
                {
                    throw new WrongAPIParameters();
                }
                return qry.ToList();
            }
        }

        #endregion DocumentAccesses

        public IEnumerable<InternalDocumentEvent> GetEventsNatively(IContext ctx, FilterDocumentEventNatively filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetEventsNativelyQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalDocumentEvent
                {
                    Id = x.Id,
                    Date = x.Date,
                    ReadDate = x.ReadDate,
                    SourcePositionId = x.SourcePositionId,
                    SourceAgentId = x.SourceAgentId,
                    SourcePositionExecutorAgentId = x.SourcePositionExecutorAgentId,
                    TargetPositionId = x.TargetPositionId,
                    TargetPositionExecutorAgentId = x.TargetPositionExecutorAgentId,
                    //...
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsEventsNatively(IContext ctx, FilterDocumentEventNatively filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = CommonQueries.GetEventsNativelyQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

    }
}