using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;

        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb)
        {
            _templateDb = templateDb;
        }

        public IEnumerable<BaseTemplateDocument> GetTemplateDocuments(IContext context)
        {
            return _templateDb.GetTemplateDocument(context);
        }

        public int AddOrUpdateTemplate(IContext context, BaseTemplateDocument template)
        {
            return _templateDb.AddOrUpdateTemplate(context, template);
        }

        public BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            return _templateDb.GetTemplateDocument(context, templateDocumentId);
        }
    }
}