using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Database.DBModel.System;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        // pss Здесь представлены варианты запросов с использованием from, join...
        public static IQueryable<FrontDocumentQuery> GetFrontDocumentQuery(DmsContext dbContext, IQueryable<FrontDocumentAccess> userAccesses)
        {
            var qry = from dc in dbContext.DocumentsSet
                      //join acc in userAccesses on dc.Id equals acc.DocumentId
                      join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                      join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                      join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                      //join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                      join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                      join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                      from exAg in ea.DefaultIfEmpty()

                      join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                      from docsubj in eg.DefaultIfEmpty()

                      join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                          egg
                      from regj in egg.DefaultIfEmpty()

                      join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                      from sendAg in ag.DefaultIfEmpty()

                      join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                      from sendAp in ap.DefaultIfEmpty()

                      where userAccesses.Select(x=>x.DocumentId).Contains(dc.Id)

                      select new FrontDocumentQuery
                      {
                          Doc = dc,
                          //Acc = acc,
                          Templ = tmpl,
                          DirName = ddir.Name,
                          //AccLevName = acl.Name,
                          SubjName = docsubj.Name,
                          DocTypeName = doctp.Name,
                          RegistrationJournalName = regj.Name,
                          RegistrationJournalNumerationPrefixFormula = regj.NumerationPrefixFormula,
                          RegistrationJournalPrefixFormula = regj.PrefixFormula,
                          RegistrationJournalSuffixFormula = regj.SuffixFormula,
                          ExecutorPosName = executor.Name,
                          ExecutorAgentName = exAg.Name,
                          SenderAgentname = sendAg.Name,
                          SenderPersonName = sendAp.FullName
                      };
            return qry;
        }

        public static IQueryable<DocumentQuery> GetDocumentQuery(DmsContext dbContext)
        {
            var qry = from dc in dbContext.DocumentsSet
                      join acc in dbContext.DocumentAccessesSet on dc.Id equals acc.DocumentId
                      join tmpl in dbContext.TemplateDocumentsSet on dc.TemplateDocumentId equals tmpl.Id
                      join ddir in dbContext.DictionaryDocumentDirectionsSet on tmpl.DocumentDirectionId equals ddir.Id
                      join doctp in dbContext.DictionaryDocumentTypesSet on tmpl.DocumentTypeId equals doctp.Id
                      join acl in dbContext.AdminAccessLevelsSet on acc.AccessLevelId equals acl.Id
                      join executor in dbContext.DictionaryPositionsSet on dc.ExecutorPositionId equals executor.Id

                      join ea in dbContext.DictionaryAgentsSet on executor.ExecutorAgentId equals ea.Id into ea
                      from exAg in ea.DefaultIfEmpty()

                      join z in dbContext.DictionaryDocumentSubjectsSet on dc.DocumentSubjectId equals z.Id into eg
                      from docsubj in eg.DefaultIfEmpty()

                      join g in dbContext.DictionaryRegistrationJournalsSet on dc.RegistrationJournalId equals g.Id into
                          egg
                      from regj in egg.DefaultIfEmpty()

                      join ag in dbContext.DictionaryAgentsSet on dc.SenderAgentId equals ag.Id into ag
                      from sendAg in ag.DefaultIfEmpty()

                      join ap in dbContext.DictionaryAgentPersonsSet on dc.SenderAgentPersonId equals ap.Id into ap
                      from sendAp in ap.DefaultIfEmpty()

                      select new DocumentQuery
                      {
                          Doc = dc,
                          Acc = acc,
                          Templ = tmpl,
                          DirName = ddir.Name,
                          AccLevName = acl.Name,
                          SubjName = docsubj.Name,
                          DocTypeName = doctp.Name,
                          RegistrationJournalName = regj.Name,
                          RegistrationJournalNumerationPrefixFormula = regj.NumerationPrefixFormula,
                          RegistrationJournalPrefixFormula = regj.PrefixFormula,
                          RegistrationJournalSuffixFormula = regj.SuffixFormula,
                          ExecutorPosName = executor.Name,
                          ExecutorAgentName = exAg.Name,
                          SenderAgentname = sendAg.Name,
                          SenderPersonName = sendAp.FullName
                      };
            return qry;
        }

        public static IQueryable<FilterDocumentFileIdentity> GetDocumentFilesMaxVersion(DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var qry = dbContext.DocumentFilesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.AttachedFileId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AttachedFileId.Contains(x.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            return dbContext.DocumentFilesSet
                .GroupBy(g => new { g.DocumentId, g.OrderNumber })
                .Select(x => new FilterDocumentFileIdentity { DocumentId = x.Key.DocumentId, OrderInDocument = x.Key.OrderNumber, Version = x.Max(s => s.Version) });
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(DmsContext dbContext, FilterDocumentAttachedFile filter)
        {
            var sq = GetDocumentFilesMaxVersion(dbContext, filter);

            return
                sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocumentId, OrderNumber = sub.OrderInDocument, sub.Version },
                    fl => new { fl.DocumentId, fl.OrderNumber, fl.Version },
                    (s, f) => new { fl = f })
                    .Join(dbContext.DictionaryAgentsSet, df => df.fl.LastChangeUserId, da => da.Id,
                        (d, a) => new { d.fl, agName = a.Name })
                    .Select(x => new FrontDocumentAttachedFile
                    {
                        Id = x.fl.Id,
                        Date = x.fl.Date,
                        DocumentId = x.fl.DocumentId,
                        Extension = x.fl.Extension,
                        FileContent = x.fl.Content,
                        FileType = x.fl.FileType,
                        FileSize = x.fl.FileSize,
                        IsAdditional = x.fl.IsAdditional,
                        Hash = x.fl.Hash,
                        LastChangeDate = x.fl.LastChangeDate,
                        LastChangeUserId = x.fl.LastChangeUserId,
                        LastChangeUserName = x.agName,
                        Name = x.fl.Name,
                        OrderInDocument = x.fl.OrderNumber,
                        Version = x.fl.Version,
                        WasChangedExternal = false
                    }).ToList();
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFilesMaxVersion(dbContext, new FilterDocumentAttachedFile { DocumentId = new List<int> { documentId } });

            return
                sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocumentId, OrderNumber = sub.OrderInDocument, sub.Version },
                    fl => new { fl.DocumentId, fl.OrderNumber, fl.Version },
                    (s, f) => new { fl = f })
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.fl.Id,
                        Date = x.fl.Date,
                        DocumentId = x.fl.DocumentId,
                        Extension = x.fl.Extension,
                        FileContent = x.fl.Content,
                        FileType = x.fl.FileType,
                        FileSize = x.fl.FileSize,
                        IsAdditional = x.fl.IsAdditional,
                        Hash = x.fl.Hash,
                        LastChangeDate = x.fl.LastChangeDate,
                        LastChangeUserId = x.fl.LastChangeUserId,
                        Name = x.fl.Name,
                        OrderInDocument = x.fl.OrderNumber,
                        Version = x.fl.Version,
                        WasChangedExternal = false
                    }).ToList();
        }

        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext)
        {
            return 
                dbContext.DocumentAccessesSet.Where(x => ctx.CurrentPositionsIdList.Contains(x.PositionId ))
                .Select(acc=> new FrontDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name
                });
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(DmsContext dbContext, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            if (docAccesses == null || !docAccesses.Any()) return null;
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x => !accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
        }

        public static InternalDocumentAccess GetInternalDocumentAccess(IContext ctx, DmsContext dbContext, int documentId)
        {

            var acc =
                dbContext.DocumentAccessesSet.FirstOrDefault(
                    x => x.DocumentId == documentId && x.PositionId == ctx.CurrentPositionId);
            if (acc != null)
            {
                return new InternalDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId,
                };
            }

            return null;
        }

        public static IEnumerable<InternalDocumentAccess> GetInternalDocumentAccesses(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentAccessesSet.Where(
                x => x.DocumentId == documentId).Select(acc => new InternalDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId
                }).ToList();
        }

        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(DmsContext dbContext, List<int> positionIds)
        {
            return dbContext.DictionaryPositionsSet.Where(x => positionIds.Contains(x.Id)).Select(x => new InternalPositionInfo
            {
                PositionId = x.Id,
                PositionName = x.Name,
                AgentId = x.ExecutorAgentId ?? 0,
                AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name : ""
            }).ToList();
        }

        public static IEnumerable<FrontDocumentEvent> GetDocumentEvents(DmsContext dbContext, FilterDocumentEvent filter)
        {
            var qry = dbContext.DocumentEventsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.EventId?.Count > 0)
                {
                    qry = qry.Where(x => filter.EventId.Contains(x.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }
            return qry.Select(x => new FrontDocumentEvent
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                Task = x.Task,
                Description = x.Description,
                EventType = (EnumEventTypes)x.EventTypeId,
                EventTypeName = x.EventType.Name,
                ImportanceEventType = (EnumImportanceEventTypes)x.EventType.ImportanceEventTypeId,
//                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                CreateDate = x.CreateDate,
                Date = x.Date,
                SourceAgentName = x.SourceAgent.Name,
                SourceAgentId = x.SourceAgentId,
                SourcePositionId = x.SourcePositionId,
                SourcePositionName = x.SourcePosition.Name,
                SourcePositionExecutorAgentName = x.SourcePosition.ExecutorAgent.Name,
                TargetAgentName = x.TargetAgent.Name,
                TargetAgentId = x.TargetAgentId,
                TargetPositionId = x.TargetPositionId,
                TargetPositionName = x.TargetPosition.Name,
                TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
            }).ToList();

        }

        public static IEnumerable<FrontDocumentWaits> GetDocumentWaits(DmsContext dbContext, FilterDocumentWait filter)
        {
            var waitsDb = dbContext.DocumentWaitsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    waitsDb = waitsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                if (filter.OnEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OnEventId == filter.OnEventId.Value);
                }

                if (filter.OffEventId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.OffEventId.HasValue && x.OffEventId.Value == filter.OffEventId.Value);
                }

                if (filter.Opened)
                {
                    waitsDb = waitsDb.Where(x => !x.OffEventId.HasValue);
                }
            }

            var waitsRes = waitsDb.Select(x => new { Wait = x, x.OnEvent, x.OffEvent });

            var waits = waitsRes.Select(x => new FrontDocumentWaits
            {
                Id = x.Wait.Id,
                DocumentId = x.Wait.DocumentId,
                ParentId = x.Wait.ParentId,
                OnEventId = x.Wait.OnEventId,
                OffEventId = x.Wait.OffEventId,
                ResultTypeId = x.Wait.ResultTypeId,
                ResultTypeName = x.Wait.ResultType.Name,
                DueDate = x.Wait.DueDate,
                AttentionDate = x.Wait.AttentionDate,
                OnEvent = x.OnEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OnEvent.Id,
                        DocumentId = x.OnEvent.DocumentId,
                        Task = x.OnEvent.Task,
                        Description = x.OnEvent.Description,
                        EventType = (EnumEventTypes)x.OnEvent.EventTypeId,
                        EventTypeName = x.OnEvent.EventType.Name,
                        ImportanceEventType = (EnumImportanceEventTypes)x.OnEvent.EventType.ImportanceEventTypeId,
                        //                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                        CreateDate = x.OnEvent.CreateDate,
                        Date = x.OnEvent.Date,
                        SourceAgentName = x.OnEvent.SourceAgent.Name,
                        SourceAgentId = x.OnEvent.SourceAgentId,
                        SourcePositionId = x.OnEvent.SourcePositionId,
                        SourcePositionName = x.OnEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.OnEvent.SourcePosition.ExecutorAgent.Name,
                        TargetAgentName = x.OnEvent.TargetAgent.Name,
                        TargetAgentId = x.OnEvent.TargetAgentId,
                        TargetPositionId = x.OnEvent.TargetPositionId,
                        TargetPositionName = x.OnEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.OnEvent.TargetPosition.ExecutorAgent.Name,
                    },
                OffEvent = x.OffEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        DocumentId = x.OffEvent.DocumentId,
                        Task = x.OffEvent.Task,
                        Description = x.OffEvent.Description,
                        EventType = (EnumEventTypes)x.OffEvent.EventTypeId,
                        EventTypeName = x.OffEvent.EventType.Name,
                        ImportanceEventType = (EnumImportanceEventTypes)x.OffEvent.EventType.ImportanceEventTypeId,
                        //                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                        CreateDate = x.OffEvent.CreateDate,
                        Date = x.OffEvent.Date,
                        SourceAgentName = x.OffEvent.SourceAgent.Name,
                        SourceAgentId = x.OffEvent.SourceAgentId,
                        SourcePositionId = x.OffEvent.SourcePositionId,
                        SourcePositionName = x.OffEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.OffEvent.SourcePosition.ExecutorAgent.Name,
                        TargetAgentName = x.OffEvent.TargetAgent.Name,
                        TargetAgentId = x.OffEvent.TargetAgentId,
                        TargetPositionId = x.OffEvent.TargetPositionId,
                        TargetPositionName = x.OffEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.OffEvent.TargetPosition.ExecutorAgent.Name,
                    }
            }).ToList();

            return waits;

        }

        public static IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(DmsContext dbContext, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    subscriptionsDb = subscriptionsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }
            }

            var subscriptionsRes = subscriptionsDb.Select(x => new { Subscription = x, x.SendEvent, x.DoneEvent });

            var subscriptions = subscriptionsRes.Select(x => new FrontDocumentSubscription
            {
                Id = x.Subscription.Id,
                DocumentId = x.Subscription.DocumentId,
                SendEventId = x.Subscription.SendEventId,
                DoneEventId = x.Subscription.DoneEventId,
                Description = x.Subscription.Description,
                //Hash = x.Subscription.Hash,
                //ChangedHash = x.Subscription.ChangedHash,
                SendEvent = x.SendEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.SendEvent.Id,
                        DocumentId = x.SendEvent.DocumentId,
                        Task = x.SendEvent.Task,
                        Description = x.SendEvent.Description,
                        EventType = (EnumEventTypes)x.SendEvent.EventTypeId,
                        EventTypeName = x.SendEvent.EventType.Name,
                        ImportanceEventType = (EnumImportanceEventTypes)x.SendEvent.EventType.ImportanceEventTypeId,
                        //                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                        CreateDate = x.SendEvent.CreateDate,
                        Date = x.SendEvent.Date,
                        SourceAgentName = x.SendEvent.SourceAgent.Name,
                        SourceAgentId = x.SendEvent.SourceAgentId,
                        SourcePositionId = x.SendEvent.SourcePositionId,
                        SourcePositionName = x.SendEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.SendEvent.SourcePosition.ExecutorAgent.Name,
                        TargetAgentName = x.SendEvent.TargetAgent.Name,
                        TargetAgentId = x.SendEvent.TargetAgentId,
                        TargetPositionId = x.SendEvent.TargetPositionId,
                        TargetPositionName = x.SendEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.SendEvent.TargetPosition.ExecutorAgent.Name,
                    },
                DoneEvent = x.DoneEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.DoneEvent.Id,
                        DocumentId = x.DoneEvent.DocumentId,
                        Task = x.DoneEvent.Task,
                        Description = x.DoneEvent.Description,
                        EventType = (EnumEventTypes)x.DoneEvent.EventTypeId,
                        EventTypeName = x.DoneEvent.EventType.Name,
                        ImportanceEventType = (EnumImportanceEventTypes)x.DoneEvent.EventType.ImportanceEventTypeId,
                        //                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                        CreateDate = x.DoneEvent.CreateDate,
                        Date = x.DoneEvent.Date,
                        SourceAgentName = x.DoneEvent.SourceAgent.Name,
                        SourceAgentId = x.DoneEvent.SourceAgentId,
                        SourcePositionId = x.DoneEvent.SourcePositionId,
                        SourcePositionName = x.DoneEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.DoneEvent.SourcePosition.ExecutorAgent.Name,
                        TargetAgentName = x.DoneEvent.TargetAgent.Name,
                        TargetAgentId = x.DoneEvent.TargetAgentId,
                        TargetPositionId = x.DoneEvent.TargetPositionId,
                        TargetPositionName = x.DoneEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.DoneEvent.TargetPosition.ExecutorAgent.Name,
                    }
            }).ToList();

            return subscriptions;

        }

        public static IEnumerable<FrontDocumentTag> GetDocumentTags(DmsContext dbContext, FilterDocumentTag filter)
        {
            var tagsDb = dbContext.DocumentTagsSet.AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    tagsDb = tagsDb.Where(x => filter.DocumentId.Contains(x.DocumentId));
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    tagsDb = tagsDb.Where(x => !x.Tag.PositionId.HasValue || filter.CurrentPositionsId.Contains(x.Tag.PositionId ?? 0));
                }
            }

            var tagsRes = tagsDb;

            var tags = tagsRes.Select(x => new FrontDocumentTag
            {
                TagId = x.TagId,
                DocumentId = x.DocumentId,
                PositionId = x.Tag.PositionId,
                PositionName = x.Tag.Position.Name,
                Color = x.Tag.Color,
                Name = x.Tag.Name,
                IsSystem = !x.Tag.PositionId.HasValue
            }).ToList();

            return tags;

        }

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(DmsContext dbContext, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.Object.Contains((EnumObjects)x.PropertyLink.ObjectId));
                }

                if (filter.RecordId?.Count > 0)
                {
                    itemsDb = itemsDb.Where(x => filter.RecordId.Contains(x.RecordId));
                }
            }

            var itemsRes = itemsDb;

            var items = itemsRes.Select(x => new FrontPropertyValue
            {
                Id = x.Id,
                RecordId = x.Id,
                PropertyLinkId = x.PropertyLinkId,
                Value = x.ValueString,
                PropertyId = x.PropertyLink.PropertyId,
                ObjectId = x.PropertyLink.ObjectId,
                Filers = x.PropertyLink.Filers,
                IsMandatory = x.PropertyLink.IsMandatory,
                PropertyCode = x.PropertyLink.Property.Code,
                PropertyDescription = x.PropertyLink.Property.Description,
                PropertyLabel = x.PropertyLink.Property.Label,
                PropertyHint = x.PropertyLink.Property.Hint,
                PropertyValueTypeId = x.PropertyLink.Property.ValueTypeId,
                PropertyOutFormat = x.PropertyLink.Property.OutFormat,
                PropertyInputFormat = x.PropertyLink.Property.InputFormat,
                PropertySelectAPI = x.PropertyLink.Property.SelectAPI,
                PropertySelectFilter = x.PropertyLink.Property.SelectFilter,
                PropertySelectFieldCode = x.PropertyLink.Property.SelectFieldCode,
                PropertySelectDescriptionFieldCode = x.PropertyLink.Property.SelectDescriptionFieldCode,
                PropertyValueTypeCode = x.PropertyLink.Property.ValueType.Code,
                PropertyValueTypeDescription = x.PropertyLink.Property.ValueType.Description,
            }).ToList();

            return items;

        }

        public static IEnumerable<BaseDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

            if (filter != null)
            {
                if (filter.PositionId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PositionId.Contains(x.pos.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }

                if (filter.SubordinatedPositions?.Count > 0)
                {
                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(y => filter.SubordinatedPositions.Contains(y.SourcePositionId)),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
                                        )
                             .Where(x => x.subordMax > 0);
                }
            }

            return qry.Select(x => new BaseDictionaryPosition
            {
                Id = x.pos.Id,
                ParentId = x.pos.ParentId,
                Name = x.pos.Name,
                DepartmentId = x.pos.DepartmentId,
                ExecutorAgentId = x.pos.ExecutorAgentId,
                ParentPositionName = x.pos.ParentPosition.Name,
                DepartmentName = x.pos.Department.Name,
                ExecutorAgentName = x.pos.ExecutorAgent.Name,
                MaxSubordinationTypeId = (x.subordMax > 0 ? (int?)x.subordMax : null)
            }).ToList();

        }

        public static IEnumerable<FrontDocument> GetLinkedDocuments(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentsSet.Where(x => (x.LinkId == documentId))
                        .Select(y => new FrontDocument
                        {
                            Id = y.Id,
                            GeneralInfo = y.TemplateDocument.DocumentDirection.Name + " " + y.TemplateDocument.DocumentType.Name,
                            RegistrationFullNumber =
                                                (!y.IsRegistered ? "#" : "") +
                                                (y.RegistrationNumber != null
                                                        ? (y.RegistrationNumberPrefix + y.RegistrationNumber.ToString() + y.RegistrationNumberSuffix)
                                                        : ("#" + y.Id.ToString())),
                            DocumentDate = y.RegistrationDate ?? y.CreateDate,
                            Description = y.Description,
                            Links = dbContext.DocumentLinksSet.Where(z => z.DocumentId == y.Id).
                                Select(z => new FrontDocumentLink
                                {
                                    Id = z.Id,
                                    GeneralInfo = z.LinkType.Name + " " +
                                                (!z.ParentDocument.IsRegistered ? "#" : "") +
                                                (z.ParentDocument.RegistrationNumber != null
                                                        ? (z.ParentDocument.RegistrationNumberPrefix + z.ParentDocument.RegistrationNumber.ToString() + z.ParentDocument.RegistrationNumberSuffix)
                                                        : ("#" + z.ParentDocument.Id.ToString()))
                                    + " " + (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate).ToString()
                                    //TODO String.Format("{0:dd.MM.yyyy}", (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate))
                                }).ToList()
                        }).ToList();
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(y => new FrontDocumentSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            SendTypeName = y.SendType.Name,
                            SendTypeCode = y.SendType.Code,
                            SendTypeIsImportant = y.SendType.IsImportant,

                            SourceAgentName = y.SourceAgent.Name,
                            SourceAgentId = y.SourceAgentId,
                            SourcePositionId = y.SourcePositionId,
                            SourcePositionName = y.SourcePosition.Name,
                            SourcePositionExecutorAgentName = y.SourcePosition.ExecutorAgent.Name,
                            TargetAgentName = y.TargetAgent.Name,
                            TargetAgentId = y.TargetAgentId,
                            TargetPositionId = y.TargetPositionId,
                            TargetPositionName = y.TargetPosition.Name,
                            TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name,

                            Task = y.Task,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            IsInitial = y.IsInitial,
                            StartEventId = y.StartEventId,
                            CloseEventId = y.CloseEventId,
                            GeneralInfo = string.Empty,

                        }).ToList();
        }

        public static IEnumerable<FrontDocumentRestrictedSendList> GetDocumentRestrictedSendList(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(y => new FrontDocumentRestrictedSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            PositionId = y.PositionId,
                            PositionName = y.Position.Name,
                            PositionExecutorAgentName = y.Position.ExecutorAgent.Name,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            GeneralInfo = string.Empty
                        }).ToList();
        }

        public static void ModifyPropertyValues(DmsContext dbContext, InternalPropertyValues model)
        {
            var propertyValues = dbContext.PropertyValuesSet.
                Where(x => x.PropertyLink.ObjectId == (int)model.Object && x.RecordId == model.RecordId)
                .Select(x => new { x.Id, x.PropertyLinkId }).ToList();

            var groupJoinItems = propertyValues
               .GroupJoin(model.PropertyValues,
                   x => x.PropertyLinkId,
                   y => y.PropertyLinkId,
                   (x, y) => new { propertyValueId = x.Id, values = y })
               .ToList();

            #region modify
            var modifyItems = groupJoinItems
                .Where(x => x.values.Count() > 0)
                .Select(x => new { x.propertyValueId, value = x.values.First() })
                .Select(x => new PropertyValues
                {
                    Id = x.propertyValueId,
                    ValueString = x.value.ValueString,
                    ValueDate = x.value.ValueDate,
                    ValueNumeric = x.value.ValueNumeric,
                    LastChangeDate = x.value.LastChangeDate,
                    LastChangeUserId = x.value.LastChangeUserId,
                });

            foreach (var item in modifyItems)
            {
                dbContext.PropertyValuesSet.Attach(item);
                var entry = dbContext.Entry(item);
                entry.Property(x => x.ValueString).IsModified = true;
                entry.Property(x => x.ValueDate).IsModified = true;
                entry.Property(x => x.ValueNumeric).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
            }
            #endregion

            #region add
            var newItems = model.PropertyValues
                .Where(x => !propertyValues.Select(y => y.PropertyLinkId).Contains(x.PropertyLinkId))
                .Select(x => new PropertyValues
                {
                    PropertyLinkId = x.PropertyLinkId,
                    RecordId = model.RecordId,
                    ValueString = x.ValueString,
                    ValueDate = x.ValueDate,
                    ValueNumeric = x.ValueNumeric,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

            dbContext.PropertyValuesSet.AddRange(newItems);
            #endregion

            #region delete
            foreach (var item in groupJoinItems.Where(x => x.values.Count() == 0).Select(x => x.propertyValueId))
            {
                var itemAtt = dbContext.PropertyValuesSet.Attach(new PropertyValues { Id = item });
                dbContext.Entry(itemAtt).State = System.Data.Entity.EntityState.Deleted;
            }
            #endregion
        }
    }
}