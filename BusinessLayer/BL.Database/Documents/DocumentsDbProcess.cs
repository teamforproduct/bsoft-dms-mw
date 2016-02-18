using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Database.DBModel.Document;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using DocumentAccesses = BL.Database.DBModel.Document.DocumentAccesses;

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        #region Document

        public void AddDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = new DBModel.Document.Documents
                {
                    TemplateDocumentId = document.TemplateDocumentId,
                    CreateDate = document.CreateDate ?? DateTime.Now,
                    DocumentSubjectId = document.DocumentSubjectId,
                    Description = document.Description,
                    IsRegistered = false,
                    RegistrationJournalId = document.RegistrationJournalId,
                    RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                    RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                    RegistrationDate = document.RegistrationDate,
                    ExecutorPositionId = document.ExecutorPositionId,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = document.LastChangeDate.Value,
                    SenderAgentId = document.SenderAgentId,
                    SenderAgentPersonId = document.SenderAgentPersonId,
                    SenderNumber = document.SenderNumber,
                    SenderDate = document.SenderDate,
                    Addressee = document.Addressee,
                };
                if (document.RestrictedSendLists != null && document.RestrictedSendLists.Any())
                {
                    doc.RestrictedSendLists = document.RestrictedSendLists.Select(x => new DocumentRestrictedSendLists
                    {
                        PositionId = x.PositionId,
                        AccessLevelId = x.AccessLevelId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).ToList();
                }

                if (document.Events != null && document.Events.Any())
                {
                    doc.Events = document.Events.Select(x => new DocumentEvents
                    {
                        CreateDate = x.CreateDate,
                        Date = x.Date,
                        Description = x.Description,
                        LastChangeDate = x.LastChangeDate,
                        LastChangeUserId = x.LastChangeUserId,
                        SourceAgentId = x.SourceAgentId,
                        SourcePositionId = x.SourcePositionId,
                        TargetAgentId = x.TargetAgentId,
                        TargetPositionId = x.TargetPositionId,
                        EventTypeId = (int)x.EventType
                    }).ToList();
                }

                if (document.SendLists != null && document.SendLists.Any())
                {
                    doc.SendLists = document.SendLists.Select(x => new DocumentSendLists()
                    {
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendTypeId = (int)x.SendType,
                        TargetPositionId = x.TargetPositionId,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevelId = (int)x.AccessLevel,
                        IsInitial = true,
                        StartEventId = null,
                        CloseEventId = null,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).ToList();
                }

                if (document.Accesses != null && document.Accesses.Any())
                {
                    doc.Accesses = document.Accesses.Select(x => new DocumentAccesses
                    {
                        LastChangeDate = x.LastChangeDate,
                        IsInWork = x.IsInWork,
                        LastChangeUserId = x.LastChangeUserId,
                        PositionId = x.PositionId,
                        AccessLevelId = (int)x.AccessLevel,
                    }).ToList();
                }

                dbContext.DocumentsSet.Add(doc);
                dbContext.SaveChanges();
                document.Id = doc.Id;
            }
        }

        public void UpdateDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = dbContext.DocumentsSet
                    .Include(x => x.Accesses)
                    .FirstOrDefault(x => x.Id == document.Id);
                if (doc != null)
                {
                    //doc.TemplateDocumentId = document.TemplateDocumentId;
                    doc.DocumentSubjectId = document.DocumentSubjectId;
                    doc.Description = document.Description;
                    doc.RegistrationJournalId = document.RegistrationJournalId;
                    doc.RegistrationNumber = document.RegistrationNumber;
                    doc.RegistrationNumberSuffix = document.RegistrationNumberSuffix;
                    doc.RegistrationNumberPrefix = document.RegistrationNumberPrefix;
                    doc.RegistrationDate = document.RegistrationDate;
                    //doc.ExecutorPositionId = document.ExecutorPositionId;
                    doc.LastChangeUserId = ctx.CurrentAgentId;
                    doc.LastChangeDate = DateTime.Now;

                    doc.SenderAgentId = document.SenderAgentId;
                    doc.SenderAgentPersonId = document.SenderAgentPersonId;
                    doc.SenderNumber = document.SenderNumber;
                    doc.SenderDate = document.SenderDate;
                    doc.Addressee = document.Addressee;

                    if (doc.Accesses.Any(x => x.PositionId == ctx.CurrentPositionId && x.AccessLevelId != (int)document.AccessLevel))
                    {
                        doc.Accesses.FirstOrDefault(x => x.PositionId == ctx.CurrentPositionId).AccessLevelId = (int)document.AccessLevel;
                    }

                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        // add only new events. New events should be without Id
                        doc.Events = document.Events.Where(x => x.Id == 0).Select(x => new DocumentEvents
                        {
                            CreateDate = x.CreateDate,
                            Date = x.Date,
                            Description = x.Description,
                            LastChangeDate = x.LastChangeDate,
                            LastChangeUserId = x.LastChangeUserId,
                            SourceAgentId = x.SourceAgentId,
                            SourcePositionId = x.SourcePositionId,
                            TargetAgentId = x.TargetAgentId,
                            TargetPositionId = x.TargetPositionId,
                            EventTypeId = (int)x.EventType
                        }).ToList();
                    }

                    if (document.Accesses != null)
                    {
                        foreach (var acc in document.Accesses)
                        {
                            foreach (var eacc in doc.Accesses.Where(x => x.Id == acc.Id))
                            {
                                if ((eacc.AccessLevelId != (int)acc.AccessLevel) ||
                                    (eacc.IsFavourite != acc.IsFavourite)
                                    || (eacc.IsInWork != acc.IsInWork) || (eacc.PositionId != acc.PositionId))
                                {
                                    eacc.LastChangeDate = DateTime.Now;
                                    eacc.LastChangeUserId = ctx.CurrentAgentId;
                                    eacc.AccessLevelId = (int)acc.AccessLevel;
                                    eacc.IsFavourite = acc.IsFavourite;
                                    eacc.IsInWork = acc.IsInWork;
                                    eacc.PositionId = acc.PositionId;
                                }
                            }
                        }

                        //var rmv_acc = doc.Accesses.Where(x => !document.Accesses.Select(s => s.Id).Contains(x.Id)).ToList();
                        //dbContext.DocumentAccessesSet.RemoveRange(rmv_acc);

                        if (document.Accesses.Any(x => x.Id == 0))
                        {
                            doc.Accesses = document.Accesses.Where(x => x.Id == 0).Select(x => new DocumentAccesses
                            {
                                LastChangeDate = x.LastChangeDate,
                                IsInWork = x.IsInWork,
                                LastChangeUserId = x.LastChangeUserId,
                                PositionId = x.PositionId,
                                AccessLevelId = (int)x.AccessLevel,
                            }).ToList();
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
        }

        public void DeleteDocument(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //ADD OTHER TABLES!!!!
                dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.DocumentId == id));
                dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.DocumentId == id));
                dbContext.DocumentsSet.RemoveRange(dbContext.DocumentsSet.Where(x => x.Id == id));
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var qry = CommonQueries.GetDocumentQuery(dbContext);

                qry = qry.Where(
                        x =>
                            ctx.CurrentPositionsIdList.Contains(x.Acc.PositionId) &&
                            (!filters.IsInWork || filters.IsInWork && x.Acc.IsInWork == filters.IsInWork));
                   
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

                if (filters.Id != null && filters.Id.Count > 0)
                {
                    qry = qry.Where(x => filters.Id.Contains(x.Doc.Id));
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

                #endregion DocumentsSetFilter

                paging.TotalItemsCount = qry.Count(); //TODO pay attention to this when we will add paging
                qry = qry.OrderByDescending(x => x.Doc.CreateDate)
                    .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                //TODO GroupJoin
                var evnt =
                    dbContext.DocumentEventsSet.Join(qry, ev => ev.DocumentId, rs => rs.Doc.Id, (e, r) => new { ev = e })
                        .GroupBy(g => g.ev.DocumentId)
                        .Select(s => new { DocID = s.Key, EvnCnt = s.Count() }).ToList();

                var fls =
                    dbContext.DocumentFilesSet.Join(qry, fl => fl.DocumentId, rs => rs.Doc.Id, (f, r) => new { fil = f })
                        .GroupBy(g => g.fil.DocumentId)
                        .Select(s => new { DocID = s.Key, FileCnt = s.Count() }).ToList();

                var links = qry.GroupJoin(dbContext.DocumentsSet.Where(x => x.LinkId.HasValue), dl => dl.Doc.LinkId, d => d.LinkId,
                                            (dl, ds) => new { DocID = dl.Doc.Id, LinkCnt = ds.Count() }).ToList();

                var res = qry.Select(x => new FrontDocument
                {
                    Id = x.Doc.Id,
                    DocumentTypeId = x.Templ.DocumentTypeId,
                    AccessLevel = (EnumDocumentAccesses)x.Acc.AccessLevelId,
                    ExecutorPositionId = x.Doc.ExecutorPositionId,
                    DocumentDirection = (EnumDocumentDirections)x.Templ.DocumentDirectionId,
                    Description = x.Doc.Description,
                    TemplateDocumentId = x.Doc.TemplateDocumentId,
                    RegistrationDate = x.Doc.RegistrationDate,
                    DocumentSubjectId = x.Doc.DocumentSubjectId,
                    IsRegistered = x.Doc.IsRegistered,
                    RegistrationNumber = x.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = x.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.Doc.RegistrationNumberSuffix,
                    RegistrationJournalId = x.Doc.RegistrationJournalId,
                    RegistrationJournalName = x.RegJurnalName,
                    RegistrationFullNumber =
                                                (!x.Doc.IsRegistered ? "#" : "") +
                                                (x.Doc.RegistrationNumber != null
                                                        ? (x.Doc.RegistrationNumberPrefix + x.Doc.RegistrationNumber.ToString() + x.Doc.RegistrationNumberSuffix)
                                                        : ("#" + x.Doc.Id.ToString())),

                    CreateDate = x.Doc.CreateDate,
                    DocumentSubjectName = x.SubjName,
                    ExecutorPositionName = x.ExecutorPosName,
                    ExecutorPositionAgentName = x.ExecutorAgentName,
                    DocumentDirectionName = x.DirName,
                    DocumentTypeName = x.DocTypeName,
                    DocumentDate = x.Doc.RegistrationDate ?? x.Doc.CreateDate,
                    IsFavourite = x.Acc.IsFavourite,
                    IsInWork = x.Acc.IsInWork,
                    LinkId = x.Doc.LinkId,
                    EventsCount = 0, //x.Doc.Events.Count,
                    NewEventCount = 0, //TODO
                    AttachedFilesCount = 0, // x.Doc.Files.Count,
                    LinkedDocumentsCount = 0 //TODO
                });

                var docs = res.ToList();
                foreach (var x1 in docs.Join(evnt, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.EventsCount = x1.ev.EvnCnt;
                    x1.doc.NewEventCount = 0;
                }
                foreach (var x1 in docs.Join(fls, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.AttachedFilesCount = x1.ev.FileCnt;
                }
                foreach (var x1 in docs.Join(links, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
                {
                    x1.doc.LinkedDocumentsCount = x1.ev.LinkCnt;
                }

                //paging.TotalPageCount = docs.Count; //TODO pay attention to this when we will add paging
                return docs;
            }
        }

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var dbDoc = CommonQueries.GetDocumentQuery(dbContext).FirstOrDefault(x => x.Doc.Id == documentId && ctx.CurrentPositionsIdList.Contains(x.Acc.PositionId));

                if (dbDoc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                var doc = new FrontDocument
                {
                    Id = dbDoc.Doc.Id,
                    TemplateDocumentId = dbDoc.Doc.TemplateDocumentId,
                    CreateDate = dbDoc.Doc.CreateDate,
                    DocumentSubjectId = dbDoc.Doc.DocumentSubjectId,
                    Description = dbDoc.Doc.Description,
                    IsRegistered = dbDoc.Doc.IsRegistered,
                    RegistrationJournalId = dbDoc.Doc.RegistrationJournalId,
                    RegistrationJournalName = dbDoc.RegJurnalName,
                    NumerationPrefixFormula = dbDoc.Doc.NumerationPrefixFormula,
                    RegistrationNumber = dbDoc.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = dbDoc.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = dbDoc.Doc.RegistrationNumberSuffix,
                    RegistrationDate = dbDoc.Doc.RegistrationDate,
                    ExecutorPositionId = dbDoc.Doc.ExecutorPositionId,
                    LastChangeUserId = dbDoc.Doc.LastChangeUserId,
                    LastChangeDate = dbDoc.Doc.LastChangeDate,
                    SenderAgentId = dbDoc.Doc.SenderAgentId,
                    SenderAgentPersonId = dbDoc.Doc.SenderAgentPersonId,
                    SenderNumber = dbDoc.Doc.SenderNumber,
                    SenderDate = dbDoc.Doc.SenderDate,
                    Addressee = dbDoc.Doc.Addressee,

                    AccessLevel = (EnumDocumentAccesses)dbDoc.Acc.AccessLevelId,
                    AccessLevelName = dbDoc.AccLevName,

                    TemplateDocumentName = dbDoc.Templ.Name,
                    IsHard = dbDoc.Templ.IsHard,
                    DocumentDirection = (EnumDocumentDirections)dbDoc.Templ.DocumentDirectionId,
                    DocumentDirectionName = dbDoc.DirName,
                    DocumentTypeId = dbDoc.Templ.DocumentTypeId,
                    DocumentTypeName = dbDoc.DocTypeName,
                    DocumentSubjectName = dbDoc.SubjName,

                    DocumentDate = dbDoc.Doc.RegistrationDate ?? dbDoc.Doc.CreateDate,
                    RegistrationFullNumber =
                                                (!dbDoc.Doc.IsRegistered ? "#" : "") +
                                                (dbDoc.Doc.RegistrationNumber != null
                                                        ? (dbDoc.Doc.RegistrationNumberPrefix + dbDoc.Doc.RegistrationNumber.ToString() + dbDoc.Doc.RegistrationNumberSuffix)
                                                        : ("#" + dbDoc.Doc.Id.ToString())),
                    GeneralInfo = dbDoc.DirName + " " + dbDoc.DocTypeName,
                    LinkId = dbDoc.Doc.LinkId,
                    IsFavourite = dbDoc.Acc.IsFavourite,
                    IsInWork = dbDoc.Acc.IsInWork,
                    Accesses = new List<BaseDocumentAccess>
                    {
                        new BaseDocumentAccess
                        {
                            LastChangeDate = dbDoc.Acc.LastChangeDate,
                            LastChangeUserId = dbDoc.Acc.LastChangeUserId,
                            IsInWork = dbDoc.Acc.IsInWork,
                            IsFavourite = dbDoc.Acc.IsFavourite,
                            PositionId = dbDoc.Acc.PositionId,
                            AccessLevel = (EnumDocumentAccesses) dbDoc.Acc.AccessLevelId,
                            AccessLevelName = dbDoc.AccLevName,
                            Id = dbDoc.Acc.Id,
                            DocumentId = dbDoc.Acc.DocumentId
                        }
                    },

                    ExecutorPositionName = dbDoc.ExecutorPosName,
                    SenderAgentName = dbDoc.SenderAgentname,
                    ExecutorPositionAgentName = dbDoc.ExecutorAgentName,
                    SenderAgentPersonName = dbDoc.SenderPersonName,
                };


                doc.Events = CommonQueries.GetDocumentEvents(dbContext, new FilterDocumentEvent {DocumentId = new List<int> { doc.Id } });
                doc.EventsCount = doc.Events.Count();
                doc.NewEventCount = 0;

                if (doc.LinkId.HasValue)
                {
                    doc.LinkedDocuments = CommonQueries.GetLinkedDocuments(dbContext, doc.LinkId.Value);
                    doc.LinkedDocumentsCount = doc.LinkedDocuments.Count();
                }


                doc.SendLists = CommonQueries.GetDocumentSendList(dbContext, documentId);
                    

                doc.SendListStageMax = (doc.SendLists == null) || (!doc.SendLists.Any()) ? 0 : doc.SendLists.Max(x => x.Stage);

                doc.RestrictedSendLists = CommonQueries.GetDocumentRestrictedSendList(dbContext, documentId);

                doc.DocumentFiles = CommonQueries.GetDocumentFiles(dbContext, documentId);
                doc.AttachedFilesCount = doc.DocumentFiles.Count();

                doc.DocumentWaits = CommonQueries.GetDocumentWaits(dbContext, new FilterDocumentWaits {DocumentId = documentId});

                return doc;
            }
        }

        public InternalDocument GetDocumentCopy(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var dbDoc = CommonQueries.GetDocumentQuery(dbContext).FirstOrDefault(x => x.Doc.Id == documentId);
                var doc = new InternalDocument
                {
                    TemplateDocumentId = dbDoc.Doc.TemplateDocumentId,
                    CreateDate = DateTime.Now,
                    DocumentSubjectId = dbDoc.Doc.DocumentSubjectId,
                    Description = dbDoc.Doc.Description,
                    IsRegistered = false,
                    ExecutorPositionId = ctx.CurrentPositionId.Value,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    SenderAgentId = dbDoc.Doc.SenderAgentId,
                    SenderAgentPersonId = dbDoc.Doc.SenderAgentPersonId,
                    Addressee = dbDoc.Doc.Addressee,
                    AccessLevel = (EnumDocumentAccesses)dbDoc.Acc.AccessLevelId,
                };

                doc.SendLists = CommonQueries.GetDocumentSendList(dbContext, documentId);
                doc.RestrictedSendLists = CommonQueries.GetDocumentRestrictedSendList(dbContext, documentId);
                doc.DocumentFiles = CommonQueries.GetDocumentFiles(dbContext, documentId);

                return doc;
            }
        }

        public InternalDocument GetInternalDocument(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var dbDoc = CommonQueries.GetDocumentQuery(dbContext).FirstOrDefault(x => x.Doc.Id == documentId && ctx.CurrentPositionsIdList.Contains(x.Acc.PositionId));

                if (dbDoc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                var doc = new InternalDocument
                {
                    Id = dbDoc.Doc.Id,
                    TemplateDocumentId = dbDoc.Doc.TemplateDocumentId,
                    CreateDate = dbDoc.Doc.CreateDate,
                    DocumentSubjectId = dbDoc.Doc.DocumentSubjectId,
                    Description = dbDoc.Doc.Description,
                    IsRegistered = dbDoc.Doc.IsRegistered,
                    RegistrationJournalId = dbDoc.Doc.RegistrationJournalId,
                    NumerationPrefixFormula = dbDoc.Doc.NumerationPrefixFormula,
                    RegistrationNumber = dbDoc.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = dbDoc.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = dbDoc.Doc.RegistrationNumberSuffix,
                    RegistrationDate = dbDoc.Doc.RegistrationDate,
                    ExecutorPositionId = dbDoc.Doc.ExecutorPositionId,
                    LastChangeUserId = dbDoc.Doc.LastChangeUserId,
                    LastChangeDate = dbDoc.Doc.LastChangeDate,
                    SenderAgentId = dbDoc.Doc.SenderAgentId,
                    SenderAgentPersonId = dbDoc.Doc.SenderAgentPersonId,
                    SenderNumber = dbDoc.Doc.SenderNumber,
                    SenderDate = dbDoc.Doc.SenderDate,
                    Addressee = dbDoc.Doc.Addressee,
                    AccessLevel = (EnumDocumentAccesses)dbDoc.Acc.AccessLevelId,
                    IsHard = dbDoc.Templ.IsHard,
                    DocumentDirection = (EnumDocumentDirections)dbDoc.Templ.DocumentDirectionId,
                    DocumentTypeId = dbDoc.Templ.DocumentTypeId,
                    DocumentDate = dbDoc.Doc.RegistrationDate ?? dbDoc.Doc.CreateDate,
                    LinkId = dbDoc.Doc.LinkId,
                };

                return doc;
            }
        }

        public InternalDocument GetCheckRegistration(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext).FirstOrDefault(x => x.Doc.Id == documentId);
                return new InternalDocument
                {
                    IsRegistered = qry.Doc.IsRegistered
                };
            }
        }

        #endregion Document

    }
}