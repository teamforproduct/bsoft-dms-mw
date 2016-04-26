using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {

        object ExecuteAction(EnumDocumentActions act, IContext context, object param);

        IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context);
        int AddOrUpdateTemplate(IContext context, ModifyTemplateDocument template,EnumTemplateDocumentsActions action);
        void DeleteTemplate(IContext context, int id);
        FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplateDocument templateDoc);

        IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext context,int templateId,FilterTemplateDocumentSendList filter);
        int AddOrUpdateTemplateSendList(IContext context, ModifyTemplateDocumentSendLists template, EnumTemplateDocumentsActions action);
        void DeleteTemplateSendList(IContext context, int id);
        FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext context, int id);

        IEnumerable<FrontTemplateDocumentRestrictedSendLists> GetTemplateDocumentRestrictedSendLists(IContext context,
            int templateId, FilterTemplateDocumentRestrictedSendList filter);
        int AddOrUpdateTemplateRestrictedSendList(IContext context, ModifyTemplateDocumentRestrictedSendLists template, EnumTemplateDocumentsActions action);
        void DeleteTemplateRestrictedSendList(IContext context, int id);
        FrontTemplateDocumentRestrictedSendLists GetTemplateDocumentRestrictedSendList(IContext context, int id);

        IEnumerable<FrontTemplateDocumentTasks> GetTemplateDocumentTasks(IContext context,
            int templateId, FilterTemplateDocumentTask filter);
        int AddOrUpdateTemplateTask(IContext context, ModifyTemplateDocumentTasks template, EnumTemplateDocumentsActions action);
        void DeleteTemplateTask(IContext context, int id);
        FrontTemplateDocumentTasks GetTemplateDocumentTask(IContext context, int id);

        IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter, int templateId);
        FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id);




    }
}