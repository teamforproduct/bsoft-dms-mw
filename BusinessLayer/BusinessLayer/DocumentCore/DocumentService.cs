using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly ICommandService _commandService;

        public DocumentService(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, ICommandService commandService)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _commandService = commandService;
        }

        #region Documents

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            return _documentDb.GetDocuments(ctx, filters, paging);
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            var doc = _documentDb.GetDocument(ctx, documentId, filter);
            var sslService = DmsResolver.Current.Get<IDocumentSendListService>();
            doc.SendListStages = CommonDocumentUtilities.GetSendListStage(doc.SendLists);
            return doc;
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement() { ObjectCode = "Documents", ActionCode = "Modify" }).ToList();
            uiElements = CommonDocumentUtilities.VerifyDocument(ctx, doc, uiElements).ToList();

            uiElements.AddRange(CommonSystemUtilities.GetPropertyUIElements(ctx,EnumObjects.Documents, new string[] { $"{nameof(doc.DocumentTypeId)}={doc.DocumentTypeId}", $"{nameof(doc.DocumentDirection)}={doc.DocumentDirection}", $"{nameof(doc.DocumentSubjectId)}={doc.DocumentSubjectId}" }));

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