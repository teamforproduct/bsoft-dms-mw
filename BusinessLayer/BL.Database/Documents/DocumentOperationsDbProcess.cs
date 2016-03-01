using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.DocumentCore.IncomingModel;
using System.Data.Entity;
using BL.CrossCutting.Helpers;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Database.Documents
{
    public class DocumentOperationsDbProcess : IDocumentOperationsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentOperationsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var res = new DocumentActionsModel();
                res.ActionsList = new Dictionary<int, List<InternalSystemAction>>();

                res.Document = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == documentId && context.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        IsRegistered = x.Doc.IsRegistered,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                        IsInWork = x.Acc.IsInWork,
                        IsFavourite = x.Acc.IsFavourite,
                    }).FirstOrDefault();

                res.PositionWithActions = dbContext.DictionaryPositionsSet.Where(x => context.CurrentPositionsIdList.Contains(x.Id))
                        .Select(x => new InternalDictionaryPositionWithActions
                        {
                            Id = x.Id,
                            Name = x.Name,
                            DepartmentId = x.DepartmentId,
                            ExecutorAgentId = x.ExecutorAgentId,
                            DepartmentName = x.Department.Name,
                            ExecutorAgentName = x.ExecutorAgent.Name,
                        }).ToList();


                foreach (int posId in context.CurrentPositionsIdList)
                {
                    var qry = dbContext.SystemActionsSet.Where(x => x.ObjectId == (int)EnumObjects.Documents
                    && x.IsVisible &&
                    (!x.IsGrantable ||
                        x.RoleActions.Any(y => (posId == y.Role.PositionId) && y.Role.UserRoles.Any(z => z.UserId == context.CurrentAgentId)))
                    );

                    var actLst = qry.Select(a => new InternalSystemAction
                    {
                        DocumentAction = (EnumDocumentActions)a.Id,
                        Object = (EnumObjects)a.ObjectId,
                        ActionCode = a.Code,
                        ObjectCode = a.Object.Code,
                        API = a.API,
                        Description = a.Description,
                    }).ToList();
                    res.ActionsList.Add(posId, actLst);

                }
                return res;
            }
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
        public InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == model.DocumentId && context.CurrentPositionsIdList.Contains(x.Acc.PositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                        ExecutorPositionId = x.Doc.ExecutorPositionId,
                        LinkId = x.Doc.LinkId,
                        LinkTypeId = model.LinkTypeId,
                    }).FirstOrDefault();

                if (doc == null) return null;

                var par = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == model.ParentDocumentId)
                    .Select(x => new { Id = x.Doc.Id, LinkId = x.Doc.LinkId }).FirstOrDefault();

                if (par == null) return null;

                doc.ParentDocumentId = par.Id;
                doc.ParentDocumentLinkId = par.LinkId;

                return doc;
            }
        }

        public void AddDocumentLink(IContext context, InternalDocument model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var link = new DocumentLinks
                {
                    DocumentId = model.Id,
                    ParentDocumentId = model.ParentDocumentId,
                    LinkTypeId = model.LinkTypeId,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate,
                };
                dbContext.DocumentLinksSet.Add(link);
                if (!model.ParentDocumentLinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.ParentDocumentId).ToList()  //TODO OPTIMIZE
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                if (!model.LinkId.HasValue)
                {
                    dbContext.DocumentsSet.Where(x => x.Id == model.Id).ToList()
                        .ForEach(x =>
                        {
                            x.LinkId = model.ParentDocumentId;
                            x.LastChangeUserId = model.LastChangeUserId;
                            x.LastChangeDate = model.LastChangeDate;
                        });
                }
                else
                {
                    dbContext.DocumentsSet.Where(x => x.LinkId == model.LinkId).ToList()
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

        #region Waits

        public void AddDocumentWaits(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                dbContext.DocumentWaitsSet.AddRange(ModelConverter.GetDbDocumentWaits(document.Waits));
                dbContext.SaveChanges();
            }
        }

        public void ChangeDocumentWait(IContext ctx, IEnumerable<InternalDocumentWait> documentWaits)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var documentWait = documentWaits.First(x => x.Id != 0);
                var oldWait = new DocumentWaits
                {
                    Id = documentWait.Id,
                    LastChangeDate = documentWait.LastChangeDate,
                    LastChangeUserId = documentWait.LastChangeUserId
                };
                dbContext.DocumentWaitsSet.Attach(oldWait);
                oldWait.OffEvent = ModelConverter.GetDbDocumentEvent(documentWait.OffEvent);
                var entry = dbContext.Entry(oldWait);

                entry.Property(x => x.Id).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;

                var newWait = ModelConverter.GetDbDocumentWait(documentWaits.First(x => x.Id == 0));
                newWait.OnEvent = oldWait.OffEvent;

                dbContext.DocumentWaitsSet.Add(newWait);
                dbContext.SaveChanges();
            }
        }

        public void CloseDocumentWait(IContext ctx, InternalDocumentWait docWait)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var wait = new DocumentWaits
                {
                    Id = docWait.Id,
                    LastChangeDate = docWait.LastChangeDate,
                    LastChangeUserId = docWait.LastChangeUserId
                };
                dbContext.DocumentWaitsSet.Attach(wait);
                wait.OffEvent = ModelConverter.GetDbDocumentEvent(docWait.OffEvent);
                var entry = dbContext.Entry(wait);

                entry.Property(x => x.Id).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ControlOffDocumentPrepare(IContext context, int eventId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = dbContext.DocumentWaitsSet
                    .Where(x => x.OnEventId == eventId && context.CurrentPositionsIdList.Contains(x.OnEvent.SourcePositionId.Value))
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        Waits = new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            OffEventId = x.OffEventId,
                                            OnEvent = new InternalDocumentEvent
                                            {
                                                Id = x.OnEvent.Id,
                                                SourcePositionId = x.OnEvent.SourcePositionId,
                                                TargetPositionId = x.OnEvent.TargetPositionId,
                                                Task = x.OnEvent.Task,
                                                Description = x.OnEvent.Description

                                            }
                                        }
                                    }
                    }).FirstOrDefault();
                return doc;

            }
        }

        #endregion Waits

        #region Events

        public void AddDocumentEvents(IContext ctx, IEnumerable<InternalDocumentEvent> docEvents)
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

        public IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext ctx, int documentId)
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

        public void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var acc = new DocumentAccesses
                {
                    Id = docAccess.Id,
                    IsFavourite = docAccess.IsFavourite,
                    LastChangeDate = docAccess.LastChangeDate,
                    LastChangeUserId = docAccess.LastChangeUserId
                };
                dbContext.DocumentAccessesSet.Attach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsFavourite).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = dbContext.DocumentAccessesSet
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsFavourite =x.IsFavourite,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            IsFavourite = x.IsFavourite,
                                        }
                                    }

                    }).FirstOrDefault();
                return doc;

            }
        }

        public void ChangeIsInWorkAccess(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var docAccess = document.Accesses.FirstOrDefault();
                var acc = new DocumentAccesses
                {
                    Id = docAccess.Id,
                    IsInWork = docAccess.IsInWork,
                    LastChangeDate = docAccess.LastChangeDate,
                    LastChangeUserId = docAccess.LastChangeUserId
                };
                dbContext.DocumentAccessesSet.Attach(acc);
                var entry = dbContext.Entry(acc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsInWork).IsModified = true;
                dbContext.DocumentEventsSet.Add(ModelConverter.GetDbDocumentEvent(document.Events.FirstOrDefault()));
                dbContext.SaveChanges();
            }
        }

        public InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var acc = dbContext.DocumentAccessesSet
                    .Where(x => x.DocumentId == documentId && x.PositionId == context.CurrentPositionId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        Accesses = new List<InternalDocumentAccess>
                                    {
                                        new InternalDocumentAccess
                                        {
                                            Id = x.Id,
                                            IsInWork = x.IsInWork,
                                        }
                                    }

                    }).FirstOrDefault();
                return acc;

            }
        }

        public void SendBySendList(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sendList = document.SendLists.First();
                var sendListDb = new DocumentSendLists
                {
                    Id = sendList.Id,
                    LastChangeDate = sendList.LastChangeDate,
                    LastChangeUserId = sendList.LastChangeUserId
                };
                dbContext.DocumentSendListsSet.Attach(sendListDb);
                sendListDb.CloseEvent = sendListDb.StartEvent = ModelConverter.GetDbDocumentEvent(sendList.StartEvent);
                var entry = dbContext.Entry(sendListDb);
                entry.Property(x => x.Id).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;

                dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, document.Id).ToList());

                dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events));
                dbContext.DocumentWaitsSet.AddRange(ModelConverter.GetDbDocumentWaits(document.Waits));
                dbContext.DocumentSubscriptionsSet.AddRange(ModelConverter.GetDbDocumentSubscriptions(document.Subscriptions));

                dbContext.SaveChanges();
            }
        }

        public void ModifyDocumentTags(IContext context, InternalDocumentTag model)
        {
            // TODO к Сергею проверить нужно ли разнести этот метод
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var dictionaryTags = dbContext.DictionaryTagsSet
                    .Where(x => !x.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.PositionId ?? 0))
                    .Where(x => model.Tags.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToList();

                var documentTags = dbContext.DocumentTagsSet
                    .Where(x => x.DocumentId == model.DocumentId)
                    .Where(x => !x.Tag.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
                    .Select(x => x.TagId)
                    .ToList();

                //Удаляем теги которые не присутствуют в списке
                dbContext.DocumentTagsSet
                    .RemoveRange(dbContext.DocumentTagsSet
                        .Where(x => x.DocumentId == model.DocumentId
                            && documentTags.Where(y => !dictionaryTags.Contains(y)).Contains(x.TagId)));

                var newDictionaryTags = dictionaryTags
                    .Where(x => !documentTags.Contains(x))
                    .Select(x => new DocumentTags
                    {
                        DocumentId = model.DocumentId,
                        TagId = x,
                        LastChangeUserId = model.LastChangeUserId,
                        LastChangeDate = model.LastChangeDate
                    });

                dbContext.DocumentTagsSet.AddRange(newDictionaryTags);

                dbContext.SaveChanges();
            }
        }

        public InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == sendList.DocumentId && context.CurrentPositionsIdList.Contains(x.Acc.PositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Waits = dbContext.DocumentWaitsSet
                    .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.Task == sendList.Task && x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                    .Select(x => new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                                Id = x.Id,
                                                OnEvent = new InternalDocumentEvent
                                                {
                                                    TargetPositionId = x.OnEvent.TargetPositionId
                                                }
                                        }
                                    }
                    ).FirstOrDefault();
                return doc;

            }
        }

        public InternalDocument SendForSigningDocumentPrepare(IContext context, InternalDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == sendList.DocumentId && context.CurrentPositionsIdList.Contains(x.Acc.PositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id
                    }).FirstOrDefault();
                if (doc == null) return null;
                /*
                doc.Waits = dbContext.DocumentWaitsSet
                    .Where(x => x.DocumentId == sendList.DocumentId && x.OnEvent.Task == sendList.Task && x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                    .Select(x => new List<InternalDocumentWait>
                                    {
                                        new InternalDocumentWait
                                        {
                                                Id = x.Id,
                                                OnEvent = new InternalDocumentEvent
                                                {
                                                    TargetPositionId = x.OnEvent.TargetPositionId
                                                }
                                        }
                                    }
                    ).FirstOrDefault();
                    */
                return doc;

            }
        }

        #region DocumentSendList    
        public InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var docDb = from doc in dbContext.DocumentsSet.Where(x => x.Id == documentId)
                            join tmp in dbContext.TemplateDocumentsSet on doc.TemplateDocumentId equals tmp.Id
                            select new { doc, tmp };

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,
                    TemplateDocumentId = x.tmp.Id,
                    IsHard = x.tmp.IsHard,
                    IsLaunchPlan = x.doc.IsLaunchPlan
                }).FirstOrDefault();

                if (docRes != null)
                {
                    docRes.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == docRes.Id)
                        .Select(x => new InternalDocumentRestrictedSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            PositionId = x.PositionId
                        }).ToList();


                    docRes.SendLists = dbContext.DocumentSendListsSet.Where(x => x.DocumentId == docRes.Id)
                        .Select(x => new InternalDocumentSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            TargetPositionId = x.TargetPositionId,
                            SendType = (EnumSendTypes)x.SendTypeId,
                            Stage = x.Stage
                        }).ToList();

                    if (docRes.IsHard)
                    {
                        docRes.TemplateDocument = new InternalTemplateDocument();

                        docRes.TemplateDocument.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendLists
                            .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                            .Select(x => new InternalTemplateDocumentRestrictedSendList
                            {
                                Id = x.Id,
                                PositionId = x.PositionId
                            }).ToList();

                        docRes.TemplateDocument.SendLists = dbContext.TemplateDocumentSendLists
                            .Where(x => x.DocumentId == docRes.TemplateDocumentId)
                            .Select(x => new InternalTemplateDocumentSendList
                            {
                                Id = x.Id,
                                TargetPositionId = x.TargetPositionId,
                                SendType = (EnumSendTypes)x.SendTypeId
                            }).ToList();
                    }
                }
                return docRes;
            }
        }

        public void AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var items = model.Select(x => new DocumentRestrictedSendLists
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

        public IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     DocumentId = model.DocumentId,
                     PositionId = x.TargetPositionId,
                     AccessLevel = (EnumDocumentAccesses)(x.AccessLevelId ?? (int)EnumDocumentAccesses.PersonalRefIO)
                 }).ToList();

                return items;
            }
        }

        public InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Id == restSendListId)
                 .Select(x => new InternalDocumentRestrictedSendList
                 {
                     Id = x.Id,
                     DocumentId = x.DocumentId
                 }).FirstOrDefault();

                return item;
            }
        }

        public void DeleteDocumentRestrictedSendList(IContext context, int restSendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restSendListId);
                if (item != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public void AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var items = ModelConverter.AddDocumentSendList(model);

                dbContext.DocumentSendListsSet.AddRange(items);
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var items = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendListId == model.StandartSendListId)
                 .Select(x => new InternalDocumentSendList
                 {
                     DocumentId = model.DocumentId,
                     Stage = x.Stage,
                     SendType = (EnumSendTypes)x.SendTypeId,
                     SourcePositionId = context.CurrentPositionId,
                     SourceAgentId = context.CurrentAgentId,
                     TargetPositionId = x.TargetPositionId,
                     Task = x.Task,
                     Description = x.Description,
                     DueDate = x.DueDate,
                     DueDay = x.DueDay,
                     IsInitial = model.IsInitial,
                     AccessLevel = (EnumDocumentAccesses)(x.AccessLevelId ?? (int)EnumDocumentAccesses.PersonalRefIO),
                     LastChangeUserId = context.CurrentAgentId,
                     LastChangeDate = DateTime.Now,
                 }).ToList();

                return items;
            }
        }

        public void ModifyDocumentSendList(IContext context, InternalDocumentSendList model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new DocumentSendLists
                {
                    Id = model.Id,
                    Stage = model.Stage,
                    SendTypeId = (int)model.SendType,
                    TargetPositionId = model.TargetPositionId,
                    Task = model.Task,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    DueDay = model.DueDay,
                    AccessLevelId = (int)model.AccessLevel,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate
                };
                dbContext.DocumentSendListsSet.Attach(item);

                var entry = dbContext.Entry(item);
                entry.Property(e => e.Stage).IsModified = true;
                entry.Property(e => e.SendTypeId).IsModified = true;
                entry.Property(e => e.TargetPositionId).IsModified = true;
                entry.Property(e => e.Task).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.DueDate).IsModified = true;
                entry.Property(e => e.DueDay).IsModified = true;
                entry.Property(e => e.AccessLevelId).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
            }
        }

        public InternalDocumentSendList DeleteDocumentSendListPrepare(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.DocumentSendListsSet.Where(x => x.Id == sendListId)
                 .Select(x => new InternalDocumentSendList
                 {
                     Id = x.Id,
                     DocumentId = x.DocumentId,
                     SourcePositionId = x.SourcePositionId
                 }).FirstOrDefault();

                return item;
            }
        }

        public void DeleteDocumentSendList(IContext context, int sendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendListId);
                if (item != null)
                {
                    dbContext.DocumentSendListsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var docDb = (from doc in dbContext.DocumentsSet.Where(x => x.Id == documentId)
                             select new { doc })
                             .GroupJoin(dbContext.DocumentSendListsSet, x => x.doc.Id, y => y.DocumentId, (x, y) => new { x.doc, sls = y });

                var docRes = docDb.Select(x => new InternalDocument
                {
                    Id = x.doc.Id,
                    ExecutorPositionId = x.doc.ExecutorPositionId,

                    SendLists = x.sls.Select(y => new InternalDocumentSendList
                    {
                        Id = y.Id,
                        Stage = y.Stage
                    }),
                }).FirstOrDefault();

                return docRes;
            }
        }

        public void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                foreach (var sl in model)
                {
                    var item = new DocumentSendLists
                    {
                        Id = sl.Id,
                        Stage = sl.Stage,
                        LastChangeUserId = sl.LastChangeUserId,
                        LastChangeDate = sl.LastChangeDate
                    };
                    dbContext.DocumentSendListsSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(e => e.Stage).IsModified = true;
                    entry.Property(e => e.LastChangeUserId).IsModified = true;
                    entry.Property(e => e.LastChangeDate).IsModified = true;
                }

                dbContext.SaveChanges();
            }
        }

        public InternalDocument LaunchDocumentSendListPrepare(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var doc = dbContext.DocumentSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.DocumentId,
                        SendLists = new List<InternalDocumentSendList>
                                    {
                                        new InternalDocumentSendList
                                        {
                                            Id = x.Id,
                                            DocumentId = x.DocumentId,
                                            Stage = x.Stage,
                                            SendType = (EnumSendTypes)x.SendTypeId,
                                            SourcePositionId = x.SourcePositionId,
                                            SourceAgentId = x.SourceAgentId,
                                            TargetPositionId = x.TargetPositionId,
                                            TargetAgentId = x.TargetAgentId,
                                            Task = x.Task,
                                            Description = x.Description,
                                            DueDay = x.DueDay,
                                            DueDate = x.DueDate,
                                            AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                                            StartEventId = x.StartEventId,
                                            CloseEventId = x.CloseEventId
                                        }
                                    }
                    }).FirstOrDefault();
                return doc;

            }
        }


        #endregion DocumentSendList     

        #region DocumentSavedFilter

        public List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var items = model.Select(x => new DocumentSavedFilters
                {
                    PositionId = x.PositionId,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                dbContext.DocumentSavedFiltersSet.AddRange(items);
                dbContext.SaveChanges();

                return items.Select(x => x.Id).ToList();
            }
        }

        public void ModifySavedFilter(IContext context, InternalDocumentSavedFilter model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new DocumentSavedFilters
                {
                    Id = model.Id,
                    PositionId = model.PositionId,
                    Icon = model.Icon,
                    Filter = model.Filter,
                    IsCommon = model.IsCommon,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate
                };
                dbContext.DocumentSavedFiltersSet.Attach(item);

                var entry = dbContext.Entry(item);
                entry.Property(e => e.PositionId).IsModified = true;
                entry.Property(e => e.Icon).IsModified = true;
                entry.Property(e => e.Filter).IsModified = true;
                entry.Property(e => e.IsCommon).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
            }
        }

        public void DeleteSavedFilter(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = dbContext.DocumentSavedFiltersSet.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion DocumentSavedFilter
    }
}