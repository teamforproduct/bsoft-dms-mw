using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentOperationsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

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

        #region DocumentWaits
        public IEnumerable<BaseDocumentWaits> GetDocumentWaits(IContext ctx,
            Expression<Func<DocumentWaits, bool>> filter,
            Expression<Func<BaseDocumentWaits, BaseDocumentWaits>> select)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var waitsDb = dbContext.DocumentWaitsSet
                    .Where(filter)
                    .Select(x => new { Wait = x, OnEvent = x.OnEvent, OffEvent = x.OffEvent });

                var waits = waitsDb.Select(x => new BaseDocumentWaits
                {
                    Id = x.Wait.Id,
                    DocumentId = x.Wait.DocumentId,
                    ParentId = x.Wait.ParentId,
                    OnEventId = x.Wait.OnEventId,
                    OffEventId = x.Wait.OffEventId,
                    ResultTypeId = x.Wait.ResultTypeId,
                    Description = x.Wait.Description,
                    DueDate = x.Wait.DueDate,
                    AttentionDate = x.Wait.AttentionDate,
                    OnEvent = x.OnEvent == null ? null :
                        new BaseDocumentEvent
                        {
                            Id = x.OnEvent.Id,
                            CreateDate = x.OnEvent.CreateDate,
                            Date = x.OnEvent.Date,
                            Description = x.OnEvent.Description,
                            LastChangeDate = x.OnEvent.LastChangeDate,
                            LastChangeUserId = x.OnEvent.LastChangeUserId,
                            SourceAgentId = x.OnEvent.SourceAgentId,
                            SourcePositionId = x.OnEvent.SourcePositionId,
                            TargetAgentId = x.OnEvent.TargetAgentId,
                            TargetPositionId = x.OnEvent.TargetPositionId,
                            EventType = (EnumEventTypes)x.OnEvent.EventTypeId
                        },
                    OffEvent = x.OffEvent == null ? null :
                        new BaseDocumentEvent
                        {
                            Id = x.OffEvent.Id,
                            CreateDate = x.OffEvent.CreateDate,
                            Date = x.OffEvent.Date,
                            Description = x.OffEvent.Description,
                            LastChangeDate = x.OffEvent.LastChangeDate,
                            LastChangeUserId = x.OffEvent.LastChangeUserId,
                            SourceAgentId = x.OffEvent.SourceAgentId,
                            SourcePositionId = x.OffEvent.SourcePositionId,
                            TargetAgentId = x.OffEvent.TargetAgentId,
                            TargetPositionId = x.OffEvent.TargetPositionId,
                            EventType = (EnumEventTypes)x.OffEvent.EventTypeId
                        }
                })
                .Select(select)
                .ToList();

                return waits;
            }
        }

        public BaseDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId)
        {
            var wait = GetDocumentWaits(ctx, x => x.OnEventId == eventId, x => x).FirstOrDefault();
            if (wait?.Id > 0)
            {
                return wait;
            }

            throw new WaitNotFoundOrUserHasNoAccess();
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
                    Id = documentWait.Id,
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
                        AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId,
                        AccessLevelName = acc.AccessLevel.Name
                    };
                }
            }
            return null;
        }
        #endregion
    }
}