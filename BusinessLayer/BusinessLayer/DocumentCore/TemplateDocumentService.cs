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

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;
        private readonly ICommandService _commandService;
        private readonly IFileStore _fStore;
        private readonly ISystemDbProcess _systemDb;


        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb,IAdminService admin,
            IFileStore fstore, ICommandService commandService, ISystemDbProcess systemDb)
        {
            _templateDb = templateDb;
           _commandService = commandService;
            _systemDb = systemDb;

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
            var uiElements = _systemDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ObjectCode = "TemplateDocuments", ActionCode = "Modify" }).ToList();
            uiElements = CommonDocumentUtilities.VerifyTemplateDocument(ctx, templateDoc, uiElements).ToList();

            uiElements.AddRange(CommonSystemUtilities.GetPropertyUIElements(ctx, EnumObjects.TemplateDocument, CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(templateDoc).ToArray()));

            return uiElements;
        }

        #endregion TemplateDocuments

        #region TemplateDocumentsSendList

        public IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext context,FilterTemplateDocumentSendList filter)
        {
            return _templateDb.GetTemplateDocumentSendLists(context,filter);
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

        #region TemplateAttachedFiles

        public IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter)
        {
            return _templateDb.GetTemplateAttachedFiles(ctx, filter);
        }

        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            return _templateDb.GetTemplateAttachedFile(ctx, id);
        }



        #endregion TemplateAttachedFiles

    }
}