using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using BL.Database.SystemDb;
using BL.Model.SystemCore.Filters;
using System.Linq;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.SystemCore.FrontModel;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;
        private readonly ICommandService _commandService;
        private readonly IFileStore _fStore;
        private readonly ISystemDbProcess _systemDb;


        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb, IAdminService admin,
            IFileStore fStore, ICommandService commandService, ISystemDbProcess systemDb)
        {
            _templateDb = templateDb;
            _commandService = commandService;
            _systemDb = systemDb;
            _fStore = fStore;

        }

        public object ExecuteAction(EnumDocumentActions act, IContext context, object param)
        {
            var cmd = TemplateCommandFactory.GetTemplateCommand(act, context, null, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region TemplateDocuments

        public IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context, FilterTemplateDocument filter, UIPaging paging)
        {
            return _templateDb.GetTemplateDocument(context, filter, paging);
        }

        public FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            return _templateDb.GetTemplateDocument(context, templateDocumentId);
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplateDocument templateDoc)
        {
            var uiElements = _systemDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ActionId = new List<int> { (int)EnumDocumentActions.ModifyTemplateDocument } }).ToList();
            uiElements = CommonDocumentUtilities.VerifyTemplateDocument(ctx, templateDoc, uiElements).ToList();
            var uiPropertyElements = CommonSystemUtilities.GetPropertyUIElements(ctx, EnumObjects.TemplateDocument, CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(templateDoc).ToArray());
            uiElements.AddRange(uiPropertyElements);
            var addProp = uiPropertyElements.Where(x => x.PropertyLinkId.HasValue && !templateDoc.Properties.Select(y => y.PropertyLinkId).ToList().Contains(x.PropertyLinkId.Value) )
                .Select(x => new FrontPropertyValue { PropertyLinkId = x.PropertyLinkId.Value }).ToList();
            templateDoc.Properties = templateDoc.Properties.Concat(addProp).ToList();
            return uiElements;
        }

        #endregion TemplateDocuments

        #region TemplateDocumentsSendList

        public IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext context, FilterTemplateDocumentSendList filter)
        {
            return _templateDb.GetTemplateDocumentSendLists(context, filter);
        }

        public FrontTemplateDocumentSendList GetTemplateDocumentSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentSendList(context, id);
        }
        #endregion TemplateDocumentsSendList

        #region TemplateDocumentsRestrictedSendList

        public IEnumerable<FrontTemplateDocumentRestrictedSendList> GetTemplateDocumentRestrictedSendLists(IContext context, FilterTemplateDocumentRestrictedSendList filter)
        {
            return _templateDb.GetTemplateDocumentRestrictedSendLists(context, filter);
        }


        public FrontTemplateDocumentRestrictedSendList GetTemplateDocumentRestrictedSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentRestrictedSendList(context, id);
        }

        #endregion TemplateDocumentsRestrictedSendList

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTask> GetTemplateDocumentTasks(IContext context, FilterTemplateDocumentTask filter)
        {
            return _templateDb.GetTemplateDocumentTasks(context, filter);
        }


        public FrontTemplateDocumentTask GetTemplateDocumentTask(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentTask(context, id);
        }

        #endregion TemplateDocumentTasks

        #region TemplateDocumentPapers

        public IEnumerable<FrontTemplateDocumentPaper> GetTemplateDocumentPapers(IContext context, FilterTemplateDocumentPaper filter)
        {
            return _templateDb.GetTemplateDocumentPapers(context, filter);
        }


        public FrontTemplateDocumentPaper GetTemplateDocumentPaper(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentPaper(context, id);
        }

        #endregion TemplateDocumentPapers

        #region TemplateAttachedFiles

        public IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter)
        {
            return _templateDb.GetTemplateAttachedFiles(ctx, filter);
        }

        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            var fl = _templateDb.GetTemplateAttachedFile(ctx, id);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            _fStore.GetFile(ctx, fl);
            return fl;
        }



        #endregion TemplateAttachedFiles

    }
}