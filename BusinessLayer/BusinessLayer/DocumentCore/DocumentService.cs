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
        public int SaveDocument(IContext context, BaseDocument document)
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
            var baseDocument = new BaseDocument
            {
                TemplateDocumentId = baseTemplateDocument.Id,
                CreateDate = DateTime.Now,
                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                Description = baseTemplateDocument.Description,
                ExecutorPositionId = context.CurrentPositionId, ////
                ExecutorAgentId = context.CurrentAgentId///////
            };
            return SaveDocument(context, baseDocument);
        }

        public int ModifyDocument(IContext context, ModifyDocument document)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            var baseDocument = new BaseDocument
            {
                Id = document.Id,
                TemplateDocumentId = document.TemplateDocumentId,
                DocumentSubjectId = document.DocumentSubjectId,
                Description = document.Description,
                ExecutorPositionId = document.ExecutorPositionId,
                ExecutorAgentId = document.ExecutorAgentId,
                SenderAgentId = document.SenderAgentId,
                SenderPerson = document.SenderPerson,
                SenderNumber = document.SenderNumber,
                SenderDate = document.SenderDate,
                Addressee = document.Addressee,
                AccessLevelId = document.AccessLevelId
            };
            return SaveDocument(context, baseDocument);
        }
    }
}