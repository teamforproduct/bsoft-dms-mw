using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Commands;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly ICommandService _commandService;

        public DocumentService(IDocumentsDbProcess documentDb, ICommandService commandService)
        {
            _documentDb = documentDb;
            _commandService = commandService;
        }

        #region Documents

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            return _documentDb.GetDocuments(ctx, filters, paging);
        }

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            var doc = _documentDb.GetDocument(ctx, documentId);
            var sslService = DmsResolver.Current.Get<IDocumentSendListService>();
            doc.SendListStages = sslService.GetSendListStage(doc.SendLists);
            return doc;
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement() { ObjectCode = "Documents", ActionCode = "Modify" });
            uiElements = CommonDocumentUtilities.VerifyDocument(ctx, doc, uiElements);
            return uiElements;
        }

        public object ExecuteAction(EnumDocumentActions act, IContext context, object param)
        {
            var cmd = DocumentCommandFactory.GetDocumentCommand(act, context, null, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #endregion Documents
    }
}