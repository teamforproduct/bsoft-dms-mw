using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.DocumentCore
{
    public interface ITemplateDocumentService
    {
        IEnumerable<BaseTemplateDocument> GetTemplateDocuments(IContext context);
        int AddOrUpdateTemplate(IContext context, BaseTemplateDocument template);
        BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId);
    }
}