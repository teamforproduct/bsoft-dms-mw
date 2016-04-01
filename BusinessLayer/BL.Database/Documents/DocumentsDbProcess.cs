﻿using System;
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

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        public DocumentsDbProcess()
        {
        }

        public void AddDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
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

                    if (document.SendLists != null && document.SendLists.Any())
                    {
                        var sendLists = document.SendLists.ToList();
                        sendLists.ForEach(x =>
                        {
                            x.DocumentId = doc.Id;
                            var taskId = doc.Tasks.Where(y => y.Task == x.TaskName).Select(y => y.Id).FirstOrDefault();
                            x.TaskId = (taskId == 0 ? null : (int?)taskId);
                        });
                        var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendLists).ToList();
                        dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                        dbContext.SaveChanges();
                    }


                    document.Id = doc.Id;

                    //TODO we schould check if it needed or not? 
                    if (document.DocumentFiles != null)
                        foreach (var fl in document.DocumentFiles)
                        {
                            fl.DocumentId = doc.Id;
                        }

                    CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumSearchObjectType.Document, EnumOperationType.AddNew);
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var acc = CommonQueries.GetDocumentAccesses(ctx, dbContext);
                if (filters.IsInWork.HasValue)
                {
                    acc = acc.Where(x => x.IsInWork == filters.IsInWork);
                }
                if (filters.AccessLevelId != null && filters.AccessLevelId.Count > 0)
                {
                    acc = acc.Where(x => filters.AccessLevelId.Contains((int)x.AccessLevel));
                }
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx, acc);

                #region DocumentsSetFilter

                if (filters.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.CreateDate >= filters.CreateFromDate.Value);
                }

                if (filters.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.CreateDate <= filters.CreateToDate.Value);
                }

                if (filters.RegistrationFromDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.RegistrationDate >= filters.RegistrationFromDate.Value);
                }

                if (filters.RegistrationToDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.RegistrationDate <= filters.RegistrationToDate.Value);
                }

                if (filters.SenderFromDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.SenderDate >= filters.SenderFromDate.Value);
                }

                if (filters.SenderToDate.HasValue)
                {
                    qry = qry.Where(x => x.Doc.SenderDate <= filters.SenderToDate.Value);
                }

                if (!String.IsNullOrEmpty(filters.Description))
                {
                    qry = qry.Where(x => x.Doc.Description.Contains(filters.Description));
                }

                if (!String.IsNullOrEmpty(filters.RegistrationNumber))
                {
                    qry =
                        qry.Where(
                            x =>
                                (x.Doc.RegistrationNumberPrefix + x.Doc.RegistrationNumber.ToString() +
                                 x.Doc.RegistrationNumberSuffix)
                                    .Contains(filters.RegistrationNumber));
                }

                if (!String.IsNullOrEmpty(filters.Addressee))
                {
                    qry = qry.Where(x => x.Doc.Addressee.Contains(filters.Addressee));
                }

                if (filters.SenderAgentPersonId != null && filters.SenderAgentPersonId.Any())
                {
                    qry = qry.Where(x => x.Doc.SenderAgentPersonId.HasValue && filters.SenderAgentPersonId.Contains(x.Doc.SenderAgentPersonId.Value));
                }

                if (!String.IsNullOrEmpty(filters.SenderNumber))
                {
                    qry = qry.Where(x => x.Doc.SenderNumber.Contains(filters.SenderNumber));
                }

                if (filters.DocumentTypeId != null && filters.DocumentTypeId.Count > 0)
                {
                    qry = qry.Where(x => filters.DocumentTypeId.Contains(x.Doc.TemplateDocument.DocumentTypeId));
                }

                if (filters.DocumentId != null && filters.DocumentId.Count > 0)
                {
                    qry = qry.Where(x => filters.DocumentId.Contains(x.Doc.Id));
                }

                if (filters.TemplateDocumentId != null && filters.TemplateDocumentId.Count > 0)
                {
                    qry = qry.Where(x => filters.TemplateDocumentId.Contains(x.Doc.TemplateDocumentId));
                }

                if (filters.DocumentDirectionId != null && filters.DocumentDirectionId.Count > 0)
                {
                    qry = qry.Where(x => filters.DocumentDirectionId.Contains(x.Templ.DocumentDirectionId));
                }

                if (filters.DocumentSubjectId != null && filters.DocumentSubjectId.Count > 0)
                {
                    qry =
                        qry.Where(
                            x =>
                                x.Doc.DocumentSubjectId.HasValue &&
                                filters.DocumentSubjectId.Contains(x.Doc.DocumentSubjectId.Value));
                }

                if (filters.RegistrationJournalId != null && filters.RegistrationJournalId.Count > 0)
                {
                    qry =
                        qry.Where(
                            x =>
                                x.Doc.RegistrationJournalId.HasValue &&
                                filters.RegistrationJournalId.Contains(x.Doc.RegistrationJournalId.Value));
                }

                if (filters.ExecutorPositionId != null && filters.ExecutorPositionId.Count > 0)
                {
                    qry = qry.Where(x => filters.ExecutorPositionId.Contains(x.Doc.ExecutorPositionId));
                }

                //TODO Проверить фильтры
                if (filters.ExecutorPositionExecutorAgentId != null && filters.ExecutorPositionExecutorAgentId.Count > 0)
                {
                    qry = qry.Where(x => filters.ExecutorPositionExecutorAgentId.Contains(x.Doc.ExecutorPositionExecutorAgentId));
                }

                if (filters.ExecutorDepartmentId != null && filters.ExecutorDepartmentId.Count > 0)
                {
                    qry = qry.Where(x => filters.ExecutorDepartmentId.Contains(x.Doc.ExecutorPosition.DepartmentId));
                }

                #region Subscription
                if ((filters.SubscriptionPositionId != null && filters.SubscriptionPositionId.Count > 0) ||
                    (filters.SubscriptionPositionExecutorAgentId != null && filters.SubscriptionPositionExecutorAgentId.Count > 0) ||
                    (filters.SubscriptionrDepartmentId != null && filters.SubscriptionrDepartmentId.Count > 0))
                {
                    var qryTmp = from Doc in qry
                                 join ds in dbContext.DocumentSubscriptionsSet on Doc.Doc.Id equals ds.DocumentId
                                 join de in dbContext.DocumentEventsSet on ds.DoneEventId equals de.Id
                                 join dpos in dbContext.DictionaryPositionsSet on de.SourcePositionId equals dpos.Id into dpos
                                 from dp in dpos.DefaultIfEmpty()
                                 where ds.DoneEventId.HasValue && ds.SubscriptionStateId == (int)EnumSubscriptionStates.Sign || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Visa || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Аgreement || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Аpproval
                                 select new { Doc, ds, de, dp };

                    if (filters.SubscriptionPositionId != null && filters.SubscriptionPositionId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionId.HasValue && filters.SubscriptionPositionId.Contains(x.de.SourcePositionId.Value));
                    }

                    if (filters.SubscriptionPositionExecutorAgentId != null && filters.SubscriptionPositionExecutorAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionExecutorAgentId.HasValue && filters.SubscriptionPositionExecutorAgentId.Contains(x.de.SourcePositionExecutorAgentId.Value));
                    }

                    if (filters.SubscriptionrDepartmentId != null && filters.SubscriptionrDepartmentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionId.HasValue && filters.SubscriptionrDepartmentId.Contains(x.dp.DepartmentId));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Subscription

                #region Event
                if (filters.EventIsNew.HasValue
                    || filters.EventFromDate.HasValue
                    || filters.EventToDate.HasValue
                    || (filters.EventTypeId != null && filters.EventTypeId.Count > 0)
                    || (filters.EventImportanceEventTypeId != null && filters.EventImportanceEventTypeId.Count > 0)
                    || !string.IsNullOrEmpty(filters.EventDescription)
                    || (filters.EventSourcePositionId != null && filters.EventSourcePositionId.Count > 0)
                    || (filters.EventSourcePositionExecutorAgentId != null && filters.EventSourcePositionExecutorAgentId.Count > 0)
                    || (filters.EventSourceAgentId != null && filters.EventSourceAgentId.Count > 0)
                    || (filters.EventTargetPositionId != null && filters.EventTargetPositionId.Count > 0)
                    || (filters.EventTargetPositionExecutorAgentId != null && filters.EventTargetPositionExecutorAgentId.Count > 0)
                    || (filters.EventTargetAgentId != null && filters.EventTargetAgentId.Count > 0))
                {
                    var qryTmp = from Doc in qry
                                 join de in dbContext.DocumentEventsSet on Doc.Doc.Id equals de.DocumentId
                                 join det in dbContext.DictionaryEventTypesSet on de.EventTypeId equals det.Id
                                 select new { Doc, de, det };
                    if (filters.EventIsNew.HasValue)
                    {
                        if (filters.EventIsNew.Value)
                        {
                            qryTmp = qryTmp.Where(x => !x.de.ReadDate.HasValue);
                        }
                        else
                        {
                            qryTmp = qryTmp.Where(x => x.de.ReadDate.HasValue);
                        }
                    }

                    if (filters.EventFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.de.CreateDate >= filters.EventFromDate.Value);
                    }

                    if (filters.EventToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.de.CreateDate <= filters.EventToDate.Value);
                    }

                    if (filters.EventTypeId != null && filters.EventTypeId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.EventTypeId.Contains(x.de.EventTypeId));
                    }

                    if (filters.EventImportanceEventTypeId != null && filters.EventImportanceEventTypeId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.EventImportanceEventTypeId.Contains(x.det.ImportanceEventTypeId));
                    }

                    if (!string.IsNullOrEmpty(filters.EventDescription))
                    {
                        qryTmp = qryTmp.Where(x => x.de.Description.Contains(filters.EventDescription));
                    }

                    if (filters.EventSourcePositionId != null && filters.EventSourcePositionId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionId.HasValue && filters.EventSourcePositionId.Contains(x.de.SourcePositionId.Value));
                    }

                    if (filters.EventSourcePositionExecutorAgentId != null && filters.EventSourcePositionExecutorAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionExecutorAgentId.HasValue && filters.EventSourcePositionExecutorAgentId.Contains(x.de.SourcePositionExecutorAgentId.Value));
                    }

                    if (filters.EventSourceAgentId != null && filters.EventSourceAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.EventSourceAgentId.Contains(x.de.SourceAgentId));
                    }

                    if (filters.EventTargetPositionId != null && filters.EventTargetPositionId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.TargetPositionId.HasValue && filters.EventTargetPositionId.Contains(x.de.TargetPositionId.Value));
                    }

                    if (filters.EventTargetPositionExecutorAgentId != null && filters.EventTargetPositionExecutorAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.TargetPositionExecutorAgentId.HasValue && filters.EventTargetPositionExecutorAgentId.Contains(x.de.TargetPositionExecutorAgentId.Value));
                    }

                    if (filters.EventTargetAgentId != null && filters.EventTargetAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => x.de.TargetAgentId.HasValue && filters.EventTargetAgentId.Contains(x.de.TargetAgentId.Value));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Event

                #region Task
                if ((filters.TaskId != null && filters.TaskId.Count > 0) ||
                    !string.IsNullOrEmpty(filters.TaskDescription))
                {
                    var qryTmp = from Doc in qry
                                 join dt in dbContext.DocumentTasksSet on Doc.Doc.Id equals dt.DocumentId
                                 select new { Doc, dt };

                    if (filters.TaskId != null && filters.TaskId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.TaskId.Contains(x.dt.Id));
                    }

                    if (!string.IsNullOrEmpty(filters.TaskDescription))
                    {
                        //TODO Какое поле нужно проверить?
                        qryTmp = qryTmp.Where(x => x.dt.Task.Contains(filters.TaskDescription));
                        //qryTmp = qryTmp.Where(x => x.dt.Description.Contains(filters.TaskDescription));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Task

                #region Tag
                if ((filters.TagId != null && filters.TagId.Count > 0) ||
                    !string.IsNullOrEmpty(filters.TagDescription))
                {
                    var qryTmp = from Doc in qry
                                 join DocTag in dbContext.DocumentTagsSet on Doc.Doc.Id equals DocTag.DocumentId
                                 join DicTag in dbContext.DictionaryTagsSet on DocTag.TagId equals DicTag.Id
                                 //TODO Нужно ли тут это условие
                                 where !DicTag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(DicTag.PositionId ?? 0)
                                 select new { Doc, DocTag, DicTag };

                    if (filters.TagId != null && filters.TagId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.TagId.Contains(x.DocTag.TagId));
                    }

                    if (!string.IsNullOrEmpty(filters.TagDescription))
                    {
                        qryTmp = qryTmp.Where(x => x.DicTag.Name.Contains(filters.TagDescription));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Tag

                //TODO Перепроверить
                #region Wait
                if (filters.WaitDueDateFromDate.HasValue
                    || filters.WaitDueDateToDate.HasValue
                    || filters.WaitCreateFromDate.HasValue
                    || filters.WaitCreateToDate.HasValue)
                {
                    var qryTmp = from Doc in qry
                                 join Wait in dbContext.DocumentWaitsSet on Doc.Doc.Id equals Wait.DocumentId
                                 join WaitOnEvent in dbContext.DocumentEventsSet on Wait.OnEventId equals WaitOnEvent.Id
                                 select new { Doc, Wait, WaitOnEvent };

                    if (filters.WaitDueDateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.Wait.DueDate >= filters.WaitDueDateFromDate.Value);
                    }

                    if (filters.WaitDueDateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.Wait.DueDate <= filters.WaitDueDateToDate.Value);
                    }

                    if (filters.WaitCreateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.WaitOnEvent.Date >= filters.WaitCreateFromDate.Value);
                    }

                    if (filters.WaitCreateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.WaitOnEvent.Date <= filters.WaitCreateToDate.Value);
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Wait

                #region File
                if (!string.IsNullOrEmpty(filters.FileName)
                    || !string.IsNullOrEmpty(filters.FileExtension)
                    || filters.FileSizeFrom.HasValue
                    || filters.FileSizeTo.HasValue
                    || filters.FileCreateFromDate.HasValue
                    || filters.FileCreateToDate.HasValue
                    || (filters.FileAgentId != null && filters.FileAgentId.Count > 0))
                {
                    var qryTmp = (from Doc in qry
                                  join DocFile in dbContext.DocumentFilesSet on Doc.Doc.Id equals DocFile.DocumentId
                                  select new { Doc, DocFile })
                                 .GroupBy(g => new { g.Doc, g.DocFile.OrderNumber })
                                 .Select(x => new { Doc = x.Key.Doc, DocFile = x.OrderByDescending(f => f.DocFile.Version).First().DocFile });

                    if (!string.IsNullOrEmpty(filters.FileName))
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Name.Contains(filters.FileName));
                    }

                    if (!string.IsNullOrEmpty(filters.FileExtension))
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Extension.Contains(filters.FileExtension));
                    }
                    if (filters.FileSizeFrom.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.FileSize >= filters.FileSizeFrom);
                    }
                    if (filters.FileSizeTo.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.FileSize <= filters.FileSizeTo);
                    }
                    if (filters.FileCreateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Date >= filters.FileCreateFromDate.Value);
                    }
                    if (filters.FileCreateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Date <= filters.FileCreateToDate);
                    }
                    if (filters.FileAgentId != null && filters.FileAgentId.Count > 0)
                    {
                        qryTmp = qryTmp.Where(x => filters.FileAgentId.Contains(x.DocFile.LastChangeUserId));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion File

                if (filters.SenderAgentId != null && filters.SenderAgentId.Count > 0)
                {
                    qry =
                        qry.Where(
                            x =>
                                x.Doc.SenderAgentId.HasValue &&
                                filters.SenderAgentId.Contains(x.Doc.SenderAgentId.Value));
                }

                if (filters.IsRegistered.HasValue)
                {
                    qry = qry.Where(x => x.Doc.IsRegistered == filters.IsRegistered.Value);
                }

                if (filters.FilterProperties?.Count() > 0)
                {
                    foreach (var filterProperty in filters.FilterProperties)
                    {
                        var qryTmp = from Doc in qry
                                     join pv in dbContext.PropertyValuesSet on new { RecordId = Doc.Doc.Id, PropertyLinkId = filterProperty.PropertyLinkId }
                                        equals new { RecordId = pv.RecordId, PropertyLinkId = pv.PropertyLinkId }
                                     select new { Doc, pv };
                        switch (filterProperty.ValueType)
                        {
                            case EnumValueTypes.Text:
                                qryTmp = qryTmp.Where(x => x.pv.ValueString.Contains(filterProperty.Text));
                                break;
                            case EnumValueTypes.Number:
                                if (filterProperty.NumberFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberFrom <= x.pv.ValueNumeric);
                                }
                                if (filterProperty.NumberTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberTo >= x.pv.ValueNumeric);
                                }
                                break;
                            case EnumValueTypes.Date:
                                if (filterProperty.DateFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateFrom <= x.pv.ValueDate);
                                }
                                if (filterProperty.DateTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateTo >= x.pv.ValueDate);
                                }
                                break;
                            case EnumValueTypes.Api:
                                if (!(filterProperty.Ids?.Count() > 0))
                                {
                                    filterProperty.Ids = new List<int>();
                                }
                                var ids = filterProperty.Ids.Select(y => (double?)y).ToList();
                                qryTmp = qryTmp.Where(x => ids.Contains(x.pv.ValueNumeric));
                                break;
                        }
                        qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                    }
                }
                #endregion DocumentsSetFilter

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderByDescending(x => x.Doc.CreateDate)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                var newevnt =
                    dbContext.DocumentEventsSet.Join(qry, ev => ev.DocumentId, rs => rs.Doc.Id, (e, r) => new { ev = e })
                    .Where(x => !x.ev.ReadDate.HasValue && x.ev.TargetPositionId.HasValue && x.ev.TargetPositionId != x.ev.SourcePositionId
                             && ctx.CurrentPositionsIdList.Contains(x.ev.TargetPositionId.Value))
                        .GroupBy(g => g.ev.DocumentId)
                        .Select(s => new { DocID = s.Key, EvnCnt = s.Count() }).ToList();

                var fls =
                    dbContext.DocumentFilesSet.Join(qry, fl => fl.DocumentId, rs => rs.Doc.Id, (f, r) => new { fil = f })
                        .GroupBy(g => g.fil.DocumentId)
                        .Select(s => new { DocID = s.Key, FileCnt = s.Count() }).ToList();

                var links = qry.GroupJoin(dbContext.DocumentsSet.Where(x => x.LinkId.HasValue), dl => dl.Doc.LinkId, d => d.LinkId,
                                            (dl, ds) => new { DocID = dl.Doc.Id, LinkCnt = ds.Count() }).ToList();

                var cnt_weits =
                    CommonQueries.GetDocumentWaitsQuery(dbContext, ctx).Where(x => !x.OffEventId.HasValue)
                    .Join(qry, w => w.DocumentId, rs => rs.Doc.Id, (w, r) => new { wt = w })
                        .GroupBy(x => x.wt.DocumentId)
                        .Select(x => new
                        {
                            DocId = x.Key,
                            OpenWaits = x.Count(),
                            Overdue = x.Count(s => s.wt.DueDate.HasValue && s.wt.DueDate.Value < DateTime.Now)
                        }).ToList();

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Doc.Id,
                    DocumentDirectionName = doc.DirName,
                    DocumentTypeName = doc.DocTypeName,

                    RegistrationNumber = doc.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.Doc.RegistrationNumberSuffix,

                    DocumentDate = doc.Doc.RegistrationDate ?? doc.Doc.CreateDate,
                    IsRegistered = doc.Doc.IsRegistered,
                    Description = doc.Doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgentName,
                    ExecutorPositionName = doc.ExecutorPosName,
                });

                var docs = res.ToList();

                docs.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

                var accs = acc.ToList();

                foreach (var doc in docs)
                {
                    doc.Accesses = accs.Where(x => x.DocumentId == doc.Id).ToList();
                    doc.IsFavourite = doc.Accesses.Any(x => x.IsFavourite);
                    doc.IsInWork = doc.Accesses.Any(x => x.IsInWork);
                    //doc.AccessLevel = doc.Accesses.Max(x => x.AccessLevel);
                    //doc.AccessLevelName = doc.Accesses.FirstOrDefault(x => x.AccessLevel == doc.AccessLevel).AccessLevelName;
                }

                foreach (var x1 in docs.Join(cnt_weits, d => d.Id, e => e.DocId, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.WaitOpenCount = x1.ev.OpenWaits;
                    x1.doc.WaitOverdueCount = x1.ev.Overdue;
                }

                foreach (var x1 in docs.Join(newevnt, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.NewEventCount = x1.ev.EvnCnt;
                }

                foreach (var x1 in docs.Join(fls, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.AttachedFilesCount = x1.ev.FileCnt;
                }

                foreach (var x1 in docs.Join(links, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.LinkedDocumentsCount = x1.ev.LinkCnt;
                }

                docs.GroupJoin(CommonQueries.GetDocumentTags(dbContext, new FilterDocumentTag { DocumentId = docs.Select(x => x.Id).ToList(), CurrentPositionsId = ctx.CurrentPositionsIdList }),
                    d => d.Id,
                    t => t.DocumentId,
                    (d, t) => d.DocumentTags = t);

                return docs;
            }
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Doc.Id == documentId);

                var doc = qry.FirstOrDefault();
                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                var accs =
                    CommonQueries.GetDocumentAccesses(ctx, dbContext).Where(x => x.DocumentId == doc.Doc.Id).ToList();

                var res = new FrontDocument
                {
                    Id = doc.Doc.Id,
                    DocumentDirection = (EnumDocumentDirections)doc.Templ.DocumentDirectionId,
                    DocumentDirectionName = doc.DirName,
                    DocumentTypeName = doc.DocTypeName,

                    DocumentDate = doc.Doc.RegistrationDate ?? doc.Doc.CreateDate,
                    IsRegistered = doc.Doc.IsRegistered,
                    Description = doc.Doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgentName,
                    ExecutorPositionName = doc.ExecutorPosName,
                    LinkedDocumentsCount = 0, //TODO

                    TemplateDocumentId = doc.Doc.TemplateDocumentId,
                    DocumentSubjectId = doc.Doc.DocumentSubjectId,
                    DocumentSubjectName = doc.SubjName,

                    RegistrationJournalId = doc.Doc.RegistrationJournalId,
                    RegistrationJournalName = doc.RegistrationJournalName,
                    RegistrationNumber = doc.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.Doc.RegistrationNumberSuffix,
                    RegistrationDate = doc.Doc.RegistrationDate,

                    ExecutorPositionId = doc.Doc.ExecutorPositionId,
                    ExecutorPositionExecutorNowAgentName = doc.ExecutorPositionExecutorNowAgentName,
                    ExecutorPositionAgentPhoneNumber = "ExecutorPositionAgentPhoneNumber", //TODO

                    SenderAgentId = doc.Doc.SenderAgentId,
                    SenderAgentName = doc.SenderAgentname,
                    SenderAgentPersonId = doc.Doc.SenderAgentPersonId,
                    SenderAgentPersonName = doc.SenderPersonName,
                    SenderNumber = doc.Doc.SenderNumber,
                    SenderDate = doc.Doc.SenderDate,
                    Addressee = doc.Doc.Addressee,

                    IsLaunchPlan = doc.Doc.IsLaunchPlan,

                    AccessLevel =
                        accs.Where(x => x.PositionId == doc.Doc.ExecutorPositionId)
                            .Select(x => x.AccessLevel)
                            .FirstOrDefault(),
                    AccessLevelName =
                        accs.Where(x => x.PositionId == doc.Doc.ExecutorPositionId)
                            .Select(x => x.AccessLevelName)
                            .FirstOrDefault(),

                    TemplateDocumentName = doc.Templ.Name,
                    IsHard = doc.Templ.IsHard,

                    LinkId = doc.Doc.LinkId,
                    IsFavourite = accs.Any(x => x.IsFavourite),
                    IsInWork = accs.Any(x => x.IsInWork),

                    Accesses = accs,
                };

                CommonQueries.ChangeRegistrationFullNumber(res, false);

                var docIds = new List<int> { res.Id };

                if (res.LinkId.HasValue)
                {
                    res.LinkedDocuments = CommonQueries.GetLinkedDocuments(ctx, dbContext, res.LinkId.Value);
                    res.LinkedDocumentsCount = res.LinkedDocuments.Count();

                    if ((filter?.DocumentsIdForAIP != null) && (filter.DocumentsIdForAIP.Any()))
                    {
                        docIds = filter?.DocumentsIdForAIP;
                    }
                }

                var cnt_waits =
                    CommonQueries.GetDocumentWaitsQuery(dbContext, ctx, res.Id).Where(x => !x.OffEventId.HasValue)
                        .GroupBy(x => x.DocumentId)
                        .Select(x => new
                        {
                            DocId = x.Key,
                            OpenWaits = x.Count(),
                            Overdue = x.Count(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.Now)
                        }).FirstOrDefault();

                if (cnt_waits != null)
                {
                    res.WaitOpenCount = cnt_waits.OpenWaits;
                    res.WaitOverdueCount = cnt_waits.Overdue;
                }

                //select only events, where sourceposition or target position are in user's current positions luist
                var evtCount = dbContext.DocumentEventsSet.Where(x => x.DocumentId == res.Id &&
                ((x.TargetPositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.TargetPositionId.Value))
                || (x.SourcePositionId.HasValue && ctx.CurrentPositionsIdList.Contains(x.SourcePositionId.Value))))
                .GroupBy(x => x.DocumentId)
                    .Select(x => new
                    {
                        docId = x.Key,
                        totalCnt = x.Count(),
                        newCnt = x.Count(s => !s.ReadDate.HasValue
                                              && s.TargetPositionId.HasValue && s.TargetPositionId != s.SourcePositionId
                                              && ctx.CurrentPositionsIdList.Contains(s.TargetPositionId.Value))
                    }).FirstOrDefault();

                if (evtCount != null)
                {
                    res.EventsCount = evtCount.totalCnt;

                    res.NewEventCount = evtCount.newCnt;
                }


                res.SendLists = CommonQueries.GetDocumentSendList(dbContext, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

                res.SendListStageMax = (res.SendLists == null) || !res.SendLists.Any() ? 0 : res.SendLists.Max(x => x.Stage);

                res.RestrictedSendLists = CommonQueries.GetDocumentRestrictedSendList(dbContext, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });

                res.DocumentFiles = CommonQueries.GetDocumentFiles(dbContext, new FilterDocumentAttachedFile { DocumentId = docIds });
                res.AttachedFilesCount = res.DocumentFiles.Count();

                res.DocumentTasks = CommonQueries.GetDocumentTasks(dbContext, new FilterDocumentTask { DocumentId = docIds });

                res.DocumentWaits = CommonQueries.GetDocumentWaits(dbContext, new FilterDocumentWait { DocumentId = docIds });

                res.DocumentTags = CommonQueries.GetDocumentTags(dbContext, new FilterDocumentTag { DocumentId = docIds, CurrentPositionsId = ctx.CurrentPositionsIdList });

                res.DocumentWorkGroup = CommonQueries.GetDocumentWorkGroup(dbContext, new FilterDictionaryPosition { DocumentIDs = docIds });

                res.DocumentSubscriptions = CommonQueries.GetDocumentSubscriptions(dbContext, new FilterDocumentSubscription { DocumentId = docIds });

                res.Properties = CommonQueries.GetPropertyValues(dbContext, new FilterPropertyValue { RecordId = new List<int> { documentId }, Object = new List<EnumObjects> { EnumObjects.Documents } });

                return res;
            }
        }

        public InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(context))
            {

                var doc = dbContext.TemplateDocumentsSet
                    .Where(x => x.Id == templateDocumentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.Id,
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

                doc.Tasks = dbContext.TemplateDocumentTasksSet.Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentTask()
                    {
                        Name = y.Task,
                        Description = y.Description,
                        PositionId = y.PositionId ?? 0,
                    }).ToList();

                doc.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentRestrictedSendList()
                    {
                        PositionId = y.PositionId,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId
                    }).ToList();

                doc.SendLists = dbContext.TemplateDocumentSendListsSet.Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentSendList()
                    {
                        SendType = (EnumSendTypes)y.SendTypeId,
                        SourcePositionId = y.SourcePositionId ?? 0,
                        TargetPositionId = y.TargetPositionId,
                        TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
                        IsAvailableWithinTask = y.IsAvailableWithinTask,
                        IsAddControl = y.IsAddControl,
                        Description = y.Description,
                        Stage = y.Stage,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                    }).ToList();

                doc.DocumentFiles = dbContext.TemplateDocumentFilesSet.Where(x => x.DocumentId == templateDocumentId).Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Extension = x.Extention,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    OrderInDocument = x.OrderNumber,
                    IsAdditional = x.IsAdditional,
                    Hash = x.Hash
                }).ToList();

                return doc;
            }
        }

        public InternalDocument CopyDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.Doc.TemplateDocumentId,
                        DocumentSubjectId = x.Doc.DocumentSubjectId,
                        Description = x.Doc.Description,
                        SenderAgentId = x.Doc.SenderAgentId,
                        SenderAgentPersonId = x.Doc.SenderAgentPersonId,
                        Addressee = x.Doc.Addressee,

                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.AccessLevel = (EnumDocumentAccesses)CommonQueries.GetDocumentAccessesesQry(dbContext, documentId, ctx).Max(x => x.AccessLevelId);
                doc.Tasks = dbContext.DocumentTasksSet
                        .Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentTask
                        {
                            Name = x.Task,
                            Description = x.Description,
                            PositionId = x.PositionId,
                        }
                        ).ToList();
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.DocumentId == documentId && x.IsInitial)
                        .Select(y => new InternalDocumentSendList
                        {
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            SourcePositionId = y.SourcePositionId,
                            TargetPositionId = y.TargetPositionId,
                            TargetAgentId = y.TargetAgentId,
                            TaskName = y.Task.Task,
                            IsAvailableWithinTask = y.IsAvailableWithinTask,
                            IsAddControl = y.IsAddControl,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            IsInitial = y.IsInitial,
                        }).ToList();
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentRestrictedSendList
                        {
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                        }).ToList();
                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(dbContext, documentId);

                return doc;
            }
        }

        public InternalDocument ModifyDocumentPrepare(IContext ctx, ModifyDocument model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == model.Id && (ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId)))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        TemplateDocumentId = x.Doc.TemplateDocumentId,
                        IsHard = x.Templ.IsHard,
                        DocumentDirection = (EnumDocumentDirections)x.Templ.DocumentDirectionId,

                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Accesses = dbContext.DocumentAccessesSet
                    .Where(x => x.DocumentId == model.Id && x.PositionId == doc.ExecutorPositionId && x.AccessLevelId != (int)model.AccessLevel)
                    .Select(x => new InternalDocumentAccess
                    {
                        Id = x.Id,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId
                    }).ToList();
                return doc;
            }
        }

        public void ModifyDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
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
                    LastChangeUserId = document.LastChangeUserId
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
                    CommonQueries.ModifyPropertyValues(dbContext, new InternalPropertyValues { Object = EnumObjects.Documents, RecordId = document.Id, PropertyValues = document.Properties });
                }

                CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumSearchObjectType.Document, EnumOperationType.Update);
                dbContext.SaveChanges();

            }
        }

        public InternalDocument DeleteDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == documentId && (ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId)))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        WaitsCount = x.Doc.Waits.Count,
                        SubscriptionsCount = x.Doc.Subscriptions.Count,
                    }).FirstOrDefault();

                if (doc == null) return null;


                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(dbContext, doc.Id);

                return doc;
            }
        }

        public void DeleteDocument(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //ADD OTHER TABLES!!!!
                dbContext.DocumentWaitsSet.RemoveRange(dbContext.DocumentWaitsSet.Where(x => x.DocumentId == id));
                dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.DocumentId == id));
                dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.DocumentId == id));
                dbContext.DocumentFilesSet.RemoveRange(dbContext.DocumentFilesSet.Where(x => x.DocumentId == id));
                dbContext.DocumentRestrictedSendListsSet.RemoveRange(dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == id));
                dbContext.DocumentSendListsSet.RemoveRange(dbContext.DocumentSendListsSet.Where(x => x.DocumentId == id));
                dbContext.DocumentTasksSet.RemoveRange(dbContext.DocumentTasksSet.Where(x => x.DocumentId == id));
                dbContext.DocumentsSet.RemoveRange(dbContext.DocumentsSet.Where(x => x.Id == id));

                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumSearchObjectType.Document, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public InternalDocument RegisterDocumentPrepare(IContext context, RegisterDocument model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Doc.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        DocumentSubjectId = x.Doc.DocumentSubjectId,
                        Description = x.Doc.Description,
                        IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        SenderAgentId = x.Doc.SenderAgentId,
                        SenderAgentPersonId = x.Doc.SenderAgentPersonId,
                        SenderNumber = x.Doc.SenderNumber,
                        SenderDate = x.Doc.SenderDate,
                        Addressee = x.Doc.Addressee,
                        LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }
                var regJournal = dbContext.DictionaryRegistrationJournalsSet
                    .Where(x => x.Id == model.RegistrationJournalId)
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

        public void RegisterDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
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

                CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumSearchObjectType.Document, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }

        public void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //get next number
                var maxNumber = (from docreg in dbContext.DocumentsSet
                                 where docreg.RegistrationJournalId == document.RegistrationJournalId
                                       && docreg.NumerationPrefixFormula == document.NumerationPrefixFormula
                                       && docreg.Id != document.Id
                                 select docreg.RegistrationNumber).Max();
                document.RegistrationNumber = (maxNumber ?? 0) + 1;
            }
        }

        public bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return !dbContext.DocumentsSet
                                .Any(x => x.RegistrationJournalId == document.RegistrationJournalId
                                         && x.NumerationPrefixFormula == document.NumerationPrefixFormula
                                         && x.RegistrationNumber == document.RegistrationNumber
                                         && x.Id != document.Id
                    );
            }
        }

        public InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == model.DocumentId && ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        IsRegistered = x.Doc.IsRegistered
                    }).FirstOrDefault();

                return doc;
            }
        }

        public void ChangeExecutorDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
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

                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                }

                //TODO При получении документа возвращаеться только один Accesses
                if (document.Accesses != null && document.Accesses.Any())
                {
                    //TODO Не сохраняеться через свойства
                    //doc.Accesses = CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, doc.Id).ToList();
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, doc.Id).ToList());
                }

                dbContext.SaveChanges();

            }
        }

        public InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == documentId && (ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId)))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        IsLaunchPlan = x.Doc.IsLaunchPlan
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet
                                    .Where(x => x.DocumentId == documentId)
                                    .Select(x => new InternalDocumentSendList
                                    {
                                        Id = x.Id,
                                    }
                                    ).ToList();
                return doc;
            }
        }

        public void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
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
            }
        }

        public InternalDocument GetBlankInternalDocumentById(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Doc.Id == documentId /*&& ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId)*/)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId
                    }).FirstOrDefault();

                return doc;
            }
        }


        #region DocumentPapers

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPapers(dbContext, filter);
            }
        }

        public FrontDocumentPaper GetDocumentPaper(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO: Refactoring
                var item = dbContext.DocumentPapersSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontDocumentPaper
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
                        LastPaperEventId = x.LastPaperEventId
                    }).FirstOrDefault();

                return item;
            }
        }
        #endregion DocumentPapers   

        #region DocumentPaperLists

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPaperLists(dbContext, filter);
            }
        }

        public FrontDocumentPaperList GetDocumentPaperListById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO: Refactoring
                var item = dbContext.DocumentPaperListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontDocumentPaperList
                    {
                        Id = x.Id,
                        Date = x.Date,
                        Description = x.Description
                    }).FirstOrDefault();

                return item;
            }
        }
        #endregion DocumentPaperLists   
    }
}