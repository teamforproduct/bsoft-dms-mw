using System;
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
using BL.Database.SystemDb;
using BL.Model.SystemCore.Filters;
using System.Linq;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.SystemCore.FrontModel;
using BL.Model.FullTextSearch;
using BL.Logic.SystemCore;

namespace BL.Logic.DocumentCore
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateDbProcess _templateDb;
        private readonly ICommandService _commandService;
        private readonly IFileStore _fStore;
        private readonly ISystemDbProcess _systemDb;


        public TemplateService(ITemplateDbProcess templateDb, IAdminService admin,
            IFileStore fStore, ICommandService commandService, ISystemDbProcess systemDb)
        {
            _templateDb = templateDb;
            _commandService = commandService;
            _systemDb = systemDb;
            _fStore = fStore;

        }

        public object ExecuteAction(EnumDocumentActions act, IContext context, object param)
        {
            //var cmd = TemplateCommandFactory.GetTemplateCommand(act, context, null, param);
            //var res = _commandService.ExecuteCommand(cmd);
            //return res;
            return null;
        }

        #region Template

        public IEnumerable<FrontTemplate> GetTemplates(IContext context, FilterTemplate filter, UIPaging paging)
        {
            return _templateDb.GetTemplate(context, filter, paging);
        }

        public IEnumerable<FrontMainTemplate> GetMainTemplate(IContext context, FullTextSearch ftSearch, FilterTemplate filter, UIPaging paging)
        {
            return FTS.Get(context, Modules.Templates, ftSearch, filter, paging, null, _templateDb.GetMainTemplate, _templateDb.GetTemplateIDs);
        }

        public FrontTemplate GetTemplate(IContext context, int templateId)
        {
            return _templateDb.GetTemplate(context, templateId);
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplate templateDoc)
        {
            var uiElements = _systemDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ActionId = new List<int> { (int)EnumDocumentActions.ModifyTemplate } }).ToList();
            uiElements = CommonDocumentUtilities.VerifyTemplate(ctx, templateDoc, uiElements).ToList();
            var uiPropertyElements = CommonSystemUtilities.GetPropertyUIElements(ctx, EnumObjects.Template, CommonDocumentUtilities.GetFilterTemplateByTemplate(templateDoc).ToArray());
            uiElements.AddRange(uiPropertyElements);
            var addProp = uiPropertyElements.Where(x => x.PropertyLinkId.HasValue && !templateDoc.Properties.Select(y => y.PropertyLinkId).ToList().Contains(x.PropertyLinkId.Value) )
                .Select(x => new FrontPropertyValue { PropertyLinkId = x.PropertyLinkId.Value, Value = string.Empty, PropertyCode = x.Code }).ToList();
            templateDoc.Properties = templateDoc.Properties.Concat(addProp).ToList();
            return uiElements;
        }

        #endregion Template

        #region TemplateSendList

        public IEnumerable<FrontTemplateSendList> GetTemplateSendLists(IContext context, FilterTemplateSendList filter)
        {
            return _templateDb.GetTemplateSendLists(context, filter);
        }

        public FrontTemplateSendList GetTemplateSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateSendList(context, id);
        }
        #endregion TemplateSendList

        #region TemplateRestrictedSendList

        public IEnumerable<FrontTemplateRestrictedSendList> GetTemplateRestrictedSendLists(IContext context, FilterTemplateRestrictedSendList filter)
        {
            return _templateDb.GetTemplateRestrictedSendLists(context, filter);
        }


        public FrontTemplateRestrictedSendList GetTemplateRestrictedSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateRestrictedSendList(context, id);
        }

        #endregion TemplateRestrictedSendList

        #region TemplateAccess

        public IEnumerable<FrontTemplateAccess> GetTemplateAccesses(IContext context, FilterTemplateAccess filter)
        {
            return _templateDb.GetTemplateAccesses(context, filter);
        }


        public FrontTemplateAccess GetTemplateAccess(IContext context, int id)
        {
            return _templateDb.GetTemplateAccess(context, id);
        }

        #endregion TemplateAccess

        #region TemplateTask

        public IEnumerable<FrontTemplateTask> GetTemplateTasks(IContext context, FilterTemplateTask filter)
        {
            return _templateDb.GetTemplateTasks(context, filter);
        }


        public FrontTemplateTask GetTemplateTask(IContext context, int id)
        {
            return _templateDb.GetTemplateTask(context, id);
        }

        #endregion TemplateTask

        #region TemplatePaper

        public IEnumerable<FrontTemplatePaper> GetTemplatePapers(IContext context, FilterTemplatePaper filter)
        {
            return _templateDb.GetTemplatePapers(context, filter);
        }


        public FrontTemplatePaper GetTemplatePaper(IContext context, int id)
        {
            return _templateDb.GetTemplatePaper(context, id);
        }

        #endregion TemplatePaper

        #region TemplateFile

        public IEnumerable<FrontTemplateFile> GetTemplateFiles(IContext ctx, FilterTemplateFile filter)
        {
            return _templateDb.GetTemplateFiles(ctx, filter);
        }

        private FrontTemplateFile GetTemplateFile(IContext ctx, int id, EnumDocumentFileType fileType)
        {
            var fl = _templateDb.GetTemplateFile(ctx, id);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            if (fileType == EnumDocumentFileType.UserFile)
            {
                _fStore.GetFile(ctx, fl, fileType);
            }
            else
            {
                _fStore.GetFile(ctx, fl, fileType);
                fl.PdfCreated = true;
                fl.PdfAcceptable = true;
                fl.LastPdfAccess = DateTime.Now;
                _templateDb.ModifyTemplateFilePdfView(ctx,fl);
            }
            return fl;
        }

        public FrontTemplateFile GetTemplateFile(IContext ctx, int id)
        {
            return GetTemplateFile(ctx, id, EnumDocumentFileType.UserFile);
        }

        public FrontTemplateFile GetTemplateFilePdf(IContext ctx, int id)
        {
            return GetTemplateFile(ctx, id, EnumDocumentFileType.PdfFile);
        }

        public FrontTemplateFile GetTemplateFilePreview(IContext ctx, int id)
        {
            return GetTemplateFile(ctx, id, EnumDocumentFileType.PdfPreview);
        }

        #endregion TemplateFile

    }
}