using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public interface ITemplatesDbProcess
    {
        IEnumerable<TemplateDocumentGet> GetDocumentTemplates(IContext context);
        int AddOrUpdateTemplate(IContext context, TemplateDocumentGet template);
    }
}