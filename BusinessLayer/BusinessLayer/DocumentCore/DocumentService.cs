using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Logic.SystemServices.FullTextSearch;
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
            if (!String.IsNullOrEmpty(filters.FullTextSearch))
            {
                var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
                var ftRes = ftService.Search(ctx, filters.FullTextSearch);
                var resWithRanges =
                    ftRes.GroupBy(x => x.DocumentId)
                        .Select(x => new {DocId = x.Key, Rate = x.Count()})
                        .OrderByDescending(x => x.Rate);
                filters.DocumentId.AddRange(resWithRanges.Select(x=>x.DocId).Take(paging.PageSize * paging.CurrentPage));
            }
            return _documentDb.GetDocuments(ctx, filters, paging);
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            var doc = _documentDb.GetDocument(ctx, documentId, filter);
            doc.SendListStages = CommonDocumentUtilities.GetSendListStage(doc.SendLists);
            doc.SendLists = null;
            return doc;
        }

        public Model.DocumentCore.ReportModel.ReportDocument GetDocumentByReport(IContext ctx, int documentId)
        {
            return _documentDb.GetDocumentByReport(ctx, documentId);
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

        #region DocumentPapers
        public FrontDocumentPaper GetDocumentPaper(IContext context, int itemId)
        {
            return _documentDb.GetDocumentPaper(context, itemId);
        }

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter)
        {
            return _documentDb.GetDocumentPapers(context, filter).ToList();
        }

        #endregion DocumentPapers    

        #region DocumentPaperLists
        public FrontDocumentPaperList GetDocumentPaperList(IContext context, int itemId)
        {
            return _documentDb.GetDocumentPaperList(context, itemId);
        }

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter)
        {
            return _documentDb.GetDocumentPaperLists(context, filter).ToList();
        }

        #endregion DocumentPaperLists        
    }
}