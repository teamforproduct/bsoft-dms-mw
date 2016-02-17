using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Exception;
using BL.Model.InternalModel;

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
        public InternalLinkedDocument GetLinkedDocument(IContext context, AddDocumentLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == model.DocumentId);
                var par = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == model.ParentDocumentId);

                return new InternalLinkedDocument()
                {
                    DocumentId = model.DocumentId,
                    ParentDocumentId = model.ParentDocumentId,
                    DocumentLinkId = doc.LinkId,
                    ParentDocumentLinkId = par.LinkId,
                    LinkTypeId = model.LinkTypeId,
                    ExecutorPositionId = doc.ExecutorPositionId
                };
            }
        }

        public void AddDocumentLink(IContext context, InternalLinkedDocument model)
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
                if (!model.ParentDocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.ParentDocumentId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                if (!model.DocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.DocumentId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.LinkId == model.DocumentLinkId).ToList().ForEach(x => x.LinkId = model.ParentDocumentId);
                }
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentLink         

        #region DocumentWaits

        public BaseDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var wait = CommonQueries.GetDocumentWaits(dbContext, new FilterDocumentWaits() {OnEventId = eventId}).FirstOrDefault();
                if (wait?.Id > 0)
                {
                    return wait;
                }
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
                var docWait = dbContext.DocumentWaitsSet.FirstOrDefault(x => x.Id == documentWait.Id);
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
                return CommonQueries.GetDocumentEvents(dbContext, filter);
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
                return CommonQueries.GetDocumentAccess(ctx, dbContext, documentId);
            }
        }
        #endregion
    }
}