﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.DocumentCore
{
    public class TemplateDocumentService : ITemplateDocumentService
    {
        private readonly ITemplateDocumentsDbProcess _templateDb;

        public TemplateDocumentService(ITemplateDocumentsDbProcess templateDb)
        {
            _templateDb = templateDb;
        }

        public IEnumerable<FrontTemplateDocument> GetTemplateDocuments(IContext context)
        {
            return _templateDb.GetTemplateDocument(context);
        }

        public int AddOrUpdateTemplate(IContext context, FrontTemplateDocument template)
        {
            return _templateDb.AddOrUpdateTemplate(context, template);
        }

        public FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            return _templateDb.GetTemplateDocument(context, templateDocumentId);
        }
    }
}