using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentService
    {
        int SaveDocument (IContext context, BaseDocument document);
        IEnumerable<BaseDocument> GetDocuments(IContext ctx);
        BaseDocument GetDocument(IContext ctx, int documentId);
    }
}