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
using BL.Model.DocumentAdditional;
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

        public void AddDocument(IContext ctx, FullDocument document)
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
                    LastChangeDate = DateTime.Now,
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
                        LastChangeUserId = ctx.CurrentAgentId,
                        LastChangeDate = DateTime.Now
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
                        LastChangeUserId = ctx.CurrentAgentId,
                        LastChangeDate = DateTime.Now
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

        public void UpdateDocument(IContext ctx, FullDocument document)
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

        private IQueryable GetDocumentFrom(DmsContext dbContext)
        {
            var qry = from dc in dbContext.DocumentsSet
                       join acc in dbContext.DocumentAccessesSet on dc.Id equals acc.DocumentId
                       join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                       join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                       join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                       //join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                       join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                       join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                       from exAg in ea.DefaultIfEmpty()

                       join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                       from docsubj in eg.DefaultIfEmpty()

                       join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                           egg
                       from regj in egg.DefaultIfEmpty()
                       select new
                       {
                           Doc = dc,
                           Acc = acc,
                           Templ = tmpl,
                           DirName = ddir.Name,
                           //AccLevName = acl.Name,
                           SubjName = docsubj.Name,
                           DocTypeName = doctp.Name,
                           RegJurnalName = regj.Name,
                           ExecutorPosName = executor.Name,
                           ExecutorAgentName = exAg.Name,

                       };
            return qry;
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var qry = (from dc in dbContext.DocumentsSet
                           join acc in dbContext.DocumentAccessesSet on dc.Id equals acc.DocumentId
                           join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                           join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                           join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                           //join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                           join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                           join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                           from exAg in ea.DefaultIfEmpty()

                           join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                           from docsubj in eg.DefaultIfEmpty()

                           join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                               egg
                           from regj in egg.DefaultIfEmpty()

                               //join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                               //from sendAg in ag.DefaultIfEmpty()

                               //join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                               //from sendAp in ap.DefaultIfEmpty()

                           where
                               ctx.CurrentPositionsIdList.Contains(acc.PositionId) &&
                               (!filters.IsInWork || filters.IsInWork && acc.IsInWork == filters.IsInWork)
                           select new
                           {
                               Doc = dc,
                               Acc = acc,
                               Templ = tmpl,
                               DirName = ddir.Name,
                               //AccLevName = acl.Name,
                               SubjName = docsubj.Name,
                               DocTypeName = doctp.Name,
                               RegJurnalName = regj.Name,
                               ExecutorPosName = executor.Name,
                               ExecutorAgentName = exAg.Name,
                               //SenderAgentname = sendAg.Name,
                               //SenderPersonName = sendAp.Name
                           });


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

                var res = qry.Select(x => new FullDocument
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

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var dbDoc = (from dc in dbContext.DocumentsSet
                             join acc in dbContext.DocumentAccessesSet on dc.Id equals acc.DocumentId
                             join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                             join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                             join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                             join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                             join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                             join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                             from exAg in ea.DefaultIfEmpty()

                             join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                             from docsubj in eg.DefaultIfEmpty()

                             join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                                 egg
                             from regj in egg.DefaultIfEmpty()

                             join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                             from sendAg in ag.DefaultIfEmpty()

                             join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                             from sendAp in ap.DefaultIfEmpty()

                             where dc.Id == documentId && ctx.CurrentPositionsIdList.Contains(acc.PositionId)
                             select new
                             {
                                 Doc = dc,
                                 Acc = acc,
                                 Templ = tmpl,
                                 DirName = ddir.Name,
                                 AccLevName = acl.Name,
                                 SubjName = docsubj.Name,
                                 DocTypeName = doctp.Name,
                                 RegJurnalName = regj.Name,
                                 ExecutorPosName = executor.Name,
                                 ExecutorAgentName = exAg.Name,
                                 SenderAgentname = sendAg.Name,
                                 SenderPersonName = sendAp.Name
                             })
                             .FirstOrDefault();

                if (dbDoc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                var doc = new FullDocument
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


                doc.Events =
                    dbContext.DocumentEventsSet.Where(x => x.DocumentId == doc.Id).Select(y => new BaseDocumentEvent
                    {
                        Id = y.Id,
                        DocumentId = y.DocumentId,
                        Description = y.Description,
                        EventType = (EnumEventTypes)y.EventTypeId,
                        ImportanceEventType = (EnumImportanceEventTypes)y.EventType.ImportanceEventTypeId,
                        CreateDate = y.CreateDate,
                        Date = y.Date,
                        EventTypeName = y.EventType.Name,
                        EventImportanceTypeName = y.EventType.ImportanceEventType.Name,
                        LastChangeUserId = y.LastChangeUserId,
                        LastChangeDate = y.LastChangeDate,
                        SourceAgenName = y.SourceAgent.Name,
                        SourceAgentId = y.SourceAgentId,
                        SourcePositionId = y.SourcePositionId,
                        SourcePositionName = y.SourcePosition.Name,
                        TargetAgenName = y.TargetAgent.Name,
                        TargetAgentId = y.TargetAgentId,
                        TargetPositionId = y.TargetPositionId,
                        TargetPositionName = y.TargetPosition.Name,
                        GeneralInfo = ""
                    }).ToList();

                doc.EventsCount = doc.Events.Count();
                doc.NewEventCount = 0;

                doc.LinkedDocuments = dbContext.DocumentsSet.Where(x => (doc.LinkId.HasValue && x.LinkId == doc.LinkId))
                        .Select(y => new FullDocument
                        {
                            Id = y.Id,
                            GeneralInfo = y.TemplateDocument.DocumentDirection.Name + " " + y.TemplateDocument.DocumentType.Name,
                            RegistrationFullNumber =
                                                (!y.IsRegistered ? "#" : "") +
                                                (y.RegistrationNumber != null
                                                        ? (y.RegistrationNumberPrefix + y.RegistrationNumber.ToString() + y.RegistrationNumberSuffix)
                                                        : ("#" + y.Id.ToString())),
                            DocumentDate = y.RegistrationDate ?? y.CreateDate,
                            Description = y.Description,
                            Links = dbContext.DocumentLinksSet.Where(z => z.DocumentId == y.Id).
                                Select(z => new ВaseDocumentLink
                                {
                                    Id = z.Id,
                                    DocumentId = z.DocumentId,
                                    ParentDocumentId = z.ParentDocumentId,
                                    GeneralInfo = z.LinkType.Name + " " +
                                                (!z.ParentDocument.IsRegistered ? "#" : "") +
                                                (z.ParentDocument.RegistrationNumber != null
                                                        ? (z.ParentDocument.RegistrationNumberPrefix + z.ParentDocument.RegistrationNumber.ToString() + z.ParentDocument.RegistrationNumberSuffix)
                                                        : ("#" + z.ParentDocument.Id.ToString()))
                                    + " " + (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate).ToString()
                                    //TODO String.Format("{0:dd.MM.yyyy}", (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate))
                                }).ToList()
                        }).ToList();
                doc.LinkedDocumentsCount = doc.LinkedDocuments.Count();

                doc.SendLists =
                    dbContext.DocumentSendListsSet.Where(x => x.DocumentId == doc.Id)
                        .Select(y => new BaseDocumentSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            SendTypeName = y.SendType.Name,
                            SendTypeCode = y.SendType.Code,
                            SendTypeIsImportant = y.SendType.IsImportant,
                            TargetPositionId = y.TargetPositionId,
                            TargetPositionName = y.TargetPosition.Name,
                            TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            IsInitial = y.IsInitial,
                            StartEventId = y.StartEventId,
                            CloseEventId = y.CloseEventId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            GeneralInfo = string.Empty
                        }).ToList();

                doc.SendListStageMax = (doc.SendLists == null) || (!doc.SendLists.Any()) ? 0 : doc.SendLists.Max(x => x.Stage);



                doc.RestrictedSendLists =
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == doc.Id)
                        .Select(y => new BaseDocumentRestrictedSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            PositionId = y.PositionId,
                            PositionName = y.Position.Name,
                            PositionExecutorAgentName = y.Position.ExecutorAgent.Name,
                            AccessLevelId = y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            GeneralInfo = string.Empty
                        }).ToList();

                var sq = dbContext.DocumentFilesSet
                                    .Where(x => x.DocumentId == documentId)
                                    .GroupBy(g => new { g.DocumentId, g.OrderNumber })
                                    .Select(x => new { DocId = x.Key.DocumentId, OrdId = x.Key.OrderNumber, MaxVers = x.Max(s => s.Version) });

                doc.DocumentFiles =
                    sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocId, sub.OrdId, VerId = sub.MaxVers },
                        fl => new { DocId = fl.DocumentId, OrdId = fl.OrderNumber, VerId = fl.Version },
                        (s, f) => new { fl = f })
                        .Join(dbContext.DictionaryAgentsSet, df => df.fl.LastChangeUserId, da => da.Id,
                            (d, a) => new { d.fl, agName = a.Name })
                        .Select(x => new DocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).ToList();

                doc.AttachedFilesCount = doc.DocumentFiles.Count();

                //doc.DocumentWaits = GetDocumentWaits(ctx,
                //    x => x.DocumentId == doc.Id,
                //    x=> x);

                return doc;
            }
        }
        #endregion Document

    }
}