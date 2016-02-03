using System;
using System.Collections.Generic;
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
        public DocumentsDbProcess()
        {
        }

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
                doc.RestrictedSendLists = document.RestrictedSendLists.Select(x => new DocumentRestrictedSendLists()
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

            if (document.Accesses != null && document.Accesses.Any(x => x.Id == 0))
            {
                doc.Accesses = document.Accesses.Where(x => x.Id == 0).Select(x => new DocumentAccesses
                {
                    LastChangeDate = x.LastChangeDate,
                    IsInWork = x.IsInWork,
                    LastChangeUserId = x.LastChangeUserId,
                    PositionId = x.PositionId,
                    AccessLevelId = (int)x.AccessType,
                }).ToList();
            }
            dbContext.SaveChanges();
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
            var qry = dbContext.DocumentsSet.AsQueryable();

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

            var res = qry.Select(x => new FullDocument
            {
                Id = x.Id,
                DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                ExecutorPositionId = x.ExecutorPositionId,
                DocumentDirectionId = x.TemplateDocument.DocumentDirectionId,
                Description = x.Description,
                TemplateDocumentId = x.TemplateDocumentId,
                RegistrationDate = x.RegistrationDate,
                DocumentSubjectId = x.DocumentSubjectId,
                RegistrationNumber = x.RegistrationNumber,
                RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                LastChangeDate = x.LastChangeDate,
                RegistrationJournalId = x.RegistrationJournalId,
                CreateDate = x.CreateDate,
                DocumentSubjectName = x.DocumentSubject.Name,
                ExecutorPositionName = x.ExecutorPosition.Name,
                LastChangeUserId = x.LastChangeUserId,
                DocumentDirectionName = x.TemplateDocument.DocumentDirection.Name,
                DocumentTypeName = x.TemplateDocument.DocumentType.Name,
                DocumentDate = x.RegistrationDate ?? x.CreateDate
            }).ToList();
            paging.TotalPageCount = res.Count(); //TODO pay attention to this when we will add paging
            return res;
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

            var doc = dbContext.DocumentsSet.Where(x => x.Id == documentId).Select(x => new FullDocument
            {
                Id = x.Id,
                TemplateDocumentId = x.TemplateDocumentId,
                CreateDate = x.CreateDate,
                DocumentSubjectId = x.DocumentSubjectId,
                Description = x.Description,
                RegistrationJournalId = x.RegistrationJournalId,
                RegistrationNumber = x.RegistrationNumber,
                RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                RegistrationDate = x.RegistrationDate,
                ExecutorPositionId = x.ExecutorPositionId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SenderAgentId = x.SenderAgentId,
                SenderAgentPersonId = x.SenderAgentPersonId,
                SenderNumber = x.SenderNumber,
                SenderDate = x.SenderDate,
                Addressee = x.Addressee,
                AccessLevelId = 30, //после добавления Access??? подумать
                TemplateDocumentName = x.TemplateDocument.Name,
                IsHard = x.TemplateDocument.IsHard,
                DocumentDirectionId = x.TemplateDocument.DocumentDirectionId,
                DocumentDirectionName = x.TemplateDocument.DocumentDirection.Name,
                DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                DocumentTypeName = x.TemplateDocument.DocumentType.Name,
                DocumentSubjectName = x.DocumentSubject.Name,
                RegistrationJournalName = x.RegistrationJournal.Name,
                ExecutorPositionName = x.ExecutorPosition.Name,
                ExecutorPositionAgentName = x.ExecutorPosition.ExecutorAgent.Name,
                SenderAgentName = x.SenderAgent.Name,
                SenderAgentPersonName = x.SenderAgentPerson.Name,
                AccessLevelName = null, //после добавления Access??? подумать
                DocumentDate = x.RegistrationDate ?? x.CreateDate,
                RegistrationFullNumber = x.RegistrationNumber != null ? x.RegistrationNumber.ToString() : "#" + x.Id.ToString(),
                GeneralInfo = x.TemplateDocument.DocumentDirection.Name + " " + x.TemplateDocument.DocumentType.Name,
                Events = x.Events.Select(y => new BaseDocumentEvent
                {
                    Id = y.Id,
                    DocumentId = y.DocumentId,
                    Description = y.Description,
                    EventType = (EnumEventTypes)y.EventTypeId,
                    ImpotanceEventType = (EnumImpotanceEventTypes)y.EventType.ImpotanceEventTypeId,
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

                SendLists = x.SendLists.Select(y => new BaseDocumentSendList
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

                RestrictedSendLists = x.RestrictedSendLists.Select(y => new BaseDocumentRestrictedSendList
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
            };
            dbContext.DocumentAccessesSet.Add(acc);
            dbContext.SaveChanges();
            return acc.Id;
        }

        public void RemoveDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var dbContext = GetUserDmsContext(ctx);
            var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == access.Id);
            if (acc != null)
            {
                dbContext.DocumentAccessesSet.Remove(acc);
                dbContext.SaveChanges();
            }
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