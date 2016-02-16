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
using BL.Model.Database;
using BL.Model.DocumentAdditional;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.Actions;
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
                    AccessLevel = (EnumDocumentAccess)x.Acc.AccessLevelId,
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
                                                (!x.Doc.IsRegistered? "#": "") +
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
                             }).FirstOrDefault();

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

                    AccessLevel = (EnumDocumentAccess)dbDoc.Acc.AccessLevelId,
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
                            AccessLevel = (EnumDocumentAccess) dbDoc.Acc.AccessLevelId,
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

                doc.LinkedDocuments = dbContext.DocumentsSet.Where(x => x.LinkId == doc.LinkId)
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
                            SendType = (EnumSendType)y.SendTypeId,
                            SendTypeName = y.SendType.Name,
                            SendTypeCode = y.SendType.Code,
                            SendTypeIsImportant = y.SendType.IsImportant,
                            TargetPositionId = y.TargetPositionId,
                            TargetPositionName = y.TargetPosition.Name,
                            TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccess)y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            IsInitial = y.IsInitial,
                            StartEventId = y.StartEventId,
                            CloseEventId = y.CloseEventId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            GeneralInfo = string.Empty
                        }).ToList();

                doc.SendListStageMax = (doc.SendLists == null) || (doc.SendLists.Count() == 0) ? 0 : doc.SendLists.Max(x => x.Stage);



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

                return doc;
            }
        }
        #endregion Document

        #region Document Event

        public int AddDocumentEvent(IContext ctx, BaseDocumentEvent docEvent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var evt = new DocumentEvents
                {
                    CreateDate = docEvent.CreateDate,
                    Date = docEvent.Date,
                    Description = docEvent.Description,
                    LastChangeDate = docEvent.LastChangeDate,
                    LastChangeUserId = docEvent.LastChangeUserId,
                    SourceAgentId = docEvent.SourceAgentId,
                    SourcePositionId = docEvent.SourcePositionId,
                    TargetAgentId = docEvent.TargetAgentId,
                    TargetPositionId = docEvent.TargetPositionId,
                    EventTypeId = (int)docEvent.EventType
                };
                dbContext.DocumentEventsSet.Add(evt);
                dbContext.SaveChanges();
                return evt.Id;
            }
        }

        public IEnumerable<BaseDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.DocumentEventsSet.AsQueryable();

                if (filter.Id?.Count > 0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                return qry.Select(x => new BaseDocumentEvent
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Description = x.Description,
                    EventType = (EnumEventTypes)x.EventTypeId,
                    ImportanceEventType = (EnumImportanceEventTypes)x.EventType.ImportanceEventTypeId,
                    CreateDate = x.CreateDate,
                    Date = x.Date,
                    EventTypeName = x.EventType.Name,
                    EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    SourceAgenName = x.SourceAgent.Name,
                    SourceAgentId = x.SourceAgentId,
                    SourcePositionId = x.SourcePositionId,
                    SourcePositionName = x.SourcePosition.Name,
                    TargetAgenName = x.TargetAgent.Name,
                    TargetAgentId = x.TargetAgentId,
                    TargetPositionId = x.TargetPositionId,
                    TargetPositionName = x.TargetPosition.Name,
                    GeneralInfo = ""
                }).ToList();
            }
        }
        #endregion Document Event


        #region Document Access

        public void SetDocumentInformation(IContext ctx, EventAccessModel access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == access.DocumentAccess.Id);
                if (acc != null)
                {
                    acc.LastChangeDate = DateTime.Now;
                    acc.IsInWork = access.DocumentAccess.IsInWork;
                    acc.LastChangeUserId = ctx.CurrentAgentId;
                    acc.PositionId = access.DocumentAccess.PositionId;
                    acc.AccessLevelId = (int)access.DocumentAccess.AccessLevel;
                    acc.IsFavourite = access.DocumentAccess.IsFavourite;
                }
                var evt = new DocumentEvents
                {
                    CreateDate = access.DocumentEvent.CreateDate,
                    Date = access.DocumentEvent.Date,
                    Description = access.DocumentEvent.Description,
                    LastChangeDate = access.DocumentEvent.LastChangeDate,
                    LastChangeUserId = access.DocumentEvent.LastChangeUserId,
                    SourceAgentId = access.DocumentEvent.SourceAgentId,
                    SourcePositionId = access.DocumentEvent.SourcePositionId,
                    TargetAgentId = access.DocumentEvent.TargetAgentId,
                    TargetPositionId = access.DocumentEvent.TargetPositionId,
                    EventTypeId = (int)access.DocumentEvent.EventType
                };
                dbContext.DocumentEventsSet.Add(evt);
                dbContext.SaveChanges();
            }
        }

        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = new DocumentAccesses
                {
                    LastChangeDate = access.LastChangeDate,
                    IsInWork = access.IsInWork,
                    LastChangeUserId = access.LastChangeUserId,
                    PositionId = access.PositionId,
                    AccessLevelId = (int)access.AccessLevel,
                    IsFavourite = access.IsFavourite
                };
                dbContext.DocumentAccessesSet.Add(acc);
                dbContext.SaveChanges();
                return acc.Id;
            }
        }

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == accessId);
                if (acc != null)
                {
                    dbContext.DocumentAccessesSet.Remove(acc);
                    dbContext.SaveChanges();
                }
            }
        }

        public void UpdateDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == access.Id);
                if (acc != null)
                {
                    acc.LastChangeDate = DateTime.Now;
                    acc.IsInWork = access.IsInWork;
                    acc.LastChangeUserId = ctx.CurrentAgentId;
                    acc.PositionId = access.PositionId;
                    acc.AccessLevelId = (int)access.AccessLevel;
                    acc.IsFavourite = access.IsFavourite;
                    dbContext.SaveChanges();
                }
            }
        }

        public BaseDocumentAccess GetDocumentAccess(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc =
                    dbContext.DocumentAccessesSet.FirstOrDefault(
                        x => x.DocumentId == documentId && x.PositionId == ctx.CurrentPositionId);
                if (acc != null)
                {
                    return new BaseDocumentAccess
                    {
                        LastChangeDate = acc.LastChangeDate,
                        LastChangeUserId = acc.LastChangeUserId,
                        Id = acc.Id,
                        PositionId = acc.PositionId,
                        IsInWork = acc.IsInWork,
                        DocumentId = acc.DocumentId,
                        IsFavourite = acc.IsFavourite,
                        AccessLevel = (EnumDocumentAccess)acc.AccessLevelId,
                        AccessLevelName = acc.AccessLevel.Name
                    };
                }
            }
            return null;
        }
        #endregion

        #region DocumentRestrictedSendLists
        public ModifyDocumentRestrictedSendList GetRestrictedSendListById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendList = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new ModifyDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        AccessLevelId = x.AccessLevelId,
                    }).FirstOrDefault();

                return sendList;
            }
        }
        public BaseDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var sendList = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new BaseDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        PositionName = x.Position.Name,
                        PositionExecutorAgentName = x.Position.ExecutorAgent.Name,
                        AccessLevelId = x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).FirstOrDefault();

                return sendList;
            }
        }
        public IEnumerable<ModifyDocumentRestrictedSendList> GetRestrictedSendList(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new ModifyDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        AccessLevelId = x.AccessLevelId,
                    }).ToList();

                return sendLists;
            }
        }

        public IEnumerable<BaseDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new BaseDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        PositionName = x.Position.Name,
                        PositionExecutorAgentName = x.Position.ExecutorAgent.Name,
                        AccessLevelId = x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).ToList();

                return sendLists;
            }
        }

        public void UpdateRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendList =
                    dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendList.Id);
                if (sendList?.Id > 0)
                {
                    sendList.DocumentId = restrictedSendList.DocumentId;
                    sendList.PositionId = restrictedSendList.PositionId;
                    sendList.AccessLevelId = restrictedSendList.AccessLevelId;
                    sendList.LastChangeUserId = ctx.CurrentAgentId;
                    sendList.LastChangeDate = DateTime.Now;

                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<int> AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = restrictedSendLists.Select(x => new DocumentRestrictedSendLists
                {
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    AccessLevelId = x.AccessLevelId,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                }).ToList();

                dbContext.DocumentRestrictedSendListsSet.AddRange(sendLists);

                dbContext.SaveChanges();

                return sendLists.Select(x => x.Id).ToList();
            }
        }

        public void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sendList = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendListId);
                if (sendList != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(sendList);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public IEnumerable<ModifyDocumentSendList> GetSendList(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new ModifyDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendType)x.SendTypeId,
                        TargetPositionId = x.TargetPositionId,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccess)x.AccessLevelId
                    }).ToList();

                return sendLists;
            }
        }

        public IEnumerable<BaseDocumentSendList> GetSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new BaseDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendType)x.SendTypeId,
                        SendTypeName = x.SendType.Name,
                        SendTypeCode = x.SendType.Code,
                        SendTypeIsImportant = x.SendType.IsImportant,
                        TargetPositionId = x.TargetPositionId,
                        TargetPositionName = x.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccess)x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                        CloseEventId = x.CloseEventId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).ToList();

                return sendLists;
            }
        }

        public ModifyDocumentSendList GetSendListById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new ModifyDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendType)x.SendTypeId,
                        TargetPositionId = x.TargetPositionId,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccess)x.AccessLevelId
                    }).FirstOrDefault();

                return sendLists;
            }
        }

        public BaseDocumentSendList GetSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new BaseDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendType)x.SendTypeId,
                        SendTypeName = x.SendType.Name,
                        SendTypeCode = x.SendType.Code,
                        SendTypeIsImportant = x.SendType.IsImportant,
                        TargetPositionId = x.TargetPositionId,
                        TargetPositionName = x.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccess)x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                        CloseEventId = x.CloseEventId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).FirstOrDefault();

                return sendLists;
            }
        }

        public void UpdateSendList(IContext ctx, ModifyDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sl = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendList.Id);
                if (sl?.Id > 0)
                {
                    sl.DocumentId = sendList.DocumentId;
                    sl.Stage = sendList.Stage;
                    sl.SendTypeId = (int)sendList.SendType;
                    sl.TargetPositionId = sendList.TargetPositionId;
                    sl.Description = sendList.Description;
                    sl.DueDate = sendList.DueDate;
                    sl.DueDay = sendList.DueDay;
                    sl.AccessLevelId = (int)sendList.AccessLevel;
                    sl.IsInitial = true;
                    sl.StartEventId = null;
                    sl.CloseEventId = null;
                    sl.LastChangeUserId = ctx.CurrentAgentId;
                    sl.LastChangeDate = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<int> AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sls = sendLists.Select(x => new DocumentSendLists
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

                dbContext.DocumentSendListsSet.AddRange(sls);
                dbContext.SaveChanges();

                return sls.Select(x => x.Id).ToList();
            }
        }

        public void DeleteSendList(IContext ctx, int sendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sendList = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendListId);
                if (sendList != null)
                {
                    dbContext.DocumentSendListsSet.Remove(sendList);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentSendLists

        #region DocumentSavedFilters

        public void AddSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var savFilter = new DocumentSavedFilters
                {
                    PositionId = ctx.CurrentPositionId,
                    Icon = savedFilter.Icon,
                    Filter = savedFilter.Filter.ToString(),
                    IsCommon = savedFilter.IsCommon,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };

                dbContext.DocumentSavedFiltersSet.Add(savFilter);
                dbContext.SaveChanges();
                savedFilter.Id = savFilter.Id;
            }
        }

        public void UpdateSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var savFilter = dbContext.DocumentSavedFiltersSet.FirstOrDefault(x => x.Id == savedFilter.Id);
                if (savFilter != null)
                {
                    savFilter.Id = savedFilter.Id;
                    savFilter.PositionId = ctx.CurrentPositionId;
                    savFilter.Icon = savedFilter.Icon;
                    savFilter.Filter = savedFilter.Filter.ToString();
                    savFilter.IsCommon = savedFilter.IsCommon;
                    savFilter.LastChangeUserId = ctx.CurrentAgentId;
                    savFilter.LastChangeDate = DateTime.Now;
                }
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.DocumentSavedFiltersSet.AsQueryable();

                //TODO: Uncomment to get the filters on the positions
                //var positionId = dbContext.Context.CurrentPositionId;
                //qry = qry.Where(x => x.PositionId == positionId);

                var res = qry.Select(x => new BaseDocumentSavedFilter
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    PositionName = x.Position.Name
                }).ToList();
                return res;
            }
        }

        public BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var savFilter =
                    dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId)
                        .Select(x => new BaseDocumentSavedFilter
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            Icon = x.Icon,
                            Filter = x.Filter,
                            IsCommon = x.IsCommon,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            PositionName = x.Position.Name
                        }).FirstOrDefault();
                return savFilter;
            }
        }
        public void DeleteSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var savFilter = dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId).FirstOrDefault();
                if (savFilter != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(savFilter);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentSavedFilters

        #region DocumentWaits
        public BaseDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var waitDb = dbContext.DocumentWaitsSet
                    .Where(x => x.OnEventId == eventId)
                    .Select(x => new { Wait = x, OnEvent = x.OnEvent, OffEvent = x.OffEvent })
                    .FirstOrDefault();

                var wait = new BaseDocumentWaits
                {
                    Id = waitDb.Wait.Id,
                    DocumentId = waitDb.Wait.DocumentId,
                    ParentId = waitDb.Wait.ParentId,
                    OnEventId = waitDb.Wait.OnEventId,
                    OffEventId = waitDb.Wait.OffEventId,
                    ResultTypeId = waitDb.Wait.ResultTypeId,
                    Description = waitDb.Wait.Description,
                    DueDate = waitDb.Wait.DueDate,
                    AttentionDate = waitDb.Wait.AttentionDate
                };
                if (waitDb.OnEvent?.Id>0)
                {
                    wait.OnEvent = new BaseDocumentEvent
                    {
                        Id = waitDb.Wait.OnEvent.Id,
                        CreateDate = waitDb.Wait.OnEvent.CreateDate,
                        Date = waitDb.Wait.OnEvent.Date,
                        Description = waitDb.Wait.OnEvent.Description,
                        LastChangeDate = waitDb.Wait.OnEvent.LastChangeDate,
                        LastChangeUserId = waitDb.Wait.OnEvent.LastChangeUserId,
                        SourceAgentId = waitDb.Wait.OnEvent.SourceAgentId,
                        SourcePositionId = waitDb.Wait.OnEvent.SourcePositionId,
                        TargetAgentId = waitDb.Wait.OnEvent.TargetAgentId,
                        TargetPositionId = waitDb.Wait.OnEvent.TargetPositionId,
                        EventType = (EnumEventTypes)waitDb.Wait.OnEvent.EventTypeId
                    };
                }
                if (waitDb.OffEvent?.Id > 0)
                {
                    wait.OffEvent = new BaseDocumentEvent
                    {
                        Id = waitDb.Wait.OffEvent?.Id ?? 0,
                        CreateDate = waitDb.Wait.OffEvent.CreateDate,
                        Date = waitDb.Wait.OffEvent.Date,
                        Description = waitDb.Wait.OffEvent.Description,
                        LastChangeDate = waitDb.Wait.OffEvent.LastChangeDate,
                        LastChangeUserId = waitDb.Wait.OffEvent.LastChangeUserId,
                        SourceAgentId = waitDb.Wait.OffEvent.SourceAgentId,
                        SourcePositionId = waitDb.Wait.OffEvent.SourcePositionId,
                        TargetAgentId = waitDb.Wait.OffEvent.TargetAgentId,
                        TargetPositionId = waitDb.Wait.OffEvent.TargetPositionId,
                        EventType = (EnumEventTypes)waitDb.Wait.OffEvent.EventTypeId
                    };
                }

                return wait;
            }
        }
        public void AddDocumentWait(IContext ctx, BaseDocumentWaits documentWait)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var docWait = new DocumentWaits
                {
                    DocumentId = documentWait.DocumentId,
                    ParentId = documentWait.ParentId,
                    OnEventId = documentWait.OnEventId,
                    OffEventId = documentWait.OffEventId,
                    ResultTypeId = documentWait.ResultTypeId,
                    Description = documentWait.Description,
                    DueDate = documentWait.DueDate,
                    AttentionDate = documentWait.AttentionDate,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };

                UpdateDocumentWaitEvents(ctx, docWait, documentWait);

                dbContext.DocumentWaitsSet.Add(docWait);
                dbContext.SaveChanges();
                documentWait.Id = docWait.Id;
                if (docWait.OnEvent?.Id > 0 && documentWait.OnEvent != null)
                {
                    documentWait.OnEvent.Id = docWait.OnEvent.Id;
                }
                if (docWait.OffEvent?.Id > 0 && documentWait.OffEvent != null)
                {
                    documentWait.OffEvent.Id = docWait.OffEvent.Id;
                }
            }
        }
        public void UpdateDocumentWait(IContext ctx, BaseDocumentWaits documentWait)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var docWait = dbContext.DocumentWaitsSet.Where(x => x.Id == documentWait.Id).FirstOrDefault();
                if (docWait?.Id > 0)
                {
                    docWait.DocumentId = documentWait.DocumentId;
                    docWait.ParentId = documentWait.ParentId;
                    docWait.OnEventId = documentWait.OnEventId;
                    docWait.OffEventId = documentWait.OffEventId;
                    docWait.ResultTypeId = documentWait.ResultTypeId;
                    docWait.Description = documentWait.Description;
                    docWait.DueDate = documentWait.DueDate;
                    docWait.AttentionDate = documentWait.AttentionDate;
                    docWait.LastChangeUserId = ctx.CurrentAgentId;
                    docWait.LastChangeDate = DateTime.Now;

                    UpdateDocumentWaitEvents(ctx, docWait, documentWait);

                    dbContext.SaveChanges();

                    if (docWait.OnEvent?.Id > 0 && documentWait.OnEvent != null)
                    {
                        documentWait.OnEvent.Id = docWait.OnEvent.Id;
                    }
                    if (docWait.OffEvent?.Id > 0 && documentWait.OffEvent != null)
                    {
                        documentWait.OffEvent.Id = docWait.OffEvent.Id;
                    }
                }
            }
        }
        private void UpdateDocumentWaitEvents(IContext ctx, DocumentWaits docWait, BaseDocumentWaits documentWait)
        {
            if (documentWait.OnEvent != null)
            {
                    docWait.OnEvent = new DocumentEvents
                    {
                        Id=documentWait.Id,
                        DocumentId = documentWait.OnEvent.DocumentId,
                        EventTypeId = (int)documentWait.OnEvent.EventType,
                        CreateDate = documentWait.OnEvent.CreateDate,
                        Date = documentWait.OnEvent.Date,
                        Description = documentWait.OnEvent.Description,
                        SourcePositionId = documentWait.OnEvent.SourcePositionId,
                        SourceAgentId = documentWait.OnEvent.SourceAgentId,
                        TargetPositionId = documentWait.OnEvent.TargetPositionId,
                        TargetAgentId = documentWait.OnEvent.TargetAgentId,
                        LastChangeUserId = ctx.CurrentAgentId,
                        LastChangeDate = DateTime.Now
                    };
            }
            if (documentWait.OffEvent != null)
            {
                    docWait.OffEvent = new DocumentEvents
                    {
                        Id = documentWait.OffEvent.Id,
                        DocumentId = documentWait.OffEvent.DocumentId,
                        EventTypeId = (int)documentWait.OffEvent.EventType,
                        CreateDate = documentWait.OffEvent.CreateDate,
                        Date = documentWait.OffEvent.Date,
                        Description = documentWait.OffEvent.Description,
                        SourcePositionId = documentWait.OffEvent.SourcePositionId,
                        SourceAgentId = documentWait.OffEvent.SourceAgentId,
                        TargetPositionId = documentWait.OffEvent.TargetPositionId,
                        TargetAgentId = documentWait.OffEvent.TargetAgentId,
                        LastChangeUserId = ctx.CurrentAgentId,
                        LastChangeDate = DateTime.Now
                    };
            }
        }
        #endregion DocumentWaits

        #region DocumentRegistration

        public bool VerifyDocumentRegistrationNumber(IContext ctx, RegisterDocument registerDocument)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = dbContext.DocumentsSet
                    .FirstOrDefault(x => x.RegistrationJournalId == registerDocument.RegistrationJournalId
                                         && x.NumerationPrefixFormula == registerDocument.NumerationPrefixFormula
                                         && x.RegistrationNumberPrefix == registerDocument.RegistrationNumberPrefix
                                         && x.RegistrationNumber == registerDocument.RegistrationNumber
                                         && x.Id != registerDocument.DocumentId
                    );
                return doc == null;
            }
        }

        public bool SetDocumentRegistration(IContext ctx, RegisterDocument registerDocument)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == registerDocument.DocumentId);
                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                if (doc.IsRegistered)
                {
                    throw new DocumentHasAlredyBeenRegistered();
                }
                var isNeedGenerateNumber = registerDocument.RegistrationNumber == null;
                var isRepeat = true;
                var isOk = false;
                while (isRepeat)
                {
                    if (isNeedGenerateNumber)
                    {
                        //get next number
                        var maxNumber = (from docreg in dbContext.DocumentsSet
                                         where docreg.RegistrationJournalId == registerDocument.RegistrationJournalId
                                               && docreg.NumerationPrefixFormula == registerDocument.NumerationPrefixFormula
                                               && docreg.Id != registerDocument.DocumentId
                                         select docreg.RegistrationNumber).Max();
                        registerDocument.RegistrationNumber = (maxNumber ?? 0) + 1;

                    }
                    doc.IsRegistered = !registerDocument.IsOnlyGetNextNumber;
                    doc.RegistrationJournalId = registerDocument.RegistrationJournalId;
                    doc.RegistrationNumber = registerDocument.RegistrationNumber;
                    doc.NumerationPrefixFormula = registerDocument.NumerationPrefixFormula;
                    doc.RegistrationNumberSuffix = registerDocument.RegistrationNumberSuffix;
                    doc.RegistrationNumberPrefix = registerDocument.RegistrationNumberPrefix;
                    doc.RegistrationDate = registerDocument.RegistrationDate;
                    doc.LastChangeUserId = ctx.CurrentAgentId;
                    doc.LastChangeDate = DateTime.Now;
                    dbContext.SaveChanges();
                    isOk = VerifyDocumentRegistrationNumber(ctx, registerDocument);
                    isRepeat = isOk ? !isOk : isNeedGenerateNumber;
                }
                if (!isOk)
                {
                    doc.IsRegistered = false;
                    doc.RegistrationJournalId = null;
                    doc.NumerationPrefixFormula = null;
                    doc.RegistrationNumber = null;
                    doc.RegistrationNumberSuffix = null;
                    doc.RegistrationNumberPrefix = null;
                    doc.RegistrationDate = null;
                    doc.LastChangeUserId = ctx.CurrentAgentId;
                    doc.LastChangeDate = DateTime.Now;
                    dbContext.SaveChanges();
                    throw new DocumentCouldNotBeRegistered();
                }
                return isOk;
            }
        }


        #endregion DocumentRegistration

        #region DocumentLink    

        public void AddDocumentLink(IContext context, ВaseDocumentLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var link = new DocumentLinks
                {
                    DocumentId = model.DocumentId,
                    ParentDocumentId = model.ParentDocumentId,
                    LinkTypeId = model.LinkTypeId,
                    LastChangeUserId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                };
                dbContext.DocumentLinksSet.Add(link);
                if (!model.ParentDocument.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.ParentDocumentId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                if (!model.Document.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.DocumentId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.LinkId == model.Document.LinkId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentLink

    }
}