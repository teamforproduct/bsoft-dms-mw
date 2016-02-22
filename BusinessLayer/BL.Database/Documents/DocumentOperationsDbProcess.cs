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
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
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

        public InternalDocument GetDocumentActionsPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == documentId && context.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                    }).FirstOrDefault();
                   return doc;
            }
        }


        #region DocumentLink    
        public InternalLinkedDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
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
                    LastChangeDate = model.LastChangeDate,
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

        public InternalDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var wait = CommonQueries.GetInternalDocumentWaits(dbContext, new FilterDocumentWaits() { OnEventId = eventId }).FirstOrDefault();
                if (wait?.Id > 0)
                {
                    return wait;
                }
            }
            throw new WaitNotFoundOrUserHasNoAccess();
        }

        public void AddDocumentWait(IContext ctx, InternalDocumentWaits documentWait)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var docWait = CommonQueries.GetDbDocumentWait(documentWait);
                UpdateDocumentWaitEvents(docWait, documentWait);

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

        public void UpdateDocumentWait(IContext ctx, InternalDocumentWaits documentWait)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var docWait = dbContext.DocumentWaitsSet.FirstOrDefault(x => x.Id == documentWait.Id);

                if (!(docWait?.Id > 0)) return;

                docWait.DocumentId = documentWait.DocumentId;
                docWait.ParentId = documentWait.ParentId;
                docWait.OnEventId = documentWait.OnEventId;
                docWait.OffEventId = documentWait.OffEventId;
                docWait.ResultTypeId = documentWait.ResultTypeId;
                docWait.Task = documentWait.Task;
                docWait.DueDate = documentWait.DueDate;
                docWait.AttentionDate = documentWait.AttentionDate;
                docWait.LastChangeUserId = documentWait.LastChangeUserId;
                docWait.LastChangeDate = documentWait.LastChangeDate;

                // UpdateDocumentWaitEvents создает новые БД ивент сущности для вейта. что происхдит со старыми ивент сущностями, если они уже есть в БД??
                // при переопределении они удаляются или они обновляются или они остаются но будут ни к чему не привязаны? 
                //TODO CHECK IT
                UpdateDocumentWaitEvents(docWait, documentWait);

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


        private void UpdateDocumentWaitEvents(DocumentWaits docWait, InternalDocumentWaits documentWait)
        {
            if (documentWait.OnEvent != null)
            {
                docWait.OnEvent = CommonQueries.GetDbDocumentEvent(documentWait.OnEvent);
            }
            if (documentWait.OffEvent != null)
            {
                docWait.OffEvent = CommonQueries.GetDbDocumentEvent(documentWait.OffEvent);
            }
        }
        #endregion DocumentWaits

        #region Document Event

        public int AddDocumentEvent(IContext ctx, InternalDocumentEvents docEvent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var evt = CommonQueries.GetDbDocumentEvent(docEvent);
                dbContext.DocumentEventsSet.Add(evt);
                dbContext.SaveChanges();
                return evt.Id;
            }
        }

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter)
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
                    acc.LastChangeDate = access.DocumentAccess.LastChangeDate;
                    acc.IsInWork = access.DocumentAccess.IsInWork;
                    acc.LastChangeUserId = access.DocumentAccess.LastChangeUserId;
                    acc.PositionId = access.DocumentAccess.PositionId;
                    acc.AccessLevelId = (int)access.DocumentAccess.AccessLevel;
                    acc.IsFavourite = access.DocumentAccess.IsFavourite;
                }
                dbContext.DocumentEventsSet.Add(CommonQueries.GetDbDocumentEvent(access.DocumentEvent));
                dbContext.SaveChanges();
            }
        }

        public int AddDocumentAccess(IContext ctx, InternalDocumentAccesses access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = CommonQueries.GetDbDocumentAccess(access);
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

        public void UpdateDocumentAccess(IContext ctx, InternalDocumentAccesses access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == access.Id);
                if (acc != null)
                {
                    acc.LastChangeDate = access.LastChangeDate;
                    acc.IsInWork = access.IsInWork;
                    acc.LastChangeUserId = access.LastChangeUserId;
                    acc.PositionId = access.PositionId;
                    acc.AccessLevelId = (int)access.AccessLevel;
                    acc.IsFavourite = access.IsFavourite;
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDocumentAccesses GetDocumentAccess(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetInternalDocumentAccess(ctx, dbContext, documentId);
            }
        }


        #endregion
    }
}