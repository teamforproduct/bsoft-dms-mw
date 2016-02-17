using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, FrontDocument document);

        int AddDocumentByTemplateDocument(IContext context, AddDocumentByTemplateDocument addDocumentByTemplateDocument);
        int ModifyDocument(IContext context, ModifyDocument document);
        void DeleteDocument(IContext cxt, int id);

        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId);

        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext cxt, FrontDocument doc);

    }
}