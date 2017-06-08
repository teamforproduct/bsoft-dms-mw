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
    public interface ITemplateService
    {

        object ExecuteAction(EnumDocumentActions act, IContext context, object param);

        IEnumerable<FrontTemplate> GetTemplates(IContext context, FilterTemplate filter, UIPaging paging);
        IEnumerable<FrontMainTemplate> GetMainTemplate(IContext context, FullTextSearch ftSearch, FilterTemplate filter, UIPaging paging);
        FrontTemplate GetTemplate(IContext context, int templateId);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontTemplate templateDoc);

        IEnumerable<FrontTemplateSendList> GetTemplateSendLists(IContext context,FilterTemplateSendList filter);
        FrontTemplateSendList GetTemplateSendList(IContext context, int id);

        IEnumerable<FrontTemplateRestrictedSendList> GetTemplateRestrictedSendLists(IContext context,FilterTemplateRestrictedSendList filter);
        FrontTemplateRestrictedSendList GetTemplateRestrictedSendList(IContext context, int id);

        IEnumerable<FrontTemplateAccess> GetTemplateAccesses(IContext context, FilterTemplateAccess filter);
        FrontTemplateAccess GetTemplateAccess(IContext context, int id);

        IEnumerable<FrontTemplateTask> GetTemplateTasks(IContext context,FilterTemplateTask filter);
        FrontTemplateTask GetTemplateTask(IContext context, int id);

        IEnumerable<FrontTemplatePaper> GetTemplatePapers(IContext context, FilterTemplatePaper filter);
        FrontTemplatePaper GetTemplatePaper(IContext context, int id);

        IEnumerable<FrontTemplateFile> GetTemplateFiles(IContext ctx, FilterTemplateFile filter);
        FrontTemplateFile GetTemplateFile(IContext ctx, int id);
        FrontTemplateFile GetTemplateFilePdf(IContext ctx, int id);
        FrontTemplateFile GetTemplateFilePreview(IContext ctx, int id);

    }
}