using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public class TemplatesDbProcess : CoreDb.CoreDb, ITemplatesDbProcess
    {
        public IEnumerable<TemplateDocumentGet> GetDocumentTemplates(IContext context)
        {
            var dbContext = GetUserDmsContext(context);
            return null;
        }

        public int AddOrUpdateTemplate(IContext context, TemplateDocumentGet template)
        {
            var dbContext = GetUserDmsContext(context);
            return 0;
        }
    }
}