using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, BaseDocument document);

        int AddDocumentByTemplateDocument(IContext context, int TemplateDocumentId);
        int ModifyDocument(IContext context, ModifyDocument document);


        IEnumerable<BaseDocument> GetDocuments(IContext ctx, FilterDocument filters);
        BaseDocument GetDocument(IContext ctx, int documentId);
    }
}