using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System.Linq;
using System;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        public int SaveDocument (IContext context, BaseDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                cmd = new AddDocument(context, document);
            }
            else
            {
                cmd = new UpdateDocument(context, document);
            }

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
            return document.Id;
        }

        public IEnumerable<BaseDocument> GetDocuments(IContext ctx, FilterDocument filters)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocuments(ctx, filters);
        }

        public BaseDocument GetDocument(IContext ctx, int documentId)
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
                                CreateDate = DateTime.Now,
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