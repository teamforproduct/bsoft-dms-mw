using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.FullTextSearch;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {

        object ExecuteAction(EnumDocumentActions act, IContext context, object param);

        IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context, FilterTemplateDocument filter, UIPaging paging);
        IEnumerable<FrontMainTemplateDocument> GetMainTemplateDocument(IContext context, FullTextSearch ftSearch, FilterTemplateDocument filter, UIPaging paging);
        FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplateDocument templateDoc);

        IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext context,FilterTemplateDocumentSendList filter);
        FrontTemplateDocumentSendList GetTemplateDocumentSendList(IContext context, int id);

        IEnumerable<FrontTemplateDocumentRestrictedSendList> GetTemplateDocumentRestrictedSendLists(IContext context,FilterTemplateDocumentRestrictedSendList filter);
        FrontTemplateDocumentRestrictedSendList GetTemplateDocumentRestrictedSendList(IContext context, int id);

        IEnumerable<FrontTemplateDocumentTask> GetTemplateDocumentTasks(IContext context,FilterTemplateDocumentTask filter);
        FrontTemplateDocumentTask GetTemplateDocumentTask(IContext context, int id);

        IEnumerable<FrontTemplateDocumentPaper> GetTemplateDocumentPapers(IContext context, FilterTemplateDocumentPaper filter);
        FrontTemplateDocumentPaper GetTemplateDocumentPaper(IContext context, int id);

        IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter);
        FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id);
        FrontTemplateAttachedFile GetTemplateAttachedFilePdf(IContext ctx, int id);
        FrontTemplateAttachedFile GetTemplateAttachedFilePreview(IContext ctx, int id);

    }
}