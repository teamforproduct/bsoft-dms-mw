using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
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

        public static void SetLastChange(IContext context, IEnumerable<LastChangeInfo> documents)
        {
            foreach (var doc in documents)
            {
                SetLastChange(context, doc);
            }
        }

        public static InternalDocumentAccess GetNewDocumentAccess(IContext context, int? documentId = null, EnumDocumentAccesses? accessLevel = null, int? positionId = null)
        {
            return new InternalDocumentAccess
            {
                DocumentId = documentId ?? 0,
                AccessLevel = accessLevel ?? EnumDocumentAccesses.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = positionId ?? context.CurrentPositionId
            };
        }

        public static IEnumerable<InternalDocumentAccess> GetNewDocumentAccesses(IContext context, int? documentId = null, EnumDocumentAccesses? accessLevel = null, int? positionId = null)
        {
            return new List<InternalDocumentAccess>
            {
                GetNewDocumentAccess(context,documentId,accessLevel,positionId),
            };
        }

        public static InternalDocumentEvent GetNewDocumentEvent(IContext context, InternalDocumentSendList model)
        {
            return new InternalDocumentEvent
            {
                DocumentId = model.DocumentId != 0 ? model.DocumentId : 0,
                EventType = (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), model.SendType.ToString()),
                Task = model.Task,
                Description = model.Description,
                SourceAgentId = model.SourceAgentId,
                SourcePositionId = model.SourcePositionId,
                TargetPositionId = model.TargetPositionId,
                TargetAgentId = model.TargetAgentId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvents(IContext context, InternalDocumentSendList model)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentEvent(context,model),
            };
        }

        public static InternalDocumentEvent GetNewDocumentEvent(IContext context, int? documentId, EnumEventTypes eventType, string description = null, string task = null, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new InternalDocumentEvent
            {
                DocumentId = documentId ?? 0,
                EventType = eventType,
                Description = description,
                Task = task,
                SourceAgentId = sourceAgentId ?? context.CurrentAgentId,
                SourcePositionId = sourcePositionId ?? context.CurrentPositionId,
                TargetPositionId = targetPositionId ?? context.CurrentPositionId,
                TargetAgentId = targetAgentId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvents(IContext context, int? documentId, EnumEventTypes eventType, string description = null, string task = null, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentEvent(context,documentId,eventType,description,task,targetPositionId,targetAgentId,sourcePositionId,sourceAgentId),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, ControlOn controlOnModel, EnumEventTypes? eventType = null)
        {
            return new InternalDocumentWait
            {
                DocumentId = controlOnModel.DocumentId,
                DueDate = controlOnModel.DueDate,
                AttentionDate = controlOnModel.AttentionDate,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = eventType == null ? null : GetNewDocumentEvent(context, controlOnModel.DocumentId, eventType.Value, controlOnModel.Description, controlOnModel.Task)
            };
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWaits(IContext context, ControlOn controlOnModel, EnumEventTypes? eventType = null, EnumEventCorrespondentType? eventCorrespondentType = null)
        {
            return new List<InternalDocumentWait>
            {
                GetNewDocumentWait(context,controlOnModel,eventType),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null, EnumEventCorrespondentType? eventCorrespondentType = null)
        {
            return new InternalDocumentWait
            {
                DocumentId = sendListModel.DocumentId,
                DueDate = new[] { sendListModel.DueDate, ( !sendListModel.DueDay.HasValue|| sendListModel.DueDay.Value < 0 ) ? null : (DateTime?)DateTime.Now.AddDays(sendListModel.DueDay.Value) }.Max(),
                //AttentionDate = sendListModel.AttentionDate,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.DocumentId, eventType.Value, sendListModel.Description, sendListModel.Task,
                                eventCorrespondentType == EnumEventCorrespondentType.FromSourceToSource? sendListModel.SourcePositionId: sendListModel.TargetPositionId, 
                                null,
                                eventCorrespondentType == EnumEventCorrespondentType.FromTargetToTarget ? sendListModel.TargetPositionId : sendListModel.SourcePositionId,
                                sendListModel.SourceAgentId
                            )
            };
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWaits(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null, EnumEventCorrespondentType? eventCorrespondentType = null)
        {
            return new List<InternalDocumentWait>
            {
                GetNewDocumentWait(context,sendListModel,eventType,eventCorrespondentType)
            };
        }

        public static InternalDocumentSubscription GetNewDocumentSubscription(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null)
        {
            return new InternalDocumentSubscription
            {
                DocumentId = sendListModel.DocumentId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                SendEvent = eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.DocumentId, eventType.Value, sendListModel.Description, sendListModel.Task,
                                sendListModel.TargetPositionId,
                                null,
                                sendListModel.SourcePositionId,
                                sendListModel.SourceAgentId
                            )
            };
        }

        public static IEnumerable<InternalDocumentSubscription> GetNewDocumentSubscriptions(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null)
        {
            return new List<InternalDocumentSubscription>
            {
                GetNewDocumentSubscription(context,sendListModel,eventType)
            };
        }

        public static InternalDocumentSendList GetNewDocumentSendList(IContext context, ModifyDocumentSendList model)
        {
            return new InternalDocumentSendList
            {
                DocumentId = model.DocumentId,
                Stage = model.Stage,
                SendType = model.SendType,
                SourcePositionId = context.CurrentPositionId,
                SourceAgentId = context.CurrentAgentId,
                TargetPositionId = model.TargetPositionId,
                Task = model.Task,
                Description = model.Description,
                DueDate = model.DueDate,
                DueDay = model.DueDay,
                AccessLevel = model.AccessLevel,
                IsInitial = model.IsInitial,

                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,

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