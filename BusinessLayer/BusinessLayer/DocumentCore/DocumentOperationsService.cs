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
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
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
            var evt = new FrontDocumentEvent
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
        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId)
        {

            var document = _operationDb.GetDocumentActionsPrepare(ctx, documentId);
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var positions = dictDb.GetDictionaryPositionsWithActions(ctx, new FilterDictionaryPosition { Id = ctx.CurrentPositionsIdList });
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            foreach (var position in positions)
            {
                position.Actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { Object = EnumObjects.Documents, IsAvailable = true, PositionsIdList = new List<int> { position.Id } });
                if (document.IsRegistered || position.Id != document.ExecutorPositionId)
                {
                    position.Actions = position.Actions.Where(x => x.Action != EnumActions.ModifyDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.Action != EnumActions.DeleteDocument).ToList();
                }
                if (document.IsRegistered)
                {
                    position.Actions = position.Actions.Where(x => x.Action != EnumActions.RegisterDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.Action != EnumActions.ChangeExecutor).ToList();
                }
                position.Actions.Where(x => x.Action == EnumActions.ControlOff).ToList()
                    .ForEach(x =>
                    {
                        x.ActionRecords = new List<InternalActionRecord>()
                        {
                            new InternalActionRecord() {Id = 1, Description = "TEST1"},
                            new InternalActionRecord() {Id = 2, Description = "TEST2"}
                        };
                    });
            }

            return positions;//actions;
        }

        public int AddDocumentAccess(IContext ctx, FrontDocumentAccess access)
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
                DocumentEvent = new FrontDocumentEvent
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
            var docWait = new FrontDocumentWaits
            {
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEvent = new FrontDocumentEvent
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
            oldWait.OffEvent = new FrontDocumentEvent
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

            var newWait = new FrontDocumentWaits
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
            docWait.OffEvent = new FrontDocumentEvent
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