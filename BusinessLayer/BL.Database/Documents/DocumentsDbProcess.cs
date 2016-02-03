using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Database.DBModel.Document;
using BL.Model.SystemCore;
using BL.Model.Enums;

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
                LastChangeDate = DateTime.Now
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
                    SendTypeId = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
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
                    AccessLevelId = (int)x.AccessType,
                }).ToList();
            }

            dbContext.DocumentsSet.Add(doc);
            dbContext.SaveChanges();
            document.Id = doc.Id;
        }

        public void UpdateDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == document.Id);
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
                doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.LastChangeUserId = dbContext.Context.CurrentAgentId;
                doc.LastChangeDate = DateTime.Now;


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
                        EventTypeId = (int) x.EventType
                    }).ToList();
                }

                if (document.Accesses != null)
                {
                    foreach (var acc in document.Accesses)
                    {
                        foreach (var eacc in doc.Accesses.Where(x => x.Id == acc.Id))
                        {
                            if ((eacc.AccessLevelId != (int) acc.AccessType) || (eacc.IsFavourtite != acc.IsFavourite)
                                || (eacc.IsInWork != acc.IsInWork) || (eacc.PositionId != acc.PositionId))
                            {
                                eacc.LastChangeDate = DateTime.Now;
                                eacc.LastChangeUserId = ctx.CurrentAgentId;
                                eacc.AccessLevelId = (int) acc.AccessType;
                                eacc.IsFavourtite = acc.IsFavourite;
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
                            AccessLevelId = (int) x.AccessType,
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
            var qry = dbContext.DocumentsSet.Include(x=>x.Accesses)
                .Where(x => x.Accesses.All(a=>a.IsInWork == filters.IsInWork && a.PositionId == ctx.CurrentPositionId));

            if (filters.CreateFromDate.HasValue)
            {
                qry = qry.Where(x => x.CreateDate >= filters.CreateFromDate.Value);
            }

            if (filters.CreateToDate.HasValue)
            {
                qry = qry.Where(x => x.CreateDate <= filters.CreateToDate.Value);
            }

            if (filters.RegistrationFromDate.HasValue)
            {
                qry = qry.Where(x => x.RegistrationDate >= filters.RegistrationFromDate.Value);
            }

            if (filters.RegistrationToDate.HasValue)
            {
                qry = qry.Where(x => x.RegistrationDate <= filters.RegistrationToDate.Value);
            }

            //if (filters.SenderFromDate.HasValue)
            //{
            //    qry = qry.Where(x => x. >= filters.SenderFromDate.Value);
            //}

            //if (filters.SenderToDate.HasValue)
            //{
            //    qry = qry.Where(x => x. <= filters.SenderToDate.Value);
            //}

            if (!String.IsNullOrEmpty(filters.Description))
            {
                qry = qry.Where(x => x.Description.Contains(filters.Description));
            }

            if (!String.IsNullOrEmpty(filters.RegistrationNumber))
            {
                qry =
                    qry.Where(
                        x =>
                            (x.RegistrationNumberPrefix + x.RegistrationNumber.ToString() + x.RegistrationNumberSuffix)
                                .Contains(filters.RegistrationNumber));
            }

            //if (!String.IsNullOrEmpty(filters.Addressee))
            //{
            //    qry = qry.Where(x => x..Contains(filters.Addressee));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderPerson))
            //{
            //    qry = qry.Where(x => x..Contains(filters.SenderPerson));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderNumber))
            //{
            //    qry = qry.Where(x => x..Contains(filters.SenderNumber));
            //}

            if (filters.DocumentTypeId != null && filters.DocumentTypeId.Count > 0)
            {
                qry = qry.Where(x => filters.DocumentTypeId.Contains(x.TemplateDocument.DocumentTypeId));
            }

            if (filters.Id != null && filters.Id.Count > 0)
            {
                qry = qry.Where(x => filters.Id.Contains(x.Id));
            }

            if (filters.TemplateDocumentId != null && filters.TemplateDocumentId.Count > 0)
            {
                qry = qry.Where(x => filters.TemplateDocumentId.Contains(x.TemplateDocumentId));
            }

            if (filters.DocumentDirectionId != null && filters.DocumentDirectionId.Count > 0)
            {
                qry = qry.Where(x => filters.DocumentDirectionId.Contains(x.TemplateDocument.DocumentDirectionId));
            }

            if (filters.DocumentSubjectId != null && filters.DocumentSubjectId.Count > 0)
            {
                qry =
                    qry.Where(
                        x =>
                            x.DocumentSubjectId.HasValue &&
                            filters.DocumentSubjectId.Contains(x.DocumentSubjectId.Value));
            }

            if (filters.RegistrationJournalId != null && filters.RegistrationJournalId.Count > 0)
            {
                qry =
                    qry.Where(
                        x =>
                            x.RegistrationJournalId.HasValue &&
                            filters.RegistrationJournalId.Contains(x.RegistrationJournalId.Value));
            }

            if (filters.ExecutorPositionId != null && filters.ExecutorPositionId.Count > 0)
            {
                qry = qry.Where(x => filters.ExecutorPositionId.Contains(x.ExecutorPositionId));
            }

            //if (filters.SenderAgentId != null && filters.SenderAgentId.Count > 0)
            //{
            //    qry = qry.Where(x => filters.SenderAgentId.Contains(x.));
            //}

            var res = qry.Select(x => new { Doc = x, Acc = x.Accesses.FirstOrDefault() })
                .Select(x => new FullDocument
            {
                Id = x.Doc.Id,
                DocumentTypeId = x.Doc.TemplateDocument.DocumentTypeId,
                ExecutorPositionId = x.Doc.ExecutorPositionId,
                DocumentDirectionId = x.Doc.TemplateDocument.DocumentDirectionId,
                Description = x.Doc.Description,
                TemplateDocumentId = x.Doc.TemplateDocumentId,
                RegistrationDate = x.Doc.RegistrationDate,
                DocumentSubjectId = x.Doc.DocumentSubjectId,
                RegistrationNumber = x.Doc.RegistrationNumber,
                RegistrationNumberPrefix = x.Doc.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Doc.RegistrationNumberSuffix,
                LastChangeDate = x.Doc.LastChangeDate,
                RegistrationJournalId = x.Doc.RegistrationJournalId,
                CreateDate = x.Doc.CreateDate,
                DocumentSubjectName = x.Doc.DocumentSubject.Name,
                ExecutorPositionName = x.Doc.ExecutorPosition.Name,
                LastChangeUserId = x.Doc.LastChangeUserId,
                DocumentDirectionName = x.Doc.TemplateDocument.DocumentDirection.Name,
                DocumentTypeName = x.Doc.TemplateDocument.DocumentType.Name,
                DocumentDate = x.Doc.RegistrationDate ?? x.Doc.CreateDate,
                IsFavourtite = x.Acc.IsFavourtite,
                IsInWork = x.Acc.IsInWork,
                EventsCount = x.Doc.Events.Count,
                NewEventCount = 0,//TODO
                AttachedFilesCount = x.Doc.Files.Count,
                LinkedDocumentsCount = 0//TODO
                }).ToList();
            paging.TotalPageCount = res.Count; //TODO pay attention to this when we will add paging
            return res;
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

            var doc = dbContext.DocumentsSet
                .Include(i => i.Accesses)
                .Include(x => x.RestrictedSendLists)
                .Include(x => x.SendLists)
                .Where(x => x.Id == documentId && x.Accesses.All(a => a.PositionId == ctx.CurrentPositionId))
                .Select(x => new {Doc = x, Acc = x.Accesses.FirstOrDefault()})
                .Select(x => new FullDocument
                {
                    Id = x.Doc.Id,
                    TemplateDocumentId = x.Doc.TemplateDocumentId,
                    CreateDate = x.Doc.CreateDate,
                    DocumentSubjectId = x.Doc.DocumentSubjectId,
                    Description = x.Doc.Description,
                    RegistrationJournalId = x.Doc.RegistrationJournalId,
                    RegistrationNumber = x.Doc.RegistrationNumber,
                    RegistrationNumberPrefix = x.Doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.Doc.RegistrationNumberSuffix,
                    RegistrationDate = x.Doc.RegistrationDate,
                    ExecutorPositionId = x.Doc.ExecutorPositionId,
                    LastChangeUserId = x.Doc.LastChangeUserId,
                    LastChangeDate = x.Doc.LastChangeDate,
                    SenderAgentId = x.Doc.SenderAgentId,
                    SenderAgentPersonId = x.Doc.SenderAgentPersonId,
                    SenderNumber = x.Doc.SenderNumber,
                    SenderDate = x.Doc.SenderDate,
                    Addressee = x.Doc.Addressee,
                    AccessLevel = (EnumDocumentAccess) x.Acc.AccessLevelId,
                    TemplateDocumentName = x.Doc.TemplateDocument.Name,
                    IsHard = x.Doc.TemplateDocument.IsHard,
                    DocumentDirectionId = x.Doc.TemplateDocument.DocumentDirectionId,
                    DocumentDirectionName = x.Doc.TemplateDocument.DocumentDirection.Name,
                    DocumentTypeId = x.Doc.TemplateDocument.DocumentTypeId,
                    DocumentTypeName = x.Doc.TemplateDocument.DocumentType.Name,
                    DocumentSubjectName = x.Doc.DocumentSubject.Name,
                    RegistrationJournalName = x.Doc.RegistrationJournal.Name,
                    ExecutorPositionName = x.Doc.ExecutorPosition.Name,
                    ExecutorPositionAgentName = x.Doc.ExecutorPosition.ExecutorAgent.Name,
                    SenderAgentName = x.Doc.SenderAgent.Name,
                    SenderAgentPersonName = x.Doc.SenderAgentPerson.Name,
                    AccessLevelName = x.Acc.AccessLevel.Name,
                    DocumentDate = x.Doc.RegistrationDate ?? x.Doc.CreateDate,
                    RegistrationFullNumber =
                        x.Doc.RegistrationNumber != null
                            ? x.Doc.RegistrationNumber.ToString()
                            : "#" + x.Doc.Id.ToString(),
                    GeneralInfo =
                        x.Doc.TemplateDocument.DocumentDirection.Name + " " + x.Doc.TemplateDocument.DocumentType.Name,

                    IsFavourtite = x.Acc.IsFavourtite,
                    IsInWork = x.Acc.IsInWork,
                    EventsCount = x.Doc.Events.Count,
                    NewEventCount = 0, //TODO
                    AttachedFilesCount = x.Doc.Files.Count,
                    LinkedDocumentsCount = 0, //TODO

                    Accesses = new List<BaseDocumentAccess>
                    {
                        new BaseDocumentAccess
                        {
                            LastChangeDate = x.Acc.LastChangeDate,
                            LastChangeUserId = x.Acc.LastChangeUserId,
                            IsInWork = x.Acc.IsInWork,
                            IsFavourite = x.Acc.IsFavourtite,
                            PositionId = x.Acc.PositionId,
                            AccessType = (EnumDocumentAccess) x.Acc.AccessLevelId,
                            AccessLevelName = x.Acc.AccessLevel.Name,
                            Id = x.Acc.Id,
                            DocumentId = x.Acc.DocumentId
                        }
                    },

                    Events = x.Doc.Events.Select(y => new BaseDocumentEvent
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
                    }).ToList(),

                    SendLists = x.Doc.SendLists.Select(y => new BaseDocumentSendList
                    {
                        Id = y.Id,
                        DocumentId = y.DocumentId,
                        OrderNumber = y.OrderNumber,
                        SendTypeId = y.SendTypeId,
                        SendTypeName = y.SendType.Name,
                        SendTypeCode = y.SendType.Code,
                        SendTypeIsImpotant = y.SendType.IsImpotant,
                        TargetPositionId = y.TargetPositionId,
                        TargetPositionName = y.TargetPosition.Name,
                        Description = y.Description,
                        DueDate = y.DueDate,
                        DueDay = y.DueDay,
                        AccessLevelId = y.AccessLevelId,
                        AccessLevelName = y.AccessLevel.Name,
                        IsInitial = y.IsInitial,
                        EventId = y.EventId,
                        LastChangeUserId = y.LastChangeUserId,
                        LastChangeDate = y.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).ToList(),

                    RestrictedSendLists = x.Doc.RestrictedSendLists.Select(y => new BaseDocumentRestrictedSendList
                    {
                        Id = y.Id,
                        DocumentId = y.DocumentId,
                        PositionId = y.PositionId,
                        PositionName = y.Position.Name,
                        AccessLevelId = y.AccessLevelId,
                        AccessLevelName = y.AccessLevel.Name,
                        LastChangeUserId = y.LastChangeUserId,
                        LastChangeDate = y.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).ToList()

                }).FirstOrDefault();

            return doc;
        }
        #endregion Documents

        #region Document Access

        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var dbContext = GetUserDmsContext(ctx);
            var acc = new DocumentAccesses
            {
                LastChangeDate = access.LastChangeDate,
                IsInWork = access.IsInWork,
                LastChangeUserId = access.LastChangeUserId,
                PositionId = access.PositionId,
                AccessLevelId = (int)access.AccessType,
                IsFavourtite = access.IsFavourite
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
                acc.AccessLevelId = (int) access.AccessType;
                acc.IsFavourtite = access.IsFavourite;
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
                    IsFavourite = acc.IsFavourtite,
                    AccessType = (EnumDocumentAccess)acc.AccessLevelId,
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
                SendTypeId = sendList.SendTypeId,
                TargetPositionId = sendList.TargetPositionId,
                Description = sendList.Description,
                DueDate = sendList.DueDate,
                DueDay = sendList.DueDay,
                AccessLevelId = sendList.AccessLevelId,
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
                    SendTypeId = sendList.SendTypeId,
                    TargetPositionId = sendList.TargetPositionId,
                    Description = sendList.Description,
                    DueDate = sendList.DueDate,
                    DueDay = sendList.DueDay,
                    AccessLevelId = sendList.AccessLevelId,
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
    }
}