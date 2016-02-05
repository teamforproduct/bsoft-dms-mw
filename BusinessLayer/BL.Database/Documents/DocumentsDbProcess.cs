using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Database.DBModel.Document;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.Actions;

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        #region Documents

        public void AddDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc = new DBModel.Document.Documents
            {
                TemplateDocumentId = document.TemplateDocumentId,
                CreateDate = document.CreateDate,
                DocumentSubjectId = document.DocumentSubjectId,
                Description = document.Description,
                RegistrationJournalId = document.RegistrationJournalId,
                RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                RegistrationDate = document.RegistrationDate,
                ExecutorPositionId = document.ExecutorPositionId,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
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
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
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
                    OrderNumber = x.OrderNumber,
                    SendTypeId = (int)x.SendType,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = (int)x.AccessLevel,
                    IsInitial = true,
                    EventId = null,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
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

        public void UpdateDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc = dbContext.DocumentsSet
                .Include(x => x.Accesses)
                .FirstOrDefault(x => x.Id == document.Id);
            if (doc != null)
            {
                doc.TemplateDocumentId = document.TemplateDocumentId;
                doc.DocumentSubjectId = document.DocumentSubjectId;
                doc.Description = document.Description;
                doc.RegistrationJournalId = document.RegistrationJournalId;
                doc.RegistrationNumber = document.RegistrationNumber;
                doc.RegistrationNumberSuffix = document.RegistrationNumberSuffix;
                doc.RegistrationNumberPrefix = document.RegistrationNumberPrefix;
                doc.RegistrationDate = document.RegistrationDate;
                //doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.LastChangeUserId = dbContext.Context.CurrentAgentId;
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
                            if ((eacc.AccessLevelId != (int)acc.AccessLevel) || (eacc.IsFavourite != acc.IsFavourite)
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

        public int AddDocumentEvent(IContext ctx, BaseDocumentEvent docEvent)
        {
            var dbContext = GetUserDmsContext(ctx);
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

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            var dbContext = GetUserDmsContext(ctx);

            var qry = (from dc in dbContext.DocumentsSet
                join acc in dbContext.DocumentAccessesSet on dc.Id equals acc.DocumentId
                join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                //join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                //join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                //from exAg in ea.DefaultIfEmpty()

                join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                from docsubj in eg.DefaultIfEmpty()

                join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into egg
                from regj in egg.DefaultIfEmpty()

                //join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                //from sendAg in ag.DefaultIfEmpty()

                //join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                //from sendAp in ap.DefaultIfEmpty()

                where acc.PositionId == ctx.CurrentPositionId && (!filters.IsInWork || filters.IsInWork && acc.IsInWork == filters.IsInWork)
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
                    //ExecutorAgentName = exAg.Name,
                    //SenderAgentname = sendAg.Name,
                    //SenderPersonName = sendAp.Name
                });

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

            //if (filters.SenderFromDate.HasValue)
            //{
            //    qry = qry.Where(x => x.Doc. >= filters.SenderFromDate.Value);
            //}

            //if (filters.SenderToDate.HasValue)
            //{
            //    qry = qry.Where(x => x.Doc. <= filters.SenderToDate.Value);
            //}

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

            //if (!String.IsNullOrEmpty(filters.Addressee))
            //{
            //    qry = qry.Where(x => x.Doc..Contains(filters.Addressee));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderPerson))
            //{
            //    qry = qry.Where(x => x.Doc..Contains(filters.SenderPerson));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderNumber))
            //{
            //    qry = qry.Where(x => x.Doc..Contains(filters.SenderNumber));
            //}

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

            //if (filters.SenderAgentId != null && filters.SenderAgentId.Count > 0)
            //{
            //    qry = qry.Where(x => filters.SenderAgentId.Contains(x.Doc.));
            //}

            var evnt =
                dbContext.DocumentEventsSet.Join(qry, ev => ev.DocumentId, rs => rs.Doc.Id, (e, r) => new {ev = e})
                    .GroupBy(g => g.ev.DocumentId)
                    .Select(s => new {DocID = s.Key, EvnCnt = s.Count()}).ToList();

            var fls =
                dbContext.DocumentFilesSet.Join(qry, fl => fl.DocumentId, rs => rs.Doc.Id, (f, r) => new {fil = f})
                    .GroupBy(g => g.fil.DocumentId)
                    .Select(s => new {DocID = s.Key, FileCnt = s.Count()}).ToList();

            var res = qry.Select(x => new FullDocument
            {
                Id = x.Doc.Id,
                DocumentTypeId = x.Templ.DocumentTypeId,
                ExecutorPositionId = x.Doc.ExecutorPositionId,
                DocumentDirection = (EnumDocumentDirections)x.Templ.DocumentDirectionId,
                Description = x.Doc.Description,
                TemplateDocumentId = x.Doc.TemplateDocumentId,
                RegistrationDate = x.Doc.RegistrationDate,
                DocumentSubjectId = x.Doc.DocumentSubjectId,
                RegistrationNumber = x.Doc.RegistrationNumber,
                RegistrationNumberPrefix = x.Doc.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Doc.RegistrationNumberSuffix,
                LastChangeDate = x.Doc.LastChangeDate,
                RegistrationJournalId = x.Doc.RegistrationJournalId,
                RegistrationJournalName = x.RegJurnalName,
                CreateDate = x.Doc.CreateDate,
                DocumentSubjectName = x.SubjName,
                ExecutorPositionName = x.ExecutorPosName,
                LastChangeUserId = x.Doc.LastChangeUserId,
                DocumentDirectionName = x.DirName,
                DocumentTypeName = x.DocTypeName,
                DocumentDate = x.Doc.RegistrationDate ?? x.Doc.CreateDate,
                IsFavourite = x.Acc.IsFavourite,
                IsInWork = x.Acc.IsInWork,
                EventsCount = 0, //x.Doc.Events.Count,
                NewEventCount = 0, //TODO
                AttachedFilesCount = 0, // x.Doc.Files.Count,
                LinkedDocumentsCount = 0 //TODO
            });

            var docs = res.ToList();
            foreach (var x1 in docs.Join(evnt, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e}))
            {
                x1.doc.EventsCount = x1.ev.EvnCnt;
                x1.doc.NewEventCount = 0;
            }
            foreach (var x1 in docs.Join(fls, d => d.Id, e => e.DocID, (d, e) => new { doc = d, ev = e }))
            {
                x1.doc.AttachedFilesCount = x1.ev.FileCnt;
            }

            paging.TotalPageCount = docs.Count; //TODO pay attention to this when we will add paging
            return res;
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

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

                         join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into egg
                         from regj in egg.DefaultIfEmpty()

                         join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                         from sendAg in ag.DefaultIfEmpty()

                         join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                         from sendAp in ap.DefaultIfEmpty()

                         where dc.Id == documentId && acc.PositionId == ctx.CurrentPositionId
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
                RegistrationJournalId = dbDoc.Doc.RegistrationJournalId,
                RegistrationJournalName = dbDoc.RegJurnalName,
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
                        dbDoc.Doc.RegistrationNumber != null
                            ? dbDoc.Doc.RegistrationNumber.ToString()
                            : "#" + dbDoc.Doc.Id.ToString(),
                GeneralInfo = dbDoc.DirName + " " + dbDoc.DocTypeName,

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
                    EventType = (EnumEventTypes) y.EventTypeId,
                    ImpotanceEventType = (EnumImpotanceEventTypes) y.EventType.ImpotanceEventTypeId,
                    CreateDate = y.CreateDate,
                    Date = y.Date,
                    EventTypeName = y.EventType.Name,
                    EventImpotanceTypeName = y.EventType.ImpotanceEventType.Name,
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

            doc.LinkedDocumentsCount = 0; //TODO

            doc.SendLists =
                dbContext.DocumentSendListsSet.Where(x => x.DocumentId == doc.Id).Select(y => new BaseDocumentSendList
                {
                    Id = y.Id,
                    DocumentId = y.DocumentId,
                    OrderNumber = y.OrderNumber,
                    SendType = (EnumSendType)y.SendTypeId,
                    SendTypeName = y.SendType.Name,
                    SendTypeCode = y.SendType.Code,
                    SendTypeIsImpotant = y.SendType.IsImpotant,
                    TargetPositionId = y.TargetPositionId,
                    TargetPositionName = y.TargetPosition.Name,
                    TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name,
                    Description = y.Description,
                    DueDate = y.DueDate,
                    DueDay = y.DueDay,
                    AccessLevel = (EnumDocumentAccess)y.AccessLevelId,
                    AccessLevelName = y.AccessLevel.Name,
                    IsInitial = y.IsInitial,
                    EventId = y.EventId,
                    LastChangeUserId = y.LastChangeUserId,
                    LastChangeDate = y.LastChangeDate,
                    GeneralInfo = string.Empty
                }).ToList();

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

            doc.AttachedFilesCount = 0; // TODO select attached files

            return doc;
        }
        #endregion Documents

        #region Document Access

        public void SetDocumentInformation(IContext ctx, EventAccessModel access)
        {
            var dbContext = GetUserDmsContext(ctx);
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

        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var dbContext = GetUserDmsContext(ctx);
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

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == accessId);
            if (acc != null)
            {
                dbContext.DocumentAccessesSet.Remove(acc);
                dbContext.SaveChanges();
            }
        }

        public void UpdateDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var dbContext = GetUserDmsContext(ctx);
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

        public BaseDocumentAccess GetDocumentAccess(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.DocumentId == documentId && x.PositionId == ctx.CurrentPositionId);
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
            return null;
        }
        #endregion

        #region DocumentRestrictedSendLists
        public int AddRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var dbContext = GetUserDmsContext(ctx);

            var sendList = new DocumentRestrictedSendLists
            {
                DocumentId = restrictedSendList.DocumentId,
                PositionId = restrictedSendList.PositionId,
                AccessLevelId = restrictedSendList.AccessLevelId,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentRestrictedSendListsSet.Add(sendList);
            dbContext.SaveChanges();
            return sendList.Id;
        }

        public void AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {
            var dbContext = GetUserDmsContext(ctx);

            foreach (var restrictedSendList in restrictedSendLists)
            {
                var sendList = new DocumentRestrictedSendLists
                {
                    DocumentId = restrictedSendList.DocumentId,
                    PositionId = restrictedSendList.PositionId,
                    AccessLevelId = restrictedSendList.AccessLevelId,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };
                dbContext.DocumentRestrictedSendListsSet.Add(sendList);
            }
            dbContext.SaveChanges();
        }

        public void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var sendList = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendListId);
            if (sendList != null)
            {
                dbContext.DocumentRestrictedSendListsSet.Remove(sendList);
                dbContext.SaveChanges();
            }
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public int AddSendList(IContext ctx, ModifyDocumentSendList sendList)
        {
            var dbContext = GetUserDmsContext(ctx);

            var sl = new DocumentSendLists
            {
                DocumentId = sendList.DocumentId,
                OrderNumber = sendList.OrderNumber,
                SendTypeId = (int)sendList.SendType,
                TargetPositionId = sendList.TargetPositionId,
                Description = sendList.Description,
                DueDate = sendList.DueDate,
                DueDay = sendList.DueDay,
                AccessLevelId = (int)sendList.AccessLevel,
                IsInitial = true,
                EventId = null,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentSendListsSet.Add(sl);
            dbContext.SaveChanges();
            return sl.Id;
        }

        public void AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists)
        {
            var dbContext = GetUserDmsContext(ctx);
            foreach (var sendList in sendLists)
            {
                var sl = new DocumentSendLists
                {
                    DocumentId = sendList.DocumentId,
                    OrderNumber = sendList.OrderNumber,
                    SendTypeId = (int)sendList.SendType,
                    TargetPositionId = sendList.TargetPositionId,
                    Description = sendList.Description,
                    DueDate = sendList.DueDate,
                    DueDay = sendList.DueDay,
                    AccessLevelId = (int)sendList.AccessLevel,
                    IsInitial = true,
                    EventId = null,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };
                dbContext.DocumentSendListsSet.Add(sl);
            }
            dbContext.SaveChanges();
        }

        public void DeleteSendList(IContext ctx, int sendListId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var sendList = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendListId);
            if (sendList != null)
            {
                dbContext.DocumentSendListsSet.Remove(sendList);
                dbContext.SaveChanges();
            }
        }
        #endregion DocumentSendLists

        #region DocumentSavedFilters

        public void AddSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            var dbContext = GetUserDmsContext(ctx);
            var savFilter = new DocumentSavedFilters
            {
                PositionId = dbContext.Context.CurrentPositionId,
                Icon = savedFilter.Icon,
                Filter = savedFilter.Filter.ToString(),
                IsCommon = savedFilter.IsCommon,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };

            dbContext.DocumentSavedFiltersSet.Add(savFilter);
            dbContext.SaveChanges();
            savedFilter.Id = savFilter.Id;
        }

        public void UpdateSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            var dbContext = GetUserDmsContext(ctx);
            var savFilter = dbContext.DocumentSavedFiltersSet.FirstOrDefault(x => x.Id == savedFilter.Id);
            if (savFilter != null)
            {
                savFilter.Id = savedFilter.Id;
                savFilter.PositionId = dbContext.Context.CurrentPositionId;
                savFilter.Icon = savedFilter.Icon;
                savFilter.Filter = savedFilter.Filter.ToString();
                savFilter.IsCommon = savedFilter.IsCommon;
                savFilter.LastChangeUserId = dbContext.Context.CurrentAgentId;
                savFilter.LastChangeDate = DateTime.Now;
            }
            dbContext.SaveChanges();
        }

        public IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            var dbContext = GetUserDmsContext(ctx);
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

        public BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            var dbContext = GetUserDmsContext(ctx);

            var savFilter = dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId).Select(x => new BaseDocumentSavedFilter
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
        public void DeleteSavedFilter(IContext ctx, int savedFilterId)
        {
            var dbContext = GetUserDmsContext(ctx);

            var savFilter = dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId).FirstOrDefault();
            if (savFilter != null)
            {
                dbContext.DocumentSavedFiltersSet.Remove(savFilter);
                dbContext.SaveChanges();
            }
        }
        #endregion DocumentSavedFilters

        #region DocumentWaits

        public void AddDocumentWait(IContext ctx, BaseDocumentWaits documentWait)
        {
            var dbContext = GetUserDmsContext(ctx);

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
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            if (documentWait.OnEvent != null)
            {
                docWait.OnEvent = new DocumentEvents
                {
                    DocumentId = documentWait.OnEvent.DocumentId,
                    EventTypeId = (int)documentWait.OnEvent.EventType,
                    CreateDate = documentWait.OnEvent.CreateDate,
                    Date = documentWait.OnEvent.Date,
                    Description = documentWait.OnEvent.Description,
                    SourcePositionId = documentWait.OnEvent.SourcePositionId,
                    SourceAgentId = documentWait.OnEvent.SourceAgentId,
                    TargetPositionId = documentWait.OnEvent.TargetPositionId,
                    TargetAgentId = documentWait.OnEvent.TargetAgentId,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };
            }
            if (documentWait.OffEvent != null)
            {
                docWait.OffEvent = new DocumentEvents
                {
                    DocumentId = documentWait.OnEvent.DocumentId,
                    EventTypeId = (int)documentWait.OnEvent.EventType,
                    CreateDate = documentWait.OnEvent.CreateDate,
                    Date = documentWait.OnEvent.Date,
                    Description = documentWait.OnEvent.Description,
                    SourcePositionId = documentWait.OnEvent.SourcePositionId,
                    SourceAgentId = documentWait.OnEvent.SourceAgentId,
                    TargetPositionId = documentWait.OnEvent.TargetPositionId,
                    TargetAgentId = documentWait.OnEvent.TargetAgentId,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };
            }

            dbContext.DocumentWaitsSet.Add(docWait);
            dbContext.SaveChanges();
            documentWait.Id = docWait.Id;
        }
        #endregion DocumentWaits

        #region DocumentTemporaryRegistrations
        public RegisterDocument GetTemporaryRegistration(IContext ctx, int temporaryRegistrationsId)
        {
            var dbContext = GetUserDmsContext(ctx);
            return dbContext.DocumentTemporaryRegistrationsSet
                        .Where(x => x.Id == temporaryRegistrationsId)
                        .Select(x => new RegisterDocument
                        {
                            Id = x.Id,
                            RegistrationJournalId = x.RegistrationJournalId,
                            RegistrationNumber = x.RegistrationNumber,
                            RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                            RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                            RegistrationDate = x.RegistrationDate
                        }).FirstOrDefault();
        }

        public int AddTemporaryRegistration(IContext ctx, RegisterDocument registerDocument)
        {
            var dbContext = GetUserDmsContext(ctx);
            var temporaryRegistration = dbContext.DocumentTemporaryRegistrationsSet.FirstOrDefault(x => x.Id == registerDocument.Id);
            if (temporaryRegistration != null)
            {
                dbContext.DocumentTemporaryRegistrationsSet.Remove(temporaryRegistration);
                dbContext.SaveChanges();
            }
            if (registerDocument.RegistrationNumber == null)
            {   //get next number
                var i = ((from doc in dbContext.DocumentsSet
                         where doc.RegistrationJournalId == registerDocument.RegistrationJournalId
                         && doc.RegistrationNumberPrefix == registerDocument.RegistrationNumberPrefix
                         select doc.RegistrationNumber ?? 0)
                         .Union
                        (from doc in dbContext.DocumentTemporaryRegistrationsSet
                         where doc.RegistrationJournalId == registerDocument.RegistrationJournalId
                         && doc.RegistrationNumberPrefix == registerDocument.RegistrationNumberPrefix
                         select doc.RegistrationNumber)).Max() + 1;


            }

            temporaryRegistration = new DocumentTemporaryRegistrations
            {
                Id = registerDocument.Id,
                RegistrationJournalId = registerDocument.RegistrationJournalId,
                RegistrationNumber = (int)registerDocument.RegistrationNumber, //???
                RegistrationNumberSuffix = registerDocument.RegistrationNumberSuffix,
                RegistrationNumberPrefix = registerDocument.RegistrationNumberPrefix,
                RegistrationDate = registerDocument.RegistrationDate,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentTemporaryRegistrationsSet.Add(temporaryRegistration);
            dbContext.SaveChanges();
            return temporaryRegistration.Id;
        }

        public void SetDocumentRegistration(IContext ctx, int temporaryRegistrationsId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var temporaryRegistration = dbContext.DocumentTemporaryRegistrationsSet.FirstOrDefault(x => x.Id == temporaryRegistrationsId);
            if (temporaryRegistration != null)
            {
                var doc = dbContext.DocumentsSet
                            .FirstOrDefault(x => x.Id == temporaryRegistration.Id);
                if (doc != null)
                {
                    doc.RegistrationJournalId = temporaryRegistration.RegistrationJournalId;
                    doc.RegistrationNumber = temporaryRegistration.RegistrationNumber;
                    doc.RegistrationNumberSuffix = temporaryRegistration.RegistrationNumberSuffix;
                    doc.RegistrationNumberPrefix = temporaryRegistration.RegistrationNumberPrefix;
                    doc.RegistrationDate = temporaryRegistration.RegistrationDate;
                    doc.LastChangeUserId = dbContext.Context.CurrentAgentId;
                    doc.LastChangeDate = DateTime.Now;
                    dbContext.DocumentTemporaryRegistrationsSet.Remove(temporaryRegistration);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentTemporaryRegistrations

    }
}