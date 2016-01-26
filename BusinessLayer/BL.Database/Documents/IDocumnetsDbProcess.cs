using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public interface IDocumnetsDbProcess
    {
        void AddDocument(IContext ctx, BaseDocument document);
        void UpdateDocument(IContext ctx, BaseDocument document);
        IEnumerable<BaseDocument> GetDocuments(IContext ctx);
        BaseDocument GetDocument(IContext ctx, int documentId);
    }
}