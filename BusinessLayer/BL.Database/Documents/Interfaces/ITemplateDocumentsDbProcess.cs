﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Database.Documents.Interfaces
{
    public interface ITemplateDocumentsDbProcess
    {
        IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext ctx);
        BaseTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId);
        BaseTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId);
        int AddOrUpdateTemplate(IContext ctx, BaseTemplateDocument template);
    }
}