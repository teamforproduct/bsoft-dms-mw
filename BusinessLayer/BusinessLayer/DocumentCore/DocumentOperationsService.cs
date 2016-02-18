using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    public class DocumentOperationsService : IDocumentOperationsService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public DocumentOperationsService(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _documentDb = documentDb;
            _operationDb = operationDb;
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

            _operationDb.AddDocumentEvent(context, evt);
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
            return _operationDb.AddDocumentAccess(ctx, access);
        }

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            _operationDb.RemoveDocumentAccess(ctx, accessId);
        }

        public void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus)
        {
            var acc = _operationDb.GetDocumentAccess(context, newStatus.DocumentId);
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

            _operationDb.SetDocumentInformation(context, ea);
        }

        public void ChangeFavouritesForDocument(IContext context, ChangeFavourites model)
        {
            var acc = _operationDb.GetDocumentAccess(context, model.DocumentId);
            acc.IsFavourite = model.IsFavourite;
            _operationDb.UpdateDocumentAccess(context, acc);
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
            _operationDb.AddDocumentWait(context, docWait);
        }

        public void ControlChange(IContext context, ControlChange model)
        {
            var oldWait = _operationDb.GetDocumentWaitByOnEventId(context, model.EventId);

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

            _operationDb.UpdateDocumentWait(context, oldWait);

            var newWait = new BaseDocumentWaits
            {
                ParentId = oldWait.Id,
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEventId = oldWait.OffEvent.Id
            };
            _operationDb.AddDocumentWait(context, newWait);
        }

        public void ControlOff(IContext context, ControlOff model)
        {
            var docWait = _operationDb.GetDocumentWaitByOnEventId(context, model.EventId);

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

            _operationDb.UpdateDocumentWait(context, docWait);
        }

        public int CopyDocument(IContext context, CopyDocument model)
        {
            var document = _documentDb.CopyDocumentPrepare(context, model.DocumentId);
            //TODO RestrictedSendLists and SendLists
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

            var evt = new InternalDocumentEvents
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

            document.Events = new List<InternalDocumentEvents> { evt };

            var acc = new InternalDocumentAccesses
            {
                AccessLevel = EnumDocumentAccesses.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = (int)context.CurrentPositionId,
            };

            document.Accesses = new List<InternalDocumentAccesses> { acc };

            //TODO process files
            document.DocumentFiles = null;

            //TODO make it with Actions
            var docService = DmsResolver.Current.Get<IDocumentService>();
            return docService.SaveDocument(context, document);
        }

        public void RegisterDocument(IContext context, RegisterDocument model)
        {
            var doc = _documentDb.RegisterDocumentPrepare(context, model.DocumentId);

            if (doc == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (doc.IsRegistered)
            {
                throw new DocumentHasAlredyBeenRegistered();
            }

            var dictDB = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dictRegJournal = dictDB.GetDictionaryRegistrationJournal(context, model.RegistrationJournalId);
            model.NumerationPrefixFormula = dictRegJournal.NumerationPrefixFormula;

            if (model.RegistrationNumber == null || model.IsOnlyGetNextNumber)
            {   //get next number
                model.RegistrationNumberPrefix = dictRegJournal.PrefixFormula;
                model.RegistrationNumberSuffix = dictRegJournal.SuffixFormula;
                model.RegistrationNumber = null;
            }
            _operationDb.SetDocumentRegistration(context, model);
        }

        public void AddDocumentLink(IContext context, AddDocumentLink model)
        {
            var docLink = _operationDb.AddDocumentLinkPrepare(context, model);
            _adminDb.VerifyAccess(context, new VerifyAccess() { ActionCode = EnumActions.ModifyDocument, PositionId = docLink.ExecutorPositionId });
            if ((docLink.DocumentLinkId.HasValue) && (docLink.ParentDocumentLinkId.HasValue) && (docLink.DocumentLinkId == docLink.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHadLink();
            }
            _operationDb.AddDocumentLink(context, docLink);
        }

        #endregion         
    }
}