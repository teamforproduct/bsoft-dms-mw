using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.System;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using System.Data.Entity;
using BL.Model.FullTextSearch;
using System;
using BL.Database.DBModel.Dictionary;
using LinqKit;

namespace BL.Database.SystemDb
{
    public class SystemDbProcess : CoreDb.CoreDb, ISystemDbProcess
    {
        public SystemDbProcess()
        {
        }

        public void InitializerDatabase(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                dbContext.SystemObjectsSet.Take(0).ToList();
            }
        }

        #region Log
        public int AddLog(IContext ctx, LogInfo log)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var nlog = new SystemLogs
                {
                    ClientId = ctx.CurrentClientId,
                    ExecutorAgentId = log.AgentId,
                    LogDate = log.Date,
                    LogLevel = (int)log.LogType,
                    LogException = log.LogException,
                    LogTrace = log.LogObjects,
                    Message = log.Message,
                };
                dbContext.LogSet.Add(nlog);
                dbContext.SaveChanges();
                return nlog.Id;
            }
        }
        #endregion

        #region Settings
        public int AddSetting(IContext ctx, string name, string value, int? agentId = null)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var cset = dbContext.SettingsSet.FirstOrDefault(x => ctx.CurrentClientId == x.ClientId && x.Key == name);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
                        ClientId = ctx.CurrentClientId,
                        ExecutorAgentId = agentId,
                        Key = name,
                        Value = value
                    };
                    dbContext.SettingsSet.Add(nsett);
                    dbContext.SaveChanges();
                    return nsett.Id;
                }

                cset.Value = value;
                cset.ExecutorAgentId = agentId;
                dbContext.SaveChanges();
                return cset.Id;
            }
        }

        public string GetSettingValue(IContext ctx, string name, int? agentId = null)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                if (agentId.HasValue)
                {
                    return
                        dbContext.SettingsSet.Where(x => ctx.CurrentClientId == x.ClientId && x.Key == name && x.ExecutorAgentId == agentId.Value)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                }
                return
                    dbContext.SettingsSet.Where(x => ctx.CurrentClientId == x.ClientId && x.Key == name).OrderBy(x => x.ExecutorAgentId)
                        .Select(x => x.Value)
                        .FirstOrDefault();
            }
        }
        #endregion

        public IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter)
        {
            {

                using (var dbContext = new DmsContext(ctx))
                {
                    var qry = dbContext.SystemUIElementsSet.AsQueryable();

                    if (filter.UIElementId?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<SystemUIElements>();
                        filterContains = filter.UIElementId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (!string.IsNullOrEmpty(filter.Code))
                    {
                        qry = qry.Where(x => x.Code.Contains(filter.Code));
                    }
                    if (!string.IsNullOrEmpty(filter.ObjectCode))
                    {
                        qry = qry.Where(x => x.Action.Object.Code.Contains(filter.ObjectCode));
                    }
                    if (!string.IsNullOrEmpty(filter.ActionCode))
                    {
                        qry = qry.Where(x => x.Action.Code.Contains(filter.ActionCode));
                    }
                    var res = qry.Select(x => new BaseSystemUIElement
                    {
                        Id = x.Id,
                        ObjectCode = x.Action.Object.Code,
                        ActionCode = x.Action.Code,
                        Code = x.Code,
                        TypeCode = x.TypeCode,
                        Label = x.Label,
                        Hint = x.Hint,
                        ValueTypeCode = x.ValueType.Code,
                        IsMandatory = x.IsMandatory,
                        IsReadOnly = x.IsReadOnly,
                        IsVisible = x.IsVisible,
                        SelectAPI = x.SelectAPI,
                        SelectFilter = x.SelectFilter,
                        SelectFieldCode = x.SelectFieldCode,
                        SelectDescriptionFieldCode = x.SelectDescriptionFieldCode,
                        ValueFieldCode = x.ValueFieldCode,
                        ValueDescriptionFieldCode = x.ValueDescriptionFieldCode,
                        Format = x.Format
                    }).ToList();

                    return res;
                }
            }
        }

        #region SystemObjects

        public IEnumerable<FrontSystemObject> GetSystemObjects(IContext ctx, FilterSystemObject filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.PropertiesSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter.SystemObjectId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<Properties>();
                    filterContains = filter.SystemObjectId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => new FrontSystemObject
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                }).ToList();
            }
        }


        #endregion SystemObjects

        #region Properties

        public IEnumerable<BaseSystemUIElement> GetPropertyUIElements(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter.PropertyLinkId != null)
                {
                    var filterContains = PredicateBuilder.False<PropertyLinks>();
                    filterContains = filter.PropertyLinkId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => new BaseSystemUIElement
                {
                    PropertyLinkId = x.Id,
                    ObjectCode = x.Object.Code,
                    ActionCode = x.Property.Code,
                    Code = x.Property.Code,
                    TypeCode = x.Property.TypeCode,
                    Label = x.Property.Label,
                    Hint = x.Property.Hint,
                    ValueTypeCode = x.Property.ValueType.Code,
                    IsMandatory = x.IsMandatory,
                    IsReadOnly = false,
                    IsVisible = true,
                    SelectAPI = x.Property.SelectAPI,
                    SelectFilter = x.Property.SelectFilter,
                    SelectFieldCode = x.Property.SelectFieldCode,
                    SelectDescriptionFieldCode = x.Property.SelectDescriptionFieldCode,
                    ValueFieldCode = x.Property.Code,
                    ValueDescriptionFieldCode = x.Property.ValueType.Description,
                    Format = x.Property.OutFormat,
                }).ToList();
            }
        }

        private IQueryable<Properties> GetPropertiesQuery(DmsContext dbContext, IContext ctx, FilterProperty filter)
        {
            var qry = dbContext.PropertiesSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.PropertyId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<Properties>();
                    filterContains = filter.PropertyId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<FrontProperty> GetProperties(IContext ctx, FilterProperty filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = GetPropertiesQuery(dbContext, ctx, filter);

                return qry.Select(x => new FrontProperty
                {
                    Id = x.Id,
                    Code = x.Code,
                    TypeCode = x.TypeCode,
                    Description = x.Description,
                    Label = x.Label,
                    Hint = x.Hint,
                    ValueTypeId = x.ValueTypeId,
                    OutFormat = x.OutFormat,
                    InputFormat = x.InputFormat,
                    SelectAPI = x.SelectAPI,
                    SelectFilter = x.SelectFilter,
                    SelectFieldCode = x.SelectFieldCode,
                    SelectDescriptionFieldCode = x.SelectDescriptionFieldCode,
                    SelectTable = x.SelectTable,
                    ValueType = !x.ValueTypeId.HasValue ? null :
                        new InternalSystemValueType
                        {
                            Id = x.ValueType.Id,
                            Code = x.ValueType.Code,
                            Description = x.ValueType.Description
                        }
                }).ToList();
            }
        }

        public int AddProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var item = new Properties
                {
                    ClientId = ctx.CurrentClientId,
                    Code = model.Code,
                    TypeCode = model.TypeCode,
                    Description = model.Description,
                    Label = model.Label,
                    Hint = model.Hint,
                    ValueTypeId = model.ValueTypeId,
                    OutFormat = model.OutFormat,
                    InputFormat = model.InputFormat,
                    SelectAPI = model.SelectAPI,
                    SelectFilter = model.SelectFilter,
                    SelectFieldCode = model.SelectFieldCode,
                    SelectDescriptionFieldCode = model.SelectDescriptionFieldCode,
                    SelectTable = model.SelectTable,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertiesSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Added;

                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdateProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var item = new Properties
                {
                    Id = model.Id,
                    ClientId = ctx.CurrentClientId,
                    Code = model.Code,
                    TypeCode = model.TypeCode,
                    Description = model.Description,
                    Label = model.Label,
                    Hint = model.Hint,
                    ValueTypeId = model.ValueTypeId,
                    OutFormat = model.OutFormat,
                    InputFormat = model.InputFormat,
                    SelectAPI = model.SelectAPI,
                    SelectFilter = model.SelectFilter,
                    SelectFieldCode = model.SelectFieldCode,
                    SelectDescriptionFieldCode = model.SelectDescriptionFieldCode,
                    SelectTable = model.SelectTable,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertiesSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var item = dbContext.PropertiesSet.FirstOrDefault(x => ctx.CurrentClientId == x.ClientId && x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertiesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion Properties

        #region PropertyLinks

        private IQueryable<PropertyLinks> GetPropertyLinksQuery(DmsContext dbContext, IContext ctx, FilterPropertyLink filter)
        {
            var qry = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.PropertyLinkId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyLinks>();
                    filterContains = filter.PropertyLinkId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.Object?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyLinks>();
                    filterContains = filter.Object.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumObjects)e.ObjectId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<InternalPropertyLink> GetInternalPropertyLinks(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                return qry.Select(x => new InternalPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    Object = (EnumObjects)x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory,
                }).ToList();
            }
        }

        public IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                return qry.Select(x => new FrontPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    Object = (EnumObjects)x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory,
                    SystemObject = new FrontSystemObject { Code = x.Object.Code }
                }).ToList();
            }
        }

        public int AddPropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var item = new PropertyLinks
                {
                    PropertyId = model.PropertyId,
                    ObjectId = (int)model.Object,
                    Filers = model.Filers,
                    IsMandatory = model.IsMandatory,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyLinksSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Added;

                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdatePropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var item = new PropertyLinks
                {
                    Id = model.Id,
                    Filers = model.Filers,
                    IsMandatory = model.IsMandatory,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyLinksSet.Attach(item);
                var entry = dbContext.Entry(item);
                entry.Property(p => p.Filers).IsModified = true;
                entry.Property(p => p.IsMandatory).IsModified = true;
                entry.Property(p => p.LastChangeDate).IsModified = true;
                entry.Property(p => p.LastChangeUserId).IsModified = true;

                dbContext.SaveChanges();
            }
        }

        public void DeletePropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var item = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertyLinksSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion PropertyLinks

        #region PropertyValues

        public IEnumerable<FrontPropertyValue> GetPropertyValuesToDocumentFromTemplateDocument(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                //TODO проверить запрос
                qry = qry.Select(x => x.Property.Links.FirstOrDefault(y => y.ObjectId == (int)EnumObjects.Documents && y.Filers == x.Filers))
                    .Where(x => x != null);

                return qry.Select(x => new FrontPropertyValue
                {
                    PropertyLinkId = x.Id,
                    PropertyCode = x.Property.Code
                }).ToList();
            }
        }

        #endregion PropertyValues

        #region Mailing

        public IEnumerable<InternalDataForMail> GetNewActionsForMailing(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => (x.SendDate == null || x.SendDate < x.LastChangeDate)
                    && ((x.TargetAgentId != null && x.SourceAgentId != x.TargetAgentId)
                    || (x.TargetPositionId != null && x.SourcePositionId != x.TargetPositionId)))
                    .Select(x => new InternalDataForMail
                    {
                        EventId = x.Id,
                        Date = x.Date,
                        Description = x.Description,
                        AddDescription = x.AddDescription,
                        DocumentId = x.DocumentId,
                        DocumentName = x.Document.Description,
                        EventType = (EnumEventTypes)x.EventTypeId,
                        DestinationAgentId = x.TargetAgentId ?? 0,
                        DestinationAgentName = (x.TargetAgent == null) ? "" : x.TargetAgent.Name,
                        DestinationPositionId = x.TargetPositionId ?? 0,
                        DestinationPositionName = (x.TargetPosition == null) ? "" : x.TargetPosition.Name,
                        SourceAgentId = x.SourceAgentId ?? 0,
                        SourceAgentName = x.SourceAgent.Name,
                        SourcePositiontId = x.SourcePositionId ?? 0,
                        SourcePositionName = x.SourcePosition == null ? "" : x.SourcePosition.Name,
                        WasUpdated = !(x.SendDate == null),
                        DestinationAgentEmail = "sergozubr@rambler.ru"
                    }).ToList();
            }
        }

        public void MarkActionsLikeMailSended(IContext ctx, InternalMailProcessed mailProcessed)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO будет ли это работать?? 
                var upd = new List<DbEntityEntry>();
                mailProcessed.ProcessedEventIds.ForEach(x =>
                {
                    var evt = new DocumentEvents { Id = x, SendDate = mailProcessed.ProcessedDate };
                    dbContext.DocumentEventsSet.Attach(evt);
                    var entry = dbContext.Entry(evt);
                    entry.Property(p => p.SendDate).IsModified = true;
                    upd.Add(entry);
                });
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region Filter Properties
        public IEnumerable<BaseSystemUIElement> GetFilterProperties(IContext ctx, FilterProperties filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.ObjectId == (int)filter.Object);

                return qry.Select(x => new BaseSystemUIElement
                {
                    PropertyLinkId = x.Id,
                    ObjectCode = filter.Object.ToString(),
                    ActionCode = x.Property.Code,
                    Code = x.Property.Code,
                    TypeCode = x.Property.TypeCode,
                    Label = x.Property.Label,
                    Hint = x.Property.Hint,
                    ValueTypeCode = x.Property.ValueType.Code,
                    SelectAPI = x.Property.SelectAPI,
                    SelectFilter = x.Property.SelectFilter,
                    SelectFieldCode = x.Property.SelectFieldCode,
                    SelectDescriptionFieldCode = x.Property.SelectDescriptionFieldCode,
                    ValueFieldCode = x.Property.Code,
                    ValueDescriptionFieldCode = x.Property.ValueType.Description,
                    Format = x.Property.OutFormat,
                }).ToList();
            }
        }

        public IEnumerable<int> GetSendListIdsForAutoPlan(IContext ctx, int? sendListId = null)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Join(dbContext.DocumentSendListsSet, d => d.Id, s => s.DocumentId, (d, s) => new { doc = d, sl = s })
                    .Where(x => ((sendListId == null && x.doc.IsLaunchPlan) || (sendListId.HasValue && sendListId.Value == x.sl.Id)) && x.sl.IsInitial && !x.sl.CloseEventId.HasValue)
                    .GroupBy(x => x.sl.DocumentId)
                    .Select(x => new
                    {
                        DocId = x.Key,
                        MinStage = x.Min(s => s.sl.Stage)
                    });

                var res = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Join(qry, s => s.DocumentId, q => q.DocId, (s, q) => new { sl = s, q })
                    .Where(x => x.sl.Stage <= x.q.MinStage && !x.sl.StartEventId.HasValue)
                    .OrderBy(x => new { x.sl.Stage, SendTypeId = x.sl.SendTypeId == (int)EnumSendTypes.SendForControl ? 0 : x.sl.SendTypeId })
                    .Select(x => x.sl.Id).ToList();

                res.AddRange(dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => !x.IsInitial && !x.CloseEventId.HasValue && x.Document.IsLaunchPlan
                                && !qry.Select(s => s.DocId).Contains(x.DocumentId))
                    .OrderBy(x => new { x.Stage, SendTypeId = x.SendTypeId == (int)EnumSendTypes.SendForControl ? 0 : x.SendTypeId })
                    .Select(x => x.Id).ToList());

                return res;
            }
        }

        public IEnumerable<int> GetDocumentIdsForClearTrashDocuments(IContext ctx, int timeMinForClearTrashDocuments)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var date = DateTime.Now.AddMinutes(-timeMinForClearTrashDocuments);
                var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => !x.IsRegistered.HasValue && !x.Waits.Any() && !x.Subscriptions.Any() && x.LastChangeDate < date)
                    .Select(x => x.Id);

                var res = qry.ToList();

                return res;
            }
        }

        #endregion Filter Properties

        #region Full text search

        public IEnumerable<FullTextIndexItem> FullTextIndexReindexDbPrepare(IContext ctx)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx))
            {
                res.AddRange(dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Select(x => new
                    {
                        DocumentId = x.Id,
                        ItemType = EnumObjects.Documents,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        regNr = (x.RegistrationNumber != null
                            ? (x.RegistrationNumberPrefix ?? "") + x.RegistrationNumber +
                              (x.RegistrationNumberSuffix ?? "")
                            : "#" + x.Id) + " ",
                        v1 = x.RegistrationJournal.Name + " " + x.RegistrationJournal.Department.Name + " ",
                        v2 = x.Description + " ",
                        v3 = x.ExecutorPositionExecutorAgent.Name + " ",
                        v4 = x.TemplateDocument.DocumentType.Name + " " + x.TemplateDocument.DocumentDirection.Name + " ",
                        v5 = x.DocumentSubject.Name + " ",
                        v6 = x.SenderAgent.Name + " " + x.SenderAgentPerson.Agent.Name + " " + x.SenderNumber + " "
                    }).ToList()
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = x.DocumentId,
                        ItemType = x.ItemType,
                        OperationType = x.OperationType,
                        ObjectId = x.ObjectId,
                        ObjectText = x.regNr + x.v1 + x.v2 + x.v3 + x.v4 + x.v5 + x.v6
                    }));

                res.AddRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                     .Select(x => new FullTextIndexItem
                     {
                         DocumentId = x.DocumentId,
                         ItemType = EnumObjects.DocumentEvents,
                         OperationType = EnumOperationType.AddNew,
                         ObjectId = x.Id,
                         ObjectText = x.Description + " " + x.AddDescription + " " + x.Task.Task + " "
                                + x.SourcePositionExecutorAgent.Name + " " + x.TargetPositionExecutorAgent.Name + " "
                                + x.SourceAgent.Name + " " + x.TargetAgent.Name + " "
                     }).ToList()
                 );

                res.AddRange(dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                     .Select(x => new FullTextIndexItem
                     {
                         DocumentId = x.DocumentId,
                         ItemType = EnumObjects.DocumentFiles,
                         OperationType = EnumOperationType.AddNew,
                         ObjectId = x.Id,
                         ObjectText = x.Name + "." + x.Extension + " "
                     }).ToList()
                 );

                res.AddRange(dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                     .Select(x => new FullTextIndexItem
                     {
                         DocumentId = x.DocumentId,
                         ItemType = EnumObjects.DocumentSendLists,
                         OperationType = EnumOperationType.AddNew,
                         ObjectId = x.Id,
                         ObjectText = x.Description + " " + x.SendType.Name + " "
                                + x.SourcePosition.Name + " " + x.TargetPosition.Name + " "
                                + x.SourcePositionExecutorAgent.Name + " " + x.TargetPositionExecutorAgent.Name + " "
                     }).ToList()
                 );

                res.AddRange(dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                     .Select(x => new FullTextIndexItem
                     {
                         DocumentId = x.DocumentId,
                         ItemType = EnumObjects.DocumentSubscriptions,
                         OperationType = EnumOperationType.AddNew,
                         ObjectId = x.Id,
                         ObjectText = x.Description + " " + x.SubscriptionState.Name + " " + x.DoneEvent.SourcePositionExecutorAgent.Name + " "
                     }).ToList()
                 );

                #region Dictionaries
                res.AddRange(dbContext.DictionaryAgentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                     .Select(x => new FullTextIndexItem
                     {
                        
                         DocumentId = 0,
                         ItemType = EnumObjects.DictionaryAgents,
                         OperationType = EnumOperationType.AddNew,
                         ObjectId = x.Id,
                         ObjectText = x.Name + " " + x.Description
                     }).ToList()
                 );

                res.AddRange(dbContext.DictionaryAgentEmployeesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentEmployees,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText =x.PersonnelNumber + " " + x.Description + " " +x.Agent.Name + " "
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentCompaniesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentCompanies,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.FullName + " " + x.OKPOCode + " " + x.Description + " " +
                                    x.TaxCode + " " + x.VATCode 
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentPersons,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.FullName + " " + x.Description + " " + x.TaxCode + " " +
                                    x.BirthDate + " " + x.PassportNumber + " " + x.PassportSerial + " " +
                                    x.PassportText
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentBanks,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Agent.Name + " " + x.Description + " " + x.MFOCode + " " + x.Swift
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryContacts,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Agent.Name + " " + x.Description + " " + x.Contact +
                                    " " + x.ContactType.Code + " " + x.ContactType.Name 
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryContactTypesSet
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryContactType,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Code + " " + x.Name
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentAddresses,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Agent.Name + " " + x.Description + " " + x.Address +
                                    " " + x.PostCode + " " + x.AddressType.Name
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAddressTypesSet
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAddressType,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Name 
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryAgentAccounts,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.AccountNumber + " " + x.Name + " " +
                                x.Agent.Name + " " + x.AgentBank.MFOCode + " " + x.AgentBank.Agent.Name
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryDocumentType,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Name 
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryDocumentSubjects,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Name
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Select(x => new FullTextIndexItem
                    {
                        DocumentId = 0,
                        ItemType = EnumObjects.DictionaryRegistrationJournals,
                        OperationType = EnumOperationType.AddNew,
                        ObjectId = x.Id,
                        ObjectText = x.Index + " " + x.Name + " " + x.Department.FullName
                    }).ToList()
                );

                res.AddRange(dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == ctx.CurrentClientId)
                   .Select(x => new FullTextIndexItem
                   {
                       DocumentId = 0,
                       ItemType = EnumObjects.DictionaryDepartments,
                       OperationType = EnumOperationType.AddNew,
                       ObjectId = x.Id,
                       ObjectText = x.FullName + " " + x.Code + " " + x.Name + " " + x.Company.Name + " " + x.ChiefPosition.FullName
                   }).ToList()
               );

                res.AddRange(dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                  .Select(x => new FullTextIndexItem
                  {
                      DocumentId = 0,
                      ItemType = EnumObjects.DictionaryPositions,
                      OperationType = EnumOperationType.AddNew,
                      ObjectId = x.Id,
                      ObjectText = x.FullName + " " + x.Name + " " + x.Department.Name + " " +
                                x.ExecutorAgent.Name + " " + x.MainExecutorAgent.Name
                  }).ToList()
              );

                res.AddRange(dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                 .Select(x => new FullTextIndexItem
                 {
                     DocumentId = 0,
                     ItemType = EnumObjects.DictionaryStandartSendLists,
                     OperationType = EnumOperationType.AddNew,
                     ObjectId = x.Id,
                     ObjectText = x.Name + " " + x.Position.Department.Name + " " + x.Position.Name
                 }).ToList()
                 );

                res.AddRange(dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == ctx.CurrentClientId)
                 .Select(x => new FullTextIndexItem
                 {
                     DocumentId = 0,
                     ItemType = EnumObjects.DictionaryStandartSendListContent,
                     OperationType = EnumOperationType.AddNew,
                     ObjectId = x.Id,
                     ObjectText = x.Task + " " + x.Description + " " + x.SendType.Name + x.StandartSendList.Name +
                                " " + x.TargetAgent.Name + " " + x.TargetPosition.Name
                 }).ToList()
                 );

                res.AddRange(dbContext.DictionaryCompaniesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                .Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryCompanies,
                    OperationType = EnumOperationType.AddNew,
                    ObjectId = x.Id,
                    ObjectText = x.Name 
                }).ToList()
              );

                res.AddRange(dbContext.DictionaryPositionExecutorsSet.Where(x => x.Position.Department.Company.ClientId == ctx.CurrentClientId)
               .Select(x => new FullTextIndexItem
               {
                   DocumentId = 0,
                   ItemType = EnumObjects.DictionaryPositionExecutors,
                   OperationType = EnumOperationType.AddNew,
                   ObjectId = x.Id,
                   ObjectText = x.Description + " " + x.Agent.Name + " " + x.EndDate + " "
                                + x.Position.Name + " " + x.PositionExecutorType.Name
               }).ToList()
            );

             res.AddRange(dbContext.DictionaryPositionExecutorTypesSet
                .Select(x => new FullTextIndexItem
             {
                 DocumentId = 0,
                 ItemType = EnumObjects.DictionaryPositionExecutorTypes,
                 OperationType = EnumOperationType.AddNew,
                 ObjectId = x.Id,
                 ObjectText = x.Name + " " + x.Code
             }).ToList()
            );

                #endregion Dictionaries

                #region DocumentTemplates

                res.AddRange(dbContext.TemplateDocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                              .Select(x => new FullTextIndexItem
                              {
                                  DocumentId = 0,
                                  ItemType = EnumObjects.TemplateDocument,
                                  OperationType = EnumOperationType.AddNew,
                                  ObjectId = x.Id,
                                  ObjectText = x.Description + " " + x.Addressee + " " + x.DocumentDirection.Name + " " +
                                    x.DocumentSubject.Name + " " + x.DocumentType.Name + " " + x.Name + " " +
                                    x.RegistrationJournal.Name + " " + x.SenderAgent.Name + " " + x.SenderAgentPerson.FullName
                              }).ToList()
                       );

                res.AddRange(dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                              .Select(x => new FullTextIndexItem
                              {
                                  DocumentId = 0,
                                  ItemType = EnumObjects.TemplateDocumentSendList,
                                  OperationType = EnumOperationType.AddNew,
                                  ObjectId = x.Id,
                                  ObjectText = x.Description + " " + x.Document.Name + " " + x.SendType.Name + " " +
                                    x.SourceAgent.Name + " " + x.TargetAgent.Name + " " + x.TargetPosition.Name
                              }).ToList()
                       );

                res.AddRange(dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                              .Select(x => new FullTextIndexItem
                              {
                                  DocumentId = 0,
                                  ItemType = EnumObjects.TemplateDocumentRestrictedSendList,
                                  OperationType = EnumOperationType.AddNew,
                                  ObjectId = x.Id,
                                  ObjectText = x.Document.Name + " " + x.Position.FullName + " " + x.Position.Name
                              }).ToList()
                       );

                res.AddRange(dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                              .Select(x => new FullTextIndexItem
                              {
                                  DocumentId = 0,
                                  ItemType = EnumObjects.TemplateDocumentTask,
                                  OperationType = EnumOperationType.AddNew,
                                  ObjectId = x.Id,
                                  ObjectText = x.Document.Name + " " + x.Position.FullName + " " + x.Position.Name + " " + x.Task
                              }).ToList()
                       );

                res.AddRange(dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId)
                              .Select(x => new FullTextIndexItem
                              {
                                  DocumentId = 0,
                                  ItemType = EnumObjects.TemplateDocumentAttachedFiles,
                                  OperationType = EnumOperationType.AddNew,
                                  ObjectId = x.Id,
                                  ObjectText = x.Document.Name + " " + x.Extention + " " + x.Name
                              }).ToList()
                       );

                #endregion DocumentTemplates
            }
            return res;
        }

        public IEnumerable<FullTextIndexItem> FullTextIndexPrepare(IContext ctx)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx))
            {
                //Add deleted item to  process processing full text index
                res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType == (int)EnumOperationType.Delete)
                    .Select(x => new FullTextIndexItem
                    {
                        Id = x.Id,
                        DocumentId = (x.ObjectType == (int)EnumObjects.Documents) ? x.ObjectId : 0,
                        ItemType = (EnumObjects)x.ObjectType,
                        OperationType = (EnumOperationType)x.OperationType,
                        ObjectId = x.ObjectId,
                        ObjectText = ""
                    }).ToList());

                var objectTypesToProcess =
                    dbContext.FullTextIndexCashSet.Select(x => x.ObjectType)
                        .Distinct()
                        .ToList()
                        .Select(x => (EnumObjects)x);

                if (objectTypesToProcess.Contains(EnumObjects.Documents))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.Documents)
                        .Join(dbContext.DocumentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d })
                        .Select(x => new
                        {
                            Id = x.ind.Id,
                            DocumentId = x.doc.Id,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ObjectId = x.doc.Id,
                            v1 = (x.doc.RegistrationNumber != null
                                ? (x.doc.RegistrationNumberPrefix ?? "") + x.doc.RegistrationNumber +
                                  (x.doc.RegistrationNumberSuffix ?? "")
                                : "#" + x.doc.Id) + " ",
                            v2 = x.doc.RegistrationJournal.Name + " " + x.doc.RegistrationJournal.Department.Name + " ",
                            v3 = x.doc.Description + " ",
                            v4 = x.doc.ExecutorPositionExecutorAgent.Name + " ",
                            v5 =
                                x.doc.TemplateDocument.DocumentType.Name + " " +
                                x.doc.TemplateDocument.DocumentDirection.Name + " ",
                            v6 = x.doc.DocumentSubject.Name + " ",
                            v7 =
                                x.doc.SenderAgent.Name + " " + x.doc.SenderAgentPerson.Agent.Name + " " +
                                x.doc.SenderNumber + " ",
                        }).ToList()
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            ItemType = x.ItemType,
                            OperationType = x.OperationType,
                            ObjectId = x.ObjectId,
                            ObjectText = x.v1 + x.v2 + x.v3 + x.v4 + x.v5 + x.v6 + x.v7
                        })
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DocumentEvents))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DocumentEvents)
                            .Join(dbContext.DocumentEventsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, evt = d })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = x.evt.DocumentId,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.evt.Id,
                                ObjectText =
                                    x.evt.Description + " " + x.evt.AddDescription + " " + x.evt.Task.Task + " "
                                    + x.evt.SourcePositionExecutorAgent.Name + " " +
                                    x.evt.TargetPositionExecutorAgent.Name + " "
                                    + x.evt.SourceAgent.Name + " " + x.evt.TargetAgent.Name + " "
                            }).ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DocumentFiles))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DocumentFiles)
                            .Join(dbContext.DocumentFilesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, fl = d })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = x.fl.DocumentId,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.fl.Id,
                                ObjectText = x.fl.Name + "." + x.fl.Extension + " "
                            }).ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DocumentSendLists))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DocumentSendLists)
                            .Join(dbContext.DocumentSendListsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, sl = d })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = x.sl.DocumentId,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.sl.Id,
                                ObjectText = x.sl.Description + " " + x.sl.SendType.Name + " "
                                             + x.sl.SourcePosition.Name + " " + x.sl.TargetPosition.Name + " "
                                             + x.sl.SourcePositionExecutorAgent.Name + " " +
                                             x.sl.TargetPositionExecutorAgent.Name + " "
                            }).ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DocumentSubscriptions))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(
                            x => x.ObjectType == (int)EnumObjects.DocumentSubscriptions)
                            .Join(dbContext.DocumentSubscriptionsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, ss = d })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = x.ss.DocumentId,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.ss.Id,
                                ObjectText =
                                    x.ss.Description + " " + x.ss.SubscriptionState.Name + " " +
                                    x.ss.DoneEvent.SourcePositionExecutorAgent.Name + " "
                            }).ToList()
                        );
                }

                #region Dictionaries
                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgents))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgents)
                            .Join(dbContext.DictionaryAgentsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, agent = d,id=d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.agent.Name + " " + x.agent.Description 
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentEmployees))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentEmployees)
                            .Join(dbContext.DictionaryAgentEmployeesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, agent = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.agent.PersonnelNumber + " " + x.agent.Description + " " +
                                    x.agent.Agent.Name  
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentCompanies))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentCompanies)
                            .Join(dbContext.DictionaryAgentCompaniesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, agent = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.agent.FullName + " " +x.agent.OKPOCode + " " + x.agent.Description + " " +
                                    x.agent.TaxCode + " " + x.agent.VATCode 
                                    
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentPersons))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentPersons)
                            .Join(dbContext.DictionaryAgentPersonsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, agent = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.agent.FullName + " " +  x.agent.Description + " " + x.agent.TaxCode + " " +
                                    x.agent.BirthDate + " " + x.agent.PassportNumber + " " + x.agent.PassportSerial + " " +
                                    x.agent.PassportText

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentBanks))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentBanks)
                            .Join(dbContext.DictionaryAgentBanksSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, agent = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.agent.Agent.Name + " " + x.agent.Description + " " + x.agent.MFOCode + " " +
                                    x.agent.Swift 

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryContacts))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryContacts)
                            .Join(dbContext.DictionaryAgentContactsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, contact = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.contact.Agent.Name + " " + x.contact.Description + " " + x.contact.Contact + 
                                    " " + x.contact.ContactType.Code + " " + x.contact.ContactType.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryContactType))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryContactType)
                            .Join(dbContext.DictionaryContactTypesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, contact = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.contact.Code + " " + x.contact.Name 
                             })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentAddresses))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentAddresses)
                            .Join(dbContext.DictionaryAgentAddressesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, address = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =
                                    x.address.Agent.Name + " " + x.address.Description + " " + x.address.Address +
                                    " " + x.address.PostCode + " " + x.address.AddressType.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAddressType))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAddressType)
                            .Join(dbContext.DictionaryAddressTypesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, address = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.address.Name 
                             })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentAccounts))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryAgentAccounts)
                            .Join(dbContext.DictionaryAgentAccountsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, account = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = 
                                x.account.AccountNumber + " " + x.account.Name + " " +
                                x.account.Agent.Name + " " + x.account.AgentBank.MFOCode + " " + 
                                x.account.AgentBank.Agent.Name
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryDocumentType))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryDocumentType)
                            .Join(dbContext.DictionaryDocumentTypesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Name 
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryDocumentSubjects))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryDocumentSubjects)
                            .Join(dbContext.DictionaryDocumentSubjectsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Name 
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryRegistrationJournals))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryRegistrationJournals)
                            .Join(dbContext.DictionaryRegistrationJournalsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })
                     
                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Index + " " + x.doc.Name + " " + x.doc.Department.FullName 
                            })
                            .ToList()
                        );
                }


                if (objectTypesToProcess.Contains(EnumObjects.DictionaryDepartments))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryDepartments)
                            .Join(dbContext.DictionaryDepartmentsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =x.doc.FullName + " " + x.doc.Code + " " + x.doc.Name + " " +
                                x.doc.Company.Name + " " + x.doc.ChiefPosition.FullName
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositions))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryPositions)
                            .Join(dbContext.DictionaryPositionsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.FullName + " " + x.doc.Name + " " + x.doc.Department.Name + " " +
                                x.doc.ExecutorAgent.Name + " " + x.doc.MainExecutorAgent.Name
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryStandartSendLists))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryStandartSendLists)
                            .Join(dbContext.DictionaryStandartSendListsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Name + " " + x.doc.Position.Department.Name + " " + x.doc.Position.Name
                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryStandartSendListContent))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryStandartSendListContent)
                            .Join(dbContext.DictionaryStandartSendListContentsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText =x.doc.Task + " " + x.doc.Description + " " + x.doc.SendType.Name + x.doc.StandartSendList.Name +
                                " " + x.doc.TargetAgent.Name + " " + x.doc.TargetPosition.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryCompanies))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryCompanies)
                            .Join(dbContext.DictionaryCompaniesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositionExecutorTypes))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryPositionExecutorTypes)
                            .Join(dbContext.DictionaryPositionExecutorTypesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Name + " " + x.doc.Code

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositionExecutors))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.DictionaryPositionExecutors)
                            .Join(dbContext.DictionaryPositionExecutorsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Description + " " + x.doc.Agent.Name + " " + x.doc.EndDate + " "
                                + x.doc.Position.Name + " " + x.doc.PositionExecutorType.Name

                            })
                            .ToList()
                        );
                }

                #endregion Dictionaries

                #region TemplateDocuments

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocument))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.TemplateDocument)
                            .Join(dbContext.TemplateDocumentsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Description + " " + x.doc.Addressee + " " + x.doc.DocumentDirection.Name + " " +
                                x.doc.DocumentSubject.Name + " " + x.doc.DocumentType.Name + " " + x.doc.Name + " " +
                                x.doc.RegistrationJournal.Name + " " + x.doc.SenderAgent.Name + " " + x.doc.SenderAgentPerson.FullName

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentSendList))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.TemplateDocumentSendList)
                            .Join(dbContext.TemplateDocumentSendListsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Description + " " + x.doc.Document.Name + " " + x.doc.SendType.Name + " " +
                                x.doc.SourceAgent.Name + " " + x.doc.TargetAgent.Name + " " + x.doc.TargetPosition.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentRestrictedSendList))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.TemplateDocumentRestrictedSendList)
                            .Join(dbContext.TemplateDocumentRestrictedSendListsSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Document.Name + " " + x.doc.Position.FullName + " " + x.doc.Position.Name

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentTask))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.TemplateDocumentTask)
                            .Join(dbContext.TemplateDocumentTasksSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Document.Name + " " + x.doc.Position.FullName + " " + x.doc.Position.Name + " " +
                                x.doc.Task

                            })
                            .ToList()
                        );
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentAttachedFiles))
                {
                    res.AddRange(
                        dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)EnumObjects.TemplateDocumentAttachedFiles)
                            .Join(dbContext.TemplateDocumentFilesSet, i => i.ObjectId, d => d.Id,
                                (i, d) => new { ind = i, doc = d, id = d.Id })

                            .Select(x => new FullTextIndexItem
                            {
                                Id = x.ind.Id,
                                DocumentId = 0,
                                ItemType = (EnumObjects)x.ind.ObjectType,
                                OperationType = (EnumOperationType)x.ind.OperationType,
                                ObjectId = x.id,
                                ObjectText = x.doc.Document.Name + " " + x.doc.Extention + " " + x.doc.Name

                            })
                            .ToList()
                        );
                }

                #endregion TemplateDocuments

            }
            return res;
        }

        public void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO Contains
                dbContext.FullTextIndexCashSet.RemoveRange(
                    dbContext.FullTextIndexCashSet.Where(x => processedIds.Contains(x.Id)));
                dbContext.SaveChanges();
            }
        }

        #endregion

    }
} 