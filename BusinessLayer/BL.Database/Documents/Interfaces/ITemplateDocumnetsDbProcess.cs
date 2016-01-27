using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public interface ITemplateDocumnetsDbProcess
    {
        IEnumerable<FullTemplateDocument> GetTemplateDocument(IContext context);
        BaseTemplateDocument GetTemplateDocument(IContext context, int TemplateDocumentId);
        int AddOrUpdateTemplate(IContext context, FullTemplateDocument template);
    }
}