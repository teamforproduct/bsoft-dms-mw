using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumnetsDbProcess
    {
        IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext context);
        BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);
        int AddOrUpdateTemplate(IContext context, BaseTemplateDocument template);
    }
}