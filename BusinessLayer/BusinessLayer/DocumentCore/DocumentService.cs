using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;
using BL.Logic.DocumentCore.Interfaces;
using System.Linq;
using BL.Model.SystemCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;
using System.Web.Script.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DocumentCore.Commands;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        #region Documents
        public int SaveDocument(IContext context, FullDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                cmd = new AddDocument(context, document);
            }
            else
            {
                cmd = new UpdateDocument(context, document);
            }

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
            return document.Id;
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocuments(ctx, filters, paging);
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var doc = documentDb.GetDocument(ctx, documentId);
            doc.SendListStages = GetSendListStage(doc.SendLists);
            return doc;
        }

        public int AddDocumentByTemplateDocument(IContext context, AddDocumentByTemplateDocument model)
        {
            var adm = DmsResolver.Current.Get<IAdminService>();
            adm.VerifyAccessForCurrentUser(context,"Documents", "AddDocument", model.CurrentPositionId);
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, model.TemplateDocumentId);
            var baseDocument = new FullDocument
            {
                TemplateDocumentId = baseTemplateDocument.Id,
                CreateDate = DateTime.Now,
                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                Description = baseTemplateDocument.Description,
                ExecutorPositionId = (int)context.CurrentPositionId, ////
                SenderAgentId = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External ? baseTemplateDocument.SenderAgentId : null,
                SenderAgentPersonId = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External ? baseTemplateDocument.SenderAgentPersonId : null,
                Addressee = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External ? baseTemplateDocument.Addressee : null
            };

            if (baseTemplateDocument.RestrictedSendLists != null && baseTemplateDocument.RestrictedSendLists.Any())
            {
                baseDocument.RestrictedSendLists = baseTemplateDocument.RestrictedSendLists.Select(x => new BaseDocumentRestrictedSendList()
                {
                    PositionId = x.PositionId,
                    AccessLevelId = (int)x.AccessLevel
                }).ToList();
            }

            if (baseTemplateDocument.SendLists != null && baseTemplateDocument.SendLists.Any())
            {
                baseDocument.SendLists = baseTemplateDocument.SendLists.Select(x => new BaseDocumentSendList
                {
                    Stage = x.Stage,
                    SendType = x.SendType,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevel = x.AccessLevel,
                    IsInitial = true,
                    StartEventId = null,
                    CloseEventId = null
                }).ToList();
            }

            var evt = new BaseDocumentEvent
            {
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
                EventType = EnumEventTypes.AddNewDocument,
                Description = "Create",
                LastChangeUserId = context.CurrentAgentId,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                SourcePositionId = (int)context.CurrentPositionId
            };

            baseDocument.Events = new List<BaseDocumentEvent> { evt };

            var acc = new BaseDocumentAccess
            {
                AccessLevel = EnumDocumentAccess.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = (int)context.CurrentPositionId,
            };

            baseDocument.Accesses = new List<BaseDocumentAccess>() { acc };

            return SaveDocument(context, baseDocument);
        }

        public int ModifyDocument(IContext context, ModifyDocument model)
        {
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var fullDoc = docDb.GetDocument(context, model.Id);
            var adm = DmsResolver.Current.Get<IAdminService>();
            adm.VerifyAccessForCurrentUser(context, "Documents", "ModifyDocument", fullDoc.ExecutorPositionId);
            context.CurrentPositionId = fullDoc.ExecutorPositionId;
            var baseDocument = new FullDocument(model);
            var templateDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseTemplateDocument = templateDb.GetTemplateDocument(context, baseDocument.TemplateDocumentId);
            if (
                (
                        (baseTemplateDocument.DocumentDirection == EnumDocumentDirections.Inner)
                    &&
                    (
                        baseDocument.SenderAgentId != null ||
                        baseDocument.SenderAgentPersonId != null ||
                        !string.IsNullOrEmpty(baseDocument.SenderNumber) ||
                        baseDocument.SenderDate != null ||
                        !string.IsNullOrEmpty(baseDocument.Addressee)
                    )
                )
                ||
                (
                        (baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External)
                    &&
                    (
                        baseDocument.SenderAgentId == null ||
                        baseDocument.SenderAgentPersonId == null ||
                        string.IsNullOrEmpty(baseDocument.SenderNumber) ||
                        baseDocument.SenderDate == null ||
                        string.IsNullOrEmpty(baseDocument.Addressee)
                    )
                )
                )
            {
                throw new WrongInformationAboutCorrespondent();
            }
            return SaveDocument(context, baseDocument);
        }

        public void DeleteDocument(IContext context, int id)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var document = documentDb.GetDocument(context, id);
            var adm = DmsResolver.Current.Get<IAdminService>();
            adm.VerifyAccessForCurrentUser(context, "Documents", "DeleteDocument", document.ExecutorPositionId);
            Command cmd = new DeleteDocument(context, document);
            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FullDocument doc)
        {
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = systemDb.GetSystemUIElements(ctx, new FilterSystemUIElement() { ObjectCode = "Documents", ActionCode = "Modify" });
            if (doc.DocumentDirection != EnumDocumentDirections.External)
            {
                var senderElem = new List<String>() { "SenderAgent", "SenderAgentPerson", "SenderNumber", "SenderDate", "Addressee" };
                uiElements = uiElements.Where(x => !senderElem.Contains(x.Code)).ToList();
            }
            return uiElements;
        }

        #endregion Documents

        #region DocumentRestrictedSendLists

        public BaseDocumentRestrictedSendList GetRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            return documentDb.GetRestrictedSendListBaseById(context,restrictedSendListId);
        }

        public IEnumerable<BaseDocumentRestrictedSendList> GetRestrictedSendLists(IContext context, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            return documentDb.GetRestrictedSendListBase(context, documentId);
        }

        public void UpdateRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validRestrictedSendLists = documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Remove(validRestrictedSendLists.FirstOrDefault(x => x.Id == restrictedSendList.Id));
            validRestrictedSendLists.Add(restrictedSendList);
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            documentDb.UpdateRestrictedSendList(context, restrictedSendList);
        }
        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validRestrictedSendLists = documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Add(restrictedSendList);
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            var id = documentDb.AddRestrictedSendList(context, new List<ModifyDocumentRestrictedSendList> { restrictedSendList }).FirstOrDefault();
            return id;
        }

        public void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var restrictedSendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentRestrictedSendList
            {
                DocumentId = model.DocumentId,
                PositionId = x.TargetPositionId,
                AccessLevelId = x.AccessLevelId.GetValueOrDefault()
            });
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validRestrictedSendLists = documentDb.GetRestrictedSendList(context, model.DocumentId).ToList();
            validRestrictedSendLists.AddRange(restrictedSendLists);
            ValidSendListsByRestrictedSendLists(context, model.DocumentId, validRestrictedSendLists);

            documentDb.AddRestrictedSendList(context, restrictedSendLists);
        }

        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var restrictedSendList = documentDb.GetRestrictedSendListById(context, restrictedSendListId);

            var validRestrictedSendLists = documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Remove(validRestrictedSendLists.FirstOrDefault(x => x.Id == restrictedSendList.Id));
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }
        #endregion DocumentRestrictedSendLists

        #region Valid SendLists

        public void ValidSendListsBySendLists(IContext context, int documentId
            , IEnumerable<ModifyDocumentSendList> sendLists)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var restrictedSendLists = documentDb.GetRestrictedSendList(context, documentId);

            ValidSendLists(context, documentId, sendLists, restrictedSendLists);
        }
        public void ValidSendListsByRestrictedSendLists(IContext context, int documentId
            , IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var sendLists = documentDb.GetSendList(context, documentId);

            ValidSendLists(context, documentId, sendLists, restrictedSendLists);
        }
        public void ValidSendLists(IContext context, int documentId
            , IEnumerable<ModifyDocumentSendList> sendLists
            , IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {

            if (restrictedSendLists.GroupBy(x => new { x.DocumentId, x.PositionId }).Any(x => x.Count() > 1))
            {
                throw new DocumentRestrictedSendListDuplication();
            }

            if (sendLists.GroupBy(x => new { x.DocumentId, x.TargetPositionId, x.SendType }).Any(x => x.Count() > 1))
            {
                throw new DocumentSendListDuplication();
            }

            if (restrictedSendLists?.Count() > 0
                && sendLists.GroupJoin(restrictedSendLists
                    , sl => sl.TargetPositionId
                    , rsl => rsl.PositionId
                    , (sl, rsls) => new { sl, rsls }).Any(x => x.rsls.Count() == 0))
            {
                throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            var templateDocumentDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var templateDocument = templateDocumentDb.GetTemplateDocumentByDocumentId(context, documentId);

            if (templateDocument == null)
            {
                throw new TemplateDocumentNotFoundOrUserHasNoAccess();
            }

            if (templateDocument.IsHard)
            {
                if (templateDocument.RestrictedSendLists.Count() > 0
                    && templateDocument.RestrictedSendLists.GroupJoin(restrictedSendLists
                        , trsl => trsl.PositionId
                        , rsl => rsl.PositionId
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentRestrictedSendListDoesNotMatchTheTemplate();
                }

                if (templateDocument.SendLists.Count() > 0
                    && templateDocument.SendLists.GroupJoin(sendLists
                        , trsl => new { trsl.TargetPositionId, trsl.SendType }
                        , rsl => new { rsl.TargetPositionId, rsl.SendType }
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentSendListDoesNotMatchTheTemplate();
                }
            }
        }
        #endregion

        #region DocumentSendLists
        private IEnumerable<BaseDocumentSendListStage> GetSendListStage(IEnumerable<BaseDocumentSendList> sendLists)
        {
            if (sendLists?.Count() > 0)
            {
                return Enumerable.Range(0, sendLists.Max(x => x.Stage)+1)
                    .GroupJoin(sendLists, s => s, sl => sl.Stage
                    , (s, sls) => new { s, sls = sls.Where(x => x.Id > 0) })
                    .Select (x=>new BaseDocumentSendListStage
                    {
                        Stage = x.s,
                        SendLists = x.sls.ToList()
                    }).ToList();

            }
            else
                return new List<BaseDocumentSendListStage>();
        }
        public IEnumerable<BaseDocumentSendListStage> GetSendListStage(IContext context, int documentId, bool isLastStage = false)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var sendLists = documentDb.GetSendListBase(context, documentId).ToList();

            if (isLastStage)
            {
                int lastStage = sendLists.Max(x => x.Stage) + 1;
                sendLists.Add(new BaseDocumentSendList { Id = 0, Stage = lastStage });
            }

            return GetSendListStage(sendLists);
        }

        public BaseDocumentSendList GetSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            return documentDb.GetSendListBaseById(context, sendListId);
        }

        public IEnumerable<BaseDocumentSendList> GetSendLists(IContext context, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            return documentDb.GetSendListBase(context, documentId).ToList();
        }

        public void UpdateSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validSendLists = documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Remove(validSendLists.FirstOrDefault(x => x.Id == sendList.Id));
            validSendLists.Add(sendList);
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            documentDb.UpdateSendList(context, sendList);
        }
        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validSendLists = documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Add(sendList);
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            var id = documentDb.AddSendList(context, new List<ModifyDocumentSendList> { sendList }).FirstOrDefault();
            return id;
        }

        public void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var sendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentSendList
            {
                DocumentId = model.DocumentId,
                Stage = x.Stage,
                SendType = (EnumSendType)x.SendTypeId,
                TargetPositionId = x.TargetPositionId,
                Description = x.Description,
                DueDate = x.DueDate,
                DueDay = x.DueDay.GetValueOrDefault(),
                AccessLevel = (EnumDocumentAccess)x.AccessLevelId.GetValueOrDefault(),
            });
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var validSendLists = documentDb.GetSendList(context, model.DocumentId).ToList();
            validSendLists.AddRange(sendLists);
            ValidSendListsBySendLists(context, model.DocumentId, validSendLists);

            documentDb.AddSendList(context, sendLists);
        }

        public void DeleteSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var sendList = documentDb.GetSendListById(context, sendListId);

            var validSendLists = documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Remove(validSendLists.FirstOrDefault(x => x.Id == sendList.Id));
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            documentDb.DeleteSendList(context, sendListId);
        }

        public bool AddSendListStage(IContext context, ModifyDocumentSendListStage model)
        {
            if (model.Stage >= 0)
            {
                var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

                var sendLists = documentDb.GetSendList(context, model.DocumentId);

                sendLists = sendLists.Where(x => x.Stage >= model.Stage);

                if (sendLists?.Count() > 0)
                    foreach (var sendList in sendLists)
                    {
                        sendList.Stage++;
                        documentDb.UpdateSendList(context, sendList);
                    }
                else
                    return true;
            }
            return false;
        }

        public void DeleteSendListStage(IContext context, ModifyDocumentSendListStage model)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var sendLists = documentDb.GetSendList(context, model.DocumentId);

            ValidSendListsBySendLists(context, model.DocumentId, sendLists.Where(x => x.Stage != model.Stage));

            foreach (var sendList in sendLists.Where(x => x.Stage == model.Stage))
            {
                documentDb.DeleteSendList(context, sendList.Id);
            }

            foreach (var sendList in sendLists.Where(x => x.Stage > model.Stage))
            {
                sendList.Stage--;
                documentDb.UpdateSendList(context, sendList);
            }

        }

        #endregion DocumentSendLists

        #region DocumentSavedFilters
        public int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            if (savedFilter.Id == 0) // new savedFilter
            {
                documentDb.AddSavedFilters(context, savedFilter);
            }
            else
            {
                documentDb.UpdateSavedFilters(context, savedFilter);
            }

            return savedFilter.Id;
        }

        public IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var savedFilters = docDb.GetSavedFilters(ctx).ToList();
            var js = new JavaScriptSerializer();
            for (int i = 0, l = savedFilters.Count; i < l; i++)
            {
                try
                {
                    savedFilters[i].Filter = js.DeserializeObject(savedFilters[i].Filter.ToString());
                }
                catch
                {
                }
            }
            return savedFilters;
        }

        public BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var savedFilter = documentDb.GetSavedFilter(ctx, savedFilterId);
            var js = new JavaScriptSerializer();
            try
            {
                savedFilter.Filter = js.DeserializeObject(savedFilter.Filter.ToString());
            }
            catch
            {
            }
            return savedFilter;
        }
        public void DeleteSavedFilter(IContext context, int savedFilterId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteSavedFilter(context, savedFilterId);
        }
        #endregion DocumentSavedFilters

        #region DocumentEvents

        public void AddDocumentComment(IContext context, AddNote note)
        {
            var evt = new BaseDocumentEvent
            {
                DocumentId = note.DocumentId,
                Description = note.Description,
                EventType = EnumEventTypes.AddNote,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                SourcePositionId = (int)context.CurrentPositionId,
                TargetPositionId = context.CurrentPositionId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,

            };

            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            db.AddDocumentEvent(context, evt);
        }

        #endregion

        #region Operation with document
        public IEnumerable<BaseSystemAction> GetDocumentActions(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var document = documentDb.GetDocument(ctx, documentId);
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { ObjectCode = "Documents", IsAvailabel = true, PositionsIdList = ctx.CurrentPositionsIdList });
            if (document.IsRegistered)
            {
                actions = actions.Where(x => x.Code != "ModifyDocument").ToList();
            }
            return actions;
        }

        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.AddDocumentAccess(ctx, access);
        }

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.RemoveDocumentAccess(ctx, accessId);
        }

        public void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var acc = db.GetDocumentAccess(context, newStatus.DocumentId);
            if (acc == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            acc.IsInWork = newStatus.IsInWork;
            var ea = new EventAccessModel
            {
                DocumentAccess = acc,
                DocumentEvent = new BaseDocumentEvent
                {
                    DocumentId = newStatus.DocumentId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    SourcePositionId = (int)context.CurrentPositionId,
                    TargetPositionId = context.CurrentPositionId,
                    Description = newStatus.Description,
                    EventType = newStatus.IsInWork ? EnumEventTypes.SetInWork : EnumEventTypes.SetOutWork,
                    LastChangeUserId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,

                }
            };

            db.SetDocumentInformation(context, ea);
        }

        public void ChangeFavouritesForDocument(IContext context, ChangeFavourites model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var acc = db.GetDocumentAccess(context, model.DocumentId);
            acc.IsFavourite = model.IsFavourite;
            db.UpdateDocumentAccess(context, acc);
        }

        public void ControlOn(IContext context, ControlOn model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var docWait = new BaseDocumentWaits
            {
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEvent = new BaseDocumentEvent
                {
                    DocumentId = model.DocumentId,
                    EventType = EnumEventTypes.ControlOn,
                    Description = model.Description,
                    SourcePositionId = (int)context.CurrentPositionId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetPositionId = context.CurrentPositionId,
                    TargetAgentId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,

                }
            };
            db.AddDocumentWait(context, docWait);
        }

        public void ControlChange(IContext context, ControlChange model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var oldWait = db.GetDocumentWaitByOnEventId(context, model.EventId);

            oldWait.OnEvent = null;
            oldWait.OffEvent = new BaseDocumentEvent
            {
                DocumentId = model.DocumentId,
                EventType = EnumEventTypes.ControlChange,
                Description = model.Description,
                SourcePositionId = (int)context.CurrentPositionId,
                SourceAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                TargetAgentId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            db.UpdateDocumentWait(context,oldWait);

            var newWait = new BaseDocumentWaits
            {
                ParentId = oldWait.Id,
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEventId = oldWait.OffEvent.Id
            };
            db.AddDocumentWait(context, newWait);
        }

        public void ControlOff(IContext context, ControlOff model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var docWait = db.GetDocumentWaitByOnEventId(context, model.EventId);

            docWait.ResultTypeId = model.ResultTypeId;

            docWait.OnEvent = null;
            docWait.OffEvent = new BaseDocumentEvent
            {
                DocumentId = model.DocumentId,
                EventType = EnumEventTypes.ControlOff,
                Description = model.Description,
                SourcePositionId = (int)context.CurrentPositionId,
                SourceAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                TargetAgentId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            db.UpdateDocumentWait(context, docWait);
        }

        public int CopyDocument(IContext context, CopyDocument model)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var document = documentDb.GetDocument(context, model.DocumentId);

            document.Id = 0;
            document.CreateDate = DateTime.Now;
            document.IsFavourite = false;
            document.RegistrationNumber = null;
            document.RegistrationNumberPrefix = null;
            document.RegistrationNumberSuffix = null;
            document.RegistrationJournalId = null;

            //if (document.RestrictedSendLists != null && document.RestrictedSendLists.Any())
            //{
            //    var restrictedSendLists = document.RestrictedSendLists.ToList();
            //    for(int i=0,l= restrictedSendLists.Count;i< l;i++)
            //    {
            //        restrictedSendLists[i].Id = 0;
            //    }
            //    document.RestrictedSendLists = restrictedSendLists;
            //}

            //if (document.SendLists != null && document.SendLists.Any())
            //{
            //    var sendLists = document.SendLists.ToList();
            //    for (int i = 0, l = sendLists.Count; i < l; i++)
            //    {
            //        sendLists[i].Id = 0;
            //    }
            //    document.SendLists = sendLists;
            //}

            var evt = new BaseDocumentEvent
            {
                EventType = EnumEventTypes.AddNewDocument,
                Description = "Create",
                LastChangeUserId = context.CurrentAgentId,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                SourcePositionId = (int)context.CurrentPositionId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,

            };

            document.Events = new List<BaseDocumentEvent> { evt };

            var acc = new BaseDocumentAccess
            {
                AccessLevel = EnumDocumentAccess.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = (int)context.CurrentPositionId,
            };

            document.Accesses = new List<BaseDocumentAccess>() { acc };

            return SaveDocument(context, document);
        }

        public void RegisterDocument(IContext context, RegisterDocument model)
        {
            var dictDB = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dictRegJournal = dictDB.GetDictionaryRegistrationJournal(context, model.RegistrationJournalId);
            model.NumerationPrefixFormula = dictRegJournal.NumerationPrefixFormula;

            var docDB = DmsResolver.Current.Get<IDocumentsDbProcess>();

            if (model.RegistrationNumber == null || model.IsOnlyGetNextNumber)
            {   //get next number
                model.RegistrationNumberPrefix = dictRegJournal.PrefixFormula;
                model.RegistrationNumberSuffix = dictRegJournal.SuffixFormula;
                model.RegistrationNumber = null;
            }
            docDB.SetDocumentRegistration(context, model);
        }

        public void AddDocumentLink(IContext context, AddDocumentLink model)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var fullDocument = documentDb.GetDocument(context, model.DocumentId);
            var adm = DmsResolver.Current.Get<IAdminService>();
            adm.VerifyAccessForCurrentUser(context, "Documents", "ModifyDocument", fullDocument.ExecutorPositionId);
            documentDb.AddDocumentLink(context, model);
        }



        #endregion
    }
}