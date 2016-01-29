using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, BaseDocument document);

        int AddDocumentByTemplateDocument(IContext context, int templateDocumentId);
        int ModifyDocument(IContext context, ModifyDocument document);

        IEnumerable<BaseDocument> GetDocuments(IContext ctx, FilterDocument filters);
        BaseDocument GetDocument(IContext ctx, int documentId);
    }
}