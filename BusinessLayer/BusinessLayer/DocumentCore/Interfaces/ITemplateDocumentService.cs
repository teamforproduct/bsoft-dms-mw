using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {
        IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context);
        int AddOrUpdateTemplate(IContext context, ModifyTemplateDocument template);
        void DeleteTemplate(IContext context, int id);
        FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);

        IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext context,int templateId,FilterTemplateDocumentSendList filter);
        int AddOrUpdateTemplateSendList(IContext context, ModifyTemplateDocumentSendLists template);
        void DeleteTemplateSendList(IContext context, int id);
        FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext context, int id);
    }
}