using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {
        IEnumerable<FullTemplateDocument> GetTemplateDocuments(IContext context);
        int AddOrUpdateTemplate(IContext context, FullTemplateDocument template);
        BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);
    }
}