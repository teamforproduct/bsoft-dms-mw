using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BL.Logic.Common
{
    public static class CommonDocumentUtilities
    {
        #region Dictionaries
        public static Dictionary<EnumDocumentActions, EnumSubscriptionStates> SubscriptionStatesForAction =
        new Dictionary<EnumDocumentActions, EnumSubscriptionStates>
        {
                { EnumDocumentActions.AffixSigning, EnumSubscriptionStates.Sign },
                { EnumDocumentActions.SelfAffixSigning, EnumSubscriptionStates.Sign },
                { EnumDocumentActions.AffixАpproval, EnumSubscriptionStates.Аpproval },
                { EnumDocumentActions.AffixVisaing, EnumSubscriptionStates.Visa },
                { EnumDocumentActions.AffixАgreement, EnumSubscriptionStates.Аgreement },
        };

        public static Dictionary<EnumDocumentActions, List<EnumEventTypes>> PermissibleEventTypesForAction =
        new Dictionary<EnumDocumentActions, List<EnumEventTypes>>
        {

                { EnumDocumentActions.ControlChange, new List<EnumEventTypes> { EnumEventTypes.ControlOn, EnumEventTypes.ControlChange } },

                { EnumDocumentActions.SendForResponsibleExecutionChange, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForResponsibleExecutionChange } },

                { EnumDocumentActions.SendForExecutionChange, new List<EnumEventTypes> { EnumEventTypes.SendForExecution, EnumEventTypes.SendForExecutionChange} },

                //{ EnumDocumentActions.SendForControlChange, new List<EnumEventTypes> { EnumEventTypes.SendForControl, EnumEventTypes.SendForControlChange} },

                { EnumDocumentActions.ControlTargetChange, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution,
                                                                                      EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange,
                                                                                     } },

                { EnumDocumentActions.ControlOff, new List<EnumEventTypes> { EnumEventTypes.ControlOn,/* EnumEventTypes.SendForControl,*/ EnumEventTypes.ControlChange/*, EnumEventTypes.SendForControlChange*/ } },

                { EnumDocumentActions.AskPostponeDueDate, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },
                { EnumDocumentActions.CancelPostponeDueDate, new List<EnumEventTypes> { EnumEventTypes.AskPostponeDueDate, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },

                { EnumDocumentActions.MarkExecution, new List<EnumEventTypes> { EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },
                { EnumDocumentActions.CancelExecution, new List<EnumEventTypes> { EnumEventTypes.MarkExecution, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },
                { EnumDocumentActions.AcceptResult, new List<EnumEventTypes> { EnumEventTypes.MarkExecution, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },
                { EnumDocumentActions.RejectResult, new List<EnumEventTypes> { EnumEventTypes.MarkExecution, EnumEventTypes.SendForResponsibleExecution, EnumEventTypes.SendForExecution, EnumEventTypes.SendForResponsibleExecutionChange, EnumEventTypes.SendForExecutionChange } },

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
        #endregion Dictionaries

        internal static string GetTemplateNameForCopy(IContext context, string name)
        {
            var _templateDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var i = 1;
            var res = name + $" ({i})";
            while (_templateDb.ExistsTemplateDocuments(context, new FilterTemplateDocument { NameExectly = res }))
            {
                i++;
                res = name + $" ({i})";
            }
            return res;
        }

        public static void SetAtrributesForNewDocument(IContext context, InternalDocument document)
        {
            document.CreateDate = DateTime.UtcNow.Date;
            document.ExecutorPositionId = context.CurrentPositionId;
            document.IsRegistered = null;
            document.IsLaunchPlan = false;
            document.LinkId = null;
            SetLastChange(context, document);
        }

        public static void SetTaskAtrributesForNewDocument(IContext context, IEnumerable<InternalDocumentTask> tasks, InternalDictionaryPositionExecutorForDocument _executorPositionExecutor)
        {
            foreach (var t in tasks)
            {
                if (t.PositionId == 0)
                {
                    t.PositionId = context.CurrentPositionId;
                    t.PositionExecutorAgentId = _executorPositionExecutor.ExecutorAgentId.Value;
                    t.PositionExecutorTypeId = _executorPositionExecutor.ExecutorTypeId;
                }
                else
                {
                    var positionExecutor = GetExecutorAgentIdByPositionId(context, t.PositionId);
                    if (positionExecutor?.ExecutorAgentId.HasValue ?? false)
                    {
                        t.PositionExecutorAgentId = positionExecutor.ExecutorAgentId.Value;
                        t.PositionExecutorTypeId = positionExecutor.ExecutorTypeId;
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

        public static void SetSendListAtrributesForNewDocument(IContext context, IEnumerable<InternalDocumentSendList> sendLists, bool? isInitial)
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
                }
                sl.SourcePositionExecutorAgentId = null;
                sl.TargetPositionExecutorAgentId = null;
                sl.SourcePositionExecutorTypeId = null;
                sl.TargetPositionExecutorTypeId = null;
                //else
                //{
                //    var positionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sl.SourcePositionId);
                //    if (positionExecutorAgentId.HasValue)
                //    {
                //        sl.SourcePositionExecutorAgentId = positionExecutorAgentId.Value;
                //    }
                //    else
                //    {
                //        throw new ExecutorAgentForPositionIsNotDefined();
                //    }
                //}
                //if (sl.TargetPositionId.HasValue)
                //{
                //    var positionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(context, sl.TargetPositionId);
                //    if (positionExecutorAgentId.HasValue)
                //    {
                //        sl.TargetPositionExecutorAgentId = positionExecutorAgentId.Value;
                //    }
                //    else
                //    {
                //        throw new ExecutorAgentForPositionIsNotDefined();
                //    }
                //}
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
                document.LastChangeDate = DateTime.UtcNow;
                document.LastChangeUserId = context.CurrentAgentId;
            }
        }

        public static void SetEventAccessess(IContext context, InternalDocumentEvent ev, List<EventAccessGroup> accessGroups, bool isVeryfyDocumentAccess = false)
        {
            if (ev != null)
            {
                var dict = DmsResolver.Current.Get<IDictionaryService>();
                var dictDB = DmsResolver.Current.Get<DictionariesDbProcess>();
                var operationDb = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();
                var docAccesses = operationDb.GetDocumentAccesses(context, ev.DocumentId);
                if (accessGroups == null) accessGroups = new List<EventAccessGroup>();
                if (!accessGroups.Any(x => x.AccessType == EnumEventAccessTypes.Source))
                    accessGroups.Add(new EventAccessGroup { AccessType = EnumEventAccessTypes.Source, AccessGroupsType = EnumEventAccessGroupsTypes.Position, RecordId = context.CurrentPositionId });
                ev.AccessGroups = accessGroups.Select(x => new InternalDocumentEventAccessGroup
                {
                    ClientId = ev.ClientId,
                    EntityTypeId = ev.EntityTypeId,
                    DocumentId = ev.DocumentId,
                    EventId = ev.Id,
                    IsActive = true,
                    AccessType = x.AccessType,
                    AccessGroupsType = x.AccessGroupsType,
                    PositionId = x.AccessGroupsType == EnumEventAccessGroupsTypes.Position ? x.RecordId : null,
                    AgentId = x.AccessGroupsType == EnumEventAccessGroupsTypes.Agent ? x.RecordId : null,
                    DepartmentId = x.AccessGroupsType == EnumEventAccessGroupsTypes.Department ? x.RecordId : null,
                    CompanyId = x.AccessGroupsType == EnumEventAccessGroupsTypes.Company ? x.RecordId : null,
                });
                SetLastChange(context, ev.AccessGroups);
                var accesses = new List<InternalDocumentEventAccess>();
                accessGroups.OrderBy(x => x.AccessType).ToList().ForEach(x =>
                  {
                      List<int> positions = new List<int>();
                      List<int> agents = new List<int>();
                      if (x.AccessGroupsType == EnumEventAccessGroupsTypes.Position && x.RecordId.HasValue)
                          positions.Add(x.RecordId.Value);
                      else if (x.AccessGroupsType == EnumEventAccessGroupsTypes.Agent && x.RecordId.HasValue)
                          agents.Add(x.RecordId.Value);
                      else if (x.AccessGroupsType == EnumEventAccessGroupsTypes.Department && x.RecordId.HasValue)
                          positions.AddRange(dict.GetChildPositions(context, null, x.RecordId.Value));
                      else if (x.AccessGroupsType == EnumEventAccessGroupsTypes.Company && x.RecordId.HasValue)
                          positions.AddRange(dict.GetChildPositions(context, null, null, x.RecordId.Value));
                      else if (x.AccessGroupsType == EnumEventAccessGroupsTypes.SendList && x.RecordId.HasValue)
                      {
                          var sendListContent = dictDB.GetInternalStandartSendListContents(context, new FilterDictionaryStandartSendListContent { StandartSendListId = new List<int> { x.RecordId.Value } });
                          positions.AddRange(sendListContent.Where(y => y.TargetPositionId.HasValue).Select(y => y.TargetPositionId.Value));
                          agents.AddRange(sendListContent.Where(y => y.TargetAgentId.HasValue && !y.TargetPositionId.HasValue).Select(y => y.TargetAgentId.Value));
                      }
                      else if (x.AccessGroupsType == EnumEventAccessGroupsTypes.WorkGroup && x.RecordId.HasValue)
                      {
                          positions.AddRange(docAccesses.Where(y => y.PositionId.HasValue).Select(y => y.PositionId.Value));
                          agents.AddRange(docAccesses.Where(y => y.AgentId.HasValue && !y.PositionId.HasValue).Select(y => y.AgentId.Value));
                      }
                      positions = positions.Where(y => !accesses.Select(z => z.PositionId).Contains(y)).ToList();
                      agents = agents.Where(y => !accesses.Select(z => z.AgentId).Contains(y)).ToList();
                      if (isVeryfyDocumentAccess)
                      {
                          positions = positions.Where(y => docAccesses.Select(z => z.PositionId).Contains(y)).ToList();
                          agents = agents.Where(y => docAccesses.Select(z => z.AgentId).Contains(y)).ToList();
                      }
                      accesses.AddRange(positions.Select(y =>
                      {
                          var positionExecutor = GetExecutorAgentIdByPositionId(context, y);
                          return new InternalDocumentEventAccess
                          {
                              ClientId = ev.ClientId,
                              EntityTypeId = ev.EntityTypeId,
                              DocumentId = ev.DocumentId,
                              EventId = ev.Id,
                              IsActive = true,
                              AccessType = x.AccessType,
                              PositionId = y,
                              AgentId = positionExecutor.ExecutorAgentId,
                              PositionExecutorTypeId = positionExecutor.ExecutorTypeId,
                              ReadDate = (x.AccessType == EnumEventAccessTypes.Source) ? DateTime.UtcNow : (DateTime?)null,
                              SendDate = (x.AccessType == EnumEventAccessTypes.Source) ? DateTime.UtcNow : (DateTime?)null,
                              ReadAgentId = (x.AccessType == EnumEventAccessTypes.Source) ? context.CurrentAgentId : (int?)null,
                              IsNew = !docAccesses.Select(z => z.PositionId).Contains(y),
                          };
                      }));
                      accesses.AddRange(agents.Select(y => new InternalDocumentEventAccess
                      {
                          ClientId = ev.ClientId,
                          EntityTypeId = ev.EntityTypeId,
                          DocumentId = ev.DocumentId,
                          EventId = ev.Id,
                          IsActive = true,
                          AccessType = x.AccessType,
                          AgentId = y,
                          ReadDate = (x.AccessType == EnumEventAccessTypes.Source) ? DateTime.UtcNow : (DateTime?)null,
                          SendDate = (x.AccessType == EnumEventAccessTypes.Source) ? DateTime.UtcNow : (DateTime?)null,
                          ReadAgentId = (x.AccessType == EnumEventAccessTypes.Source) ? context.CurrentAgentId : (int?)null,
                          IsNew = !docAccesses.Select(z => z.AgentId).Contains(y),
                      }));
                  });
                SetLastChange(context, accesses);
                ev.Accesses = accesses;
            }
        }
        public static void SetLastChange(IContext context, IEnumerable<LastChangeInfo> documents)
        {
            documents.ToList().ForEach(x => SetLastChange(context, x));
        }

        public static InternalDocumentAccess GetNewDocumentAccess(IContext context, int entityTypeId, int? documentId = null, EnumDocumentAccesses? accessLevel = null, int? positionId = null)
        {
            var res = new InternalDocumentAccess
            {
                DocumentId = documentId ?? 0,
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                AccessLevel = accessLevel ?? EnumDocumentAccesses.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                PositionId = positionId ?? context.CurrentPositionId
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentAccess> GetNewDocumentAccesses(IContext context, int entityTypeId, int? documentId = null, EnumDocumentAccesses? accessLevel = null, int? positionId = null)
        {
            return new List<InternalDocumentAccess>
            {
                GetNewDocumentAccess(context,entityTypeId,documentId,accessLevel,positionId),
            };
        }

        public static InternalDocumentEvent GetNewDocumentEvent(IContext context, InternalDocumentSendList model, EnumEventTypes? eventType = null)
        {
            var sourcePositionExecutor = GetExecutorAgentIdByPositionId(context, model.SourcePositionId);
            var targetPositionExecutor = GetExecutorAgentIdByPositionId(context, model.TargetPositionId);
            var res = new InternalDocumentEvent
            {
                DocumentId = model.DocumentId != 0 ? model.DocumentId : 0,
                ClientId = context.CurrentClientId,
                EntityTypeId = model.EntityTypeId,
                EventType = eventType ?? (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), model.SendType.ToString()),
                TaskId = model.TaskId,
                IsAvailableWithinTask = model.TaskId.HasValue ? model.IsAvailableWithinTask : false,
                Description = model.Description,
                SourceAgentId = model.SourceAgentId,
                SourcePositionId = model.SourcePositionId,
                SourcePositionExecutorAgentId = sourcePositionExecutor?.ExecutorAgentId,
                SourcePositionExecutorTypeId = sourcePositionExecutor?.ExecutorTypeId,
                TargetPositionId = model.TargetPositionId,
                TargetPositionExecutorAgentId = targetPositionExecutor?.ExecutorAgentId,
                TargetPositionExecutorTypeId = targetPositionExecutor?.ExecutorTypeId,
                TargetAgentId = model.TargetAgentId,
                Date = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvents(IContext context, InternalDocumentSendList model, EnumEventTypes? eventType = null)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentEvent(context, model,eventType),
            };
        }

        public static InternalDocumentEvent GetNewDocumentEvent(IContext context, int entityTypeId, int? documentId, EnumEventTypes eventType, DateTime? eventDate = null, string description = null, string addDescription = null, int? taskId = null, bool isAvailableWithinTask = false, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null, 
                                                                List<EventAccessGroup> accessGroups = null, bool isVeryfyDocumentAccess = false)
        {
            var sourcePositionExecutor = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId);
            var targetPositionExecutor = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId);
            var res = new InternalDocumentEvent
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = documentId ?? 0,
                EventType = eventType,
                TaskId = taskId,
                IsAvailableWithinTask = taskId.HasValue ? isAvailableWithinTask : false,
                Description = description,
                AddDescription = addDescription,
                SourceAgentId = sourceAgentId ?? context.CurrentAgentId,
                SourcePositionId = sourcePositionId ?? context.CurrentPositionId,
                SourcePositionExecutorAgentId = sourcePositionExecutor?.ExecutorAgentId,
                SourcePositionExecutorTypeId = sourcePositionExecutor?.ExecutorTypeId,
                TargetPositionId = targetPositionId ?? context.CurrentPositionId,
                TargetPositionExecutorAgentId = targetPositionExecutor?.ExecutorAgentId,
                TargetPositionExecutorTypeId = targetPositionExecutor?.ExecutorTypeId,
                TargetAgentId = targetAgentId,
                Date = eventDate ?? DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
            };
            SetLastChange(context, res);
            SetEventAccessess(context, res, accessGroups, isVeryfyDocumentAccess);
            return res;
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentEvents(IContext context, int entityTypeId, int? documentId, EnumEventTypes eventType, DateTime? eventDate = null, string description = null, string addDescription = null, int? taskId = null, bool isAvailableWithinTask = false, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null, List<EventAccessGroup> accessGroups = null)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentEvent(context,entityTypeId, documentId,eventType,eventDate,description,addDescription,taskId,isAvailableWithinTask,targetPositionId,targetAgentId,sourcePositionId,sourceAgentId,accessGroups),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, int entityTypeId, int documentId, InternalDocumentEvent onEvent = null)
        {
            var res = new InternalDocumentWait
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = documentId,
                OnEvent = onEvent,
            };
            SetLastChange(context, res);
            return res;
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, int entityTypeId, ControlOn controlOnModel, EnumEventTypes? eventType = null, int? taskId = null, bool isAvailableWithinTask = false)
        {
            var res = new InternalDocumentWait
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = controlOnModel.DocumentId,
                DueDate = controlOnModel.DueDate,
                AttentionDate = controlOnModel.AttentionDate,
                OnEvent = eventType == null ? null : GetNewDocumentEvent(context, entityTypeId, controlOnModel.DocumentId, eventType.Value, controlOnModel.EventDate, controlOnModel.Description, null, taskId, isAvailableWithinTask)
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWaits(IContext context, int entityTypeId, ControlOn controlOnModel, EnumEventTypes? eventType = null, int? taskId = null, bool isAvailableWithinTask = false)
        {
            return new List<InternalDocumentWait>
            {
                GetNewDocumentWait(context,entityTypeId,controlOnModel,eventType, taskId, isAvailableWithinTask),
            };
        }

        public static InternalDocumentWait GetNewDocumentWait(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes eventType, EnumEventCorrespondentType? eventCorrespondentType = null, bool? isTakeMainDueDate = null)
        {
            var res = new InternalDocumentWait
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = sendListModel.EntityTypeId,
                DocumentId = sendListModel.DocumentId,
                DueDate = eventType == EnumEventTypes.ControlOn && !(isTakeMainDueDate ?? false)
                            ? new[] {   sendListModel.SelfDueDate,
                                        (!sendListModel.SelfDueDay.HasValue || sendListModel.SelfDueDay.Value < 0) ? null : (DateTime?)DateTime.UtcNow.AddDays(sendListModel.SelfDueDay.Value)
                                    }.Max()
                            : new[] {   sendListModel.DueDate,
                                        (!sendListModel.DueDay.HasValue || sendListModel.DueDay.Value < 0) ? null : (DateTime?)DateTime.UtcNow.AddDays(sendListModel.DueDay.Value)
                                    }.Max(),
                AttentionDate = eventType == EnumEventTypes.ControlOn && !(isTakeMainDueDate ?? false)
                            ? new[] {   sendListModel.SelfAttentionDate,
                                        (!sendListModel.SelfAttentionDay.HasValue || sendListModel.SelfAttentionDay.Value < 0) ? null : (DateTime?)DateTime.UtcNow.AddDays(sendListModel.SelfAttentionDay.Value)
                                    }.Max()
                            : null,
                OnEvent = //eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.EntityTypeId, sendListModel.DocumentId, eventType, null,
                                ((eventType == EnumEventTypes.ControlOn && !string.IsNullOrEmpty(sendListModel.SelfDescription)) ? sendListModel.SelfDescription : sendListModel.Description),
                                null, sendListModel.TaskId, sendListModel.IsAvailableWithinTask,
                                eventCorrespondentType == EnumEventCorrespondentType.FromSourceToSource ? sendListModel.SourcePositionId : sendListModel.TargetPositionId,
                                null,
                                eventCorrespondentType == EnumEventCorrespondentType.FromTargetToTarget ? sendListModel.TargetPositionId : sendListModel.SourcePositionId,
                                sendListModel.SourceAgentId
                            )
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentWait> GetNewDocumentWaits(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes eventType, EnumEventCorrespondentType? eventCorrespondentType = null, bool? isTakeMainDueDate = null)
        {
            return new List<InternalDocumentWait>
            {
                GetNewDocumentWait(context,sendListModel,eventType,eventCorrespondentType,isTakeMainDueDate)
            };
        }

        public static InternalDocumentSubscription GetNewDocumentSubscription(IContext context, InternalDocumentSendList sendListModel, EnumEventTypes? eventType = null)
        {
            var res = new InternalDocumentSubscription
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = sendListModel.EntityTypeId,
                DocumentId = sendListModel.DocumentId,
                SubscriptionStates = EnumSubscriptionStates.No,
                SendEvent = eventType == null ? null :
                            GetNewDocumentEvent
                            (
                                context, sendListModel.EntityTypeId, sendListModel.DocumentId, eventType.Value, null, sendListModel.Description, null, sendListModel.TaskId, sendListModel.IsAvailableWithinTask,
                                sendListModel.TargetPositionId,
                                null,
                                sendListModel.SourcePositionId,
                                sendListModel.SourceAgentId
                            )
            };
            SetLastChange(context, res);
            return res;
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
                var positionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(context, positionId ?? context.CurrentPositionId);
                if (!positionExecutor?.ExecutorAgentId.HasValue ?? true)
                {
                    throw new ExecutorAgentForPositionIsNotDefined();
                }
                document.Tasks = new List<InternalDocumentTask>
                {
                    new InternalDocumentTask
                    {
                        DocumentId = document.Id,
                        ClientId = document.ClientId,
                        EntityTypeId = document.EntityTypeId,
                        PositionId = context.CurrentPositionId,
                        PositionExecutorAgentId = positionExecutor.ExecutorAgentId.Value,
                        PositionExecutorTypeId = positionExecutor.ExecutorTypeId,
                        AgentId = context.CurrentAgentId,
                        Name = task,
                        Description = description,
                    }
                };
                SetLastChange(context, document.Tasks);
            }
            return taskId;
        }

        public static InternalDocumentSendList GetNewDocumentSendList(IContext context, int entityTypeId, BaseModifyDocumentSendList model, int? taskId = null)
        {
            var res = new InternalDocumentSendList
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = model.DocumentId,
                Stage = model.Stage,
                StageType = model.StageType,
                SendType = model.SendType,
                SourcePositionId = context.CurrentPositionId,
                SourcePositionExecutorAgentId = null,
                SourcePositionExecutorTypeId = null,
                SourceAgentId = context.CurrentAgentId,
                TargetPositionId = model.TargetPositionId,
                TargetPositionExecutorAgentId = null,
                TargetPositionExecutorTypeId = null,
                TargetAgentId = model.TargetAgentId,
                TaskId = taskId,
                Description = model.Description,
                DueDate = model.DueDate,
                DueDay = model.DueDay,
                AccessLevel = model.AccessLevel,
                IsInitial = model.IsInitial,
                IsAvailableWithinTask = model.IsAvailableWithinTask,
                IsWorkGroup = model.IsWorkGroup,
                IsAddControl = model.IsAddControl,
                SelfDueDate = model.SelfDueDate,
                SelfDueDay = model.SelfDueDay,
                SelfDescription = model.SelfDescription,
                SelfAttentionDate = model.SelfAttentionDate,
                SelfAttentionDay = model.SelfAttentionDay,
            };
            SetLastChange(context, res);
            return res;
        }

        public static InternalDocumentRestrictedSendList GetNewDocumentRestrictedSendList(IContext context, int entityTypeId, ModifyDocumentRestrictedSendList model)
        {
            var res = new InternalDocumentRestrictedSendList
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                AccessLevel = model.AccessLevel,
                DocumentId = model.DocumentId,
                PositionId = model.PositionId,
            };
            SetLastChange(context, res);
            return res;
        }

        public static InternalDocumentPaper GetNewDocumentPaper(IContext context, int entityTypeId, BaseModifyDocumentPaper model, int orderNumber)
        {
            var res = new InternalDocumentPaper
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = model.DocumentId,
                Name = model.Name,
                Description = model.Description,
                IsMain = model.IsMain,
                IsOriginal = model.IsOriginal,
                IsCopy = model.IsCopy,
                PageQuantity = model.PageQuantity,
                OrderNumber = orderNumber,
                Events = GetNewDocumentPaperEvents(context, (int)EnumEntytiTypes.Document, model.DocumentId, null, EnumEventTypes.AddNewPaper),
                IsInWork = true,
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentPaper> GetNewDocumentPapers(IContext context, int entityTypeId, BaseModifyDocumentPaper model, int maxOrderNumber)
        {
            var res = new List<InternalDocumentPaper>();
            for (int i = 1, l = model.PaperQuantity; i <= l; i++)
            {
                res.Add(GetNewDocumentPaper(context, entityTypeId, model, maxOrderNumber + i));
            }
            return res;
        }

        public static InternalTemplateDocumentPaper GetNewTemplateDocumentPaper(IContext context, AddTemplateDocumentPaper model, int orderNumber)
        {
            var res = new InternalTemplateDocumentPaper
            {
                DocumentId = model.DocumentId,
                Name = model.Name,
                Description = model.Description,
                IsMain = model.IsMain,
                IsOriginal = model.IsOriginal,
                IsCopy = model.IsCopy,
                PageQuantity = model.PageQuantity,
                OrderNumber = orderNumber,
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalTemplateDocumentPaper> GetNewTemplateDocumentPapers(IContext context, AddTemplateDocumentPaper model, int maxOrderNumber)
        {
            var res = new List<InternalTemplateDocumentPaper>();
            for (int i = 1, l = model.PaperQuantity; i <= l; i++)
            {
                res.Add(GetNewTemplateDocumentPaper(context, model, maxOrderNumber + i));
            }
            return res;
        }

        public static InternalDocumentEvent GetNewDocumentPaperEvent(IContext context, int entityTypeId, int documentId, int? paperId, EnumEventTypes eventType, string description = null, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null, bool IsMarkPlan = true, bool IsMarkRecieve = true)
        {
            var sourcePositionExecutor = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId);
            var targetPositionExecutor = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId);
            var res = new InternalDocumentEvent
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = entityTypeId,
                DocumentId = documentId,
                PaperId = paperId ?? 0,
                EventType = eventType,
                Date = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                Description = description,
                SourceAgentId = sourceAgentId ?? context.CurrentAgentId,
                SourcePositionId = sourcePositionId ?? context.CurrentPositionId,
                SourcePositionExecutorAgentId = IsMarkPlan ? sourcePositionExecutor.ExecutorAgentId : null,
                SourcePositionExecutorTypeId = IsMarkPlan ? sourcePositionExecutor.ExecutorTypeId : null,
                TargetPositionId = targetPositionId ?? context.CurrentPositionId,
                TargetPositionExecutorAgentId = IsMarkPlan ? targetPositionExecutor.ExecutorAgentId : null,
                TargetPositionExecutorTypeId = IsMarkPlan ? targetPositionExecutor.ExecutorTypeId : null,
                TargetAgentId = targetAgentId,
                PaperPlanAgentId = IsMarkPlan ? (int?)(sourceAgentId ?? context.CurrentAgentId) : null,
                PaperPlanDate = IsMarkPlan ? (DateTime?)DateTime.UtcNow : null,
                PaperSendAgentId = IsMarkRecieve ? (int?)context.CurrentAgentId : null,
                PaperSendDate = IsMarkRecieve ? (DateTime?)DateTime.UtcNow : null,
                PaperRecieveAgentId = IsMarkRecieve ? (int?)context.CurrentAgentId : null,
                PaperRecieveDate = IsMarkRecieve ? (DateTime?)DateTime.UtcNow : null,
            };
            SetLastChange(context, res);
            return res;
        }

        public static IEnumerable<InternalDocumentEvent> GetNewDocumentPaperEvents(IContext context, int entityTypeId, int documentId, int? paperId, EnumEventTypes eventType, string description = null, int? targetPositionId = null, int? targetAgentId = null, int? sourcePositionId = null, int? sourceAgentId = null, bool IsMarkPlan = true, bool IsMarkRecieve = true)
        {
            return new List<InternalDocumentEvent>
            {
                GetNewDocumentPaperEvent(context, entityTypeId, documentId, paperId, eventType, description, targetPositionId, targetAgentId, sourcePositionId, sourceAgentId, IsMarkPlan, IsMarkRecieve)
            };
        }

        public static void PlanDocumentPaperFromSendList(IContext context, InternalDocument document, InternalDocumentSendList model)
        {
            var operationDb = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();
            document.Papers = operationDb.PlanDocumentPaperFromSendListPrepare(context, model.Id);
            if (document.Papers?.Any() ?? false)
            {
                if (
                    document.Papers.Any(
                        x =>
                            //x.LastPaperEvent.SourcePositionId != model.SourcePositionId ||
                            x.LastPaperEvent.TargetPositionId != model.SourcePositionId ||
                            //x.LastPaperEvent.SourceAgentId != model.SourceAgentId ||
                            //x.LastPaperEvent.TargetAgentId != model.TargetAgentId ||
                            x.LastPaperEvent.PaperRecieveDate == null))

                {
                    throw new CouldNotPerformOperationWithPaper();
                }
                foreach (var paper in document.Papers.ToList())
                {
                    //paper.LastPaperEventId = null;
                    paper.LastPaperEvent = CommonDocumentUtilities.GetNewDocumentPaperEvent(context, document.EntityTypeId, document.Id, paper.Id,
                        EnumEventTypes.MoveDocumentPaper, null, model.TargetPositionId, model.TargetAgentId, model.SourcePositionId, model.SourceAgentId, true, false);
                    CommonDocumentUtilities.SetLastChange(context, paper);
                    paper.LastPaperEventId = null;
                    paper.LastPaperEvent.Id = paper.NextPaperEventId.Value;

                }
            }
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

                //if (docTemplate.DocumentSubjectId.HasValue)
                //{
                //    uiElements?.Where(x => x.Code.Equals("DocumentSubject", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
                //    doc.DocumentSubjectId = docTemplate.DocumentSubjectId;
                //}

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

                if (docTemplate.Properties?.Any() ?? false)
                {
                    var docTempProp = docTemplate.Properties.Where(x => x.Value != null).ToList();
                    if (docTempProp.Any())
                    {
                        if (doc.Properties == null)
                            doc.Properties = new List<FrontPropertyValue>();

                        var propNotSet = docTempProp.GroupJoin(doc.Properties,
                                            dp => new { PropertyCode = dp.PropertyCode },
                                            dtp => new { PropertyCode = dtp.PropertyCode },
                                            (dtp, dps) => new
                                            {
                                                dtp = dtp,
                                                dp = dps.FirstOrDefault()
                                            })
                                        .Where(x => x.dp == null || x.dtp.Value != x.dp.Value)
                                        .ToList();

                        if (propNotSet.Where(x => x.dp != null).Any())
                        {
                            doc.Properties = doc.Properties.GroupJoin(
                                propNotSet.Where(x => x.dp != null).ToList(),
                                dp => new { PropertyCode = dp.PropertyCode },
                                dtp => new { PropertyCode = dtp.dp.PropertyCode },
                                (dp, dtps) =>
                                {
                                    if (dtps.Any())
                                    {
                                        var dtp = dtps.First();
                                        dp.Value = dtp.dtp.Value;
                                        dp.DisplayValue = dtp.dtp.DisplayValue;
                                    }
                                    return dp;
                                }).ToList();
                        }

                        if (propNotSet.Where(x => x.dp == null).Any())
                        {
                            var _sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
                            var propLinks = _sysDb.GetPropertyValuesToDocumentFromTemplateDocument(ctx, new FilterPropertyLink { PropertyLinkId = propNotSet.Where(x => x.dp == null).Select(x => x.dtp.PropertyLinkId).ToList() });

                            var props = doc.Properties.ToList();

                            props.AddRange(
                                propNotSet.Where(x => x.dp == null)
                                            .Join(propLinks,
                                                dtp => new { PropertyCode = dtp.dtp.PropertyCode },
                                                pl => new { PropertyCode = pl.PropertyCode },
                                                (dtp, pl) => { dtp.dtp.PropertyLinkId = pl.PropertyLinkId; return dtp.dtp; }).ToList()
                                                );

                            doc.Properties = props;
                        }

                        uiElements?.Where(x => docTempProp.Select(y => y.PropertyCode).Contains(x.Code)).ToList().ForEach(x => x.IsReadOnly = true);
                    }
                }

            }
            return uiElements;
        }

        public static IEnumerable<BaseSystemUIElement> VerifyTemplateDocument(IContext ctx, FrontTemplateDocument templateDoc, IEnumerable<BaseSystemUIElement> uiElements)
        {
            if (templateDoc.DocumentDirection != EnumDocumentDirections.Incoming)
            {
                if (uiElements != null)
                {
                    var senderElements = new List<string> { "SenderAgent", "SenderAgentPerson", "Addressee" };
                    uiElements = uiElements.Where(x => !senderElements.Contains(x.Code)).ToList();
                }
                templateDoc.SenderAgentId = null;
                templateDoc.SenderAgentPersonId = null;
                templateDoc.Addressee = null;
            }

            if ((templateDoc.DocumentDirection == EnumDocumentDirections.Incoming) && (uiElements == null)
                    &&
                    (
                        templateDoc.SenderAgentId == null ||
                        templateDoc.SenderAgentPersonId == null ||
                        string.IsNullOrEmpty(templateDoc.Addressee)
                    )
                )
            {
                throw new NeedInformationAboutCorrespondent();
            }
            if (templateDoc.IsUsedInDocument ?? false)
            {
                uiElements?.Where(x => x.Code.Equals("DocumentDirection", StringComparison.OrdinalIgnoreCase) ||
                                        x.Code.Equals("DocumentType", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => x.IsReadOnly = true);
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
            //TODO Малинин. Надо вернуть, когда начнутся реальные тесты
            //if (doc.RestrictedSendLists?.Count() > 0
            //    && doc.SendLists.Where(sl => sl.TargetPositionId.HasValue).GroupJoin(doc.RestrictedSendLists
            //        , sl => sl.TargetPositionId
            //        , rsl => rsl.PositionId
            //        , (sl, rsls) => new { sl, rsls }).Any(x => x.rsls.Count() == 0))
            //{
            //    throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            //}

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
            if (sendLists.Any())
            {
                var stages = Enumerable.Range(0, sendLists.Max(x => x.Stage ?? 0) + 1)
                    .GroupJoin(sendLists, s => s, sl => sl.Stage
                    , (s, sls) => new { s, sls = sls.Where(x => x.Id > 0) })
                    .Select(x => new FrontDocumentSendListStage
                    {
                        Stage = x.s,
                        SendLists = x.sls.OrderBy(y => y.Id).ToList(),
                        IsClose = x.sls.All(y => y.CloseEventId.HasValue),
                        IsOpen = false
                    }).ToList();

                foreach (var stage in stages.OrderBy(x => x.Stage))
                {
                    stage.IsOpen = true;
                    if (!stage.IsClose)
                    {
                        break;
                    }
                }

                return stages;
            }
            return new List<FrontDocumentSendListStage>();
        }

        public static InternalTemplateAttachedFile GetNewTemplateAttachedFile(IContext context, InternalTemplateAttachedFile src, int? newOrderNumber = null)
        {
            var res = new InternalTemplateAttachedFile
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = src.EntityTypeId,
                DocumentId = src.DocumentId,
                Extension = src.Extension,
                Name = src.Name,
                FileType = src.FileType,
                FileSize = src.FileSize,
                Type = src.Type,
                FileContent = src.FileContent,
                OrderInDocument = newOrderNumber ?? src.OrderInDocument,
                Hash = src.Hash,
                Description = src.Description,
                PdfCreated = src.PdfCreated,
                LastPdfAccess = src.LastPdfAccess
            };
            SetLastChange(context, res);
            return res;
        }

        public static InternalDocumentAttachedFile GetNewDocumentAttachedFile(IContext context, InternalDocumentAttachedFile src, int? newOrderNumber = null, int? newVersion = null)
        {
            var res = new InternalDocumentAttachedFile
            {
                ClientId = context.CurrentClientId,
                EntityTypeId = src.EntityTypeId,
                Extension = src.Extension,
                Name = src.Name,
                FileType = src.FileType,
                FileSize = src.FileSize,
                Type = src.Type,
                Description = src.Description,
                IsMainVersion = true,
                IsWorkedOut = src.IsWorkedOut,
                IsDeleted = src.IsDeleted,
                FileContent = src.FileContent,
                Hash = src.Hash,
                OrderInDocument = newOrderNumber ?? src.OrderInDocument,
                Date = DateTime.UtcNow,
                Version = newVersion ?? 1,
                WasChangedExternal = false,
                ExecutorPositionId = src.ExecutorPositionId,
                ExecutorPositionExecutorAgentId = src.ExecutorPositionExecutorAgentId,
                ExecutorPositionExecutorTypeId = src.ExecutorPositionExecutorTypeId,
                PdfCreated = src.PdfCreated,
                LastPdfAccess = src.LastPdfAccess
            };
            SetLastChange(context, res);
            return res;
        }

        public static void CorrectModel(IContext context, AddTemplateDocumentSendList model)
        {
            if (model.SendType == EnumSendTypes.SendForInformation || model.SendType == EnumSendTypes.SendForConsideration)
            {
                model.DueDay = null;
            }
            if (!model.IsAddControl)
            {
                model.SelfAttentionDay = null;
                model.SelfDueDay = null;
                model.SelfDescription = null;
            }
            if (model.TaskId == null)
            {
                model.IsAvailableWithinTask = false;
            }
            if (model.SendType != EnumSendTypes.SendForExecution || model.TaskId == null)
            {
                model.IsWorkGroup = false;
            }
            if (model.SendType == EnumSendTypes.SendForInformationExternal)
            {
                model.TargetPositionId = null;
            }
            else
            {
                model.TargetAgentId = null;
            }

        }

        public static InternalDictionaryPositionExecutorForDocument GetExecutorAgentIdByPositionId(IContext context, int? positionId)
        {
            if (positionId.HasValue)
            {
                var dict = DmsResolver.Current.Get<DictionariesDbProcess>();
                return dict.GetExecutorAgentIdByPositionId(context, positionId.Value);
            }
            else
                return null;
        }

        public static List<int> GetSourcePositionsForSubordinationVeification(IContext context, InternalDocumentSendList sendList, InternalDocument document, bool isTakeSendList = false)
        {
            var res = new List<int> { sendList.SourcePositionId };
            if (document.Events?.Any() ?? false)
            {
                res.AddRange(document.Events.Select(x => x.SourcePositionId.Value));
            }
            if (document.Waits?.Any() ?? false)
            {
                res.AddRange(document.Waits.Select(x => x.OnEvent.SourcePositionId.Value));
            }
            if (document.Subscriptions?.Any() ?? false)
            {
                res.AddRange(document.Subscriptions.Select(x => x.DoneEvent.SourcePositionId.Value));
            }
            if (isTakeSendList && (document.SendLists?.Any() ?? false))
            {
                res.AddRange(document.SendLists
                    .Where(x => x.TargetPositionId.HasValue && x.Stage < sendList.Stage && (x.SendType == EnumSendTypes.SendForSigning || x.SendType == EnumSendTypes.SendForVisaing || x.SendType == EnumSendTypes.SendForАgreement || x.SendType == EnumSendTypes.SendForАpproval))
                    .Select(x => x.TargetPositionId.Value));
            }
            return res;
        }

        #region RegistrationNumber
        public static void FormationRegistrationNumberByFormula(InternalDocument doc, InternalDocumnRegistration model)
        {
            doc.RegistrationJournalPrefixFormula = FormationRegistrationNumberByFormula(doc.RegistrationJournalPrefixFormula, doc, model);
            doc.RegistrationJournalSuffixFormula = FormationRegistrationNumberByFormula(doc.RegistrationJournalSuffixFormula, doc, model);
        }

        private static string FormationRegistrationNumberByFormula(string formula, InternalDocument doc, InternalDocumnRegistration model)
        {
            if (string.IsNullOrEmpty(formula)) return formula;
            string pattern = "@/(.*?)/@";
            string patternFilterSymbol = "c", patternFormulaSymbol = "v", patternLengthSymbol = "l", patternFormatSymbol = "f";
            string patternFilter = GetPatternFilter(patternFilterSymbol), patternFormula = GetPatternFilter(patternFormulaSymbol), patternLength = GetPatternFilter(patternLengthSymbol), patternFormat = GetPatternFilter(patternFormatSymbol);
            string res = string.Copy(formula);
            foreach (Match mFormula in Regex.Matches(formula, pattern))
            {
                var oldValue = mFormula.Value;
                var newValue = string.Empty;
                var mFilters = Regex.Matches(oldValue, patternFilter);
                var isContainsInFilter = mFilters.Count == 0;
                if (!isContainsInFilter)
                {
                    foreach (Match mFilter in mFilters)
                    {
                        var filter = GetPatternFilterSymbolReplace(mFilter.Value, patternFilterSymbol);
                        var template = CommonDocumentUtilities.GetFilterTemplateByDocument(doc).ToArray();
                        if (CommonSystemUtilities.IsContainsInFilter(filter, template))
                        {
                            isContainsInFilter = true;
                            break;
                        }
                    }
                }

                if (isContainsInFilter)
                {
                    var mLengths = Regex.Matches(mFormula.Value, patternLength);
                    int length = 0;
                    foreach (Match mLength in mLengths)
                    {
                        if (int.TryParse(GetPatternFilterSymbolReplace(mLength.Value, patternLengthSymbol), out length))
                            break;
                    }

                    var mFormats = Regex.Matches(mFormula.Value, patternFormat);
                    string format = string.Empty;
                    foreach (Match mFormat in mFormats)
                    {
                        format = GetPatternFilterSymbolReplace(mFormat.Value, patternFormatSymbol);
                        break;
                    }

                    var mFormulaValues = Regex.Matches(mFormula.Value, patternFormula);
                    foreach (Match mFormulaValue in mFormulaValues)
                    {
                        var formulaValue = (EnumSystemFormulas)Enum.Parse(typeof(EnumSystemFormulas), GetPatternFilterSymbolReplace(mFormulaValue.Value, patternFormulaSymbol));

                        switch (formulaValue)
                        {
                            case EnumSystemFormulas.RegistrationJournalId:
                                newValue = model.RegistrationJournalId.GetValueOrDefault().ToString("D" + length);
                                break;
                            case EnumSystemFormulas.RegistrationJournalIndex:
                                newValue = model.RegistrationJournalIndex;
                                break;
                            case EnumSystemFormulas.InitiativeRegistrationFullNumber:
                                newValue = model.InitiativeRegistrationFullNumber;
                                break;
                            case EnumSystemFormulas.InitiativeRegistrationNumberPrefix:
                                newValue = model.InitiativeRegistrationNumberPrefix;
                                break;
                            case EnumSystemFormulas.InitiativeRegistrationNumberSuffix:
                                newValue = model.InitiativeRegistrationNumberSuffix;
                                break;
                            case EnumSystemFormulas.InitiativeRegistrationNumber:
                                if (model.InitiativeRegistrationNumber.HasValue)
                                    newValue = model.InitiativeRegistrationNumber.Value.ToString("D" + length);
                                break;
                            case EnumSystemFormulas.Date:
                                if (string.IsNullOrEmpty(format))
                                    format = "YYYY";
                                newValue = model.RegistrationDate.ToString(format);
                                break;
                            case EnumSystemFormulas.ExecutorPositionDepartmentCode:
                                newValue = model.ExecutorPositionDepartmentCode;
                                break;
                            case EnumSystemFormulas.SubscriptionsPositionDepartmentCode:
                                newValue = model.SubscriptionsPositionDepartmentCode;
                                break;
                            case EnumSystemFormulas.RegistrationJournalDepartmentCode:
                                newValue = model.RegistrationJournalDepartmentCode;
                                break;
                            case EnumSystemFormulas.CurrentPositionDepartmentCode:
                                newValue = model.CurrentPositionDepartmentCode;
                                break;
                            case EnumSystemFormulas.InitiativeRegistrationSenderNumber:
                                newValue = model.InitiativeRegistrationSenderNumber;
                                break;
                            case EnumSystemFormulas.DocumentSendListLastAgentExternalFirstSymbolName:
                                newValue = model.DocumentSendListLastAgentExternalFirstSymbolName;
                                break;
                            case EnumSystemFormulas.OrdinalNumberDocumentLinkForCorrespondent:
                                newValue = model.OrdinalNumberDocumentLinkForCorrespondent.ToString("D" + length);
                                break;
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(newValue)) newValue = string.Empty;
                res = res.Replace(oldValue, newValue);
            }
            return res;
        }

        private static string GetPatternFilterSymbolReplace(string input, string symbol)
        {
            if (string.IsNullOrEmpty(input)) input = string.Empty;
            return input.Replace("{" + symbol + "/", string.Empty).Replace("/" + symbol + "}", "");
        }

        private static string GetPatternFilter(string symbol)
        {
            return "{" + symbol + "/(.*?)/" + symbol + "}";
        }

        /// <summary>
        /// Формирует список значений документа для фильтрации в динамических свойствах, формирование регистрационого номера по формуле
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetFilterTemplateByDocument(InternalDocument doc)
        {
            var res = new List<string>();
            if (doc.DocumentTypeId > 0)
                res.Add($"{nameof(doc.DocumentTypeId)}={doc.DocumentTypeId}");
            if (doc.DocumentDirection > 0)
                res.Add($"{nameof(doc.DocumentDirection)}={doc.DocumentDirection}");
            //if (doc.DocumentSubjectId.HasValue && doc.DocumentSubjectId > 0)
            //    res.Add($"{nameof(doc.DocumentSubjectId)}={doc.DocumentSubjectId}");
            return res;
        }

        public static IEnumerable<string> GetFilterTemplateByDocument(FrontDocument doc)
        {
            var res = new List<string>();
            if (doc.DocumentTypeId.HasValue && doc.DocumentTypeId > 0)
                res.Add($"{nameof(doc.DocumentTypeId)}={doc.DocumentTypeId}");
            if (doc.DocumentDirection.HasValue && doc.DocumentDirection > 0)
                res.Add($"{nameof(doc.DocumentDirection)}={doc.DocumentDirection}");
            //if (doc.DocumentSubjectId.HasValue && doc.DocumentSubjectId > 0)
            //    res.Add($"{nameof(doc.DocumentSubjectId)}={doc.DocumentSubjectId}");
            return res;
        }

        public static IEnumerable<string> GetFilterTemplateByTemplateDocument(FrontTemplateDocument templateDoc)
        {
            var res = new List<string>();
            if (templateDoc.DocumentTypeId > 0)
                res.Add($"{nameof(templateDoc.DocumentTypeId)}={templateDoc.DocumentTypeId}");
            if (templateDoc.DocumentDirection > 0)
                res.Add($"{nameof(templateDoc.DocumentDirection)}={templateDoc.DocumentDirection}");
            //if (templateDoc.DocumentSubjectId.HasValue && templateDoc.DocumentSubjectId > 0)
            //    res.Add($"{nameof(templateDoc.DocumentSubjectId)}={templateDoc.DocumentSubjectId}");
            return res;
        }

        public static IEnumerable<string> GetFilterTemplateByTemplateDocument(InternalTemplateDocument templateDoc)
        {
            var res = new List<string>();
            if (templateDoc.DocumentTypeId > 0)
                res.Add($"{nameof(templateDoc.DocumentTypeId)}={templateDoc.DocumentTypeId}");
            if (templateDoc.DocumentDirection > 0)
                res.Add($"{nameof(templateDoc.DocumentDirection)}={templateDoc.DocumentDirection}");
            //if (templateDoc.DocumentSubjectId.HasValue && templateDoc.DocumentSubjectId > 0)
            //    res.Add($"{nameof(templateDoc.DocumentSubjectId)}={templateDoc.DocumentSubjectId}");
            return res;
        }
        #endregion RegistrationNumber

        #region Property
        public static InternalPropertyValue GetNewPropertyValue(ModifyPropertyValue model)
        {
            var item = new InternalPropertyValue
            {
                PropertyLinkId = model.PropertyLinkId,
                ValueString = model.Value
            };
            double tmpNumeric;
            if (double.TryParse(model.Value, out tmpNumeric))
            {
                item.ValueString = null;
                item.ValueNumeric = tmpNumeric;
            }
            DateTime tmpDate;
            if (DateTime.TryParse(model.Value, out tmpDate))
            {
                item.ValueString = null;
                item.ValueDate = tmpDate;
            }
            return item;
        }

        public static IEnumerable<InternalPropertyValue> GetNewPropertyValues(IEnumerable<ModifyPropertyValue> model)
        {
            return model.Select(GetNewPropertyValue);
        }

        public static InternalPropertyValue GetNewPropertyValue(FrontPropertyValue model)
        {
            var value = model.Value != null ? model.Value.ToString() : null;
            var item = new InternalPropertyValue
            {
                PropertyLinkId = model.PropertyLinkId,
                ValueString = value
            };
            double tmpNumeric;
            if (double.TryParse(value, out tmpNumeric))
            {
                item.ValueString = null;
                item.ValueNumeric = tmpNumeric;
            }
            DateTime tmpDate;
            if (DateTime.TryParse(value, out tmpDate))
            {
                item.ValueString = null;
                item.ValueDate = tmpDate;
            }
            return item;
        }

        public static IEnumerable<InternalPropertyValue> GetNewPropertyValues(IEnumerable<FrontPropertyValue> model)
        {
            return model.Select(GetNewPropertyValue);
        }
        #endregion Property

        public static List<int> GetLinkedDocuments(int id, IEnumerable<InternalDocumentLink> link)
        {
            List<int> res = new List<int> { id };
            List<int> resInit = new List<int> { id };
            var oldCount = 1;
            var newCount = 0;
            while (oldCount != newCount && res.Count > 0)
            {
                oldCount = res.Count;
                res = link.Where(x => res.Contains(x.DocumentId) || res.Contains(x.ParentDocumentId)).Select(x => x.DocumentId)
                        .Concat(link.Where(x => res.Contains(x.DocumentId) || res.Contains(x.ParentDocumentId)).Select(x => x.ParentDocumentId))
                        .Concat(resInit)
                        .GroupBy(x => x).Select(x => x.Key).ToList();
                newCount = res.Count;
            }
            return res;
        }

        public static void ThrowError(IContext context, Exception ex, InternalDocumentSendList model)
        {
            if (ex != null)
            {
                if (model.Stage.HasValue)
                {
                    model.AddDescription = (ex is DmsExceptions) ? "##l@DmsExceptions:" + ex.GetType().Name + "@l##" : ex.Message;
                    var _operationDb = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();
                    _operationDb.ModifyDocumentSendListAddDescription(context, model);
                }
                throw ex;
            }
        }


    }
}