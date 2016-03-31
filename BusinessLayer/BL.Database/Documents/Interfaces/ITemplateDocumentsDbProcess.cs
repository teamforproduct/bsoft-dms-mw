using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {

        #region TemplateDocument
        IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx);
        FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, ModifyTemplateDocument template);
        void DeleteTemplate(IContext ctx, int id);
        bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template);
        bool CanModifyTemplate(IContext ctx, int templateId);
        #endregion TemplateDocument

        #region TemplateDocumentSendList
        IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext ctx,int templateId,FilterTemplateDocumentSendList filter);
        FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext ctx, int templateDocumentId);
        int AddOrUpdateTemplateSendList(IContext ctx, ModifyTemplateDocumentSendLists template);
        void DeleteTemplateSendList(IContext ctx, int id);
        #endregion TemplateDocumentSendList
    }
}