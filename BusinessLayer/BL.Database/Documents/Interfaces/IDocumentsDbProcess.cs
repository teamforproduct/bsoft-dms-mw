using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, FullDocument document);
        void UpdateDocument(IContext ctx, FullDocument document);
        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FullDocument GetDocument(IContext ctx, int documentId);

        IEnumerable<ModifyDocumentRestrictedSendList> GetRestrictedSendList(IContext ctx, int documentId);
        IEnumerable<BaseDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId);
        ModifyDocumentRestrictedSendList GetRestrictedSendListById(IContext ctx, int id);
        BaseDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id);
        void UpdateRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList);
        IEnumerable<int> AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId);

        IEnumerable<ModifyDocumentSendList> GetSendList(IContext ctx, int documentId);
        IEnumerable<BaseDocumentSendList> GetSendListBase(IContext ctx, int documentId);
        ModifyDocumentSendList GetSendListById(IContext ctx, int id);
        BaseDocumentSendList GetSendListBaseById(IContext ctx, int id);
        void UpdateSendList(IContext ctx, ModifyDocumentSendList sendList);
        IEnumerable<int> AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists);
        void DeleteSendList(IContext ctx, int sendListId);

        int AddDocumentEvent(IContext ctx, BaseDocumentEvent docEvent);
        IEnumerable<BaseDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

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

        bool VerifyDocumentRegistrationNumber(IContext ctx, RegisterDocument registerDocument);
        bool SetDocumentRegistration(IContext ctx, RegisterDocument registerDocument);
        void DeleteDocument(IContext context, int id);
        void AddDocumentLink(IContext context, AddDocumentLink model);
    }
}