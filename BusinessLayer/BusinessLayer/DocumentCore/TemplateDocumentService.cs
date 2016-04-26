using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using BL.Database.SystemDb;
using BL.Model.SystemCore.Filters;
using System.Linq;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;
        private readonly IAdminService _admin;
        private readonly ICommandService _commandService;
        private readonly IFileStore _fStore;
        private readonly ISystemDbProcess _systemDb;


        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb,IAdminService admin,
            IFileStore fstore, ICommandService commandService, ISystemDbProcess systemDb)
        {
            _templateDb = templateDb;
            _admin = admin;
            _fStore = fstore;
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
        public IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context)
        {
            return _templateDb.GetTemplateDocument(context);
        }

        public int AddOrUpdateTemplate(IContext context, ModifyTemplateDocument template, EnumTemplateDocumentsActions action)
        {

            _admin.VerifyAccess(context, action);

            if (!_templateDb.CanModifyTemplate(context, template))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            CommonDocumentUtilities.SetLastChange(context, template);
            return _templateDb.AddOrUpdateTemplate(context, template);
        }

        public void DeleteTemplate(IContext context, int id)
        {
            _admin.VerifyAccess(context, EnumTemplateDocumentsActions.DeleteTemplateDocument);
            if (!_templateDb.CanModifyTemplate(context, id))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            _templateDb.DeleteTemplate(context, id);

        }
        public FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
           
            return _templateDb.GetTemplateDocument(context, templateDocumentId);
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplateDocument templateDoc)
        {
            var uiElements = _systemDb.GetSystemUIElements(ctx, new FilterSystemUIElement { ObjectCode = "TemplateDocuments", ActionCode = "Modify" }).ToList();
            uiElements = CommonDocumentUtilities.VerifyTemplateDocument(ctx, templateDoc, uiElements).ToList();

            uiElements.AddRange(CommonSystemUtilities.GetPropertyUIElements(ctx, EnumObjects.TemplateDocuments, CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(templateDoc).ToArray()));

            return uiElements;
        }

        #endregion TemplateDocuments
        #region TemplateDocumentsSendList

        public IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext context,int templateId,FilterTemplateDocumentSendList filter)
        {
            return _templateDb.GetTemplateDocumentSendLists(context,templateId,filter);
        }

        public int AddOrUpdateTemplateSendList(IContext context, ModifyTemplateDocumentSendLists template,EnumTemplateDocumentsActions action)
        {
            _admin.VerifyAccess(context, action);
            CommonDocumentUtilities.SetLastChange(context, template);
            return _templateDb.AddOrUpdateTemplateSendList(context, template);
      

        }

        public void DeleteTemplateSendList(IContext context, int id)
        {
            _admin.VerifyAccess(context, EnumTemplateDocumentsActions.DeleteTemplateDocumentSendList);
            _templateDb.DeleteTemplateSendList(context, id);
        }

        public FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentSendList(context, id);
        }
        #endregion TemplateDocumentsSendList

        #region TemplateDocumentsRestrictedSendList

        public IEnumerable<FrontTemplateDocumentRestrictedSendLists> GetTemplateDocumentRestrictedSendLists(
            IContext context,
            int templateId, FilterTemplateDocumentRestrictedSendList filter)
        {
            return _templateDb.GetTemplateDocumentRestrictedSendLists(context, templateId, filter);
        }

        public int AddOrUpdateTemplateRestrictedSendList(IContext context,
            ModifyTemplateDocumentRestrictedSendLists template, EnumTemplateDocumentsActions action)
        {
            _admin.VerifyAccess(context, action);
            CommonDocumentUtilities.SetLastChange(context, template);
            return _templateDb.AddOrUpdateTemplateRestrictedSendList(context, template);
        }

        public void DeleteTemplateRestrictedSendList(IContext context, int id)
        {
            _admin.VerifyAccess(context, EnumTemplateDocumentsActions.DeleteTemplateDocumentRestrictedSendList);
            _templateDb.DeleteTemplateRestrictedSendList(context, id);
        }

        public FrontTemplateDocumentRestrictedSendLists GetTemplateDocumentRestrictedSendList(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentRestrictedSendList(context, id);
        }

        #endregion TemplateDocumentsRestrictedSendList

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTasks> GetTemplateDocumentTasks(IContext context,
            int templateId, FilterTemplateDocumentTask filter)
        {
            return _templateDb.GetTemplateDocumentTasks(context, templateId, filter);
        }

        public int AddOrUpdateTemplateTask(IContext context, ModifyTemplateDocumentTasks template, EnumTemplateDocumentsActions action)
        {
            _admin.VerifyAccess(context, action);
            var tmp=new InternalTemplateDocumentTask(template);
            CommonDocumentUtilities.SetLastChange(context, tmp);
            return _templateDb.AddOrUpdateTemplateTask(context, tmp);
        }

        public void DeleteTemplateTask(IContext context, int id)
        {
            _admin.VerifyAccess(context, EnumTemplateDocumentsActions.DeleteTemplateDocumentTask);
            _templateDb.DeleteTemplateTask(context, id);
        }

        public FrontTemplateDocumentTasks GetTemplateDocumentTask(IContext context, int id)
        {
            return _templateDb.GetTemplateDocumentTask(context, id);
        }

        #endregion TemplateDocumentTasks

        #region TemplateAttachedFiles

        public IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx,
            FilterTemplateAttachedFile filter, int templateId)
        {
            return _templateDb.GetTemplateAttachedFiles(ctx, filter,templateId);
        }

        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            return _templateDb.GetTemplateAttachedFile(ctx, id);
        }



        #endregion TemplateAttachedFiles

    }
}