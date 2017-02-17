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

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {
        #region TemplateDocument
        IQueryable<TemplateDocuments> GetTemplateDocumentQuery(IContext ctx, DmsContext dbContext, FilterTemplateDocument filter);
        bool ExistsTemplateDocuments(IContext context, FilterTemplateDocument filter);
        IEnumerable<FrontMainTemplateDocument> GetMainTemplateDocument(IContext ctx, FilterTemplateDocument filter, UIPaging paging);
        IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx, FilterTemplateDocument filter, UIPaging paging);
        FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, InternalTemplateDocument template);
        InternalTemplateDocument DeleteTemplatePrepare(IContext ctx, int id);
        InternalTemplateDocument CopyTemplatePrepare(IContext ctx, int id);
        void DeleteTemplate(IContext ctx, int id);
        bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template);
        bool CanAddTemplate(IContext ctx, AddTemplateDocument template);
        bool CanModifyTemplate(IContext ctx, int templateId);
        #endregion TemplateDocument

        #region TemplateDocumentSendList
        IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext ctx,FilterTemplateDocumentSendList filter);
        FrontTemplateDocumentSendList GetTemplateDocumentSendList(IContext ctx, int templateDocumentId);
        int AddOrUpdateTemplateSendList(IContext ctx, InternalTemplateDocumentSendList template);
        void DeleteTemplateSendList(IContext ctx, int id);
        #endregion TemplateDocumentSendList

        #region TemplateDocumentRestrictedSendList
        IEnumerable<FrontTemplateDocumentRestrictedSendList> GetTemplateDocumentRestrictedSendLists(IContext ctx, FilterTemplateDocumentRestrictedSendList filter);
        bool ExistsTemplateDocumentRestrictedSendLists(IContext ctx, FilterTemplateDocumentRestrictedSendList filter);
        FrontTemplateDocumentRestrictedSendList GetTemplateDocumentRestrictedSendList(IContext ctx, int id);
        int AddOrUpdateTemplateRestrictedSendList(IContext ctx, InternalTemplateDocumentRestrictedSendList template);
        void DeleteTemplateRestrictedSendList(IContext ctx, int id);
        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentAccess
        IEnumerable<FrontTemplateDocumentAccess> GetTemplateDocumentAccesses(IContext ctx, FilterTemplateDocumentAccess filter);
        bool ExistsTemplateDocumentAccesses(IContext ctx, FilterTemplateDocumentAccess filter);
        FrontTemplateDocumentAccess GetTemplateDocumentAccess(IContext ctx, int id);
        int AddOrUpdateTemplateAccess(IContext ctx, InternalTemplateDocumentAccess template);
        void DeleteTemplateAccess(IContext ctx, int id);
        #endregion TemplateDocumentAccess

        #region TemplateDocumentPapers
        IEnumerable<FrontTemplateDocumentPaper> GetTemplateDocumentPapers(IContext ctx, FilterTemplateDocumentPaper filter);
        bool ExistsTemplateDocumentPapers(IContext ctx, FilterTemplateDocumentPaper filter);
        FrontTemplateDocumentPaper GetTemplateDocumentPaper(IContext ctx, int id);
        IEnumerable<int> AddTemplateDocumentPapers(IContext context, IEnumerable<InternalTemplateDocumentPaper> papers);
        InternalTemplateDocument ModifyTemplatePaperPrepare(IContext ctx, int? id, AddTemplateDocumentPaper Paper);
        void ModifyTemplatePaper(IContext context, InternalTemplateDocumentPaper item);
        void DeleteTemplatePaper(IContext ctx, int id);
        #endregion TemplateDocumentPapers

        #region TemplateDocumentTasks
        IEnumerable<FrontTemplateDocumentTask> GetTemplateDocumentTasks(IContext ctx, FilterTemplateDocumentTask filter);
        FrontTemplateDocumentTask GetTemplateDocumentTask(IContext ctx, int id);
        int AddOrUpdateTemplateTask(IContext ctx, InternalTemplateDocumentTask template);
        bool CanAddTemplateTask(IContext ctx, AddTemplateDocumentTask task);
        void DeleteTemplateTask(IContext ctx, int id);
        #endregion TemplateDocumentTasks

        #region TemplateAttachedFiles
        IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter);
        bool ExistsTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter);

        FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id);
        int GetNextFileOrderNumber(IContext ctx, int templateId);
        int AddNewFile(IContext ctx, InternalTemplateAttachedFile docFile);
        void UpdateFile(IContext ctx, InternalTemplateAttachedFile docFile);
        void UpdateFilePdfView(IContext ctx, InternalTemplateAttachedFile docFile);
        InternalTemplateAttachedFile UpdateFilePrepare(IContext ctx, int id);
        void DeleteTemplateAttachedFile(IContext ctx, int id);
        InternalTemplateAttachedFile DeleteTemplateAttachedFilePrepare(IContext ctx, int id);
        bool CanAddTemplateAttachedFile(IContext ctx, AddTemplateAttachedFile file);

        #endregion TemplateAttachedFiles


    }
}