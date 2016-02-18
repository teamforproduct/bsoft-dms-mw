using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {
        IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx);
        FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, FrontTemplateDocument template);
    }
}