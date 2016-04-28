using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {

        #region TemplateDocument
        IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx);
        FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, InternalTemplateDocument template, IEnumerable<InternalPropertyValue> properties);
        void DeleteTemplate(IContext ctx, int id);
        bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template);
        bool CanAddTemplate(IContext ctx, ModifyTemplateDocument template);
        bool CanModifyTemplate(IContext ctx, int templateId);
        #endregion TemplateDocument

        #region TemplateDocumentSendList
        IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext ctx,int templateId,FilterTemplateDocumentSendList filter);
        FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext ctx, int templateDocumentId);
        int AddOrUpdateTemplateSendList(IContext ctx, InternalTemplateDocumentSendList template);
        void DeleteTemplateSendList(IContext ctx, int id);
        #endregion TemplateDocumentSendList

        #region TemplateDocumentRestrictedSendList
        IEnumerable<FrontTemplateDocumentRestrictedSendLists> GetTemplateDocumentRestrictedSendLists(IContext ctx, int templateId, FilterTemplateDocumentRestrictedSendList filter);
        FrontTemplateDocumentRestrictedSendLists GetTemplateDocumentRestrictedSendList(IContext ctx, int id);
        int AddOrUpdateTemplateRestrictedSendList(IContext ctx, InternalTemplateDocumentRestrictedSendList template);
        void DeleteTemplateRestrictedSendList(IContext ctx, int id);
        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentTasks
        IEnumerable<FrontTemplateDocumentTasks> GetTemplateDocumentTasks(IContext ctx, int templateId, FilterTemplateDocumentTask filter);
        FrontTemplateDocumentTasks GetTemplateDocumentTask(IContext ctx, int id);
        int AddOrUpdateTemplateTask(IContext ctx, InternalTemplateDocumentTask template);
        bool CanAddTemplateTask(IContext ctx, ModifyTemplateDocumentTasks task);
        void DeleteTemplateTask(IContext ctx, int id);
        #endregion TemplateDocumentTasks
        #region TemplateAttachedFiles

        IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter,
            int templateId);

        FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id);
        int GetNextFileOrderNumber(IContext ctx, int templateId);
        int AddNewFile(IContext ctx, InternalTemplateAttachedFile docFile);
        void UpdateFile(IContext ctx, InternalTemplateAttachedFile docFile);
        void DeleteTemplateAttachedFile(IContext ctx, InternalTemplateAttachedFile docFile);
        bool CanAddTemplateAttachedFile(IContext ctx, ModifyTemplateAttachedFile file);

        #endregion TemplateAttachedFiles


    }
}