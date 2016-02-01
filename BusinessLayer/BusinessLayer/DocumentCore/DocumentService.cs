using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;
using BL.Logic.DocumentCore.Interfaces;
using System.Linq;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        public int SaveDocument(IContext context, FullDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                var evt = new BaseDocumentEvent
                {
                    EventType = DocumentEventTypes.AddNewDocument,
                    Description = "Creat",
                    CreateDate = DateTime.Now,
                    Date = DateTime.Now,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    TargetPositionId = context.CurrentPositionId,
                    SourcePositionId = context.CurrentPositionId
                };

                document.Events = new List<BaseDocumentEvent> {evt};
                
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

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocuments(ctx, filters, paging);
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

            if (baseTemplateDocument.SendLists != null && baseTemplateDocument.SendLists.Count() > 0)
            {
                baseDocument.SendLists = baseTemplateDocument.SendLists.Select(x => new BaseDocumentSendList()
                {
                    OrderNumber = x.OrderNumber,
                    SendTypeId = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    IsInitial = true,
                    EventId = null
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