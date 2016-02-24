using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Logic.Helpers;
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
using BL.Model.SystemCore;
using BL.Model.Enums;

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
                    DocumentId = model.DocumentId.Value,
                    ParentDocumentId = model.ParentDocumentId.Value,
                    LinkTypeId = model.LinkTypeId,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate,
                };
                dbContext.DocumentLinksSet.Add(link);
                if (!model.ParentDocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.ParentDocumentId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                if (!model.DocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.DocumentId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.LinkId == model.DocumentLinkId).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
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
                var docWait = ModelConverter.GetDbDocumentWait(documentWait);
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
                docWait.OnEvent = ModelConverter.GetDbDocumentEvent(documentWait.OnEvent);
            }
            if (documentWait.OffEvent != null)
            {
                docWait.OffEvent = ModelConverter.GetDbDocumentEvent(documentWait.OffEvent);
            }
        }
        #endregion DocumentWaits

        #region Document Event

        public int AddDocumentEvent(IContext ctx, InternalDocumentEvents docEvent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var evt = ModelConverter.GetDbDocumentEvent(docEvent);
                dbContext.DocumentEventsSet.Add(evt);
                dbContext.SaveChanges();
                return evt.Id;
            }
        }

        public void AddDocumentEvents(IContext ctx, IEnumerable<InternalDocumentEvents> docEvents)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var evt = ModelConverter.GetDbDocumentEvents(docEvents);
                dbContext.DocumentEventsSet.AddRange(evt);
                dbContext.SaveChanges();
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

        public int AddDocumentAccess(IContext ctx, InternalDocumentAccesses access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = ModelConverter.GetDbDocumentAccess(access);
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

        public IEnumerable<InternalDocumentAccesses> GetDocumentAccesses(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetInternalDocumentAccesses(dbContext, documentId);
            }
        }

        public IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetInternalPositionsInfo(dbContext, positionIds);
            }
        }

        public void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccesses docAccess)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var acc = new DocumentAccesses {Id = docAccess.Id, IsFavourite = !docAccess .IsFavourite};
                dbContext.DocumentAccessesSet.Attach(acc);
                acc.LastChangeDate = docAccess.LastChangeDate;
                acc.LastChangeUserId = docAccess.LastChangeUserId;
                acc.IsFavourite = docAccess.IsFavourite;
                dbContext.SaveChanges();
            }
        }

        public InternalDocumentAccesses ChangeIsFavouriteAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var acc = dbContext.DocumentAccessesSet
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocumentAccesses
                    {
                        Id = x.Id,
                        IsFavourite = x.IsFavourite,
                    }).FirstOrDefault();
                return acc;

            }
        }

        public void ChangeIsInWorkAccess(IContext ctx, InternalDocumentAccesses access)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var acc = dbContext.DocumentAccessesSet.FirstOrDefault(x => x.Id == access.Id);
                if (acc != null)
                {
                    acc.LastChangeDate = access.LastChangeDate;
                    acc.LastChangeUserId = access.LastChangeUserId;
                    acc.IsInWork = access.IsInWork;
                }
                dbContext.DocumentEventsSet.Add(ModelConverter.GetDbDocumentEvent(access.DocumentEvent));
                dbContext.SaveChanges();
            }
        }

        public InternalDocumentAccesses ChangeIsInWorkAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var acc = dbContext.DocumentAccessesSet
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocumentAccesses
                    {
                        Id = x.Id,
                        IsInWork = x.IsInWork,
                    }).FirstOrDefault();
                return acc;

            }
        }



        #endregion

        #region DocumentSendList    
        public InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var docDb = (from doc in dbContext.DocumentsSet.Where(x => x.Id == documentId)
                             join tmp in dbContext.TemplateDocumentsSet on doc.TemplateDocumentId equals tmp.Id
                             select new { doc, tmp })
                             .GroupJoin(dbContext.DocumentRestrictedSendListsSet, x => x.doc.Id, y => y.DocumentId, (x, y) => new { x.doc, x.tmp, rsls = y })
                             .GroupJoin(dbContext.DocumentSendListsSet, x => x.doc.Id, y => y.DocumentId, (x, y) => new { x.doc, x.tmp, x.rsls, sls = y })
                             .GroupJoin(dbContext.TemplateDocumentRestrictedSendLists, x => x.tmp.Id, y => y.DocumentId, (x, y) => new { x.doc, x.tmp, x.rsls, x.sls, trsls = y })
                             .GroupJoin(dbContext.TemplateDocumentSendLists, x => x.tmp.Id, y => y.DocumentId, (x, y) => new { x.doc, x.tmp, x.rsls, x.sls, x.trsls, tsls = y });

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,
                    TemplateDocumentId = x.tmp.Id,
                    IsHard = x.tmp.IsHard,

                    RestrictedSendLists = x.rsls.Select(y => new InternalDocumentRestrictedSendLists
                    {
                        DocumentId = y.DocumentId,
                        PositionId = y.PositionId
                    }),

                    SendLists = x.sls.Select(y => new InternalDocumentSendLists
                    {
                        DocumentId = y.DocumentId,
                        TargetPositionId = y.TargetPositionId,
                        SendType = (EnumSendTypes)y.SendTypeId
                    }),
                    TemplateDocument = !x.tmp.IsHard ? null :
                      new InternalTemplateDocument
                      {
                          RestrictedSendLists = x.trsls.Select(y => new InternalTemplateDocumentRestrictedSendLists
                          {
                              PositionId = y.PositionId
                          }),
                          SendLists = x.tsls.Select(y => new InternalTemplateDocumentSendLists
                          {
                              TargetPositionId = y.TargetPositionId,
                              SendType = (EnumSendTypes)y.SendTypeId
                          }),
                      }
                }).FirstOrDefault();

                return docRes;
            }
        }

        public void AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendLists> model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var items = model.Select(x=> new DocumentRestrictedSendLists
                {
                    AccessLevelId = (int)x.AccessLevel,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
                
                dbContext.DocumentRestrictedSendListsSet.AddRange(items);
                dbContext.SaveChanges();
            }
        }

        #endregion DocumentSendList         
    }
}