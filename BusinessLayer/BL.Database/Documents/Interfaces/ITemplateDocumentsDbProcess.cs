using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using System.Linq;
using BL.Database.DBModel.Template;
using BL.Database.DatabaseContext;
using BL.Model.Common;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDbProcess
    {
        #region Template
//        IQueryable<TemplateDocuments> GetTemplateDocumentQuery(IContext ctx, FilterTemplateDocument filter);
        bool ExistsTemplates(IContext context, FilterTemplate filter);
        IEnumerable<FrontMainTemplate> GetMainTemplate(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sotring);
        List<int> GetTemplateIDs(IContext ctx, IBaseFilter filter, UISorting sotring);
        IEnumerable<FrontTemplate> GetTemplate(IContext ctx, FilterTemplate filter, UIPaging paging);
        FrontTemplate GetTemplate(IContext ctx, int templateDocumentId);
        FrontTemplate GetTemplateByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, InternalTemplate template);
        InternalTemplate DeleteTemplatePrepare(IContext ctx, int id);
        InternalTemplate CopyTemplatePrepare(IContext ctx, int id);
        void DeleteTemplate(IContext ctx, int id);
        bool CanModifyTemplate(IContext ctx, ModifyTemplate template);
        bool CanAddTemplate(IContext ctx, AddTemplate template);
        bool CanModifyTemplate(IContext ctx, int templateId);
        #endregion Template

        #region TemplateSendList
        IEnumerable<FrontTemplateSendList> GetTemplateSendLists(IContext ctx,FilterTemplateSendList filter);
        FrontTemplateSendList GetTemplateSendList(IContext ctx, int templateDocumentId);
        int ModifyTemplateSendList(IContext ctx, InternalTemplateSendList template);
        int AddTemplateSendList(IContext ctx, InternalTemplateSendList template);

        void DeleteTemplateSendList(IContext ctx, int id);
        #endregion TemplateSendList

        #region TemplateRestrictedSendList
        IEnumerable<FrontTemplateRestrictedSendList> GetTemplateRestrictedSendLists(IContext ctx, FilterTemplateRestrictedSendList filter);
        bool ExistsTemplateRestrictedSendLists(IContext ctx, FilterTemplateRestrictedSendList filter);
        FrontTemplateRestrictedSendList GetTemplateRestrictedSendList(IContext ctx, int id);
        int AddOrUpdateTemplateRestrictedSendList(IContext ctx, InternalTemplateRestrictedSendList template);
        void DeleteTemplateRestrictedSendList(IContext ctx, int id);
        #endregion TemplateRestrictedSendList

        #region TemplateAccess
        IEnumerable<FrontTemplateAccess> GetTemplateAccesses(IContext ctx, FilterTemplateAccess filter);
        bool ExistsTemplateAccesses(IContext ctx, FilterTemplateAccess filter);
        FrontTemplateAccess GetTemplateAccess(IContext ctx, int id);
        int AddOrUpdateTemplateAccess(IContext ctx, InternalTemplateAccess template);
        void DeleteTemplateAccess(IContext ctx, int id);
        #endregion TemplateAccess

        #region TemplatePaper
        IEnumerable<FrontTemplatePaper> GetTemplatePapers(IContext ctx, FilterTemplatePaper filter);
        bool ExistsTemplatePapers(IContext ctx, FilterTemplatePaper filter);
        FrontTemplatePaper GetTemplatePaper(IContext ctx, int id);
        IEnumerable<int> AddTemplatePapers(IContext context, IEnumerable<InternalTemplatePaper> papers);
        InternalTemplate ModifyTemplatePaperPrepare(IContext ctx, int? id, AddTemplatePaper Paper);
        void ModifyTemplatePaper(IContext context, InternalTemplatePaper item);
        void DeleteTemplatePaper(IContext ctx, int id);
        #endregion TemplatePaper

        #region TemplateTask
        IEnumerable<FrontTemplateTask> GetTemplateTasks(IContext ctx, FilterTemplateTask filter);
        FrontTemplateTask GetTemplateTask(IContext ctx, int id);
        int AddOrUpdateTemplateTask(IContext ctx, InternalTemplateTask template);
        bool CanAddTemplateTask(IContext ctx, AddTemplateTask task);
        void DeleteTemplateTask(IContext ctx, int id);
        #endregion TemplateTask

        #region TemplateFile
        IEnumerable<FrontTemplateFile> GetTemplateFiles(IContext ctx, FilterTemplateFile filter);
        bool ExistsTemplateFiles(IContext ctx, FilterTemplateFile filter);

        FrontTemplateFile GetTemplateFile(IContext ctx, int id);
        int GetNextFileOrderNumber(IContext ctx, int templateId);
        int AddTemplateFile(IContext ctx, InternalTemplateFile docFile);
        void ModifyTemplateFile(IContext ctx, InternalTemplateFile docFile);
        void ModifyTemplateFilePdfView(IContext ctx, InternalTemplateFile docFile);
        InternalTemplateFile ModifyTemplateFilePrepare(IContext ctx, int id);
        void DeleteTemplateFile(IContext ctx, int id);
        InternalTemplateFile DeleteTemplateFilePrepare(IContext ctx, int id);
        bool CanAddTemplateFile(IContext ctx, AddTemplateFile model, BaseFile file);

        #endregion TemplateFile


    }
}