using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        public IEnumerable<BaseTemplateDocument> GetTemplateDocuments(IContext context)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            return db.GetTemplateDocument(context);
        }

        public int AddOrUpdateTemplate(IContext context, BaseTemplateDocument template)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            return db.AddOrUpdateTemplate(context, template);
        }

        public BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            return db.GetTemplateDocument(context, templateDocumentId);
        }
    }
}