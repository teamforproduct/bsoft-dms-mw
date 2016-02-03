using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, FullDocument document);

        int AddDocumentByTemplateDocument(IContext context, int templateDocumentId);
        int ModifyDocument(IContext context, ModifyDocument document);

        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FullDocument GetDocument(IContext ctx, int documentId);

        int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);
        void DeleteRestrictedSendList(IContext context, int restrictedSendListId);

        int AddSendList(IContext ctx, ModifyDocumentSendList sendList);
        void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model);
        void DeleteSendList(IContext context, int sendListId);

        int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx);
        BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext context, int savedFilterId);

        int AddDocumentAccess(IContext ctx, BaseDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);

        void AddDocumentComment(IContext context, int documentId, string comment);

        void EndWorkWithDocument(IContext context, int documentId);
        void ResumeWorkWithDocument(IContext context, int documentId);
        void AddDocumentToFavourite(IContext context, int documentId);
        void RemoveDocumentFromFavourite(IContext context, int documentId);
    }
}