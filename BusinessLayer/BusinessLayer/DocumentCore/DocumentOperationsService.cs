using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore
{
    public class DocumentOperationsService : IDocumentOperationsService
    {
        private readonly IDocumentsDbProcess _documentDb;

        public DocumentOperationsService(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

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

            _documentDb.AddDocumentEvent(context, evt);
        }

        #endregion

        #region Operation with document
        public IEnumerable<BaseSystemAction> GetDocumentActions(IContext ctx, int documentId)
        {
            var document = _documentDb.GetDocument(ctx, documentId);
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { ObjectCode = "Documents", IsAvailable = true, PositionsIdList = ctx.CurrentPositionsIdList });
            if (document.IsRegistered)
            {
                actions = actions.Where(x => x.Code != "ModifyDocument").ToList();
            }
            return actions;
        }

        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            return _documentDb.AddDocumentAccess(ctx, access);
        }

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            _documentDb.RemoveDocumentAccess(ctx, accessId);
        }

        public void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus)
        {
            var acc = _documentDb.GetDocumentAccess(context, newStatus.DocumentId);
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

            _documentDb.SetDocumentInformation(context, ea);
        }

        public void ChangeFavouritesForDocument(IContext context, ChangeFavourites model)
        {
            var acc = _documentDb.GetDocumentAccess(context, model.DocumentId);
            acc.IsFavourite = model.IsFavourite;
            _documentDb.UpdateDocumentAccess(context, acc);
        }

        public void ControlOn(IContext context, ControlOn model)
        {
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
            _documentDb.AddDocumentWait(context, docWait);
        }

        public void ControlChange(IContext context, ControlChange model)
        {
            var oldWait = _documentDb.GetDocumentWaitByOnEventId(context, model.EventId);

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

            _documentDb.UpdateDocumentWait(context, oldWait);

            var newWait = new BaseDocumentWaits
            {
                ParentId = oldWait.Id,
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEventId = oldWait.OffEvent.Id
            };
            _documentDb.AddDocumentWait(context, newWait);
        }

        public void ControlOff(IContext context, ControlOff model)
        {
            var docWait = _documentDb.GetDocumentWaitByOnEventId(context, model.EventId);

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

            _documentDb.UpdateDocumentWait(context, docWait);
        }

        public int CopyDocument(IContext context, CopyDocument model)
        {
            var document = _documentDb.GetDocument(context, model.DocumentId);

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
            //TODO make it with Actions
            var docService = DmsResolver.Current.Get<IDocumentService>();
            return docService.SaveDocument(context, document);
        }

        public void RegisterDocument(IContext context, RegisterDocument model)
        {
            var dictDB = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dictRegJournal = dictDB.GetDictionaryRegistrationJournal(context, model.RegistrationJournalId);
            model.NumerationPrefixFormula = dictRegJournal.NumerationPrefixFormula;

            if (model.RegistrationNumber == null || model.IsOnlyGetNextNumber)
            {   //get next number
                model.RegistrationNumberPrefix = dictRegJournal.PrefixFormula;
                model.RegistrationNumberSuffix = dictRegJournal.SuffixFormula;
                model.RegistrationNumber = null;
            }
            _documentDb.SetDocumentRegistration(context, model);
        }

        public void AddDocumentLink(IContext context, AddDocumentLink model)
        {
            var docLink = new ВaseDocumentLink(model);
            docLink.Document = _documentDb.GetDocument(context, docLink.DocumentId);
            var adm = DmsResolver.Current.Get<IAdminService>();
            adm.VerifyAccessForCurrentUser(context, "Documents", "ModifyDocument", docLink.Document.ExecutorPositionId);
            docLink.ParentDocument = _documentDb.GetDocument(context, docLink.ParentDocumentId);
            if ((docLink.Document.LinkId.HasValue) && (docLink.ParentDocument.LinkId.HasValue) && (docLink.Document.LinkId == docLink.ParentDocument.LinkId))
            {
                throw new DocumentHasAlreadyHadLink();
            }
            _documentDb.AddDocumentLink(context, docLink);
        }

        #endregion         
    }
}