using System;
using System.Collections.Generic;
using System.Linq;
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
using BL.CrossCutting.Helpers;
using BL.CrossCutting.DependencyInjection;
using System.Text.RegularExpressions;

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

            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

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
                transaction.Complete();
            }
        }

        public int GetDocumentIdBySendListId(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    dbContext.DocumentSendListsSet.Where(x => x.Id == id).Select(x => x.DocumentId).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterBase filter, UIPaging paging,
            EnumGroupCountType? groupCountType = null)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                List<FrontDocument> docs = null;

                #region main qry

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

                #endregion main qry

                #region Position filters for counters preparing

                var filterAccessPositionsContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionsContains = ctx.CurrentPositionsAccessLevel.Aggregate(
                    filterAccessPositionsContains,
                    (current, value) =>
                        current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand());

                var filterWaitPositionsContains = PredicateBuilder.New<DocumentWaits>();
                filterWaitPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterWaitPositionsContains,
                    (current, value) =>
                        current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value)
                            .Expand());

                #endregion Position filters for counters preparing

                #region Paging

                if (paging.Sort == EnumSort.IncomingIds && filter?.Document?.DocumentId?.Count() > 0)
                {
                    #region IncomingIds

                    var sortDocIds = filter.Document.DocumentId.Select((x, i) => new {DocId = x, Index = i}).ToList();
                    var docIds = qry.Select(x => x.Id).ToList();

                    docIds = docIds.Join(sortDocIds, o => o, i => i.DocId, (o, i) => i)
                        .OrderBy(x => x.Index).Select(x => x.DocId).ToList();

                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = docIds.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        docs = new List<FrontDocument>();
                    }
                    else
                    {
                        if (!paging.IsAll)
                        {
                            docIds =
                                docIds.Skip(paging.PageSize*(paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                        }

                        if (docIds.Count > 0)
                        {
                            var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                            filterContains = docIds.Aggregate(filterContains,
                                (current, value) => current.Or(e => e.Id == value).Expand());

                            qry = dbContext.DocumentsSet.Where(filterContains);
                        }
                        else
                        {
                            qry = dbContext.DocumentsSet.Where(x => false);
                        }
                    }

                    #endregion IncomingIds
                }
                else if (filter?.FullTextSearchSearch?.FullTextSearchResult != null)
                {
                    #region FullTextSearchDocumentId

                    #region groupCount

                    if (groupCountType == EnumGroupCountType.Tags)
                    {
                        var docTags = filter.FullTextSearchSearch.FullTextSearchResult.GroupBy(x => x.ParentId)
                            .Select(x => string.Join(" ", x.Select(y => y.Filters)))
                            .Select(
                                x =>
                                    x.Split(' ')
                                        .Where(
                                            y =>
                                                !string.IsNullOrEmpty(y) &&
                                                y[y.Length - 1].ToString() == FullTextFilterTypes.Tag)
                                        .Select(y => y.Substring(0, y.Length - 1)).Distinct().ToList())
                            .SelectMany(x => x)
                            .GroupBy(x => x)
                            .Select(x => new {Id = x.Key, Count = x.Count()})
                            .ToList();
                        var tagCounters = dbContext.DictionaryTagsSet.Select(x => new FrontDocumentTag
                        {
                            TagId = x.Id,
                            PositionId = x.PositionId,
                            PositionName = x.Position.Name,
                            Color = x.Color,
                            Name = x.Name,
                            IsSystem = !x.PositionId.HasValue,
                        }).ToList();
                        tagCounters.ForEach(
                            x =>
                                x.DocCount =
                                    docTags.Where(y => y.Id == x.TagId.ToString()).Select(y => y.Count).FirstOrDefault());
                        docs = new List<FrontDocument>
                        {
                            new FrontDocument
                            {
                                DocumentTags = tagCounters.Where(x => x.DocCount > 0).OrderBy(x => x.Name).ToList()
                            }
                        };
                    }
                    else if (groupCountType == EnumGroupCountType.Positions)
                    {
                        var docPositions = filter.FullTextSearchSearch.FullTextSearchResult.GroupBy(x => x.ParentId)
                            .Select(x => string.Join(" ", x.Select(y => y.Filters)))
                            .Select(
                                x =>
                                    x.Split(' ')
                                        .Where(
                                            y =>
                                                !string.IsNullOrEmpty(y) &&
                                                y[y.Length - 1].ToString() == FullTextFilterTypes.WorkGroupPosition)
                                        .Select(y => y.Substring(0, y.Length - 1)).Distinct().ToList())
                            .SelectMany(x => x)
                            .GroupBy(x => x)
                            .Select(x => new {Id = x.Key, Count = x.Count()})
                            .ToList();
                        var positionCounters = dbContext.DictionaryPositionsSet.Select(x => new FrontDictionaryPosition
                        {
                            Id = x.Id,
                            Name = x.Name,
                            DepartmentId = x.DepartmentId,
                            ExecutorAgentId = x.ExecutorAgentId,
                            DepartmentName = x.Department.Name,
                            ExecutorAgentName =
                                x.ExecutorAgent.Name +
                                (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : (string) null),
                        }).ToList();
                        positionCounters.ForEach(
                            x =>
                                x.DocCount =
                                    docPositions.Where(y => y.Id == x.Id.ToString())
                                        .Select(y => y.Count)
                                        .FirstOrDefault());
                        docs = new List<FrontDocument>
                        {
                            new FrontDocument
                            {
                                DocumentWorkGroup =
                                    positionCounters.Where(x => x.DocCount > 0)
                                        .OrderBy(x => x.ExecutorAgentName)
                                        .ToList()
                            }
                        };
                    }
                    else
                        #endregion groupCount

                    {
                        if ((paging.IsOnlyCounter ?? true) && !filter.FullTextSearchSearch.IsNotAll)
                        {
                            var ftDocs = filter.FullTextSearchSearch.FullTextSearchResult.GroupBy(x => x.ParentId);
                            paging.TotalItemsCount = ftDocs.Count();
                            if (paging.IsCalculateAddCounter ?? false)
                            {
                                var ftDocsAdd = ftDocs.Select(x => string.Join(" ", x.Select(y => y.Security))).ToList();
                                var accF = ctx.GetAccessFilterForFullText($".{FullTextFilterTypes.IsFavourite}..");
                                var accN = ctx.GetAccessFilterForFullText($"..{FullTextFilterTypes.IsEventNew}.");
                                var accC = ctx.GetAccessFilterForFullText($"...{FullTextFilterTypes.IsWaitOpened}");
                                paging.Counters = new UICounters
                                {
                                    Counter1 = ftDocsAdd.Count(x => accF.Any(y => Regex.IsMatch($" {x} ", $" {y} "))),
                                    Counter2 = ftDocsAdd.Count(x => accN.Any(y => Regex.IsMatch($" {x} ", $" {y} "))),
                                    Counter3 = ftDocsAdd.Count(x => accC.Any(y => Regex.IsMatch($" {x} ", $" {y} "))),
                                };
                            }
                        }
                        if (paging.IsOnlyCounter ?? false)
                        {
                            docs = new List<FrontDocument>();
                        }
                        else
                        {
                            var docIds =
                                filter.FullTextSearchSearch.FullTextSearchResult.GroupBy(x => x.ParentId)
                                    .Select(x => x.Key)
                                    .OrderByDescending(x => x)
                                    .ToList();
                            if (!paging.IsAll)
                            {
                                docIds =
                                    docIds.Skip(paging.PageSize*(paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                            }

                            if (docIds.Count > 0)
                            {
                                var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                                filterContains = docIds.Aggregate(filterContains,
                                    (current, value) => current.Or(e => e.Id == value).Expand());

                                qry = dbContext.DocumentsSet.Where(filterContains)
                                    .OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.Id);
                            }
                            else
                            {
                                qry = dbContext.DocumentsSet.Where(x => false);
                            }
                        }
                    }

                    #endregion FullTextSearchDocumentId
                }
                else
                {
                    #region Others

                    #region groupCount

                    if (groupCountType == EnumGroupCountType.Tags)
                    {
                        var qryT = qry;
                        if (filter?.Document?.TagId?.Count > 1)
                        {
                            qryT = qryT.Where(x => x.Tags.Count == filter.Document.TagId.Count);
                        }
                        var qryTagCounters = dbContext.DictionaryTagsSet.Select(x => new FrontDocumentTag
                        {
                            TagId = x.Id,
                            PositionId = x.PositionId,
                            PositionName = x.Position.Name,
                            Color = x.Color,
                            Name = x.Name,
                            IsSystem = !x.PositionId.HasValue,
                            DocCount = x.Documents.Count(y => qryT.Select(z => z.Id).Contains(y.DocumentId))
                        }).Where(x => x.DocCount > 0);
                        var tagCounters = qryTagCounters.OrderBy(x => x.Name).ToList();
                        docs = new List<FrontDocument> {new FrontDocument {DocumentTags = tagCounters}};
                    }
                    else if (groupCountType == EnumGroupCountType.Positions)
                    {
                        var qryPositionCounters = dbContext.DictionaryPositionsSet.Select(
                            x => new FrontDictionaryPosition
                            {
                                Id = x.Id,
                                Name = x.Name,
                                DepartmentId = x.DepartmentId,
                                ExecutorAgentId = x.ExecutorAgentId,
                                DepartmentName = x.Department.Name,
                                ExecutorAgentName =
                                    x.ExecutorAgent.Name +
                                    (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : (string) null),
                                DocCount = x.DocumentAccesses.Count(y => qry.Select(z => z.Id).Contains(y.DocumentId))
                            }).Where(x => x.DocCount > 0);
                        var positionCounters = qryPositionCounters.OrderBy(x => x.ExecutorAgentName).ToList();
                        docs = new List<FrontDocument> {new FrontDocument {DocumentWorkGroup = positionCounters}};
                    }
                    else
                        #endregion groupCount

                    {
                        if (paging.IsOnlyCounter ?? true)
                        {
                            if (!(paging.IsCalculateAddCounter ?? false))
                            {
                                paging.TotalItemsCount = qry.Count();
                            }
                            else
                            {
                                var qryAcc = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                                    .Where(x => qry.Select(y => y.Id).Contains(x.DocumentId))
                                    .Where(filterAccessPositionsContains)
                                    .GroupBy(x => x.DocumentId).Select(x => new
                                    {
                                        DocumentId = x.Key,
                                        CountFavourite = x.Max(z => z.IsFavourite ? 1 : 0),
                                        CountNewEvents = x.Max(z => (z.CountNewEvents ?? 0) > 0 ? 1 : 0),
                                        CountWaits = x.Max(z => (z.CountWaits ?? 0) > 0 ? 1 : 0),
                                    }).GroupBy(x => true).Select(x => new
                                    {
                                        Count = x.Count(),
                                        CountFavourite = x.Sum(y => y.CountFavourite),
                                        CountNewEvents = x.Sum(y => y.CountNewEvents),
                                        CountWaits = x.Sum(y => y.CountWaits),
                                    });
                                var counts = qryAcc.FirstOrDefault();
                                paging.TotalItemsCount = counts?.Count ?? 0;
                                paging.Counters = new UICounters
                                {
                                    Counter1 = counts?.CountNewEvents ?? 0,
                                    Counter2 = counts?.CountWaits ?? 0,
                                    Counter3 = counts?.CountFavourite ?? 0,
                                };
                            }
                        }
                        if (paging.IsOnlyCounter ?? false)
                        {
                            docs = new List<FrontDocument>();
                        }
                        else
                        {
                            qry = qry.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.Id);
                            if (!paging.IsAll)
                            {
                                var skip = paging.PageSize*(paging.CurrentPage - 1);
                                var take = paging.PageSize;

                                qry = qry.Skip(() => skip).Take(() => take);
                            }
                        }
                    }

                    #endregion Others
                }

                #endregion Paging

                if (docs == null)
                {

                    if ((paging?.IsAll ?? true) &&
                        (filter?.Document == null || ((filter.Document.DocumentId?.Count ?? 0) == 0)))
                    {
                        throw new WrongAPIParameters();
                    }

                    if (!string.IsNullOrEmpty(filter?.FullTextSearchSearch?.FullTextSearchString))
                        FileLogger.AppendTextToFile(
                            $"{DateTime.Now} '{filter?.FullTextSearchSearch?.FullTextSearchString}' *************** start fetch from db",
                            @"C:\TEMPLOGS\fulltext.log");


                    #region model filling

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
                        ExecutorPositionId = doc.ExecutorPositionId,
                        ExecutorPositionExecutorAgentId = doc.ExecutorPositionExecutorAgentId,
                        ExecutorPositionExecutorAgentName =
                            doc.ExecutorPositionExecutorAgent.Name +
                            (doc.ExecutorPositionExecutorType.Suffix != null
                                ? " (" + doc.ExecutorPositionExecutorType.Suffix + ")"
                                : null),
                        ExecutorPositionName = doc.ExecutorPosition.Name,

                        WaitCount =
                            doc.Waits.AsQueryable()
                                .Where(filterWaitPositionsContains)
                                .Where(x => !x.OffEventId.HasValue)
                                .GroupBy(x => x.DocumentId)
                                .Select(
                                    x =>
                                        new UICounters
                                        {
                                            Counter1 = x.Count(),
                                            Counter2 =
                                                x.Count(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow)
                                        })
                                .FirstOrDefault(),

                        //NewEventCount = doc.Events.AsQueryable().Where(filterNewEventContains).Count(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId),

                        AttachedFilesCount =
                            doc.Files.Count(
                                fl => fl.IsMainVersion && !fl.IsDeleted && fl.TypeId != (int) EnumFileTypes.SubscribePdf),

                        LinkId = doc.LinkId,
                    });

                    docs = res.ToList();

                    if (!string.IsNullOrEmpty(filter?.FullTextSearchSearch?.FullTextSearchString))
                        FileLogger.AppendTextToFile(
                            $"{DateTime.Now} '{filter?.FullTextSearchSearch?.FullTextSearchString}' *************** finish fetch from db",
                            @"C:\TEMPLOGS\fulltext.log");

                    //TODO Sort
                    if (paging.Sort == EnumSort.IncomingIds && filter?.Document?.DocumentId != null &&
                        filter.Document.DocumentId.Count > 0)
                    {
                        docs = docs.OrderBy(x => filter.Document.DocumentId.IndexOf(x.Id)).ToList();
                    }
                    //else if (filter?.FullTextSearchSearch?.FullTextSearchId != null && filter.FullTextSearchSearch.FullTextSearchId.Any())
                    //{
                    //    docs = docs.OrderBy(x => filter.FullTextSearchSearch.FullTextSearchId.IndexOf(x.Id)).ToList();
                    //}

                    if (docs.Any(x => x.LinkId.HasValue))
                    {
                        var filterLinkIdContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                        filterLinkIdContains =
                            docs.GroupBy(x => x.LinkId)
                                .Where(x => x.Key.HasValue)
                                .Select(x => x.Key)
                                .Aggregate(filterLinkIdContains,
                                    (current, value) => current.Or(e => e.LinkId == value).Expand());

                        var links = CommonQueries.GetDocumentQuery(dbContext, ctx, null, null, true)
                            .Where(filterLinkIdContains)
                            .GroupBy(x => x.LinkId.Value)
                            .Select(x => new {LinkId = x.Key, Count = x.Count()})
                            .ToList();

                        docs.ForEach(x =>
                        {
                            x.LinkedDocumentsCount = links.FirstOrDefault(y => y.LinkId == x.LinkId)?.Count ?? 0;
                            x.LinkedDocumentsCount = x.LinkedDocumentsCount < 2 ? 0 : x.LinkedDocumentsCount - 1;
                        });
                    }

                    docs.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

                    var acc =
                        CommonQueries.GetDocumentAccessesQuery(ctx, dbContext,
                            new FilterDocumentAccess {DocumentId = docs.Select(x => x.Id).ToList()})
                            .GroupBy(x => x.DocumentId)
                            .Select(x => new
                            {
                                DocumentId = x.Key,
                                IsFavourite = x.Any(y => y.IsFavourite),
                                IsInWork = x.Any(y => y.IsInWork),
                                NewEventCount = x.Sum(y => y.CountNewEvents),
                                //CountWaits = x.Sum(y => y.CountWaits),
                                //OverDueCountWaits = x.Sum(y => y.OverDueCountWaits),
                            }).ToList();

                    foreach (var doc in docs)
                    {
                        var docAccs = acc.FirstOrDefault(x => x.DocumentId == doc.Id);
                        if (docAccs == null)
                        {
                            doc.IsInWork = true;
                        }
                        else
                        {
                            doc.IsFavourite = docAccs.IsFavourite;
                            doc.IsInWork = docAccs.IsInWork;
                            doc.NewEventCount = (docAccs.NewEventCount ?? 0) != 0 ? docAccs.NewEventCount : null;
                            //if ((docAccs.CountWaits ?? 0) !=0)
                            //{
                            //    doc.WaitCount = new UICounters
                            //    {
                            //        Counter1 = docAccs.CountWaits,
                            //        Counter2 = (docAccs.OverDueCountWaits ?? 0) != 0 ? docAccs.OverDueCountWaits : null,
                            //    };
                            //}                           
                        }
                    }
                    docs =
                        docs.GroupJoin(
                            CommonQueries.GetDocumentTags(dbContext, ctx,
                                new FilterDocumentTag
                                {
                                    DocumentId = docs.Select(x => x.Id).ToList(),
                                    CurrentPositionsId = ctx.CurrentPositionsIdList
                                }),
                            d => d.Id,
                            t => t.DocumentId,
                            (d, t) =>
                            {
                                d.DocumentTags = t.ToList();
                                return d;
                            }).ToList();

                    #endregion model filling
                }
                transaction.Complete();
                return docs;
            }
        }

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var accs =
                    CommonQueries.GetDocumentAccessesQuery(ctx, dbContext, null).Where(x => x.DocumentId == documentId)
                        .Select(acc => new FrontDocumentAccess
                        {
                            Id = acc.Id,
                            PositionId = acc.PositionId,
                            IsInWork = acc.IsInWork,
                            DocumentId = acc.DocumentId,
                            IsFavourite = acc.IsFavourite,
                            AccessLevelId = acc.AccessLevelId,
                            AccessLevelName = acc.AccessLevel.Name,
                            CountNewEvents = acc.CountNewEvents,
                            CountWaits = acc.CountWaits,
                            OverDueCountWaits = acc.OverDueCountWaits,
                        }).ToList();

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Id,
                    DocumentDirection = (EnumDocumentDirections) doc.DocumentDirectionId,
                    DocumentDirectionName = doc.DocumentDirection.Name,
                    DocumentTypeName = doc.DocumentType.Name,
                    DocumentDate = doc.RegistrationDate ?? doc.CreateDate,
                    IsRegistered = doc.IsRegistered,
                    Description = doc.Description,
                    ExecutorPositionExecutorAgentName =
                        doc.ExecutorPositionExecutorAgent.Name +
                        (doc.ExecutorPositionExecutorType.Suffix != null
                            ? " (" + doc.ExecutorPositionExecutorType.Suffix + ")"
                            : null),
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    LinkedDocumentsCount = 0, //TODO

                    TemplateDocumentId = doc.TemplateDocumentId,
                    DocumentSubject = doc.DocumentSubject,

                    RegistrationJournalId = doc.RegistrationJournalId,
                    RegistrationJournalName = doc.RegistrationJournal.Name,
                    RegistrationNumber = doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.RegistrationNumberSuffix,
                    RegistrationDate = doc.RegistrationDate,

                    ExecutorPositionId = doc.ExecutorPositionId,
                    ExecutorPositionExecutorAgentId = doc.ExecutorPositionExecutorAgentId,

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
                res.IsInWork = accs.Any() ? accs.Any(x => x.IsInWork) : true;
                res.Accesses = accs;

                CommonQueries.ChangeRegistrationFullNumber(res, false);

                var docIds = new List<int> {res.Id};

                if (res.LinkId.HasValue)
                {
                    res.LinkedDocuments = CommonQueries.GetLinkedDocuments(ctx, dbContext, res.LinkId.Value);
                    var linkedDocumentsCount = res.LinkedDocuments.Count();
                    if (linkedDocumentsCount > 1)
                    {
                        res.LinkedDocuments =
                            res.LinkedDocuments.OrderBy(x => x.Id == documentId ? 0 : 1).ThenBy(x => x.DocumentDate);
                    }
                    res.LinkedDocumentsCount = linkedDocumentsCount < 2 ? 0 : linkedDocumentsCount - 1;
                    //if (filter?.DocumentsIdForAIP?.Count() > 0)
                    //{
                    //    docIds = filter.DocumentsIdForAIP;
                    //}
                    //else
                    {
                        docIds = res.LinkedDocuments.Select(x => x.Id).ToList();
                    }
                }

                res.SendLists = CommonQueries.GetDocumentSendList(dbContext, ctx,
                    new FilterDocumentSendList {DocumentId = new List<int> {documentId}});

                res.SendListStageMax = (res.SendLists == null) || !res.SendLists.Any()
                    ? 0
                    : res.SendLists.Max(x => x.Stage);

                res.RestrictedSendLists =
                    DmsResolver.Current.Get<IDocumentSendListsDbProcess>().GetRestrictedSendLists(ctx, documentId);

                res.DocumentTags = CommonQueries.GetDocumentTags(dbContext, ctx,
                    new FilterDocumentTag {DocumentId = docIds, CurrentPositionsId = ctx.CurrentPositionsIdList});

                res.DocumentWorkGroup = CommonQueries.GetDocumentWorkGroup(dbContext, ctx,
                    new FilterDictionaryPosition {DocumentIDs = docIds});

                res.Properties = CommonQueries.GetPropertyValues(dbContext, ctx,
                    new FilterPropertyValue
                    {
                        RecordId = new List<int> {documentId},
                        Object = new List<EnumObjects> {EnumObjects.Documents}
                    });
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<int> GetLinkedDocumentIds(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetLinkedDocumentIds(ctx, dbContext, documentId);
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ReportDocumentForDigitalSignaturePrepare(IContext ctx, DigitalSignatureDocumentPdf model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentDigitalSignaturePrepare(dbContext, ctx, model.DocumentId,
                    new List<EnumSubscriptionStates>
                    {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                    });

                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                transaction.Complete();
                return doc;
            }
        }

        public FrontReport ReportDocumentForDigitalSignature(IContext ctx, DigitalSignatureDocumentPdf model,
            bool isUseInternalSign, bool isUseCertificateSign)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                CommonQueries.GetDocumentHash(dbContext, ctx, model.DocumentId, isUseInternalSign, isUseCertificateSign,
                    null, model.ServerPath, false, false, false);

                var subscriptionStates = new List<EnumSubscriptionStates>
                {
                    EnumSubscriptionStates.Sign,
                    EnumSubscriptionStates.Visa,
                    EnumSubscriptionStates.Аgreement,
                    EnumSubscriptionStates.Аpproval
                };

                InternalDocument doc = CommonQueries.GetDocumentDigitalSignaturePrepare(dbContext, ctx, model.DocumentId,
                    subscriptionStates);

                if (model.IsAddSubscription)
                {
                    var subscriptions = doc.Subscriptions.ToList();
                    subscriptions.Add(
                        dbContext.DictionaryPositionsSet.Where(x => x.Id == ctx.CurrentPositionId)
                            .Select(
                                x =>
                                    new InternalDocumentSubscription
                                    {
                                        Id = 0,
                                        DocumentId = model.DocumentId,
                                        DoneEventSourcePositionName = x.Name,
                                        DoneEventSourcePositionExecutorAgentName = x.ExecutorAgent.Name
                                    })
                            .FirstOrDefault());
                    doc.Subscriptions = subscriptions;
                }

                var pdf = CommonQueries.GetDocumentCertificateSignPdf(dbContext, ctx, doc);
                transaction.Complete();
                return pdf;
            }
        }

        public InternalDocument ReportRegistrationCardDocumentPrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var doc = qry.Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                    IsRegistered = x.IsRegistered,
                    ExecutorPositionId = x.ExecutorPositionId,
                }).FirstOrDefault();

                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument ReportRegistrationCardDocument(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
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
                    ClientId = doc.ClientId,
                    EntityTypeId = doc.EntityTypeId,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    Addressee = doc.Addressee,
                    Description = doc.Description,
                    SenderAgentName = doc.SenderAgent?.Name,
                    SenderAgentPersonName = doc.SenderAgentPerson?.Agent.Name,
                };

                var docIds = new List<int> {res.Id};

                var maxDateTime = DateTime.UtcNow.AddYears(50);

                res.Waits =
                    CommonQueries.GetDocumentWaitQuery(ctx, dbContext,
                        new FilterDocumentWait {DocumentId = new List<int> {res.Id}})
                        .Select(x => new InternalDocumentWait
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            CreateDate = x.OnEvent.Date,
                            TargetPositionName = x.OnEvent.TargetPosition.Name,
                            TargetPositionExecutorAgentName =
                                x.OnEvent.TargetPositionExecutorAgent.Name +
                                (x.OnEvent.TargetPositionExecutorType.Suffix != null
                                    ? " (" + x.OnEvent.TargetPositionExecutorType.Suffix + ")"
                                    : null),
                            SourcePositionName = x.OnEvent.SourcePosition.Name,
                            SourcePositionExecutorAgentName =
                                x.OnEvent.SourcePositionExecutorAgent.Name +
                                (x.OnEvent.SourcePositionExecutorType.Suffix != null
                                    ? " (" + x.OnEvent.SourcePositionExecutorType.Suffix + ")"
                                    : null),
                            DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                            IsClosed = x.OffEventId != null,
                            ResultTypeName = x.ResultType.Name,
                            AttentionDate = x.AttentionDate,
                            OnEventTypeName = x.OnEvent.EventType.Name,
                            OffEventDate = x.OffEventId.HasValue ? x.OffEvent.CreateDate : (DateTime?) null
                        }).ToList();

                res.Subscriptions =
                    CommonQueries.GetDocumentSubscriptionsQuery(dbContext,
                        new FilterDocumentSubscription
                        {
                            DocumentId = new List<int> {res.Id},
                            SubscriptionStates = new List<EnumSubscriptionStates> {EnumSubscriptionStates.Sign}
                        }, ctx)
                        .Select(x => new InternalDocumentSubscription
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            SubscriptionStatesName = x.SubscriptionState.Name,
                            DoneEventSourcePositionName =
                                x.DoneEventId.HasValue ? x.DoneEvent.SourcePosition.Name : string.Empty,
                            DoneEventSourcePositionExecutorAgentName = x.DoneEventId.HasValue
                                ? x.DoneEvent.SourcePositionExecutorAgent.Name +
                                  (x.DoneEvent.SourcePositionExecutorType.Suffix != null
                                      ? " (" + x.DoneEvent.SourcePositionExecutorType.Suffix + ")"
                                      : null)
                                : string.Empty
                        }).ToList();
                transaction.Complete();
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
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var qry = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.PaperListId == paperListId);

                var res = qry.GroupBy(x => x.Document)
                    .Select(x => x.Key)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
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
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    DocumentId = x.DocumentId,
                    SourcePositionName = x.SourcePosition.Name,
                    SourcePositionExecutorAgentName =
                        x.SourcePositionExecutorAgent.Name +
                        (x.SourcePositionExecutorType.Suffix != null
                            ? " (" + x.SourcePositionExecutorType.Suffix + ")"
                            : null),
                    TargetPositionName = x.TargetPosition.Name,
                    TargetPositionExecutorAgentName =
                        x.TargetPositionExecutorAgent.Name +
                        (x.TargetPositionExecutorType.Suffix != null
                            ? " (" + x.TargetPositionExecutorType.Suffix + ")"
                            : null),
                    PaperId = x.PaperId,
                    Paper = !x.PaperId.HasValue
                        ? null
                        : new InternalDocumentPaper
                        {
                            Id = x.Paper.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.Paper.DocumentId,
                            Name = x.Paper.Name,
                            Description = x.Paper.Description
                        }
                }).ToList();

                res = res.GroupJoin(events, o => o.Id, i => i.DocumentId, (o, i) =>
                {
                    o.Events = i.ToList();
                    return o;
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument AddDocumentPrepare(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = Transactions.GetTransaction())
            {

                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.Id == templateDocumentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentSubject = x.DocumentSubject,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                        RegistrationJournalId = x.RegistrationJournalId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.Tasks =
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                        .Where(y => y.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentTask
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            Name = y.Task,
                            Description = y.Description,
                            PositionId = y.PositionId ?? 0,
                        }).ToList();

                doc.RestrictedSendLists =
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(
                        x => x.Document.ClientId == ctx.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentRestrictedSendList()
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses) y.AccessLevelId
                        }).ToList();

                doc.SendLists =
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                        .Where(y => y.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentSendList()
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            StageType = (EnumStageTypes?) y.StageTypeId,
                            SendType = (EnumSendTypes) y.SendTypeId,
                            //SourcePositionId = y.SourcePositionId??0,
                            TargetPositionId = y.TargetPositionId,
                            TargetAgentId = y.TargetAgentId,
                            TaskName = y.Task.Task,
                            IsAvailableWithinTask = y.IsAvailableWithinTask,
                            IsWorkGroup = y.IsWorkGroup,
                            IsAddControl = y.IsAddControl,
                            SelfDueDay = y.SelfDueDay,
                            SelfDescription = y.SelfDescription,
                            SelfAttentionDay = y.SelfAttentionDay,
                            Description = y.Description,
                            Stage = y.Stage,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses) y.AccessLevelId,
                        }).ToList();

                doc.DocumentFiles =
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == templateDocumentId)
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.Id,
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            DocumentId = x.DocumentId,
                            Extension = x.Extention,
                            Name = x.Name,
                            FileType = x.FileType,
                            FileSize = x.FileSize,
                            OrderInDocument = x.OrderNumber,
                            Type = (EnumFileTypes) x.TypeId,
                            Hash = x.Hash,
                            Description = x.Description,
                        }).ToList();
                doc.Papers =
                    dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentPaper
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            Name = y.Name,
                            Description = y.Description,
                            IsCopy = y.IsCopy,
                            IsMain = y.IsMain,
                            IsOriginal = y.IsOriginal,
                            OrderNumber = y.OrderNumber,
                            PageQuantity = y.PageQuantity,
                        }).ToList();
                doc.Properties =
                    CommonQueries.GetInternalPropertyValues(dbContext, ctx,
                        new FilterPropertyValue
                        {
                            Object = new List<EnumObjects> {EnumObjects.TemplateDocument},
                            RecordId = new List<int> {templateDocumentId}
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument CopyDocumentPrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                        DocumentSubject = x.DocumentSubject,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,

                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.AccessLevel =
                    (EnumDocumentAccesses)
                        CommonQueries.GetDocumentAccessesesQry(dbContext, documentId, ctx).Max(x => x.AccessLevelId);
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new InternalDocumentTask
                    {
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        Name = x.Task,
                        Description = x.Description,
                        PositionId = x.PositionId,
                    }
                    ).ToList();
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId && x.IsInitial)
                    .Select(y => new InternalDocumentSendList
                    {
                        ClientId = y.ClientId,
                        EntityTypeId = y.EntityTypeId,
                        Stage = y.Stage,
                        StageType = (EnumStageTypes?) y.StageTypeId,
                        SendType = (EnumSendTypes) y.SendTypeId,
                        SourcePositionId = y.SourcePositionId,
                        TargetPositionId = y.TargetPositionId,
                        TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
                        IsAvailableWithinTask = y.IsAvailableWithinTask,
                        IsWorkGroup = y.IsWorkGroup,
                        IsAddControl = y.IsAddControl,
                        SelfDescription = y.SelfDescription,
                        SelfDueDate = y.SelfDueDate,
                        SelfDueDay = y.SelfDueDay,
                        SelfAttentionDate = y.SelfAttentionDate,
                        SelfAttentionDay = y.SelfAttentionDay,
                        Description = y.Description,
                        DueDate = y.DueDate,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumDocumentAccesses) y.AccessLevelId,
                        IsInitial = y.IsInitial,
                    }).ToList();
                doc.RestrictedSendLists =
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentRestrictedSendList
                        {
                            ClientId = y.ClientId,
                            EntityTypeId = y.EntityTypeId,
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses) y.AccessLevelId,
                        }).ToList();
                doc.Papers = dbContext.DocumentPapersSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId)
                    .Select(y => new InternalDocumentPaper
                    {
                        ClientId = y.ClientId,
                        EntityTypeId = y.EntityTypeId,
                        Name = y.Name,
                        Description = y.Description,
                        IsCopy = y.IsCopy,
                        IsInWork = y.IsInWork,
                        IsMain = y.IsMain,
                        IsOriginal = y.IsOriginal,
                        OrderNumber = y.OrderNumber,
                        PageQuantity = y.PageQuantity,
                    }).ToList();
                doc.DocumentFiles =
                    CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId)
                        .Where(x => x.Type != EnumFileTypes.SubscribePdf)
                        .ToList();

                doc.Properties =
                    CommonQueries.GetInternalPropertyValues(dbContext, ctx,
                        new FilterPropertyValue
                        {
                            Object = new List<EnumObjects> {EnumObjects.Documents},
                            RecordId = new List<int> {documentId}
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void AddDocument(IContext ctx, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
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
                    doc.RestrictedSendLists =
                        ModelConverter.GetDbDocumentRestrictedSendLists(document.RestrictedSendLists).ToList();
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
                        if (document.Tasks?.Any(y => y.Id == 0) ?? false)
                        {
                            var taskId = doc.Tasks.Where(y => y.Task == x.TaskName).Select(y => y.Id).FirstOrDefault();
                            x.TaskId = (taskId == 0 ? null : (int?) taskId);
                        }
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
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, document.Id, EnumObjects.Documents,
                    EnumOperationType.AddFull);
                transaction.Complete();

            }
        }

        public InternalDocument ModifyDocumentPrepare(IContext ctx, ModifyDocument model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true, true, true)
                    .Where(x => x.Id == model.Id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        IsHard = x.TemplateDocument.IsHard,
                        DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                        DocumentTypeId = x.DocumentTypeId,
                        IsRegistered = x.IsRegistered,
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Accesses = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(
                        x =>
                            x.DocumentId == model.Id && x.PositionId == doc.ExecutorPositionId &&
                            x.AccessLevelId != model.AccessLevelId)
                    .Select(x => new InternalDocumentAccess
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
                        IsInWork = x.IsInWork,
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void ModifyDocument(IContext ctx, InternalDocument document, bool isUseInternalSign,
            bool isUseCertificateSign, string serverMapPath)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.DocumentSubject).IsModified = true;
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
                    var acc = ModelConverter.GetDbDocumentAccess(docAccess);
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

                CommonQueries.GetDocumentHash(dbContext, ctx, document.Id, isUseInternalSign, isUseCertificateSign, null,
                    serverMapPath, false, false); //TODO ЗАЧЕМ ТУТ ЭТО?
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, document.Id, EnumObjects.Documents,
                    EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public InternalDocument DeleteDocumentPrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true, true, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsRegistered = x.IsRegistered,
                        LinkId = x.LinkId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        WaitsCount = x.Waits.Count,
                        SubscriptionsCount = x.Subscriptions.Count,
                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, doc.Id);
                transaction.Complete();
                return doc;
            }
        }

        public void DeleteDocument(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                //ADD OTHER TABLES!!!!
                dbContext.DocumentPapersSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == id)
                    .ToList() //TODO OPTIMIZE
                    .ForEach(x =>
                    {
                        x.LastPaperEventId = null;
                    });
                //TODO придумать с удалением для полнотекста
                dbContext.DocumentEventsSet.RemoveRange(
                    dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.SaveChanges();
                dbContext.DocumentTagsSet.RemoveRange(
                    dbContext.DocumentTagsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.DocumentAccessesSet.RemoveRange(
                    dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.DocumentFilesSet.RemoveRange(
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.DocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.DocumentSendListsSet.RemoveRange(
                    dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));
                dbContext.DocumentTasksSet.RemoveRange(
                    dbContext.DocumentTasksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));

                dbContext.DocumentPapersSet.RemoveRange(
                    dbContext.DocumentPapersSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == id));

                CommonQueries.DeletePropertyValues(dbContext, ctx,
                    new FilterPropertyValue
                    {
                        Object = new List<EnumObjects> {EnumObjects.Documents},
                        RecordId = new List<int> {id}
                    });

                dbContext.DocumentsSet.RemoveRange(
                    dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.Id == id));

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, id, EnumObjects.Documents, EnumOperationType.Delete);
                transaction.Complete();

            }
        }

        public InternalDocument RegisterDocumentPrepare(IContext ctx, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, false, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentSubject = x.DocumentSubject,
                        Description = x.Description,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        SenderNumber = x.SenderNumber,
                        SenderDate = x.SenderDate,
                        Addressee = x.Addressee,
                        LinkId = x.LinkId,

                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }
                var strDocumentDirection = ((int) doc.DocumentDirection).ToString();
                doc.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.SubscriptionState.IsSuccess)
                    .Select(x => new InternalDocumentSubscription
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        SubscriptionStatesId = x.SubscriptionStateId,
                        SubscriptionStatesIsSuccess = x.SubscriptionState.IsSuccess,
                        DoneEvent = new InternalDocumentEvent
                        {
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            SourcePositionId = x.DoneEvent.SourcePositionId,
                        }
                    }).ToList();
                var regJournal =
                    dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.Id == model.RegistrationJournalId)
                        .Where(x => x.DirectionCodes.Contains(strDocumentDirection))
                        .Where(x => dbContext.AdminRegistrationJournalPositionsSet
                            .Where(
                                y =>
                                    y.PositionId == ctx.CurrentPositionId &&
                                    y.RegJournalAccessTypeId == (int) EnumRegistrationJournalAccessTypes.Registration)
                            .Select(y => y.RegJournalId).Contains(x.Id))
                        .Select(x => new {x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula})
                        .FirstOrDefault();

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
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocumnRegistration RegisterModelDocumentPrepare(IContext ctx, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new
                    {
                        DocumentId = x.Id,
                        LinkId = x.LinkId,
                        SenderAgentId = x.SenderAgentId,
                        ExecutorPositionDepartmentCode = x.ExecutorPosition.Department.Code,
                        SubscriptionsPositionDepartmentCode = x.Subscriptions
                            .Where(y => y.SubscriptionStateId == (int) EnumSubscriptionStates.Sign)
                            .OrderBy(y => y.LastChangeDate).Take(1)
                            .Select(y => y.DoneEvent.SourcePosition.Department.Code).FirstOrDefault(),
                        DocumentSendListLastAgentExternalFirstSymbolName = x.SendLists
                            .Where(y => y.SendTypeId == (int) EnumSendTypes.SendForInformationExternal)
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
                    DocumentSendListLastAgentExternalFirstSymbolName =
                        doc.DocumentSendListLastAgentExternalFirstSymbolName
                };

                if (!string.IsNullOrEmpty(res.DocumentSendListLastAgentExternalFirstSymbolName))
                    res.DocumentSendListLastAgentExternalFirstSymbolName =
                        res.DocumentSendListLastAgentExternalFirstSymbolName.Substring(0, 1);

                //TODO ??? если doc.LinkId==null || doc.SenderAgentId ==null
                res.OrdinalNumberDocumentLinkForCorrespondent = dbContext.DocumentsSet.Where(
                    x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.LinkId == doc.LinkId && x.SenderAgentId == doc.SenderAgentId && x.IsRegistered == true)
                    .Count() + 1;

                var regJournal =
                    dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.Id == model.RegistrationJournalId)
                        .Select(
                            x =>
                                new
                                {
                                    x.Id,
                                    x.NumerationPrefixFormula,
                                    x.PrefixFormula,
                                    x.SuffixFormula,
                                    x.Index,
                                    RegistrationJournalDepartmentCode = x.Department.Code
                                }).FirstOrDefault();

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

                var initiativeDoc = dbContext.DocumentLinksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == doc.DocumentId)
                    .OrderBy(x => x.LastChangeDate)
                    .Select(x => x.ParentDocument)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
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

                res.CurrentPositionDepartmentCode =
                    dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                        .Where(x => x.Id == model.CurrentPositionId)
                        .Select(x => x.Department.Code).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public void RegisterDocument(IContext ctx, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
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
                    dbContext.DocumentEventsSet.RemoveRange(
                        dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                            .Where(x => x.DocumentId == document.Id && x.EventTypeId == (int) EnumEventTypes.Registered));
                }

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, document.Id, EnumObjects.Documents,
                    EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                //get next number
                var maxNumber = (from docreg in dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
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
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = !dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
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
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(
                        x =>
                            x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId &&
                            x.TypeId == (int) EnumFileTypes.Main) // !x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                doc.RestrictedSendLists =
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == model.DocumentId)
                        .Select(x => new InternalDocumentRestrictedSendList
                        {
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PositionId = x.PositionId,
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument ChangePositionDocumentPrepare(IContext ctx, ChangePosition model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true, true, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(
                        x =>
                            x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId &&
                            x.TypeId == (int) EnumFileTypes.Main) //!x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void ChangeExecutorDocument(IContext ctx, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.ExecutorPositionId).IsModified = true;
                entry.Property(x => x.ExecutorPositionExecutorAgentId).IsModified = true;
                entry.Property(x => x.ExecutorPositionExecutorTypeId).IsModified = true;


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

                if (document.Papers != null &&
                    document.Papers.Any(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null))
                {
                    foreach (
                        var paper in
                            document.Papers.Where(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null)
                                .ToList())
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
                        entryFile.Property(e => e.ExecutorPositionExecutorTypeId).IsModified = true;
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
                        entryTask.Property(e => e.PositionExecutorTypeId).IsModified = true;
                        entryTask.Property(e => e.LastChangeUserId).IsModified = true;
                        entryTask.Property(e => e.LastChangeDate).IsModified = true;
                        dbContext.SaveChanges();
                    }
                }
                dbContext.SaveChanges();
                CommonQueries.ModifyDocumentAccessesStatistics(dbContext, ctx, document.Id);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, document.Id, EnumObjects.Documents,
                    EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void ChangePositionDocument(IContext ctx, ChangePosition model, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.Id == model.DocumentId && x.ExecutorPositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.ExecutorPositionId = model.NewPositionId;
                        //TODO исполнители по должности!!!
                    });
                dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.ExecutorPositionId = model.NewPositionId;
                    });
                dbContext.DocumentTasksSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.PositionId = model.NewPositionId;
                    });
                dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SourcePositionId = model.NewPositionId;
                    });
                dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.TargetPositionId = model.NewPositionId;
                    });
                dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SourcePositionId = model.NewPositionId;
                    });
                dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.TargetPositionId = model.NewPositionId;
                    });
                dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.PositionId = model.NewPositionId;
                    });
                dbContext.DocumentAccessesSet.RemoveRange(
                    dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.PositionId = model.NewPositionId;
                    });
                dbContext.DocumentTaskAccessesSet.RemoveRange(
                    dbContext.DocumentTaskAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                dbContext.DocumentTaskAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.PositionId = model.NewPositionId;
                    });

                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(
                        ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                }

                dbContext.SaveChanges();
                CommonQueries.ModifyDocumentAccessesStatistics(dbContext, ctx, document.Id);
                dbContext.SaveChanges();
                transaction.Complete();

            }
        }

        public InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true, true, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsLaunchPlan = x.IsLaunchPlan
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new InternalDocumentSendList
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }
                    ).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
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

        #region DocumentPapers

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter,
            UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentPapers(dbContext, ctx, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentPaper GetDocumentPaper(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    CommonQueries.GetDocumentPapers(dbContext, ctx, new FilterDocumentPaper {Id = new List<int> {id}},
                        null).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentPapers   

        #region DocumentPaperLists

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter,
            UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentPaperLists(dbContext, ctx, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentPaperList GetDocumentPaperList(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    CommonQueries.GetDocumentPaperLists(dbContext, ctx,
                        new FilterDocumentPaperList {PaperListId = new List<int> {id}}, null).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentPaperLists   

        #region DocumentAccesses

        public IEnumerable<InternalDocumentAccess> CheckIsInWorkForControlsPrepare(IContext ctx,
            FilterDocumentAccess filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentAccessesQuery(ctx, dbContext, filter, true)
                    .Where(x => !x.IsInWork && x.CountWaits > 0);

                var res =
                    qry.Select(x => new InternalDocumentAccess {DocumentId = x.DocumentId, PositionId = x.PositionId})
                        .ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filter,
            UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentAccessesQuery(ctx, dbContext, filter);
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
                        var skip = paging.PageSize*(paging.CurrentPage - 1);
                        var take = paging.PageSize;
                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }
                if ((paging?.IsAll ?? true) && (filter == null || ((filter.DocumentId?.Count ?? 0) == 0)))
                {
                    throw new WrongAPIParameters();
                }
                var res = qry.Select(acc => new FrontDocumentAccess
                {
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevelId = acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name,
                    CountNewEvents = acc.CountNewEvents,
                    CountWaits = acc.CountWaits,
                    OverDueCountWaits = acc.OverDueCountWaits,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentAccesses

        public IEnumerable<InternalDocumentEvent> GetEventsNatively(IContext ctx, FilterDocumentEventNatively filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetEventsNativelyQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalDocumentEvent
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
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
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetEventsNativelyQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

    }
}