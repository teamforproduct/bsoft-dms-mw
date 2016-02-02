using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
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
    }
}