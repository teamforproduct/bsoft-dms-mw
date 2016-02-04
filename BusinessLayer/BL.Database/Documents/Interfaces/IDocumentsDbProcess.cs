using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, FullDocument document);
        void UpdateDocument(IContext ctx, FullDocument document);
        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FullDocument GetDocument(IContext ctx, int documentId);

        int AddRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList);
        void AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId);

        int AddSendList(IContext ctx, ModifyDocumentSendList sendList);
        void AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists);
        void DeleteSendList(IContext ctx, int sendListId);

        int AddDocumentEvent(IContext ctx, BaseDocumentEvent docEvent);

        int AddDocumentAccess(IContext ctx, BaseDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);
        void UpdateDocumentAccess(IContext ctx, BaseDocumentAccess access);
        BaseDocumentAccess GetDocumentAccess(IContext ctx, int documentId);

        void SetDocumentInformation(IContext ctx, EventAccessModel access);

        void AddSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter);
        void UpdateSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx);
        BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext ctx, int savedFilterId);

        void AddDocumentWait(IContext ctx, BaseDocumentWaits documentWait);
    }
}