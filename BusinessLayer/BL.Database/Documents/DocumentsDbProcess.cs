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
using BL.Model.Common;
using EntityFramework.Extensions;

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        public DocumentsDbProcess()
        {
        }

        public void GetCountDocuments(IContext context, LicenceInfo licence)
        {
            if (licence == null)
            {
                throw new LicenceError();
            }

            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id).AsQueryable(); //Without security restrictions

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

        public int GetDocumentIdBySendListId(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    dbContext.DocumentSendListsSet.Where(x => x.Id == id).Select(x => x.DocumentId).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext context, FilterBase filter, UIPaging paging, EnumGroupCountType? groupCountType = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                IQueryable<DBModel.Document.Documents> qry = null;
                List<FrontDocument> docs = null;

                #region Position filters for counters preparing

                var filterAccessPositionsContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionsContains = context.CurrentPositionsAccessLevel.Aggregate(filterAccessPositionsContains,
                    (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand());

                //var filterWaitPositionsContains = PredicateBuilder.New<DocumentWaits>();
                //filterWaitPositionsContains = context.CurrentPositionsIdList.Aggregate(filterWaitPositionsContains,
                //    (current, value) =>
                //        current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value)
                //            .Expand());

                #endregion Position filters for counters preparing

                #region QueryTypes

                if (paging.Sort == EnumSort.IncomingIds && filter?.Document?.DocumentId?.Count() > 0)
                {
                    #region IncomingIds

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
                        docs = new List<FrontDocument>();
                    }
                    else
                    {
                        if (!paging.IsAll)
                        {
                            docIds =
                                docIds.Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                        }

                        if (docIds.Count > 0)
                        {
                            var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                            filterContains = docIds.Aggregate(filterContains,
                                (current, value) => current.Or(e => e.Id == value).Expand());

                            qry = dbContext.DocumentsSet.Where(filterContains); //Without security restrictions
                        }
                        else
                        {
                            qry = dbContext.DocumentsSet.Where(x => false);//Without security restrictions
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
                            .Select(x => new { Id = x.Key, Count = x.Count() })
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
                            .Select(x => new { Id = x.Key, Count = x.Count() })
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
                                (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : (string)null),
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

                    #endregion groupCount
                    else
                    {
                        if ((paging.IsOnlyCounter ?? true) && !filter.FullTextSearchSearch.IsNotAll)
                        {
                            var ftDocs = filter.FullTextSearchSearch.FullTextSearchResult.GroupBy(x => x.ParentId);
                            paging.TotalItemsCount = ftDocs.Count();
                            if (paging.IsCalculateAddCounter ?? false)
                            {
                                var ftDocsAdd = ftDocs.Select(x => string.Join(" ", x.Select(y => y.Security))).ToList();
                                var accF = context.GetAccessFilterForFullText($".{FullTextFilterTypes.IsFavourite}..");
                                var accN = context.GetAccessFilterForFullText($"..{FullTextFilterTypes.IsEventNew}.");
                                var accC = context.GetAccessFilterForFullText($"...{FullTextFilterTypes.IsWaitOpened}");
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
                                    docIds.Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize).ToList();
                            }

                            if (docIds.Count > 0)
                            {
                                var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                                filterContains = docIds.Aggregate(filterContains,
                                    (current, value) => current.Or(e => e.Id == value).Expand());

                                qry = dbContext.DocumentsSet.Where(filterContains) //Without security restrictions
                                    .OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.Id);
                            }
                            else
                            {
                                qry = dbContext.DocumentsSet.Where(x => false); //Without security restrictions
                            }
                        }
                    }

                    #endregion FullTextSearchDocumentId
                }
                else
                {
                    #region Main branch

                    #region main qry

                    qry = CommonQueries.GetDocumentQuery(context, filter?.Document, false);

                    if (filter?.File != null)
                    {
                        var files = CommonQueries.GetDocumentFileQuery(context, filter?.File);
                        qry = qry.Where(x => files.Select(y => y.DocumentId).Contains(x.Id));
                    }

                    if (filter?.Event != null)
                    {
                        var events = CommonQueries.GetDocumentEventQuery(context, filter?.Event);
                        qry = qry.Where(x => events.Select(y => y.DocumentId).Contains(x.Id));
                    }

                    if (filter?.Wait != null)
                    {
                        var waits = CommonQueries.GetDocumentWaitQuery(context, filter?.Wait);
                        qry = qry.Where(x => waits.Select(y => y.DocumentId).Contains(x.Id));
                    }

                    #endregion main qry

                    #region groupCount

                    if (groupCountType == EnumGroupCountType.Tags)
                    {
                        //var qryT = qry;
                        //if (filter?.Document?.TagId?.Count > 1)
                        //{
                        //    qryT = qryT.Where(x => x.Tags.Count == filter.Document.TagId.Count);
                        //}
                        var qryTagCounters = dbContext.DictionaryTagsSet.Select(x => new FrontDocumentTag
                        {
                            TagId = x.Id,
                            PositionId = x.PositionId,
                            PositionName = x.Position.Name,
                            Color = x.Color,
                            Name = x.Name,
                            IsSystem = !x.PositionId.HasValue,
                            DocCount = x.Documents.Count(y => qry.Select(z => z.Id).Contains(y.DocumentId))
                        }).Where(x => x.DocCount > 0);
                        var tagCounters = qryTagCounters.OrderBy(x => x.Name).ToList();
                        docs = new List<FrontDocument> { new FrontDocument { DocumentTags = tagCounters } };
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
                                    (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : (string)null),
                                DocCount = x.DocumentAccesses.Count(y => qry.Select(z => z.Id).Contains(y.DocumentId))
                            }).Where(x => x.DocCount > 0);
                        var positionCounters = qryPositionCounters.OrderBy(x => x.ExecutorAgentName).ToList();
                        docs = new List<FrontDocument> { new FrontDocument { DocumentWorkGroup = positionCounters } };
                    }

                    #endregion groupCount

                    #region Paging
                    else
                    {
                        if (paging.IsOnlyCounter ?? true)
                        {
                            if (!(paging.IsCalculateAddCounter ?? false))
                            {
                                paging.TotalItemsCount = qry.Count();
                            }
                            else
                            {
                                var qryAcc = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
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
                                var skip = paging.PageSize * (paging.CurrentPage - 1);
                                var take = paging.PageSize;

                                qry = qry.Skip(() => skip).Take(() => take);
                            }
                        }
                    }
                    #endregion Paging

                    #endregion Main branch
                }

                #endregion QueryTypes

                if (docs == null)
                {
                    if ((paging?.IsAll ?? true) && (filter?.Document == null || ((filter.Document.DocumentId?.Count ?? 0) == 0)))
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

                        //WaitCount =
                        //    doc.Waits.AsQueryable()
                        //        .Where(filterWaitPositionsContains)
                        //        .Where(x => !x.OffEventId.HasValue)
                        //        .GroupBy(x => x.DocumentId)
                        //        .Select(
                        //            x =>
                        //                new UICounters
                        //                {
                        //                    Counter1 = x.Count(),
                        //                    Counter2 =
                        //                        x.Count(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow)
                        //                })
                        //        .FirstOrDefault(),

                        //NewEventCount = doc.Events.AsQueryable().Where(filterNewEventContains).Count(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId),

                        AttachedFilesCount =
                            doc.Files.Count(
                                fl => fl.IsMainVersion && !fl.IsDeleted && fl.TypeId != (int)EnumFileTypes.SubscribePdf),

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

                        var links = CommonQueries.GetDocumentQuery(context, null)
                            .Where(filterLinkIdContains)
                            .GroupBy(x => x.LinkId.Value)
                            .Select(x => new { LinkId = x.Key, Count = x.Count() })
                            .ToList();

                        docs.ForEach(x =>
                        {
                            x.LinkedDocumentsCount = links.FirstOrDefault(y => y.LinkId == x.LinkId)?.Count ?? 0;
                            x.LinkedDocumentsCount = x.LinkedDocumentsCount < 2 ? 0 : x.LinkedDocumentsCount - 1;
                        });
                    }

                    docs.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));

                    var acc = CommonQueries.GetDocumentAccessesQuery(context, new FilterDocumentAccess { DocumentId = docs.Select(x => x.Id).ToList() })
                            .GroupBy(x => x.DocumentId)
                            .Select(x => new
                            {
                                DocumentId = x.Key,
                                IsFavourite = x.Any(y => y.IsFavourite),
                                IsInWork = x.Any(y => y.IsInWork),
                                NewEventCount = x.Sum(y => y.CountNewEvents),
                                CountWaits = x.Sum(y => y.CountWaits),
                                OverDueCountWaits = x.Sum(y => y.OverDueCountWaits),
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
                            if ((docAccs.CountWaits ?? 0) != 0)
                            {
                                doc.WaitCount = new UICounters
                                {
                                    Counter1 = docAccs.CountWaits,
                                    Counter2 = (docAccs.OverDueCountWaits ?? 0) != 0 ? docAccs.OverDueCountWaits : null,
                                };
                            }
                        }
                    }
                    docs =                              //TODO DEL???
                        docs.GroupJoin(
                            GetDocumentTags(context,
                                new FilterDocumentTag
                                {
                                    DocumentId = docs.Select(x => x.Id).ToList(),
                                    //CurrentPositionsId = context.CurrentPositionsIdList
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

        public FrontDocument GetDocument(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsIgnoreRegistered = true, }, false);
                var accs = CommonQueries.GetDocumentAccessesQuery(context, null)
                        .Where(x => x.DocumentId == documentId)
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
                    DocumentDirection = (EnumDocumentDirections)doc.DocumentDirectionId,
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

                CommonQueries.SetRegistrationFullNumber(res, false);

                var docIds = new List<int> { res.Id };

                if (res.LinkId.HasValue)
                {
                    res.LinkedDocuments = GetLinkedDocuments(context, res.LinkId.Value);                    //TODO DEL!!!!
                    var linkedDocumentsCount = res.LinkedDocuments.Count();
                    if (linkedDocumentsCount > 1)
                    {
                        res.LinkedDocuments =
                            res.LinkedDocuments.OrderBy(x => x.Id == documentId ? 0 : 1).ThenBy(x => x.DocumentDate);
                    }
                    res.LinkedDocumentsCount = linkedDocumentsCount < 2 ? 0 : linkedDocumentsCount - 1;
                    docIds = res.LinkedDocuments.Select(x => x.Id).ToList();

                }
                var sendListDbProcess = DmsResolver.Current.Get<IDocumentSendListsDbProcess>();
                res.SendLists = sendListDbProcess.GetSendLists(context, documentId);                        //TODO DEL!!!!

                res.SendListStageMax = (res.SendLists == null) || !res.SendLists.Any()                      //TODO ????
                    ? 0
                    : res.SendLists.Max(x => x.Stage);

                res.RestrictedSendLists = sendListDbProcess.GetRestrictedSendLists(context, documentId);    //TODO DEL!!!!

                var docOperDbProcess = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();             //TODO DEL!!!!
                res.DocumentWorkGroup = docOperDbProcess.GetDocumentWorkGroup                               //TODO DEL!!!!
                    (context, new FilterDictionaryPosition { DocumentIDs = docIds });

                res.DocumentTags = GetDocumentTags(context,                                                //TODO DEL!!!!
                    new FilterDocumentTag { DocumentId = new List<int> { res.Id }/*, CurrentPositionsId = context.CurrentPositionsIdList*/ });
                res.Properties = CommonQueries.GetPropertyValues(context,
                    new FilterPropertyValue
                    {
                        RecordId = new List<int> { documentId },
                        Object = new List<EnumObjects> { EnumObjects.Documents }
                    });
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentTag> GetDocumentTags(IContext context, FilterDocumentTag filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentTagsQuery(context, filter);
                var res = qry.Select(x => new FrontDocumentTag
                {
                    TagId = x.TagId,
                    DocumentId = x.DocumentId,
                    PositionId = x.Tag.PositionId,
                    PositionName = x.Tag.Position.Name,
                    Color = x.Tag.Color,
                    Name = x.Tag.Name,
                    IsSystem = !x.Tag.PositionId.HasValue
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public int GetDocumentTagsCounter(IContext context, FilterDocumentTag filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentTagsQuery(context, filter);
                var res = qry.Count();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontDocument> GetLinkedDocuments(IContext context, int linkId)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                //var acc = CommonQueries.GetDocumentAccesses(context, dbContext, true);
                var filterAccessPositionContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionContains = context.CurrentPositionsAccessLevel.Aggregate(filterAccessPositionContains,
                    (current, value) => current.Or(e => (e.PositionId == value.Key && e.AccessLevelId >= value.Value)).Expand());

                var items = CommonQueries.GetDocumentQuery(context, new FilterDocument { LinkId = new List<int> { linkId } })
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
                                ExecutorPositionId = y.ExecutorPositionId,
                                ExecutorPositionName = y.ExecutorPosition.Name,
                                ExecutorPositionExecutorAgentId = y.ExecutorPositionExecutorAgentId,
                                ExecutorPositionExecutorAgentName = y.ExecutorPositionExecutorAgent.Name + (y.ExecutorPositionExecutorType.Suffix != null ? " (" + y.ExecutorPositionExecutorType.Suffix + ")" : null),
                                Links = y.LinksDocuments
                                        .Where(z => z.Document.Accesses.AsQueryable().Any(filterAccessPositionContains))
                                        .Where(z => z.ParentDocument.Accesses.AsQueryable().Any(filterAccessPositionContains))
                                        .OrderBy(z => z.LastChangeDate)
                                        .Select(z => new FrontDocumentLink
                                        {
                                            Id = z.Id,
                                            DocumentId = z.DocumentId,
                                            ParentDocumentId = z.ParentDocumentId,
                                            LinkTypeName = z.LinkType.Name,
                                            IsParent = true,
                                            RegistrationNumber = z.ParentDocument.RegistrationNumber,
                                            RegistrationNumberPrefix = z.ParentDocument.RegistrationNumberPrefix,
                                            RegistrationNumberSuffix = z.ParentDocument.RegistrationNumberSuffix,
                                            RegistrationFullNumber = "#" + z.ParentDocument.Id.ToString(),
                                            DocumentDate = (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate),
                                            ExecutorPositionId = z.ExecutorPositionId,
                                            ExecutorPositionName = z.ExecutorPosition.Name,
                                            ExecutorPositionExecutorAgentId = z.ExecutorPositionExecutorAgentId,
                                            ExecutorPositionExecutorAgentName = z.ExecutorPositionExecutorAgent.Name + (z.ExecutorPositionExecutorType.Suffix != null ? " (" + z.ExecutorPositionExecutorType.Suffix + ")" : null),
                                        }).Concat
                                        (y.LinksParentDocuments.OrderBy(z => z.LastChangeDate)
                                        .Select(z => new FrontDocumentLink
                                        {
                                            Id = z.Id,
                                            DocumentId = z.DocumentId,
                                            ParentDocumentId = z.ParentDocumentId,
                                            LinkTypeName = z.LinkType.Name,
                                            IsParent = false,
                                            RegistrationNumber = z.Document.RegistrationNumber,
                                            RegistrationNumberPrefix = z.Document.RegistrationNumberPrefix,
                                            RegistrationNumberSuffix = z.Document.RegistrationNumberSuffix,
                                            RegistrationFullNumber = "#" + z.Document.Id.ToString(),
                                            DocumentDate = (z.Document.RegistrationDate ?? z.Document.CreateDate),
                                            ExecutorPositionId = z.ExecutorPositionId,
                                            ExecutorPositionName = z.ExecutorPosition.Name,
                                            ExecutorPositionExecutorAgentId = z.ExecutorPositionExecutorAgentId,
                                            ExecutorPositionExecutorAgentName = z.ExecutorPositionExecutorAgent.Name + (z.ExecutorPositionExecutorType.Suffix != null ? " (" + z.ExecutorPositionExecutorType.Suffix + ")" : null),
                                        })),
                            }).ToList();
                var docOperDbProcess = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();
                items.ForEach(x =>
                {
                    CommonQueries.SetRegistrationFullNumber(x);
                    var links = x.Links.ToList();
                    links.ForEach(y =>
                    {
                        CommonQueries.SetRegistrationFullNumber(y);
                        y.CanDelete = context.CurrentPositionsIdList.Contains(y.ExecutorPositionId ?? (int)EnumSystemPositions.AdminPosition);
                    });
                    x.Links = links;
                    x.DocumentWorkGroup = docOperDbProcess.GetDocumentWorkGroup(context, new FilterDictionaryPosition { DocumentIDs = new List<int> { x.Id } });
                    //TODO x.Accesses = acc.Where(y => y.DocumentId == x.Id).ToList();
                });

                transaction.Complete();
                return items;
            }
        }

        public FrontDocumentLinkShot GetLinkedDocumentIds(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetLinkedDocumentIds(context, documentId);
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ReportDocumentForDigitalSignaturePrepare(IContext context, DigitalSignatureDocumentPdf model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentDigitalSignaturePrepare(context, model.DocumentId,
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

        public FrontReport ReportDocumentForDigitalSignature(IContext context, DigitalSignatureDocumentPdf model,
            bool isUseInternalSign, bool isUseCertificateSign)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                CommonQueries.GetDocumentHash(context, model.DocumentId, isUseInternalSign, isUseCertificateSign,
                    null, model.ServerPath, false, false, false);

                var subscriptionStates = new List<EnumSubscriptionStates>
                {
                    EnumSubscriptionStates.Sign,
                    EnumSubscriptionStates.Visa,
                    EnumSubscriptionStates.Аgreement,
                    EnumSubscriptionStates.Аpproval
                };

                InternalDocument doc = CommonQueries.GetDocumentDigitalSignaturePrepare(context, model.DocumentId,
                    subscriptionStates);

                if (model.IsAddSubscription)
                {
                    var subscriptions = doc.Subscriptions.ToList();
                    subscriptions.Add(
                        dbContext.DictionaryPositionsSet.Where(x => x.Id == context.CurrentPositionId)
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

                var pdf = CommonQueries.GetDocumentCertificateSignPdf(context, doc);
                transaction.Complete();
                return pdf;
            }
        }

        public InternalDocument ReportRegistrationCardDocumentPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId } })
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
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

        public InternalDocument ReportRegistrationCardDocument(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId } })
                    .Select(doc => new InternalDocument
                    {
                        Id = doc.Id,
                        ClientId = doc.ClientId,
                        EntityTypeId = doc.EntityTypeId,
                        DocumentTypeName = doc.TemplateDocument.DocumentType.Name,
                        ExecutorPositionName = doc.ExecutorPosition.Name,
                        Addressee = doc.Addressee,
                        Description = doc.Description,
                        SenderAgentName = doc.SenderAgent.Name,
                        SenderAgentPersonName = doc.SenderAgentPerson.Agent.Name,
                    })
                    .FirstOrDefault();
                if (res == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                var docIds = new List<int> { res.Id };

                var maxDateTime = DateTime.UtcNow.AddYears(50);

                res.Waits = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { DocumentId = new List<int> { res.Id } })
                        .Select(x => new InternalDocumentWait
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            CreateDate = x.OnEvent.Date,
                            //TargetPositionName = x.OnEvent.TargetPosition.Name,
                            //TargetPositionExecutorAgentName =
                            //    x.OnEvent.TargetPositionExecutorAgent.Name +
                            //    (x.OnEvent.TargetPositionExecutorType.Suffix != null
                            //        ? " (" + x.OnEvent.TargetPositionExecutorType.Suffix + ")"
                            //        : null),
                            //SourcePositionName = x.OnEvent.SourcePosition.Name,
                            //SourcePositionExecutorAgentName =
                            //    x.OnEvent.SourcePositionExecutorAgent.Name +
                            //    (x.OnEvent.SourcePositionExecutorType.Suffix != null
                            //        ? " (" + x.OnEvent.SourcePositionExecutorType.Suffix + ")"
                            //        : null),
                            DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                            IsClosed = x.OffEventId != null,
                            ResultTypeName = x.ResultType.Name,
                            AttentionDate = x.AttentionDate,
                            OnEventTypeName = x.OnEvent.EventType.Name,
                            OffEventDate = x.OffEventId.HasValue ? x.OffEvent.CreateDate : (DateTime?)null
                        }).ToList();

                res.Subscriptions =
                    CommonQueries.GetDocumentSubscriptionsQuery(context,
                        new FilterDocumentSubscription
                        {
                            DocumentId = new List<int> { res.Id },
                            SubscriptionStates = new List<EnumSubscriptionStates> { EnumSubscriptionStates.Sign }
                        })
                        .Select(x => new InternalDocumentSubscription
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            SubscriptionStatesName = x.SubscriptionState.Name,
                            //DoneEventSourcePositionName =
                            //    x.DoneEventId.HasValue ? x.DoneEvent.SourcePosition.Name : string.Empty,
                            //DoneEventSourcePositionExecutorAgentName = x.DoneEventId.HasValue
                            //    ? x.DoneEvent.SourcePositionExecutorAgent.Name +
                            //      (x.DoneEvent.SourcePositionExecutorType.Suffix != null
                            //          ? " (" + x.DoneEvent.SourcePositionExecutorType.Suffix + ")"
                            //          : null)
                            //    : string.Empty
                        }).ToList();
                transaction.Complete();
                return res;
            }
        }

        //public InternalDocument ReportTransmissionDocumentPaperEventPrepare(IContext context, int documentId)
        //{
        //    var dbContext = context.DbContext as DmsContext;
        //    {
        //        var qry = CommonQueries.GetDocumentQuery(dbContext, context).Where(x => x.Doc.Id == documentId);

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

        public List<InternalDocument> ReportRegisterTransmissionDocuments(IContext context, int paperListId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var qry = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
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
                    //SourcePositionName = x.SourcePosition.Name,
                    //SourcePositionExecutorAgentName =
                    //    x.SourcePositionExecutorAgent.Name +
                    //    (x.SourcePositionExecutorType.Suffix != null
                    //        ? " (" + x.SourcePositionExecutorType.Suffix + ")"
                    //        : null),
                    //TargetPositionName = x.TargetPosition.Name,
                    //TargetPositionExecutorAgentName =
                    //    x.TargetPositionExecutorAgent.Name +
                    //    (x.TargetPositionExecutorType.Suffix != null
                    //        ? " (" + x.TargetPositionExecutorType.Suffix + ")"
                    //        : null),
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

        public InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
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
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                        RegistrationJournalId = x.RegistrationJournalId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.Tasks =
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id)
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
                        x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentRestrictedSendList()
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            PositionId = y.PositionId,
                            AccessLevel = (EnumAccessLevels)y.AccessLevelId
                        }).ToList();

                doc.SendLists =
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(y => y.DocumentId == templateDocumentId)
                        .Select(y => new InternalDocumentSendList()
                        {
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            StageType = (EnumStageTypes?)y.StageTypeId,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            //SourcePositionId = y.SourcePositionId??0,
                            TargetPositionId = y.TargetPositionId,
                            TargetAgentId = y.TargetAgentId,
                            TaskName = y.Task.Task,
                            IsWorkGroup = y.IsWorkGroup,
                            IsAddControl = y.IsAddControl,
                            SelfDueDay = y.SelfDueDay,
                            SelfDescription = y.SelfDescription,
                            SelfAttentionDay = y.SelfAttentionDay,
                            Description = y.Description,
                            Stage = y.Stage,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumAccessLevels)y.AccessLevelId,
                        }).ToList();

                doc.DocumentFiles =
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == templateDocumentId)
                        .Select(x => new InternalDocumentFile
                        {
                            Id = x.Id,
                            ClientId = doc.ClientId,
                            EntityTypeId = doc.EntityTypeId,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber,
                            Type = (EnumFileTypes)x.TypeId,
                            Hash = x.Hash,
                            Description = x.Description,
                            File = new BaseFile
                            {
                                Extension = x.Extention,
                                Name = x.Name,
                                FileType = x.FileType,
                                FileSize = x.FileSize,
                            }
                        }).ToList();
                doc.Papers =
                    dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id)
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
                    CommonQueries.GetInternalPropertyValues(context,
                        new FilterPropertyValue
                        {
                            Object = new List<EnumObjects> { EnumObjects.TemplateDocument },
                            RecordId = new List<int> { templateDocumentId }
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument CopyDocumentPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId } })
                    .Select(x => new InternalDocument
                    {
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
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
                    (EnumAccessLevels)
                        CommonQueries.GetDocumentAccessesesQry(context, documentId).Max(x => x.AccessLevelId);
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
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
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == documentId && x.IsInitial)
                    .Select(y => new InternalDocumentSendList
                    {
                        ClientId = y.ClientId,
                        EntityTypeId = y.EntityTypeId,
                        Stage = y.Stage,
                        StageType = (EnumStageTypes?)y.StageTypeId,
                        SendType = (EnumSendTypes)y.SendTypeId,
                        //SourcePositionId = y.SourcePositionId,
                        //TargetPositionId = y.TargetPositionId,
                        //TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
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
                        AccessLevel = (EnumAccessLevels)y.AccessLevelId,
                        IsInitial = y.IsInitial,
                    }).ToList();
                doc.Papers = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id)
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
                    CommonQueries.GetInternalDocumentFiles(context, documentId)
                        .Where(x => x.Type != EnumFileTypes.SubscribePdf)
                        .ToList();

                doc.Properties =
                    CommonQueries.GetInternalPropertyValues(context,
                        new FilterPropertyValue
                        {
                            Object = new List<EnumObjects> { EnumObjects.Documents },
                            RecordId = new List<int> { documentId }
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void AddDocument(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
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
                            x.TaskId = (taskId == 0 ? null : (int?)taskId);
                        }
                    });
                    sendListsDb = ModelConverter.GetDbDocumentSendLists(sendLists, true).ToList();
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
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents,
                    EnumOperationType.AddFull);
                transaction.Complete();

            }
        }

        public InternalDocument ModifyDocumentPrepare(IContext context, ModifyDocument model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.Id }, IsIgnoreRegistered = true, IsExecutorPosition = true, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        IsHard = x.TemplateDocument.IsHard,
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                        DocumentTypeId = x.DocumentTypeId,
                        IsRegistered = x.IsRegistered,
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Accesses = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
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
                        AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                        IsInWork = x.IsInWork,
                    }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public void ModifyDocument(IContext context, InternalDocument document, bool isUseInternalSign,
            bool isUseCertificateSign, string serverMapPath)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.SafeAttach(doc);
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
                    dbContext.SafeAttach(acc);
                    var entryAcc = dbContext.Entry(acc);
                    entryAcc.Property(x => x.LastChangeDate).IsModified = true;
                    entryAcc.Property(x => x.LastChangeUserId).IsModified = true;
                    entryAcc.Property(x => x.AccessLevelId).IsModified = true;
                }

                if (document.Properties != null)
                {
                    CommonQueries.ModifyPropertyValues(context,
                        new InternalPropertyValues
                        {
                            Object = EnumObjects.Documents,
                            RecordId = document.Id,
                            PropertyValues = document.Properties
                        });
                }
                dbContext.SaveChanges();

                CommonQueries.GetDocumentHash(context, document.Id, isUseInternalSign, isUseCertificateSign, null,
                    serverMapPath, false, false); //TODO ЗАЧЕМ ТУТ ЭТО?
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents,
                    EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public InternalDocument DeleteDocumentPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsExecutorPosition = true, IsInWork = true })
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

                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(context, doc.Id);
                transaction.Complete();
                return doc;
            }
        }

        public void DeleteDocument(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                //ADD OTHER TABLES!!!!
                dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == id)
                    .Update(x => new DocumentPapers { LastPaperEventId = null });
                dbContext.DocumentFileLinksSet.RemoveRange(dbContext.DocumentFileLinksSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Event.DocumentId == id));
                dbContext.DocumentFileLinksSet.RemoveRange(dbContext.DocumentFileLinksSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.File.DocumentId == id));
                dbContext.DocumentFilesSet.RemoveRange(dbContext.DocumentFilesSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentEventAccessesSet.RemoveRange(dbContext.DocumentEventAccessesSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentEventAccessGroupsSet.RemoveRange(
                    dbContext.DocumentEventAccessGroupsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                //TODO придумать с удалением для полнотекста
                dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.SaveChanges();

                dbContext.DocumentTagsSet.RemoveRange(
                    dbContext.DocumentTagsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentAccessesSet.RemoveRange(
                    dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentSendListAccessGroupsSet.RemoveRange(
                    dbContext.DocumentSendListAccessGroupsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentSendListsSet.RemoveRange(
                    dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentTasksSet.RemoveRange(
                    dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.DocumentPapersSet.RemoveRange(
                    dbContext.DocumentPapersSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.DocumentId == id));

                CommonQueries.DeletePropertyValues(context,
                    new FilterPropertyValue
                    {
                        Object = new List<EnumObjects> { EnumObjects.Documents },
                        RecordId = new List<int> { id }
                    });

                dbContext.DocumentsSet.RemoveRange(
                    dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == id)); //Without security restrictions

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.Documents, EnumOperationType.Delete);
                transaction.Complete();

            }
        }

        public InternalDocument RegisterDocumentPrepare(IContext context, RegisterDocumentBase model)
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
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }
                var strDocumentDirection = ((int)doc.DocumentDirection).ToString();
                doc.Subscriptions = dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == context.Client.Id)
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
                            SourcePositionId = x.DoneEvent.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source).PositionId,
                        }
                    }).ToList();
                var regJournal =
                    dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.Id == model.RegistrationJournalId)
                        .Where(x => x.DirectionCodes.Contains(strDocumentDirection))
                        .Where(x => dbContext.AdminRegistrationJournalPositionsSet
                            .Where(
                                y =>
                                    y.PositionId == context.CurrentPositionId &&
                                    y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                            .Select(y => y.RegJournalId).Contains(x.Id))
                        .Select(x => new { x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula })
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

        public InternalDocumnRegistration RegisterModelDocumentPrepare(IContext context, RegisterDocumentBase model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new
                    {
                        DocumentId = x.Id,
                        LinkId = x.LinkId,
                        SenderAgentId = x.SenderAgentId,
                        ExecutorPositionDepartmentCode = x.ExecutorPosition.Department.Index,
                        SubscriptionsPositionDepartmentCode = x.Subscriptions
                            .Where(y => y.SubscriptionStateId == (int)EnumSubscriptionStates.Sign)
                            .OrderBy(y => y.LastChangeDate).Take(1)
                            .Select(y => y.DoneEvent.Accesses.FirstOrDefault(z => z.AccessTypeId == (int)EnumEventAccessTypes.Source).Position.Department.Index).FirstOrDefault(),
                        DocumentSendListLastAgentExternalFirstSymbolName = x.SendLists
                            .Where(y => y.SendTypeId == (int)EnumSendTypes.SendForInformationExternal)
                            .OrderByDescending(y => y.LastChangeDate).Take(1)
                            .Select(y => y.AccessGroups.FirstOrDefault(z => z.AccessTypeId == (int)EnumEventAccessTypes.Target).Agent.Name).FirstOrDefault()
                    }).FirstOrDefault();

                if (doc == null) return null;

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
                res.OrdinalNumberDocumentLinkForCorrespondent = dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                    .Where(x => x.LinkId == doc.LinkId && x.SenderAgentId == doc.SenderAgentId && x.IsRegistered == true)
                    .Count() + 1;

                var regJournal =
                    dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.Id == model.RegistrationJournalId)
                        .Select(x => new
                        {
                            x.Id,
                            x.NumerationPrefixFormula,
                            x.PrefixFormula,
                            x.SuffixFormula,
                            x.Index,
                            RegistrationJournalDepartmentCode = x.Department.Index
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

                var initiativeDoc = dbContext.DocumentLinksSet.Where(x => x.ClientId == context.Client.Id)
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
                    dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.Client.Id)
                        .Where(x => x.Id == model.CurrentPositionId)
                        .Select(x => x.Department.Index).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public void RegisterDocument(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.SafeAttach(doc);
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
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.ClientId == context.Client.Id)
                                                                .Where(x => x.DocumentId == document.Id && x.EventTypeId == (int)EnumEventTypes.Registered));
                }

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents,
                    EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void GetNextDocumentRegistrationNumber(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var maxNumber = dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                                 .Where(x => x.RegistrationJournalId == document.RegistrationJournalId
                                       && x.NumerationPrefixFormula == document.NumerationPrefixFormula
                                       && x.Id != document.Id)
                                 .Select(x => x.RegistrationNumber).Max();
                document.RegistrationNumber = (maxNumber ?? 0) + 1;
                transaction.Complete();
            }
        }

        public bool VerifyDocumentRegistrationNumber(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = !dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                    .Any(x => x.RegistrationJournalId == document.RegistrationJournalId
                              && x.NumerationPrefixFormula == document.NumerationPrefixFormula
                              && x.RegistrationNumber == document.RegistrationNumber
                              && x.Id != document.Id);
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ChangeExecutorDocumentPrepare(IContext context, ChangeExecutor model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsExecutorPosition = true, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = CommonQueries.GetDocumentFileQuery(context, new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId }, Types = new List<EnumFileTypes> { EnumFileTypes.Main } })
                    .Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
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

        public InternalDocument ChangePositionDocumentPrepare(IContext context, ChangePosition model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsExecutorPosition = true, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = CommonQueries.GetDocumentFileQuery(context, new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId }, Types = new List<EnumFileTypes> { EnumFileTypes.Main } })
                    .Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
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

        public void ChangeExecutorDocument(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.SafeAttach(doc);
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
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(context, document.Accesses, doc.Id).ToList());
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
                        dbContext.SafeAttach(paperDb);
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
                        dbContext.SafeAttach(fileDb);
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
                        dbContext.SafeAttach(taskDb);
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
                CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id);
                CommonQueries.AddFullTextCacheInfo(context, document.Id, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void ChangePositionDocument(IContext context, ChangePosition model, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DocumentsSet.Where(x => x.ClientId == context.Client.Id) //Without security restrictions
                    .Where(x => x.Id == model.DocumentId && x.ExecutorPositionId == model.OldPositionId)
                    .Update(x => new DBModel.Document.Documents { ExecutorPositionId = model.NewPositionId, });//TODO исполнители по должности!!!
                dbContext.DocumentFilesSet.Where(x => x.ClientId == context.Client.Id)//Without security restrictions
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == model.OldPositionId)
                    .Update(x => new DocumentFiles { ExecutorPositionId = model.NewPositionId });
                dbContext.DocumentTasksSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .Update(x => new DocumentTasks { PositionId = model.NewPositionId });
                //TODO Update EventAccess? key! Source-Target Up to logics
                //dbContext.DocumentEventsSet.Where(x => x.ClientId == context.CurrentClientId)
                //    .Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId)
                //    .ToList()
                //    .ForEach(x =>
                //    {
                //        x.SourcePositionId = model.NewPositionId;
                //    });
                //dbContext.DocumentEventsSet.Where(x => x.ClientId == context.CurrentClientId)
                //    .Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId)
                //    .ToList()
                //    .ForEach(x =>
                //    {
                //        x.TargetPositionId = model.NewPositionId;
                //    });
                //dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.CurrentClientId)
                //    .Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId)
                //    .ToList()
                //    .ForEach(x =>
                //    {
                //        x.SourcePositionId = model.NewPositionId;
                //    });
                //dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.CurrentClientId)
                //    .Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId)
                //    .ToList()
                //    .ForEach(x =>
                //    {
                //        x.TargetPositionId = model.NewPositionId;
                //    });
                dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == context.Client.Id)  //TODO KEY!!!
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .Update(x => new DocumentRestrictedSendLists { PositionId = model.NewPositionId });
                dbContext.DocumentAccessesSet.RemoveRange(
                    dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                        .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId)
                    .Update(x => new DocumentAccesses { PositionId = model.NewPositionId });
                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
                CommonQueries.ModifyDocumentAccessesStatistics(context, document.Id);
                transaction.Complete();

            }
        }

        public InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsExecutorPosition = true, IsInWork = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsLaunchPlan = x.IsLaunchPlan
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.Client.Id)
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

        public void ChangeIsLaunchPlanDocument(IContext context, InternalDocument document)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = ModelConverter.GetDbDocument(document);
                dbContext.SafeAttach(doc);
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

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentPapers(context, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentPaper GetDocumentPaper(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    CommonQueries.GetDocumentPapers(context, new FilterDocumentPaper { Id = new List<int> { id } },
                        null).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentPapers   

        #region DocumentPaperLists

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter,
            UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentPaperLists(context, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentPaperList GetDocumentPaperList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    CommonQueries.GetDocumentPaperLists(context,
                        new FilterDocumentPaperList { PaperListId = new List<int> { id } }, null).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion DocumentPaperLists   

        #region DocumentAccesses

        public IEnumerable<InternalDocumentAccess> CheckIsInWorkForControlsPrepare(IContext context, FilterDocumentAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentAccessesQuery(context, filter, true)
                    .Where(x => !x.IsInWork && x.CountWaits > 0);

                var res =
                    qry.Select(x => new InternalDocumentAccess { DocumentId = x.DocumentId, PositionId = x.PositionId })
                        .ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext context, FilterDocumentAccess filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentAccessesQuery(context, filter);
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

        public IEnumerable<InternalDocumentEvent> GetEventsNatively(IContext context, FilterDocumentEventNatively filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetEventsNativelyQuery(context, filter);

                var res = qry.Select(x => new InternalDocumentEvent
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    Date = x.Date,
                    //ReadDate = x.ReadDate,
                    //SourcePositionId = x.SourcePositionId,            //CHANGE Source-Target
                    //SourceAgentId = x.SourceAgentId,
                    //SourcePositionExecutorAgentId = x.SourcePositionExecutorAgentId,
                    //TargetPositionId = x.TargetPositionId,
                    //TargetPositionExecutorAgentId = x.TargetPositionExecutorAgentId,
                    //...
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsEventsNatively(IContext context, FilterDocumentEventNatively filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetEventsNativelyQuery(context, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

    }
}