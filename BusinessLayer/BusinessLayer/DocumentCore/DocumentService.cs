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

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            var doc = _documentDb.GetDocument(ctx, documentId);
            var sslService = DmsResolver.Current.Get<IDocumentSendListService>();
            doc.SendListStages = CommonDocumentUtilities.GetSendListStage(doc.SendLists);
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

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId)
        {

            var document = _operationDb.GetDocumentActionsPrepare(ctx, documentId);
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var positions = dictDb.GetDictionaryPositionsWithActions(ctx, new FilterDictionaryPosition { PositionId = ctx.CurrentPositionsIdList });
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            foreach (var position in positions)
            {
                position.Actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { Object = EnumObjects.Documents, IsAvailable = true, PositionsIdList = new List<int> { position.Id } });
                if (document.IsRegistered || position.Id != document.ExecutorPositionId)
                {
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ModifyDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.DeleteDocument).ToList();
                }
                if (document.IsRegistered)
                {
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.RegisterDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ChangeExecutor).ToList();
                }
                position.Actions.Where(x => x.DocumentAction == EnumDocumentActions.ControlOff).ToList()
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
    }
}