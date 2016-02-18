using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, InternalDocument document);
        void UpdateDocument(IContext ctx, InternalDocument document);
        void DeleteDocument(IContext context, int id);

        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId);

        InternalDocument GetInternalDocument(IContext ctx, int documentId);


        InternalDocument RegisterDocumentPrepare(IContext ctx, int documentId);
        InternalDocument CopyDocumentPrepare(IContext ctx, int documentId);
        InternalDocument AddDocumentByTemplateDocumentPrepare(IContext context, int templateDocumentId);
    }
}