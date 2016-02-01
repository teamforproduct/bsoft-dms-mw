using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;
using BL.Logic.DocumentCore.Interfaces;
using System.Linq;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        public int SaveDocument(IContext context, FullDocument document)
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

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocuments(ctx, filters);
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocument(ctx, documentId);
        }

        public int AddDocumentByTemplateDocument(IContext context, int templateDocumentId)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, templateDocumentId);
            var baseDocument = new FullDocument
            {
                TemplateDocumentId = baseTemplateDocument.Id,
                CreateDate = DateTime.Now,
                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                Description = baseTemplateDocument.Description,
                ExecutorPositionId = context.CurrentPositionId, ////
                SenderAgentId = baseTemplateDocument.SenderAgentId,
                SenderAgentPersonId = baseTemplateDocument.SenderAgentPersonId,
                Addressee = baseTemplateDocument.Addressee
            };

            if (baseTemplateDocument.RestrictedSendLists!=null&& baseTemplateDocument.RestrictedSendLists.Count()>0)
            {
                baseDocument.RestrictedSendLists = baseTemplateDocument.RestrictedSendLists.Select(x => new BaseDocumentRestrictedSendList()
                {
                    PositionId = x.PositionId,
                    AccessLevelId = x.AccessLevelId
                }).ToList();
            }
            return SaveDocument(context, baseDocument);
        }

        public int ModifyDocument(IContext context, ModifyDocument document)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseDocument = new FullDocument(document);
            return SaveDocument(context, baseDocument);
        }

        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddRestrictedSendList(context, restrictedSendList);
            return id;
        }
        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }

        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddSendList(context, sendList);
            return id;
        }
        public void DeleteSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteSendList(context, sendListId);
        }
    }
}