﻿using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore.Filters;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly ICommandService _commandService;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public DocumentService(IDocumentsDbProcess documentDb, ICommandService commandService, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _commandService = commandService;
            _operationDb = operationDb;
        }

        #region Documents

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            return _documentDb.GetDocuments(ctx, filters, paging);
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            var doc = _documentDb.GetDocument(ctx, documentId, filter);
            doc.SendListStages = CommonDocumentUtilities.GetSendListStage(doc.SendLists);
            doc.SendLists = null;
            return doc;
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ObjectCode = "Documents", ActionCode = "Modify" }).ToList();
            uiElements = CommonDocumentUtilities.VerifyDocument(ctx, doc, uiElements).ToList();

            uiElements.AddRange(CommonSystemUtilities.GetPropertyUIElements(ctx,EnumObjects.Documents, new[] { $"{nameof(doc.DocumentTypeId)}={doc.DocumentTypeId}", $"{nameof(doc.DocumentDirection)}={doc.DocumentDirection}", $"{nameof(doc.DocumentSubjectId)}={doc.DocumentSubjectId}" }));

            return uiElements;
        }

        public object ExecuteAction(EnumDocumentActions act, IContext context, object param)
        {
            var cmd = DocumentCommandFactory.GetDocumentCommand(act, context, null, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        public FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId)
        {
            return _operationDb.GetDocumentEvent(ctx, eventId);
        }

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter, UIPaging paging)
        {
            return _operationDb.GetDocumentEvents(ctx, filter, paging);
        }

        public IEnumerable<FrontDocumentEvent> GetEventsForDocument(IContext ctx, int documentId, UIPaging paging)
        {
            return _operationDb.GetDocumentEvents(ctx, new FilterDocumentEvent {DocumentId = documentId}, paging);
        }
        #endregion Documents

    }
}