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
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocuments(ctx, filters);
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            return documentDb.GetDocument(ctx, documentId);
        }

        public int AddDocumentByTemplateDocument(IContext context, int templateDocumentId)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, templateDocumentId);
            var baseDocument = new FullDocument
            {
                TemplateDocumentId = baseTemplateDocument.Id,
                CreateDate = DateTime.Now,
                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                Description = baseTemplateDocument.Description,
                ExecutorPositionId = context.CurrentPositionId, ////
                ExecutorAgentId = context.CurrentAgentId///////
            };

            if (baseTemplateDocument.RestrictedSendLists!=null&& baseTemplateDocument.RestrictedSendLists.Count>0)
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
            var db = DmsResolver.Current.Get<ITemplateDocumnetsDbProcess>();
            var baseDocument = new FullDocument
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

        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            var id = documentDb.AddRestrictedSendList(context, restrictedSendList);
            return id;
        }
        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }

        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            var id = documentDb.AddSendList(context, sendList);
            return id;
        }
        public void DeleteSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            documentDb.DeleteSendList(context, sendListId);
        }
    }
}