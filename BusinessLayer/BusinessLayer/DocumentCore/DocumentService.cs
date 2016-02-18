﻿using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;
using BL.Logic.DocumentCore.Interfaces;
using System.Linq;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore.Commands;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;
        private readonly ITemplateDocumentsDbProcess _templateDb;

        public DocumentService(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb, ITemplateDocumentsDbProcess templateDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
            _templateDb = templateDb;
        }

        #region Documents
        public int SaveDocument(IContext context, InternalDocument document)
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

        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            return _documentDb.GetDocuments(ctx, filters, paging);
        }

        public FrontDocument GetDocument(IContext ctx, int documentId)
        {
            var doc = _documentDb.GetDocument(ctx, documentId);
            var sslService = DmsResolver.Current.Get<IDocumentSendListService>();
            doc.SendListStages = sslService.GetSendListStage(doc.SendLists);
            return doc;
        }

        public int AddDocumentByTemplateDocument(IContext context, AddDocumentByTemplateDocument model)
        {
            _adminDb.VerifyAccess(context, new VerifyAccess() { ActionCode = EnumActions.AddDocument, PositionId = model.CurrentPositionId });
            var newDoc = _documentDb.AddDocumentByTemplateDocumentPrepare(context, model.TemplateDocumentId);
            newDoc.CreateDate = DateTime.Now;
            newDoc.ExecutorPositionId = context.CurrentPositionId.Value;
            newDoc.IsRegistered = false;
            newDoc.LastChangeDate = DateTime.Now;
            newDoc.LastChangeUserId = context.CurrentAgentId;
            newDoc.SendLists.ToList().ForEach(x =>
                {
                    x.IsInitial = true;
                    x.StartEventId = null;
                    x.CloseEventId = null;
                });
            newDoc.Events = GetEventForNewDocument(context);
            newDoc.Accesses = GetAccessesForNewDocument(context);

            return SaveDocument(context, newDoc);
        }

        public int ModifyDocument(IContext context, ModifyDocument model)
        {
            //TODO make with command
            var doc = _documentDb.GetInternalDocument(context, model.Id);
            _adminDb.VerifyAccess(context, new VerifyAccess() { ActionCode = EnumActions.ModifyDocument, PositionId = doc.ExecutorPositionId });
            context.CurrentPositionId = doc.ExecutorPositionId;
            //var docUpd = new InternalDocument(model, doc);
            doc.Description = model.Description;
            doc.DocumentSubjectId = model.DocumentSubjectId;
            doc.SenderAgentId = model.SenderAgentId;
            doc.SenderAgentPersonId = model.SenderAgentPersonId;
            doc.SenderNumber = model.SenderNumber;
            doc.SenderDate = model.SenderDate;
            doc.Addressee = model.Addressee;
            doc.AccessLevel = model.AccessLevel;

            VerifyDocument(context, new FrontDocument(doc), null);
            return SaveDocument(context, doc);
        }

        public void DeleteDocument(IContext context, int id)
        {
            var doc = _documentDb.GetDocument(context, id);
            _adminDb.VerifyAccess(context, new VerifyAccess() { ActionCode = EnumActions.DeleteDocument, PositionId = doc.ExecutorPositionId });
            Command cmd = new DeleteDocument(context, doc);
            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
        }

        public IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc)
        {
            var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            var uiElements = sysDb.GetSystemUIElements(ctx, new FilterSystemUIElement() { ObjectCode = "Documents", ActionCode = "Modify" });
            uiElements = VerifyDocument(ctx, doc, uiElements);
            return uiElements;
        }

        private IEnumerable<BaseSystemUIElement> VerifyDocument(IContext ctx, FrontDocument doc, IEnumerable<BaseSystemUIElement> uiElements)
        {
            if (doc.DocumentDirection != EnumDocumentDirections.External)
            {
                if (uiElements != null)
                {
                    var senderElements = new List<String>() { "SenderAgent", "SenderAgentPerson", "SenderNumber", "SenderDate", "Addressee" };
                    uiElements = uiElements.Where(x => !senderElements.Contains(x.Code)).ToList();
                }
                doc.SenderAgentId = null;
                doc.SenderAgentPersonId = null;
                doc.SenderNumber = null;
                doc.SenderDate = null;
                doc.Addressee = null;
            }

            if ((doc.DocumentDirection == EnumDocumentDirections.External) && (uiElements == null)
                    &&
                    (
                        doc.SenderAgentId == null ||
                        doc.SenderAgentPersonId == null ||
                        string.IsNullOrEmpty(doc.SenderNumber) ||
                        doc.SenderDate == null ||
                        string.IsNullOrEmpty(doc.Addressee)
                    )
                )
            {
                throw new NeedInformationAboutCorrespondent();
            }

            if (doc.IsHard)
            {
                var docTemplate = _templateDb.GetTemplateDocument(ctx, doc.TemplateDocumentId);

                if (docTemplate.DocumentSubjectId.HasValue)
                {
                    if (uiElements != null)
                    {
                        uiElements.Where(x => x.Code.Equals("DocumentSubject", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                    }
                    doc.DocumentSubjectId = docTemplate.DocumentSubjectId;
                }

                if (doc.DocumentDirection == EnumDocumentDirections.External)
                {
                    if (docTemplate.SenderAgentId.HasValue)
                    {
                        if (uiElements != null)
                        {
                            uiElements.Where(x => x.Code.Equals("SenderAgent", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        }
                        doc.SenderAgentId = docTemplate.SenderAgentId;
                    }
                    if (docTemplate.SenderAgentPersonId.HasValue)
                    {
                        if (uiElements != null)
                        {
                            uiElements.Where(x => x.Code.Equals("SenderAgentPerson", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        }
                        doc.SenderAgentPersonId = docTemplate.SenderAgentPersonId;
                    }
                    if (!string.IsNullOrEmpty(docTemplate.Addressee))
                    {
                        if (uiElements != null)
                        {
                            uiElements.Where(x => x.Code.Equals("Addressee", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        }
                        doc.Addressee = docTemplate.Addressee;
                    }
                }
            }
            return uiElements;
        }

        public int CopyDocument(IContext context, CopyDocument model)
        {
            var document = _documentDb.CopyDocumentPrepare(context, model.DocumentId);

            foreach (var sl in document.SendLists)
            {
                sl.LastChangeDate = DateTime.Now;
                sl.LastChangeUserId = context.CurrentAgentId;
            }

            foreach (var sl in document.RestrictedSendLists)
            {
                sl.LastChangeDate = DateTime.Now;
                sl.LastChangeUserId = context.CurrentAgentId;
            }

            document.Events = GetEventForNewDocument(context);
            document.Accesses = GetAccessesForNewDocument(context);

            //TODO process files
            document.DocumentFiles = null;

            //TODO make it with Actions
            var docService = DmsResolver.Current.Get<IDocumentService>();
            return docService.SaveDocument(context, document);
        }

        #endregion Documents

        #region help methods

        private List<InternalDocumentEvents> GetEventForNewDocument(IContext context)
        {
            return new List<InternalDocumentEvents>
            {
                new InternalDocumentEvents
                {
                    EventType = EnumEventTypes.AddNewDocument,
                    Description = "Create",
                    LastChangeUserId = context.CurrentAgentId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    TargetPositionId = context.CurrentPositionId,
                    SourcePositionId = (int) context.CurrentPositionId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }

            };
        }

        private List<InternalDocumentAccesses> GetAccessesForNewDocument(IContext context)
        {
            return new List<InternalDocumentAccesses>
            {
                new InternalDocumentAccesses
                {
                    AccessLevel = EnumDocumentAccesses.PersonalRefIO,
                    IsInWork = true,
                    IsFavourite = false,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    PositionId = (int) context.CurrentPositionId,
                }
            };
        }

        #endregion
    }
}