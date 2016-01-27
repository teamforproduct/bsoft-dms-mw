using System;
using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        public int SaveDocument (IContext context, BaseDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                cmd = new BaseSaveDocument(context, document);
            }
            else
            {
                cmd = new BaseUpdateDocument(context, document);
            }

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
            return document.Id;
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, DocumentFilter filters)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocuments(ctx, filters);
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocument(ctx, documentId);
        }

        public int AddDocumentByTemplateDocument(IContext context, int TemplateDocumentId)
        {
            // var documentDb = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, TemplateDocumentId);
            var baseDocument = new BaseDocument {

                                TemplateDocumentId = baseTemplateDocument.Id,
                                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                                Description = baseTemplateDocument.Description,
                                RestrictedSendListId = baseTemplateDocument.RestrictedSendListId,
                                ExecutorPositionId  = 0, ////
                                ExecutorAgentId = 0///////

            };
            return SaveDocument (context, baseDocument);
        }
    }
}