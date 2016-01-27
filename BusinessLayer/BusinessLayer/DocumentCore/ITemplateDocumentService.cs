using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {
        IEnumerable<TemplateDocumentGet> GetDocumentTemplates(IContext context);
        int AddOrUpdateTemplate(IContext context, TemplateDocumentGet template);
    }
}