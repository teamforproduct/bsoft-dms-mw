using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.InternalModel;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
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
                          SenderPersonName = sendAp.Name
                      };
            return qry;
        }

        public static IQueryable<FilterDocumentFileIdentity> GetDocumentFilesMaxVersion(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentFilesSet
                .Where(x => x.DocumentId == documentId)
                .GroupBy(g => new { g.DocumentId, g.OrderNumber })
                .Select(x => new FilterDocumentFileIdentity  { DocumentId = x.Key.DocumentId, OrderInDocument = x.Key.OrderNumber, Version = x.Max(s => s.Version) });
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFilesMaxVersion(dbContext, documentId);

            return
                sq.Join(dbContext.DocumentFilesSet, sub => new { sub.DocumentId, OrderNumber = sub.OrderInDocument, sub.Version },
                    fl => new { fl.DocumentId, fl.OrderNumber, fl.Version },
                    (s, f) => new {fl = f})
                    .Join(dbContext.DictionaryAgentsSet, df => df.fl.LastChangeUserId, da => da.Id,
                        (d, a) => new {d.fl, agName = a.Name})
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
            var sq = GetDocumentFilesMaxVersion(dbContext, documentId);

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

        public static FrontDocumentAccess GetDocumentAccess(IContext ctx, DmsContext dbContext, int documentId)
        {

            var acc =
                dbContext.DocumentAccessesSet.FirstOrDefault(
                    x => x.DocumentId == documentId && x.PositionId == ctx.CurrentPositionId);
            if (acc != null)
            {
                return new FrontDocumentAccess
                {
                    LastChangeDate = acc.LastChangeDate,
                    LastChangeUserId = acc.LastChangeUserId,
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevel = (EnumDocumentAccesses) acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name
                };
            }

            return null;
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(DmsContext dbContext, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x=>!accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
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
                    AccessLevel = (EnumDocumentAccesses) acc.AccessLevelId
                }).ToList();
        }

        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(DmsContext dbContext, List<int> positionIds)
        {
            return dbContext.DictionaryPositionsSet.Where(x => positionIds.Contains(x.Id)).Select(x => new InternalPositionInfo
            {
                PositionId = x.Id,
                PositionName = x.Name,
                AgentId = x.ExecutorAgentId ?? 0,
                AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name:""
                }).ToList();
        }

        public static IEnumerable<FrontDocumentEvent> GetDocumentEvents(DmsContext dbContext,FilterDocumentEvent filter)
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
                Description = x.Description,
                EventType = (EnumEventTypes) x.EventTypeId,
                ImportanceEventType = (EnumImportanceEventTypes) x.EventType.ImportanceEventTypeId,
                CreateDate = x.CreateDate,
                Date = x.Date,
                EventTypeName = x.EventType.Name,
                EventImportanceTypeName = x.EventType.ImportanceEventType.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SourceAgenName = x.SourceAgent.Name,
                SourceAgentId = x.SourceAgentId,
                SourcePositionId = x.SourcePositionId,
                SourcePositionName = x.SourcePosition.Name,
                TargetAgenName = x.TargetAgent.Name,
                TargetAgentId = x.TargetAgentId,
                TargetPositionId = x.TargetPositionId,
                TargetPositionName = x.TargetPosition.Name,
                GeneralInfo = ""
            }).ToList();

        }

        public static IEnumerable<InternalDocumentEvent> GetInternalDocumentEvents(DmsContext dbContext, FilterDocumentEvent filter)
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
            return qry.Select(x => new InternalDocumentEvent
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                Description = x.Description,
                EventType = (EnumEventTypes)x.EventTypeId,
                //ImportanceEventType = (EnumImportanceEventTypes)x.EventType.ImportanceEventTypeId,
                CreateDate = x.CreateDate,
                Date = x.Date,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SourceAgentId = x.SourceAgentId,
                SourcePositionId = x.SourcePositionId,
                TargetAgentId = x.TargetAgentId,
                TargetPositionId = x.TargetPositionId,
            }).ToList();

        }

        public static IEnumerable<InternalDocumentWait> GetInternalDocumentWaits(DmsContext dbContext, FilterDocumentWaits filter)
        {
            var waitsDb = dbContext.DocumentWaitsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.DocumentId == filter.DocumentId.Value);
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

            var waits = waitsRes.Select(x => new InternalDocumentWait
            {
                Id = x.Wait.Id,
                DocumentId = x.Wait.DocumentId,
                ParentId = x.Wait.ParentId,
                OnEventId = x.Wait.OnEventId,
                OffEventId = x.Wait.OffEventId,
                ResultTypeId = x.Wait.ResultTypeId,
                Task = x.Wait.Task,
                DueDate = x.Wait.DueDate,
                AttentionDate = x.Wait.AttentionDate,
                OnEvent = x.OnEvent == null
                    ? null
                    : new InternalDocumentEvent
                    {
                        Id = x.OnEvent.Id,
                        CreateDate = x.OnEvent.CreateDate,
                        Date = x.OnEvent.Date,
                        Description = x.OnEvent.Description,
                        LastChangeDate = x.OnEvent.LastChangeDate,
                        LastChangeUserId = x.OnEvent.LastChangeUserId,
                        SourceAgentId = x.OnEvent.SourceAgentId,
                        SourcePositionId = x.OnEvent.SourcePositionId,
                        TargetAgentId = x.OnEvent.TargetAgentId,
                        TargetPositionId = x.OnEvent.TargetPositionId,
                        EventType = (EnumEventTypes)x.OnEvent.EventTypeId
                    },
                OffEvent = x.OffEvent == null
                    ? null
                    : new InternalDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        CreateDate = x.OffEvent.CreateDate,
                        Date = x.OffEvent.Date,
                        Description = x.OffEvent.Description,
                        LastChangeDate = x.OffEvent.LastChangeDate,
                        LastChangeUserId = x.OffEvent.LastChangeUserId,
                        SourceAgentId = x.OffEvent.SourceAgentId,
                        SourcePositionId = x.OffEvent.SourcePositionId,
                        TargetAgentId = x.OffEvent.TargetAgentId,
                        TargetPositionId = x.OffEvent.TargetPositionId,
                        EventType = (EnumEventTypes)x.OffEvent.EventTypeId
                    }
            }).ToList();

            return waits;

        }

        public static IEnumerable<FrontDocumentWaits> GetDocumentWaits(DmsContext dbContext, FilterDocumentWaits filter)
        {
            var waitsDb = dbContext.DocumentWaitsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.HasValue)
                {
                    waitsDb = waitsDb.Where(x => x.DocumentId == filter.DocumentId.Value);
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

            var waitsRes = waitsDb.Select(x => new {Wait = x, x.OnEvent, x.OffEvent});

            var waits = waitsRes.Select(x => new FrontDocumentWaits
            {
                Id = x.Wait.Id,
                DocumentId = x.Wait.DocumentId,
                ParentId = x.Wait.ParentId,
                OnEventId = x.Wait.OnEventId,
                OffEventId = x.Wait.OffEventId,
                ResultTypeId = x.Wait.ResultTypeId,
                Task = x.Wait.Task,
                DueDate = x.Wait.DueDate,
                AttentionDate = x.Wait.AttentionDate,
                OnEvent = x.OnEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OnEvent.Id,
                        CreateDate = x.OnEvent.CreateDate,
                        Date = x.OnEvent.Date,
                        Description = x.OnEvent.Description,
                        LastChangeDate = x.OnEvent.LastChangeDate,
                        LastChangeUserId = x.OnEvent.LastChangeUserId,
                        SourceAgentId = x.OnEvent.SourceAgentId,
                        SourcePositionId = x.OnEvent.SourcePositionId,
                        TargetAgentId = x.OnEvent.TargetAgentId,
                        TargetPositionId = x.OnEvent.TargetPositionId,
                        EventType = (EnumEventTypes) x.OnEvent.EventTypeId
                    },
                OffEvent = x.OffEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        CreateDate = x.OffEvent.CreateDate,
                        Date = x.OffEvent.Date,
                        Description = x.OffEvent.Description,
                        LastChangeDate = x.OffEvent.LastChangeDate,
                        LastChangeUserId = x.OffEvent.LastChangeUserId,
                        SourceAgentId = x.OffEvent.SourceAgentId,
                        SourcePositionId = x.OffEvent.SourcePositionId,
                        TargetAgentId = x.OffEvent.TargetAgentId,
                        TargetPositionId = x.OffEvent.TargetPositionId,
                        EventType = (EnumEventTypes) x.OffEvent.EventTypeId
                    }
            }).ToList();

            return waits;

        }

        public static IEnumerable<FrontDocumentTag> GetDocumentTags(DmsContext dbContext, FilterDocumentTag filter)
        {
            var tagsDb = dbContext.DocumentTagsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.HasValue)
                {
                    tagsDb = tagsDb.Where(x => x.DocumentId == filter.DocumentId.Value);
                }

                if(filter.CurrentPositionsId?.Count>0)
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

        public static IEnumerable<BaseDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

            if (filter!=null)
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
                            TargetPositionId = y.TargetPositionId,
                            TargetPositionName = y.TargetPosition.Name,
                            TargetAgentName = y.TargetPosition.ExecutorAgent.Name,
                            Task = y.Task,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            AccessLevelName = y.AccessLevel.Name,
                            IsInitial = y.IsInitial,
                            StartEventId = y.StartEventId,
                            CloseEventId = y.CloseEventId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            GeneralInfo = string.Empty
                        }).ToList();
        }

        public static IEnumerable<InternalDocumentSendList> GetInternalDocumentSendList(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            TargetPositionId = y.TargetPositionId,
                            Task = y.Task,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            IsInitial = y.IsInitial,
                            StartEventId = y.StartEventId,
                            CloseEventId = y.CloseEventId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
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

        public static IEnumerable<InternalDocumentRestrictedSendList> GetInternalDocumentRestrictedSendList(DmsContext dbContext, int documentId)
        {
            return dbContext.DocumentRestrictedSendListsSet.Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentRestrictedSendList
                        {
                            Id = y.Id,
                            DocumentId = y.DocumentId,
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                        }).ToList();
        }
    }
}