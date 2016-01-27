using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        public IEnumerable<TemplateDocumentGet> GetDocumentTemplates(IContext context)
        {
            var db = DmsResolver.Current.Get<ITemplatesDbProcess>();
            return db.GetDocumentTemplates(context);
        }

        public int AddOrUpdateTemplate(IContext context, TemplateDocumentGet template)
        {
            var db = DmsResolver.Current.Get<ITemplatesDbProcess>();
            return db.AddOrUpdateTemplate(context, template);
        }
    }
}