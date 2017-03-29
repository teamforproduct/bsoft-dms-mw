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
using BL.Model.DocumentCore.Actions;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.FullTextSearch;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Context;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly ICommandService _commandService;
        private readonly IAdminService _adminService;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly ILogger _logger;

        public DocumentService(IDocumentsDbProcess documentDb, ICommandService commandService, IAdminService adminService, IDocumentOperationsDbProcess operationDb, ILogger logger)
        {
            _documentDb = documentDb;
            _adminService = adminService;
            _commandService = commandService;
            _operationDb = operationDb;
            _logger = logger;
        }

        #region Documents

        public void GetCountDocuments(IContext ctx, LicenceInfo licence)
        {
            _documentDb.GetCountDocuments(ctx, licence);
        }

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterBase filter, UIPaging paging, EnumGroupCountType? groupCountType = null)
        {
//            if (paging.IsCalculateAddCounter ?? false && !(paging.IsOnlyCounter??true))  paging.IsOnlyCounter = true; //TODO убрать после разборок с счетчиками
            if (!string.IsNullOrEmpty(filter?.FullTextSearchSearch?.FullTextSearchString))
            {
                FileLogger.AppendTextToFile($"", @"C:\TEMPLOGS\fulltext.log");
                FileLogger.AppendTextToFile($"{DateTime.Now} '{filter?.FullTextSearchSearch?.FullTextSearchString}' *************** StartSearchIDInLucena ", @"C:\TEMPLOGS\fulltext.log");
                //                var testSearch = DmsResolver.Current.Get<IFullTextSearchService>().SearchItems(ctx, "417757 file", new FullTextSearchFilter { Module = Modules.Documents, Accesses = new List<int> { 1037, 1041, 1044 } });
                var addFilter = ((filter?.Document?.IsInWork ?? false) ? FullTextFilterTypes.IsInWork : FullTextFilterTypes.NoFilter)
                                + ((filter?.Document?.IsFavourite ?? false) ? FullTextFilterTypes.IsFavourite : FullTextFilterTypes.NoFilter)
                                + ((filter?.Event?.IsNew ?? false) ? FullTextFilterTypes.IsEventNew : FullTextFilterTypes.NoFilter)
                                + ((filter?.Wait?.IsOpened ?? false) ? FullTextFilterTypes.IsWaitOpened : FullTextFilterTypes.NoFilter);
                var filtersWG = (filter?.Document?.SimultaneousAccessPositionId?.Any() ?? false)
                                ? filter.Document.SimultaneousAccessPositionId.Select(y => $"{y}{FullTextFilterTypes.WorkGroupPosition}").ToList()
                                : new List<string>();
                if (groupCountType.HasValue && groupCountType.Value == EnumGroupCountType.Positions && !filtersWG.Any())
                {
                    filtersWG.Add($"*{FullTextFilterTypes.WorkGroupPosition}");
                }
                var filtersTags = (filter?.Document?.TagId?.Any() ?? false)
                                ? filter.Document.TagId.Select(y => $"{y}{FullTextFilterTypes.Tag}").ToList()
                                : new List<string>();
                if (groupCountType.HasValue && groupCountType.Value == EnumGroupCountType.Tags && !filtersTags.Any())
                {
                    filtersTags.Add($"*{FullTextFilterTypes.Tag}");
                }
                var fullTextSearchFilter = new FullTextSearchFilter
                {
                    IsNotSplitText = true,
                    Module = Modules.Documents,
                    Accesses = ctx.GetAccessFilterForFullText(addFilter),
                    Filters = filtersTags.Concat(filtersWG).ToList(),
                };
                bool IsNotAll;
                filter.FullTextSearchSearch.FullTextSearchResult = DmsResolver.Current.Get<IFullTextSearchService>().SearchItems(out IsNotAll, ctx, filter.FullTextSearchSearch.FullTextSearchString, fullTextSearchFilter, paging);
                filter.FullTextSearchSearch.IsNotAll = IsNotAll;
                if (IsNotAll) paging.IsNotAll = IsNotAll;
                FileLogger.AppendTextToFile($"{DateTime.Now} '{filter?.FullTextSearchSearch?.FullTextSearchString}' FinishSearchIDInLucena: {filter.FullTextSearchSearch.FullTextSearchResult.Count()} rows", @"C:\TEMPLOGS\fulltext.log");
            }
            var res = _documentDb.GetDocuments(ctx, filter, paging, groupCountType);
            if (!string.IsNullOrEmpty(filter?.FullTextSearchSearch?.FullTextSearchString))
                FileLogger.AppendTextToFile($"{DateTime.Now} '{filter?.FullTextSearchSearch?.FullTextSearchString}' *************** We have result: {res.Count()} rows", @"C:\TEMPLOGS\fulltext.log");
            if (!string.IsNullOrEmpty(filter?.FullTextSearchSearch?.FullTextSearchString) && !filter.FullTextSearchSearch.IsDontSaveSearchQueryLog && !groupCountType.HasValue && !(paging.IsOnlyCounter ?? false) && res.Any())
            {
                DmsResolver.Current.Get<ILogger>()
                    .AddSearchQueryLog(ctx, new InternalSearchQueryLog
                    {
                        ModuleId = Modules.GetId(Modules.Documents),
                        SearchQueryText = filter?.FullTextSearchSearch?.FullTextSearchString,
                    });
            }
            return res;
        }

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            var doc = _documentDb.GetDocument(ctx, documentId);
            doc.SendListStages = CommonDocumentUtilities.GetSendListStage(doc.SendLists);
            doc.SendLists = null;
            return doc;
        }

        public IEnumerable<int> GetLinkedDocumentIds(IContext ctx, int documentId)
        {
            return _documentDb.GetLinkedDocumentIds(ctx, documentId);
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ActionId = new List<int> { (int)EnumDocumentActions.ModifyDocument } }).ToList();

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

        public IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterBase filter, UIPaging paging)
        {
            return _operationDb.GetDocumentEvents(ctx, filter, paging);
        }

        public IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterBase filter, UIPaging paging)
        {
            return _operationDb.GetDocumentWaits(ctx, filter, paging);
        }

        public IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging)
        {
            return _operationDb.GetDocumentSubscriptions(ctx, filter, paging);
        }

        public IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(IContext ctx, FilterDictionaryPosition filter, UIPaging paging)
        {
            return _operationDb.GetDocumentWorkGroup(ctx, filter, paging);
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

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter, UIPaging paging)
        {
            return _documentDb.GetDocumentPaperLists(context, filter, paging).ToList();
        }

        public IEnumerable<FrontDocumentPaperList> GetMainDocumentPaperLists(IContext context, FullTextSearch ftSearch, FilterDocumentPaperList filter, UIPaging paging)
        {
            var newFilter = new FilterDocumentPaperList();

            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                //newFilter.PaperListId = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentEmployees, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }
            return _documentDb.GetDocumentPaperLists(context, newFilter, paging).ToList();
        }

        #endregion DocumentPaperLists        

        #region DocumentAccesses

        public IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filters, UIPaging paging)
        {
            return _documentDb.GetDocumentAccesses(ctx, filters, paging);
        }

        public void CheckIsInWorkForControls(IContext ctx, FilterDocumentAccess filter)
        {
            AdminContext adminCtx;
            if (ctx is AdminContext)
                adminCtx = ctx as AdminContext;
            else
                adminCtx = new AdminContext(ctx);
            var list = _documentDb.CheckIsInWorkForControlsPrepare(ctx, filter).Where(x => x.PositionId.HasValue).ToList();
            list.ForEach(x=>
            ExecuteAction(EnumDocumentActions.StartWork, adminCtx, new ChangeWorkStatus {CurrentPositionId = x.PositionId.Value,DocumentId = x.DocumentId }));
        }

        #endregion DocumentAccesses 
    }
}