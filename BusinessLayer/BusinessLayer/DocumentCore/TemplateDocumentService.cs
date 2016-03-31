using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Exception;

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

        public int AddOrUpdateTemplate(IContext context, ModifyTemplateDocument template)
        {

            if (!_templateDb.CanModifyTemplate(context, template))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            CommonDocumentUtilities.SetLastChange(context, template);
            return _templateDb.AddOrUpdateTemplate(context, template);
        }

        public void DeleteTemplate(IContext context, int id)
        {
            if (!_templateDb.CanModifyTemplate(context, id))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            _templateDb.DeleteTemplate(context, id);

        }
        public FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
           
            return _templateDb.GetTemplateDocument(context, templateDocumentId);
        }

        
    }
}