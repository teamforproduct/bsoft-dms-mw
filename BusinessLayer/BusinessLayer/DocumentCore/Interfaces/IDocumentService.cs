using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, FullDocument document);

        int AddDocumentByTemplateDocument(IContext context, int templateDocumentId);
        int ModifyDocument(IContext context, ModifyDocument document);

        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters);
        FullDocument GetDocument(IContext ctx, int documentId);

        int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        void DeleteRestrictedSendList(IContext context, int restrictedSendListId);
    }
}