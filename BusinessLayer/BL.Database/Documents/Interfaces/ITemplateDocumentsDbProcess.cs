using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {
        IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext ctx);
        BaseTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        int AddOrUpdateTemplate(IContext ctx, BaseTemplateDocument template);
    }
}