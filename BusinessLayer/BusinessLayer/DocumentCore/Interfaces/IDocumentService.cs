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

        void AddDocumentComment(IContext context, int documentId, string comment);
    }
}