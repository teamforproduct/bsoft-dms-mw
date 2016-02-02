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
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        #region Documents
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

            if (baseTemplateDocument.RestrictedSendLists != null && baseTemplateDocument.RestrictedSendLists.Any())
            {
                baseDocument.RestrictedSendLists = baseTemplateDocument.RestrictedSendLists.Select(x => new BaseDocumentRestrictedSendList()
                {
                    PositionId = x.PositionId,
                    AccessLevelId = x.AccessLevelId
                }).ToList();
            }

            if (baseTemplateDocument.SendLists != null && baseTemplateDocument.SendLists.Any())
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

            var evt = new BaseDocumentEvent
            {
                EventType = EnumEventTypes.AddNewDocument,
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

            baseDocument.Events = new List<BaseDocumentEvent> { evt };

            var acc = new DocumentAccess
            {
                AccessType = EnumDocumentAccess.PersonalRefIO,
                IsInWork = true,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = context.CurrentPositionId,
            };

            baseDocument.Accesses = new List<DocumentAccess>() { acc };

            return SaveDocument(context, baseDocument);
        }

        public int ModifyDocument(IContext context, ModifyDocument document)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseDocument = new FullDocument(document);
            return SaveDocument(context, baseDocument);
        }
        #endregion Documents

        #region DocumentRestrictedSendLists
        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddRestrictedSendList(context, restrictedSendList);
            return id;
        }

        public void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var restrictedSendLists = dicStandSendList.StandartSendListContents.Select(x=>new ModifyDocumentRestrictedSendList {
                DocumentId = model.DocumentId,
                PositionId = x.TargetPositionId,
                AccessLevelId = x.AccessLevelId.GetValueOrDefault()
            });
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            docDb.AddRestrictedSendList(context, restrictedSendLists);
        }

        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddSendList(context, sendList);
            return id;
        }

        public void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var sendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentSendList
            {
                DocumentId = model.DocumentId,
                OrderNumber = x.OrderNumber,
                SendTypeId = x.SendTypeId,
                TargetPositionId = x.TargetPositionId,
                Description = x.Description,
                DueDate = x.DueDate,
                DueDay = x.DueDay.GetValueOrDefault(),
                AccessLevelId = x.AccessLevelId.GetValueOrDefault(),
            });
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            docDb.AddSendList(context, sendLists);
        }

        public void DeleteSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteSendList(context, sendListId);
        }
        #endregion DocumentSendLists
    }
}