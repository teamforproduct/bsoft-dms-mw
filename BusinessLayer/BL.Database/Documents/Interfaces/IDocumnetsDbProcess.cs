using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumnetsDbProcess
    {
        void AddDocument(IContext ctx, BaseDocument document);
        void UpdateDocument(IContext ctx, BaseDocument document);
        IEnumerable<FullDocument> GetDocuments(IContext ctx, DocumentFilter filters);
        FullDocument GetDocument(IContext ctx, int documentId);
    }
}