using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, FrontDocument document);
        void UpdateDocument(IContext ctx, FrontDocument document);
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId);
        InternalDocument GetInternalDocument(IContext ctx, int documentId);
        void DeleteDocument(IContext context, int id);

        InternalDocument GetCheckRegistration(IContext ctx, int documentId);
        InternalDocument GetDocumentCopy(IContext ctx, int documentId);
    }
}