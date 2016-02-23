using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;

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

        public static void SetLastChange(IContext context, InternalDocument document)
        {
            document.LastChangeDate = DateTime.Now;
            document.LastChangeUserId = context.CurrentAgentId;
        }

        public static void SetLastChange(IContext context, InternalDocumentAccesses document)
        {
            document.LastChangeDate = DateTime.Now;
            document.LastChangeUserId = context.CurrentAgentId;
        }

        public static List<InternalDocumentEvents> GetNewEvent(IContext context, EnumEventTypes eventType, string description)
        {
            return new List<InternalDocumentEvents>
            {
                new InternalDocumentEvents
                {
                    EventType = eventType,
                    Description = description,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    TargetPositionId = context.CurrentPositionId,
                    SourcePositionId = context.CurrentPositionId,
                    LastChangeUserId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }
            };
        }

        public static List<InternalDocumentEvents> GetEventForChangeExecutorDocument(IContext context, ChangeExecutor model)
        {
            return new List<InternalDocumentEvents>
            {
                new InternalDocumentEvents
                {
                    EventType = EnumEventTypes.ChangeExecutor,
                    Description = model.Description,
                    LastChangeUserId = context.CurrentAgentId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    TargetPositionId = model.PositionId,
                    SourcePositionId = context.CurrentPositionId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }

            };
        }

        public static List<InternalDocumentAccesses> GetAccessesForNewDocument(IContext context)
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
                    PositionId = context.CurrentPositionId
                }
            };
        }

        public static List<InternalDocumentAccesses> GetAccessesForChangeExecutorDocument(IContext context, ChangeExecutor model)
        {
            return new List<InternalDocumentAccesses>
            {
                new InternalDocumentAccesses
                {
                    AccessLevel = model.AccessLevel,
                    IsInWork = true,
                    IsFavourite = false,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    PositionId = model.PositionId
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

    }
}