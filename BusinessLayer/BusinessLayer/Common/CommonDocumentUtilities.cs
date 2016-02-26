﻿using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.Common
{
    public static class CommonDocumentUtilities
    {

        public static void SetAtrributesForNewDocument(IContext context, InternalDocument document)
        {
            document.CreateDate = DateTime.Now;
            document.ExecutorPositionId = context.CurrentPositionId;
            document.IsRegistered = false;
            document.IsLaunchPlan = false;
            document.LinkId = null;
            SetLastChange(context, document);
        }

        public static void SetLastChange(IContext context, LastChangeInfo document)
        {
            document.LastChangeDate = DateTime.Now;
            document.LastChangeUserId = context.CurrentAgentId;
        }

        public static IEnumerable<InternalDocumentAccess> GetNewDocumentAccess(IContext context)
        {
            return new List<InternalDocumentAccess>
            {
                new InternalDocumentAccess
                {
                    AccessLevel = EnumDocumentAccesses.PersonalRefIO,
                    IsInWork = true,
                    IsFavourite = false,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    PositionId = context.CurrentPositionId
                }
            };
        }

        public static IEnumerable<InternalDocumentAccess> GetNewDocumentAccess(IContext context, EnumDocumentAccesses accessLevel, int positionId)
        {
            return new List<InternalDocumentAccess>
            {
                new InternalDocumentAccess
                {
                    AccessLevel = accessLevel,
                    IsInWork = true,
                    IsFavourite = false,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    PositionId = positionId
                }
            };
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvent(IContext context, int? idDocument, EnumEventTypes eventType, string description, int? targetPositionId = null)
        {
            return new List<InternalDocumentEvent>
            {
                new InternalDocumentEvent
                {
                    DocumentId = idDocument??0,
                    EventType = eventType,
                    Description = description,
                    SourceAgentId = context.CurrentAgentId,
                    SourcePositionId = context.CurrentPositionId,
                    TargetPositionId = targetPositionId??context.CurrentPositionId,
                    LastChangeUserId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }
            };
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWait(IContext context, ControlOn controlOnModel, EnumEventTypes? eventType = null, int? targetPositionId = null)
        {
            return new List<InternalDocumentWait>
            {
                new InternalDocumentWait
                {
                DocumentId = controlOnModel.DocumentId,
                Task = controlOnModel.Task,
                DueDate = controlOnModel.DueDate,
                AttentionDate = controlOnModel.AttentionDate,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = eventType ==null ? null:GetNewDocumentEvent(context, controlOnModel.DocumentId, eventType.Value, $"{controlOnModel.Task} / {controlOnModel.Description}", targetPositionId).FirstOrDefault()
                }
            };
        }

        public static IEnumerable<BaseSystemUIElement> VerifyDocument(IContext ctx, FrontDocument doc, IEnumerable<BaseSystemUIElement> uiElements)
        {
            if (doc.DocumentDirection != EnumDocumentDirections.External)
            {
                if (uiElements != null)
                {
                    var senderElements = new List<string> { "SenderAgent", "SenderAgentPerson", "SenderNumber", "SenderDate", "Addressee" };
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
                var _templateDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
                var docTemplate = _templateDb.GetTemplateDocument(ctx, doc.TemplateDocumentId);

                if (docTemplate.DocumentSubjectId.HasValue)
                {
                    uiElements?.Where(x => x.Code.Equals("DocumentSubject", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                    doc.DocumentSubjectId = docTemplate.DocumentSubjectId;
                }

                if (doc.DocumentDirection == EnumDocumentDirections.External)
                {
                    if (docTemplate.SenderAgentId.HasValue)
                    {
                        uiElements?.Where(x => x.Code.Equals("SenderAgent", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        doc.SenderAgentId = docTemplate.SenderAgentId;
                    }
                    if (docTemplate.SenderAgentPersonId.HasValue)
                    {
                        uiElements?.Where(x => x.Code.Equals("SenderAgentPerson", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        doc.SenderAgentPersonId = docTemplate.SenderAgentPersonId;
                    }
                    if (!string.IsNullOrEmpty(docTemplate.Addressee))
                    {
                        uiElements?.Where(x => x.Code.Equals("Addressee", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                        doc.Addressee = docTemplate.Addressee;
                    }
                }
            }
            return uiElements;
        }

        public static void VerifySendLists(InternalDocument doc)
        {

            if (doc.RestrictedSendLists.GroupBy(x => new { x.DocumentId, x.PositionId }).Any(x => x.Count() > 1))
            {
                throw new DocumentRestrictedSendListDuplication();
            }

            if (doc.SendLists.GroupBy(x => new { x.DocumentId, x.TargetPositionId, x.SendType }).Any(x => x.Count() > 1))
            {
                throw new DocumentSendListDuplication();
            }

            if (doc.RestrictedSendLists?.Count() > 0
                && doc.SendLists.GroupJoin(doc.RestrictedSendLists
                    , sl => sl.TargetPositionId
                    , rsl => rsl.PositionId
                    , (sl, rsls) => new { sl, rsls }).Any(x => x.rsls.Count() == 0))
            {
                throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            if (doc.IsHard)
            {
                if (doc.TemplateDocument.RestrictedSendLists.Any()
                    && doc.TemplateDocument.RestrictedSendLists.GroupJoin(doc.RestrictedSendLists
                        , trsl => trsl.PositionId
                        , rsl => rsl.PositionId
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentRestrictedSendListDoesNotMatchTheTemplate();
                }

                if (doc.TemplateDocument.SendLists.Any()
                    && doc.TemplateDocument.SendLists.GroupJoin(doc.SendLists
                        , trsl => new { trsl.TargetPositionId, trsl.SendType }
                        , rsl => new { rsl.TargetPositionId, rsl.SendType }
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentSendListDoesNotMatchTheTemplate();
                }
            }
        }

        public static IEnumerable<FrontDocumentSendListStage> GetSendListStage(IEnumerable<FrontDocumentSendList> sendLists)
        {
            if (sendLists?.Count() > 0)
            {
                return Enumerable.Range(0, sendLists.Max(x => x.Stage) + 1)
                    .GroupJoin(sendLists, s => s, sl => sl.Stage
                    , (s, sls) => new { s, sls = sls.Where(x => x.Id > 0) })
                    .Select(x => new FrontDocumentSendListStage
                    {
                        Stage = x.s,
                        SendLists = x.sls.ToList()
                    }).ToList();

            }
            else
                return new List<FrontDocumentSendListStage>();
        }
    }
}