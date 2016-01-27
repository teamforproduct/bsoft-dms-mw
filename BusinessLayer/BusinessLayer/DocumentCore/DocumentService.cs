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

        public IEnumerable<BaseDocument> GetDocuments(IContext ctx)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocuments(ctx);
        }

        public BaseDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocument(ctx, documentId);
        }
    }
}