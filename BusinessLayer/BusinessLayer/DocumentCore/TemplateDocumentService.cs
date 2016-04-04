﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;
        private readonly IAdminService _admin;


        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb,IAdminService admin)
        {
            _templateDb = templateDb;
            _admin = admin;
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
        
       
    }
}