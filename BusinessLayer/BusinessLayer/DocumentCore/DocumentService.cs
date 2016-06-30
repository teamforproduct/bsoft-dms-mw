using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using BL.Model.DocumentCore.Actions;
using BL.Model.Exception;

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

        public void GetCountDocuments(IContext ctx, LicenceInfo licence)
        {
            _documentDb.GetCountDocuments(ctx, licence);
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            if (!String.IsNullOrEmpty(filters.FullTextSearch))
            {
                var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
                var ftRes = ftService.SearchDocument(ctx, filters.FullTextSearch);
                if (ftRes != null)
                {
                    var resWithRanges =
                        ftRes.GroupBy(x => x.DocumentId)
                            .Select(x => new { DocId = x.Key, Rate = x.Count() })
                            .OrderByDescending(x => x.Rate);
                    filters.FullTextSearchDocumentId = resWithRanges.Select(x => x.DocId).ToList();
                }
                else
                {
                    filters.FullTextSearchDocumentId = new List<int>();
                }
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


        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ObjectCode = "Documents", ActionCode = "Modify" }).ToList();

            uiElements.AddRange(CommonSystemUtilities.GetPropertyUIElements(ctx, EnumObjects.Documents, CommonDocumentUtilities.GetFilterTemplateByDocument(doc).ToArray()));

            uiElements = CommonDocumentUtilities.VerifyDocument(ctx, doc, uiElements).ToList();

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

        public IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterDocumentWait filter, UIPaging paging)
        {
            return _operationDb.GetDocumentWaits(ctx, filter, paging);
        }

        public IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging)
        {
            return _operationDb.GetDocumentSubscriptions(ctx, filter, paging);
        }

        public IEnumerable<FrontDocumentEvent> GetEventsForDocument(IContext ctx, int documentId, UIPaging paging)
        {
            return _operationDb.GetDocumentEvents(ctx, new FilterDocumentEvent { DocumentId = documentId }, paging);
        }

        public FrontRegistrationFullNumber GetNextRegisterDocumentNumber(IContext ctx, RegisterDocumentBase model)
        {
            var document = _documentDb.RegisterDocumentPrepare(ctx, model);

            if (document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!document.RegistrationJournalId.HasValue)
            {
                throw new DictionaryRecordWasNotFound();
            }
            if (document.IsRegistered.HasValue && document.IsRegistered.Value)
            {
                throw new DocumentHasAlredyBeenRegistered();
            }

            document.RegistrationDate = model.RegistrationDate;

            var registerModel = _documentDb.RegisterModelDocumentPrepare(ctx, model);
            CommonDocumentUtilities.FormationRegistrationNumberByFormula(document, registerModel);
            document.RegistrationNumberPrefix = document.RegistrationJournalPrefixFormula;
            document.RegistrationNumberSuffix = document.RegistrationJournalSuffixFormula;

            _documentDb.GetNextDocumentRegistrationNumber(ctx, document);

            var res = new FrontRegistrationFullNumber
            {
                DocumentId = document.Id,
                RegistrationNumber = document.RegistrationNumber,
                RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                DocumentDate = document.RegistrationDate,
                RegistrationFullNumber = (document.RegistrationNumberPrefix ?? "") + document.RegistrationNumber + (document.RegistrationNumberSuffix ?? "")
            };

            return res;
        }
        #endregion Documents

        #region DocumentPapers
        public FrontDocumentPaper GetDocumentPaper(IContext context, int itemId)
        {
            return _documentDb.GetDocumentPaper(context, itemId);
        }

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter, UIPaging paging)
        {
            return _documentDb.GetDocumentPapers(context, filter, paging);
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

        #region DocumentAccesses

        public IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filters, UIPaging paging)
        {
            return _documentDb.GetDocumentAccesses(ctx, filters, paging);
        }

        #endregion DocumentAccesses 
    }
}