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

        void DeleteDocument(IContext context, int id);
    }
}