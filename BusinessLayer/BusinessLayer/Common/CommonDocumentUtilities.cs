using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
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

        public static Dictionary<EnumDocumentActions, EnumSubscriptionStates> SubscriptionStatesForAction =
    new Dictionary<EnumDocumentActions, EnumSubscriptionStates>
        {
                { EnumDocumentActions.AffixSigning, EnumSubscriptionStates.Sign },
                { EnumDocumentActions.AffixАpproval, EnumSubscriptionStates.Аpproval },
                { EnumDocumentActions.AffixVisaing, EnumSubscriptionStates.Visa },
                { EnumDocumentActions.AffixАgreement, EnumSubscriptionStates.Аgreement },

        };

        public static Dictionary<EnumDocumentActions, List<EnumEventTypes>> PermissibleEventTypesForAction =
    new Dictionary<EnumDocumentActions, List<EnumEventTypes>>
        {
                { EnumDocumentActions.ControlChange, new List<EnumEventTypes> { EnumEventTypes.ControlOn, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution } },
                { EnumDocumentActions.ControlTargetChange, new List<EnumEventTypes> { EnumEventTypes.ControlOn, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution } },

                { EnumDocumentActions.ControlOff, new List<EnumEventTypes> { EnumEventTypes.ControlOn } },

                { EnumDocumentActions.MarkExecution, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution } },
                { EnumDocumentActions.AcceptResult, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution } },
                { EnumDocumentActions.RejectResult, new List<EnumEventTypes> { EnumEventTypes.MarkExecution} },

                { EnumDocumentActions.RejectSigning, new List<EnumEventTypes> { EnumEventTypes.SendForSigning} },
                { EnumDocumentActions.RejectАpproval, new List<EnumEventTypes> { EnumEventTypes.SendForАpproval } },
                { EnumDocumentActions.RejectVisaing, new List<EnumEventTypes> { EnumEventTypes.SendForVisaing } },
                { EnumDocumentActions.RejectАgreement, new List<EnumEventTypes> { EnumEventTypes.SendForАgreement } },

                { EnumDocumentActions.WithdrawSigning, new List<EnumEventTypes> { EnumEventTypes.SendForSigning} },
                { EnumDocumentActions.WithdrawАpproval, new List<EnumEventTypes> { EnumEventTypes.SendForАpproval } },
                { EnumDocumentActions.WithdrawVisaing, new List<EnumEventTypes> { EnumEventTypes.SendForVisaing } },
                { EnumDocumentActions.WithdrawАgreement, new List<EnumEventTypes> { EnumEventTypes.SendForАgreement } },

                { EnumDocumentActions.AffixSigning, new List<EnumEventTypes> { EnumEventTypes.SendForSigning} },
                { EnumDocumentActions.AffixАpproval, new List<EnumEventTypes> { EnumEventTypes.SendForАpproval } },
                { EnumDocumentActions.AffixVisaing, new List<EnumEventTypes> { EnumEventTypes.SendForVisaing } },
                { EnumDocumentActions.AffixАgreement, new List<EnumEventTypes> { EnumEventTypes.SendForАgreement } },

        };

        public static void SetAtrributesForNewDocument(IContext context, InternalDocument document)
        {
            document.CreateDate = DateTime.Now;
            document.ExecutorPositionId = context.CurrentPositionId;
            document.IsRegistered = false;
            document.IsLaunchPlan = false;
            document.LinkId = null;
            SetLastChange(context, document);
        }

        public static void SetTaskAtrributesForNewDocument(IContext context, IEnumerable<InternalDocumentTask> tasks, int _executorPositionExecutorAgentId)
        {
            foreach (var t in tasks)
            {
                if (t.PositionId == 0)
                {
                    t.PositionId = context.CurrentPositionId;
                    t.PositionExecutorAgentId = _executorPositionExecutorAgentId;
                }
                else
                {
                    var positionExecutorAgentId = GetExecutorAgentIdByPositionId(context, t.PositionId);
                    if (positionExecutorAgentId.HasValue)
                    {
                        t.PositionExecutorAgentId = positionExecutorAgentId.Value;
                    }
                    else
                    {
                        throw new ExecutorAgentForPositionIsNotDefined();
                    }
                }
                t.AgentId = context.CurrentAgentId;
                SetLastChange(context, t);
            }
        }

        public static void SetSendListAtrributesForNewDocument(IContext context, IEnumerable<InternalDocumentSendList> sendLists, int _executorPositionExecutorAgentId, bool? isInitial)
        {
            foreach (var sl in sendLists)
            {
                if (isInitial.HasValue)
                {
                    sl.IsInitial = isInitial.Value;
                }
                if (sl.SourcePositionId == 0)
                {
                    sl.SourcePositionId = context.CurrentPositionId;
                    sl.SourcePositionExecutorAgentId = _executorPositionExecutorAgentId;
                }
                else
                {
                    var positionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sl.SourcePositionId);
                    if (positionExecutorAgentId.HasValue)
                    {
                        sl.SourcePositionExecutorAgentId = positionExecutorAgentId.Value;
                    }
                    else
                    {
                        throw new ExecutorAgentForPositionIsNotDefined();
                    }
                }
                if (sl.TargetPositionId.HasValue)
                {
                    var positionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(context, sl.TargetPositionId);
                    if (positionExecutorAgentId.HasValue)
                    {
                        sl.TargetPositionExecutorAgentId = positionExecutorAgentId.Value;
                    }
                    else
                    {
                        throw new ExecutorAgentForPositionIsNotDefined();
                    }

                }
                sl.StartEventId = null;
                sl.CloseEventId = null;
                sl.SourceAgentId = context.CurrentAgentId;
                SetLastChange(context, sl);
            }
        }

        public static void SetLastChange(IContext context, LastChangeInfo document)
        {
            if (document != null)
            {
                document.LastChangeDate = DateTime.Now;
                document.LastChangeUserId = context.CurrentAgentId;
            }
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
                TaskId = model.TaskId,
                IsAvailableWithinTask = model.IsAvailableWithinTask,
                Description = model.Description,
                SourceAgentId = model.SourceAgentId,
                SourcePositionId = model.SourcePositionId,
                SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, model.SourcePositionId),
                TargetPositionId = model.TargetPositionId,
                TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, model.TargetPositionId),
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

        public static InternalDocumentEvent GetNewDocumentEvent(IContext context, int? documentId, EnumEventTypes eventType, DateTime? eventDate = null, string description = null, int? taskId = null, bool isAvailableWithinTask = false, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new InternalDocumentEvent
            {
                DocumentId = documentId ?? 0,
                EventType = eventType,
                TaskId = taskId,
                IsAvailableWithinTask = isAvailableWithinTask,
                Description = description,
                SourceAgentId = sourceAgentId ?? context.CurrentAgentId,
                SourcePositionId = sourcePositionId ?? context.CurrentPositionId,
                SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId),
                TargetPositionId = targetPositionId ?? context.CurrentPositionId,
                TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId),
                TargetAgentId = targetAgentId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = eventDate ?? DateTime.Now,
                CreateDate = DateTime.Now,
            };
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvents(IContext context, int? documentId, EnumEventTypes eventType, DateTime? eventDate = null, string description = null, int? taskId = null, bool isAvailableWithinTask = false, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentEvent(context,documentId,eventType,eventDate,description,taskId,isAvailableWithinTask,targetPositionId,targetAgentId,sourcePositionId,sourceAgentId),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, int documentId, InternalDocumentEvent onEvent = null)
        {
            return new InternalDocumentWait
            {
                DocumentId = documentId,
                OnEvent = onEvent,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, ControlOn controlOnModel, EnumEventTypes? eventType = null, int? taskId = null, bool isAvailableWithinTask = false)
        {
            return new InternalDocumentWait
            {
                DocumentId = controlOnModel.DocumentId,
                DueDate = controlOnModel.DueDate,
                AttentionDate = controlOnModel.AttentionDate,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = eventType == null ? null : GetNewDocumentEvent(context, controlOnModel.DocumentId, eventType.Value, controlOnModel.EventDate, controlOnModel.Description, taskId, isAvailableWithinTask)
            };
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWaits(IContext context, ControlOn controlOnModel, EnumEventTypes? eventType = null, int? taskId = null, bool isAvailableWithinTask = false)
        {
            return new List<InternalDocumentWait>
            {
                GetNewDocumentWait(context,controlOnModel,eventType, taskId, isAvailableWithinTask),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null, EnumEventCorrespondentType? eventCorrespondentType = null)
        {
            return new InternalDocumentWait
            {
                DocumentId = sendListModel.DocumentId,
                DueDate = new[] { sendListModel.DueDate, (!sendListModel.DueDay.HasValue || sendListModel.DueDay.Value < 0) ? null : (DateTime?)DateTime.Now.AddDays(sendListModel.DueDay.Value) }.Max(),
                //AttentionDate = sendListModel.AttentionDate,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.DocumentId, eventType.Value, null, sendListModel.Description, sendListModel.TaskId, sendListModel.IsAvailableWithinTask,
                                eventCorrespondentType == EnumEventCorrespondentType.FromSourceToSource ? sendListModel.SourcePositionId : sendListModel.TargetPositionId,
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
                SubscriptionStates = EnumSubscriptionStates.No,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                SendEvent = eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.DocumentId, eventType.Value, null, sendListModel.Description, sendListModel.TaskId, sendListModel.IsAvailableWithinTask,
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

        public static int? GetDocumentTaskOrCreateNew(IContext context, InternalDocument document, string task, int? positionId = null, string description = null)
        {
            int? taskId = null;
            if (document.Tasks?.Any() ?? false)
            {
                taskId = document.Tasks.Select(x => x.Id).First();
                //document.Tasks = null;
            }
            else
            {
                if (string.IsNullOrEmpty(task)) return taskId;
                var positionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(context,
                    positionId ?? context.CurrentPositionId);
                if (!positionExecutorAgentId.HasValue)
                {
                    throw new ExecutorAgentForPositionIsNotDefined();
                }
                document.Tasks = new List<InternalDocumentTask>
                {
                    new InternalDocumentTask
                    {
                        DocumentId = document.Id,
                        PositionId = context.CurrentPositionId,
                        PositionExecutorAgentId = positionExecutorAgentId.Value,
                        AgentId = context.CurrentAgentId,
                        Name = task,
                        Description = description,
                        LastChangeUserId = context.CurrentAgentId,
                        LastChangeDate = DateTime.Now,
                    }
                };
            }
            return taskId;
        }

        public static InternalDocumentSendList GetNewDocumentSendList(IContext context, ModifyDocumentSendList model, int? taskId = null)
        {
            var executorPositionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(context, context.CurrentPositionId);
            if (!executorPositionExecutorAgentId.HasValue)
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }
            return new InternalDocumentSendList
            {
                DocumentId = model.DocumentId,
                Stage = model.Stage,
                SendType = model.SendType,
                SourcePositionId = context.CurrentPositionId,
                SourcePositionExecutorAgentId = executorPositionExecutorAgentId.Value,
                SourceAgentId = context.CurrentAgentId,
                TargetPositionId = model.TargetPositionId,
                TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, model.TargetPositionId),
                TargetAgentId = model.TargetAgentId,
                TaskId = taskId,
                Description = model.Description,
                DueDate = model.DueDate,
                DueDay = model.DueDay,
                AccessLevel = model.AccessLevel,
                IsInitial = model.IsInitial,
                IsAvailableWithinTask = model.IsAvailableWithinTask,
                IsAddControl = model.IsAddControl,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,

            };
        }

        public static InternalDocumentRestrictedSendList GetNewDocumentRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList model)
        {
            return new InternalDocumentRestrictedSendList
            {
                AccessLevel = model.AccessLevel,
                DocumentId = model.DocumentId,
                PositionId = model.PositionId,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,

            };
        }

        public static InternalDocumentPaper GetNewDocumentPaper(IContext context, ModifyDocumentPapers model)
        {
            return new InternalDocumentPaper
            {
                DocumentId = model.DocumentId,
                Name = model.Name,
                Description = model.Description,
                IsMain = model.IsMain,
                IsOriginal = model.IsOriginal,
                IsCopy = model.IsCopy,
                PageQuantity = model.PageQuantity,
                OrderNumber = model.OrderNumber,
                Events = GetNewDocumentPaperEvents(context,null,EnumEventTypes.AddNewPaper),
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
            };
        }

        public static IEnumerable<InternalDocumentPaper> GetNewDocumentPapers(IContext context, ModifyDocumentPapers model)
        {
            return new List<InternalDocumentPaper>
            {
                GetNewDocumentPaper(context,model)
            };
        }

        public static InternalDocumentPaperEvent GetNewDocumentPaperEvent(IContext context, int? paperId, EnumEventTypes eventType, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new InternalDocumentPaperEvent
            {
                PaperId = paperId??0,
                EventType = eventType,
                SourceAgentId = sourceAgentId ?? context.CurrentAgentId,
                SourcePositionId = sourcePositionId ?? context.CurrentPositionId,
                SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId),
                TargetPositionId = targetPositionId ?? context.CurrentPositionId,
                TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId),
                TargetAgentId = targetAgentId,
                PlanAgentId = context.CurrentPositionId,
                PlanDate = DateTime.Now,
                SendAgentId = context.CurrentPositionId,
                SendDate = DateTime.Now,
                RecieveAgentId = context.CurrentPositionId,
                RecieveDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
            };
        }

        public static IEnumerable<InternalDocumentPaperEvent> GetNewDocumentPaperEvents(IContext context, int? paperId, EnumEventTypes eventType, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null)
        {
            return new List<InternalDocumentPaperEvent>
            {
                GetNewDocumentPaperEvent(context,  paperId, eventType, targetPositionId, targetAgentId, sourcePositionId, sourceAgentId )
            };
        }

        public static IEnumerable<BaseSystemUIElement> VerifyDocument(IContext ctx, FrontDocument doc, IEnumerable<BaseSystemUIElement> uiElements)
        {
            if (doc.DocumentDirection != EnumDocumentDirections.Incoming)
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

            if ((doc.DocumentDirection == EnumDocumentDirections.Incoming) && (uiElements == null)
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

            if (doc.IsHard.Value)
            {
                var _templateDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
                var docTemplate = _templateDb.GetTemplateDocument(ctx, doc.TemplateDocumentId.Value);

                if (docTemplate.DocumentSubjectId.HasValue)
                {
                    uiElements?.Where(x => x.Code.Equals("DocumentSubject", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                    doc.DocumentSubjectId = docTemplate.DocumentSubjectId;
                }

                if (doc.DocumentDirection == EnumDocumentDirections.Incoming)
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

            //TODO Малинин. Решить вопрос
            //if (doc.SendLists.GroupBy(x => new { x.DocumentId, x.TargetPositionId, x.SendType }).Any(x => x.Count() > 1))
            //{
            //    throw new DocumentSendListDuplication();
            //}

            if (doc.RestrictedSendLists?.Count() > 0
                && doc.SendLists.Where(sl => sl.TargetPositionId.HasValue).GroupJoin(doc.RestrictedSendLists
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
            return new List<FrontDocumentSendListStage>();
        }

        public static InternalTemplateAttachedFile GetNewTemplateAttachedFile(InternalTemplateAttachedFile src, int? newOrderNumber = null)
        {
            return new InternalTemplateAttachedFile
            {
                Extension = src.Extension,
                Name = src.Name,
                FileType = src.FileType,
                FileSize = src.FileSize,
                IsAdditional = src.IsAdditional,
                FileContent = src.FileContent,
                OrderInDocument = newOrderNumber ?? src.OrderInDocument,
                Hash = src.Hash
            };
        }

        public static InternalDocumentAttachedFile GetNewDocumentAttachedFile(InternalDocumentAttachedFile src, int? newOrderNumber = null, int? newVersion = null)
        {
            return new InternalDocumentAttachedFile
            {
                Extension = src.Extension,
                Name = src.Name,
                FileType = src.FileType,
                FileSize = src.FileSize,
                IsAdditional = src.IsAdditional,
                FileContent = src.FileContent,
                Hash = src.Hash,
                OrderInDocument = newOrderNumber ?? src.OrderInDocument,
                Date = DateTime.Now,
                Version = newVersion ?? 1,
                WasChangedExternal = false
            };
        }

        public static int? GetExecutorAgentIdByPositionId(IContext context, int? positionId)
        {
            if (positionId.HasValue)
            {
                var dict = DmsResolver.Current.Get<IDictionariesDbProcess>();
                return dict.GetExecutorAgentIdByPositionId(context, positionId.Value);
            }
            else
                return null;
        }

    }
}