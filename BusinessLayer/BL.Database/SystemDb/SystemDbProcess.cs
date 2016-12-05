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
using LinqKit;
using System.Transactions;
using BL.Model.Tree;
using BL.Database.Common;
using EntityFramework.Extensions;
using BL.CrossCutting.Helpers;

namespace BL.Database.SystemDb
{
    public class SystemDbProcess : CoreDb.CoreDb, ISystemDbProcess
    {
        public SystemDbProcess()
        {
        }

        public void InitializerDatabase(IContext ctx)
        {

        }

        #region [+] Logs ...

        public IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemLogsQuery(context, dbContext, filter);

                qry = qry.OrderByDescending(x => x.LogDate);

                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontSystemLog>();
                    }

                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;
                    qry = qry.Skip(() => skip).Take(() => take);

                }

                var res = qry.Select(x => new FrontSystemLog
                {
                    Id = x.Id,
                    LogLevel = x.LogLevel,
                    LogLevelName = ((EnumLogTypes)x.LogLevel).ToString(),
                    Message = x.Message,
                    LogTrace = x.LogTrace,
                    LogException = x.LogException,
                    ObjectLog = x.ObjectLog,
                    ExecutorAgentId = x.ExecutorAgentId,
                    ExecutorAgent = x.Agent.Name,
                    LogDate = x.LogDate,
                    ObjectId = x.ObjectId,
                    ObjectName = x.Object.Description,
                    ActionId = x.ActionId,
                    ActionName = x.Action.Description,
                    RecordId = x.RecordId,
                    ClientId = x.ClientId,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<SystemLogs> GetSystemLogsQuery(IContext context, DmsContext dbContext, FilterSystemLog filter)
        {
            var qry = dbContext.LogSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.ObjectIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.ObjectIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ObjectId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.ActionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.ActionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ActionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.ExecutorAgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.ExecutorAgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorAgentId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.ExecutorAgentName))
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.ExecutorAgentName)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.RecordIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.RecordIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RecordId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.LogLevels?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = filter.LogLevels.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.LogLevel == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.LogDateFrom.HasValue)
                {
                    qry = qry.Where(x => x.LogDate >= filter.LogDateFrom.Value);
                }

                if (filter.LogDateTo.HasValue)
                {
                    qry = qry.Where(x => x.LogDate <= filter.LogDateTo.Value);
                }
                if (!String.IsNullOrEmpty(filter.Message))
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Message)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.Message.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.LogTrace))
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.LogTrace)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.LogTrace.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.LogException))
                {
                    var filterContains = PredicateBuilder.False<SystemLogs>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.LogException)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.LogException.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public int AddLog(IContext ctx, LogInfo log)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var nlog = new SystemLogs
                {
                    ClientId = ctx.CurrentClientId,
                    ExecutorAgentId = log.AgentId,
                    LogDate = log.Date,
                    LogLevel = (int)log.LogType,
                    LogException = log.LogException,
                    LogTrace = log.LogTrace,
                    ObjectLog = log.LogObject,
                    Message = log.Message,
                    ObjectId = log.ObjectId,
                    ActionId = log.ActionId,
                    RecordId = log.RecordId,
                };
                dbContext.LogSet.Add(nlog);
                dbContext.SaveChanges();
                transaction.Complete();
                return nlog.Id;
            }
        }

        #endregion

        #region [+] Settings ...

        public int MergeSetting(IContext ctx, InternalSystemSetting model)
        {
            var res = 0;
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var cset = dbContext.SettingsSet.FirstOrDefault(x => ctx.CurrentClientId == x.ClientId && x.Key == model.Key);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
                        ClientId = ctx.CurrentClientId,
                        ExecutorAgentId = model.AgentId,
                        Key = model.Key,
                        Value = model.Value,
                        ValueTypeId = (int)model.ValueType,
                        AccessType = model.AccessType,
                        Name = model.Name,
                        Description = model.Description,
                        Order = model.Order,
                        SettingTypeId = model.SettingTypeId,
                    };
                    dbContext.SettingsSet.Add(nsett);
                    dbContext.SaveChanges();
                    res = nsett.Id;
                }
                else
                {
                    cset.Value = model.Value;

                    if (model.ValueType > 0)
                    {
                        cset.ValueTypeId = (int)model.ValueType;
                    }

                    cset.ExecutorAgentId = model.AgentId;
                    dbContext.SaveChanges();
                    res = cset.Id;
                }
                transaction.Complete();
                return res;
            }
        }

        public string GetSettingValue(IContext ctx, FilterSystemSetting filter)
        {
            var res = string.Empty;
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                if (filter.AgentId.HasValue)
                {
                    res = dbContext.SettingsSet.Where(
                            x =>
                                ctx.CurrentClientId == x.ClientId && x.Key == filter.Key && x.ExecutorAgentId == filter.AgentId.Value)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                }
                else
                {
                    res = dbContext.SettingsSet.Where(x => ctx.CurrentClientId == x.ClientId && x.Key == filter.Key)
                            .OrderBy(x => x.ExecutorAgentId)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                }
                transaction.Complete();
                return res;
            }
        }

        /// <summary>
        /// Возвращает список настройек. ВНИМАНИЕ!!! Значения полей типа password заменяются на NULL
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<FrontSystemSetting> GetSystemSettings(IContext ctx, FilterSystemSetting filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSettingsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontSystemSetting()
                {
                    Id = x.Id,
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = (EnumValueTypes)x.ValueTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    ValueTypeCode = x.ValueTypes.Code,
                    SettingTypeName = x.SettingType.Name,
                    Order = x.Order,
                    OrderSettingType = x.SettingType.Order,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<InternalSystemSetting> GetSystemSettingsInternal(IContext ctx, FilterSystemSetting filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSettingsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalSystemSetting()
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = (EnumValueTypes)x.ValueTypeId,
                    AgentId = x.ExecutorAgentId,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<SystemSettings> GetWhereSettings(ref IQueryable<SystemSettings> qry, FilterSystemSetting filter)
        {
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemSettings>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.Key))
                {
                    qry = qry.Where(x => x.Key == filter.Key);
                }

                if (!string.IsNullOrEmpty(filter.Value))
                {
                    qry = qry.Where(x => x.Value == filter.Value);
                }

                if (filter.AgentId.HasValue)
                {
                    qry = qry.Where(x => x.ExecutorAgentId == filter.AgentId.Value);
                }
            }
            return qry;
        }

        public IQueryable<SystemSettings> GetSettingsQuery(IContext context, DmsContext dbContext, FilterSystemSetting filter)
        {
            var qry = dbContext.SettingsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Id);

            qry = GetWhereSettings(ref qry, filter);

            return qry;
        }

        #endregion

        public IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter)
        {


            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemUIElementsSet.AsQueryable();

                if (filter.UIElementId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemUIElements>();
                    filterContains = filter.UIElementId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.ActionId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemUIElements>();
                    filterContains = filter.ActionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ActionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.ObjectId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemUIElements>();
                    filterContains = filter.ObjectId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Action.ObjectId == value).Expand());
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
                qry = qry.OrderBy(x => x.Order);
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
                transaction.Complete();
                return res;
            }

        }

        #region [+] SystemObjects ...
        public void AddSystemObject(IContext context, InternalSystemObject item)
        {
            AddSystemObject(context, SystemModelConverter.GetDbSystemObject(context, item));
        }

        public void UpdateSystemObject(IContext context, InternalSystemObject item)
        {
            UpdateSystemObject(context, SystemModelConverter.GetDbSystemObject(context, item));
        }

        public void DeleteSystemObjects(IContext context, FilterSystemObject filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemObjectsQuery(context, dbContext, filter);

                var objects = qry.Select(x => x.Id).ToList();

                if (objects.Count > 0)
                {
                    DeleteSystemActions(context, new FilterSystemAction() { ObjectIDs = objects });

                    qry.Delete();
                }
                transaction.Complete();
            }
        }

        public void AddSystemObject(IContext context, SystemObjects item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = dbContext.Database.BeginTransaction())
            {
                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemObjects] ON");

                dbContext.Database.ExecuteSqlCommand(
                String.Format(@"INSERT INTO[DMS].[SystemObjects]
                (Id, Code, [Description]) 
                VALUES
                ({0},{1},{2})",
                item.Id, "'" + item.Code + "'", "'" + item.Description + "'")
                );

                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemObjects] OFF");

                transaction.Commit();
            }
        }

        public void UpdateSystemObject(IContext context, SystemObjects item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.SystemObjectsSet.Attach(item);
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemObjectsQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontSystemObject
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        //public IEnumerable<FrontSystemObject> GetSystemObjectsWithActions(IContext context, FilterSystemObject filterObject, FilterSystemAction filterAction)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    using (var transaction = Transactions.GetTransaction())
        //    {
        //        var qry = GetSystemObjectsQuery(context, dbContext, filterObject);

        //        var filterContains = PredicateBuilder.False<SystemActions>();

        //        //filterContains = GetWhereSystemActions(x.Actions, filterAction);

        //        //CommonFilterUtilites.GetWhereExpressions(filter.Description).Aggregate(filterContains,
        //        //    (current, value) => current.Or(e => e.Description.Contains(value)).Expand());

        //        //qry = qry.Where(filterContains);

        //        return qry.Select(x => new FrontSystemObject
        //        {
        //            Id = x.Id,
        //            Code = x.Code,
        //            Description = x.Description,
        //            Actions = x.Actions.AsQueryable()
        //            .Where(filterContains)
        //            .Select(y => new FrontSystemAction()
        //            {
        //                Id = y.Id,
        //                Code = y.Code,
        //                Description = y.Description
        //            }
        //            ).ToList(),

        //        }).ToList();
        //    }
        //}
        public IEnumerable<int> GetObjectsByActions(IContext context, FilterSystemAction filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemActionsSet.AsQueryable();

                qry = GetWhereSystemActions(ref qry, filter);

                var res = qry.Select(x => x.ObjectId).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetSystemObjectsForTree(IContext context, int roleId, FilterSystemObject filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemObjectsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Id);

                var res = qry.Select(x => new FrontSystemObjectForDIP
                {
                    Id = x.Id,
                    Name = x.Description,
                    SearchText = x.Description,
                    TreeId = string.Concat(x.Id.ToString(), "_", (int)EnumObjects.SystemObjects),
                    TreeParentId = string.Empty,
                    ObjectId = (int)EnumObjects.SystemObjects,
                    IsActive = true,
                    IsList = !(x.Actions.Where(y => y.ObjectId == x.Id).Any()),
                    RoleId = roleId,
                    SystemObjectId = x.Id,
                }).ToList();

                transaction.Complete();

                return res;

            }
        }

        public IEnumerable<TreeItem> GetSystemActionsForTree(IContext context, int roleId, FilterSystemAction filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemActionsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Id);

                var res = qry.Select(x => new FrontSystemActionForDIP
                {
                    Id = x.Id,
                    Name = x.Description,
                    SearchText = x.Description,
                    TreeId = string.Concat(x.Id.ToString(), "_", (int)EnumObjects.SystemActions),
                    TreeParentId = string.Concat(x.ObjectId.ToString(), "_", (int)EnumObjects.SystemObjects),
                    ObjectId = (int)EnumObjects.SystemActions,
                    IsActive = true,
                    IsList = true,
                    IsChecked = x.RoleActions.Any(y => y.RoleId == roleId & !y.RecordId.HasValue),
                    RoleId = roleId,
                    ActionId = x.Id,
                }).ToList();

                transaction.Complete();

                return res;

            }
        }

        public IQueryable<SystemObjects> GetSystemObjectsQuery(IContext context, DmsContext dbContext, FilterSystemObject filter)
        {
            var qry = dbContext.SystemObjectsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemObjects>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<SystemObjects>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Description))
                {
                    var filterContains = PredicateBuilder.False<SystemObjects>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Description.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        #endregion

        #region [+] SystemActions

        public void AddSystemAction(IContext context, InternalSystemAction item)
        {
            AddSystemAction(context, SystemModelConverter.GetDbSystemAction(context, item));
        }

        public void UpdateSystemAction(IContext context, InternalSystemAction item)
        {
            UpdateSystemAction(context, SystemModelConverter.GetDbSystemAction(context, item));
        }

        public void DeleteSystemActions(IContext context, FilterSystemAction filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemActionsQuery(context, dbContext, filter);

                var actions = qry.Select(x => x.Id).ToList();

                if (actions.Count() > 0)
                {
                    dbContext.AdminRoleActionsSet.Where(x => actions.Contains(x.ActionId)).Delete();
                }

                qry.Delete();

                transaction.Complete();

            }

        }

        public void AddSystemAction(IContext context, SystemActions item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemActions] ON");

                dbContext.Database.ExecuteSqlCommand(
                String.Format(@"INSERT INTO[DMS].[SystemActions]
                (Id, ObjectId, Code, API, [Description], IsGrantable, IsGrantableByRecordId, IsVisible, IsVisibleInMenu,  GrantId, Category) 
                VALUES
                ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})",
                item.Id, item.ObjectId, "'" + item.Code + "'", "'" + item.API + "'", "'" + item.Description + "'", item.IsGrantable ? 1 : 0, item.IsGrantableByRecordId ? 1 : 0, item.IsVisible ? 1 : 0, item.IsVisibleInMenu ? 1 : 0, item.GrantId.ToString() == string.Empty ? "null" : item.GrantId.ToString(), item.Category ?? "null")
                );

                //dbContext.SystemActionsSet.Add(item);

                //dbContext.SaveChanges();

                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemActions] OFF");

                transaction.Complete();
            }
        }

        public void UpdateSystemAction(IContext context, SystemActions item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.SystemActionsSet.Attach(item);
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<FrontSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemActionsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontSystemAction
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    ObjectId = x.ObjectId,
                    ObjectCode = x.Object.Code,
                    ObjectDescription = x.Object.Description
                }).ToList();

                transaction.Complete();

                return res;
            }
        }



        public IEnumerable<InternalSystemAction> GetInternalSystemActions(IContext ctx, FilterSystemAction filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemActionsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalSystemAction
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    API = x.API,
                    Category = x.Category,
                    GrantId = x.GrantId,
                    IsGrantable = x.IsGrantable,
                    IsGrantableByRecordId = x.IsGrantableByRecordId,
                    IsVisible = x.IsVisible,
                    ObjectId = (EnumObjects)x.ObjectId
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IQueryable<SystemActions> GetSystemActionsQuery(IContext context, DmsContext dbContext, FilterSystemAction filter)
        {
            var qry = dbContext.SystemActionsSet.AsQueryable();

            qry = GetWhereSystemActions(ref qry, filter);

            return qry;
        }

        private IQueryable<SystemActions> GetWhereSystemActions(ref IQueryable<SystemActions> qry, FilterSystemAction filter)
        {
            if (filter == null) return qry;

            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<SystemActions>();
                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<SystemActions>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }
            if (filter.ObjectIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<SystemActions>();
                filterContains = filter.ObjectIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ObjectId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                var filterContains = PredicateBuilder.False<SystemActions>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Description.Contains(value)).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.IsGrantable.HasValue)
            {
                qry = qry.Where(x => x.IsGrantable == filter.IsGrantable);
            }

            if (filter.IsGrantableByRecordId.HasValue)
            {
                qry = qry.Where(x => x.IsGrantableByRecordId == filter.IsGrantableByRecordId);
            }

            if (filter.IsVisible.HasValue)
            {
                qry = qry.Where(x => x.IsVisible == filter.IsVisible);
            }

            return qry;
        }

        #endregion


        public IEnumerable<FrontSystemFormat> GetSystemFormats(IContext ctx, FilterSystemFormat filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemFormatsSet.AsQueryable();

                var res = qry.Select(x => new FrontSystemFormat
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontSystemFormula> GetSystemFormulas(IContext ctx, FilterSystemFormula filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemFormulasSet.AsQueryable();

                var res = qry.Select(x => new FrontSystemFormula
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Example = x.Example
                }).ToList();

                transaction.Complete();

                return res;
            }
        }
        public IEnumerable<FrontSystemPattern> GetSystemPatterns(IContext ctx, FilterSystemPattern filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemPatternsSet.AsQueryable();

                var res = qry.Select(x => new FrontSystemPattern
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }
        public IEnumerable<FrontSystemValueType> GetSystemValueTypes(IContext ctx, FilterSystemValueType filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemValueTypesSet.AsQueryable();

                var res = qry.Select(x => new FrontSystemValueType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                }).ToList();

                transaction.Complete();

                return res;

            }
        }



        #region [+] Properties ...

        public IEnumerable<BaseSystemUIElement> GetPropertyUIElements(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry =
                    dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter.PropertyLinkId != null)
                {
                    var filterContains = PredicateBuilder.False<PropertyLinks>();
                    filterContains = filter.PropertyLinkId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => new BaseSystemUIElement
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

                transaction.Complete();

                return res;
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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPropertiesQuery(dbContext, ctx, filter);

                var res = qry.Select(x => new FrontProperty
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
                    ValueType = !x.ValueTypeId.HasValue
                        ? null
                        : new InternalSystemValueType
                        {
                            Id = x.ValueType.Id,
                            Code = x.ValueType.Code,
                            Description = x.ValueType.Description
                        }
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public int AddProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return item.Id;
            }
        }

        public void UpdateProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeleteProperty(IContext ctx, InternalProperty model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var item =
                    dbContext.PropertiesSet.FirstOrDefault(x => ctx.CurrentClientId == x.ClientId && x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertiesSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                var res = qry.Select(x => new InternalPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    Object = (EnumObjects)x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                var res = qry.Select(x => new FrontPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    Object = (EnumObjects)x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory,
                    SystemObject = new FrontSystemObject { Code = x.Object.Code }
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public int AddPropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
                return item.Id;
            }
        }

        public void UpdatePropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeletePropertyLink(IContext ctx, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertyLinksSet.Remove(item);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
        }

        #endregion PropertyLinks

        #region PropertyValues

        public IEnumerable<FrontPropertyValue> GetPropertyValuesToDocumentFromTemplateDocument(IContext ctx, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPropertyLinksQuery(dbContext, ctx, filter);

                //TODO проверить запрос
                qry = qry.Select(
                    x =>
                        x.Property.Links.FirstOrDefault(
                            y => y.ObjectId == (int)EnumObjects.Documents && y.Filers == x.Filers))
                    .Where(x => x != null);

                var res = qry.Select(x => new FrontPropertyValue
                {
                    PropertyLinkId = x.Id,
                    PropertyCode = x.Property.Code
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        #endregion PropertyValues

        #region Mailing

        public IEnumerable<InternalDataForMail> GetNewActionsForMailing(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                // RODO DestinationAgentEmail = "sergozubr@rambler.ru"
                var res = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
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
                transaction.Complete();

                return res;
            }
        }

        public void MarkActionsLikeMailSended(IContext ctx, InternalMailProcessed mailProcessed)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        #endregion

        #region Filter Properties

        public IEnumerable<BaseSystemUIElement> GetFilterProperties(IContext ctx, FilterProperties filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.PropertyLinksSet.Where(x => x.Property.ClientId == ctx.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.ObjectId == (int)filter.Object);

                var res = qry.Select(x => new BaseSystemUIElement
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

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<int> GetSendListIdsForAutoPlan(IContext ctx, int? sendListId = null, int? documentId = null)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                if (sendListId.HasValue)
                    return new List<int> { sendListId.GetValueOrDefault() };

                var qryPrepare = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Where(x => x.IsInitial && !x.CloseEventId.HasValue).AsQueryable();

                if (documentId.HasValue)
                {
                    qryPrepare = qryPrepare.Where(x => x.DocumentId == documentId);
                }

                var qry = qryPrepare.GroupBy(x => x.DocumentId)
                    .Select(x => new
                    {
                        DocId = x.Key,
                        MinStage = x.Min(s => s.Stage)
                    });

                var sendListsSet = dbContext.DocumentSendListsSet
                                    .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                    .Where(x => x.Document.IsLaunchPlan)
                                    .AsQueryable();

                if (documentId.HasValue)
                {
                    sendListsSet = sendListsSet.Where(x => x.DocumentId == documentId);
                }

                var qry2 = sendListsSet
                    .Join(qry, s => s.DocumentId, q => q.DocId, (s, q) => new { sl = s, q })
                    .Where(x => x.sl.Stage <= x.q.MinStage && !x.sl.StartEventId.HasValue)
                    .OrderBy(x => x.sl.DocumentId)
                    .ThenBy(
                        x =>
                            new
                            {
                                x.sl.Stage,
                                SendTypeId = x.sl.SendTypeId == (int)EnumSendTypes.SendForControl ? 0 : x.sl.SendTypeId
                            })
                    .Select(x => x.sl.Id);

                if (!documentId.HasValue)
                {
                    qry2 = qry2.Take(50);
                }

                var res = qry2.ToList();

                var qry3 = sendListsSet
                    .Where(x => !x.IsInitial && !x.StartEventId.HasValue
                                && !qry.Select(s => s.DocId).Contains(x.DocumentId))
                    .OrderBy(x => x.DocumentId)
                    .ThenBy(
                        x =>
                            new
                            {
                                x.Stage,
                                SendTypeId = x.SendTypeId == (int)EnumSendTypes.SendForControl ? 0 : x.SendTypeId
                            })
                    .Select(x => x.Id);

                if (!documentId.HasValue)
                {
                    qry3 = qry3.Take(50);
                }

                res.AddRange(qry3.ToList());

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<int> GetDocumentIdsForClearTrashDocuments(IContext ctx, int timeMinForClearTrashDocuments)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var date = DateTime.UtcNow.AddMinutes(-timeMinForClearTrashDocuments);
                var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(
                        x =>
                            !x.IsRegistered.HasValue && !x.Waits.Any() && !x.Subscriptions.Any() &&
                            x.LastChangeDate < date)
                    .Select(x => x.Id);

                var res = qry.ToList();

                transaction.Complete();

                return res;
            }
        }

        #endregion Filter Properties

        #region Full text search

        public int GetEntityNumbers(IContext ctx, EnumObjects objType)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                int res = 0;
                switch (objType)
                {
                    case EnumObjects.Documents:
                        res = dbContext.DocumentsSet.Count(x => x.TemplateDocument.ClientId == ctx.CurrentClientId);
                        break;
                    case EnumObjects.DocumentSendLists:
                        res = dbContext.DocumentSendListsSet.Count(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);
                        break;
                    case EnumObjects.DocumentFiles:
                        res = dbContext.DocumentFilesSet.Count(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId && !x.IsDeleted);
                        break;
                    case EnumObjects.DocumentEvents:
                        res = dbContext.DocumentEventsSet.Count(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);
                        break;
                    case EnumObjects.DocumentSubscriptions:
                        res = dbContext.DocumentSubscriptionsSet.Count(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);
                        break;

                    default:
                        return 0;
                }

                transaction.Complete();

                return res;
            }
        }

        public int GetCurrentMaxCasheId(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.FullTextIndexCashSet.Any() ? dbContext.FullTextIndexCashSet.Max(x => x.Id) : 0;
                transaction.Complete();
                return res;
            }
        }

        //private string Concat(params object[] values) => string.Join(" ", values);

        // перепостраивает все индексы, относящиеся к общей части
        public IEnumerable<FullTextIndexItem> FullTextIndexNonDocumentsReindexDbPrepare(IContext ctx)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;

                #region Dictionaries

                res.AddRange(dbContext.DictionaryAgentsSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgents,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Name + " " + x.Description
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentEmployeesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentEmployees,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.PersonnelNumber + " " + x.Description + " " + x.Agent.Name + " " + x.Agent.AgentPerson.FullName + " " + x.Agent.AgentPerson.BirthDate + " " + x.Agent.AgentPerson.PassportDate + " " + x.Agent.AgentPerson.PassportNumber + " " + x.Agent.AgentPerson.PassportSerial + " " + x.Agent.AgentPerson.PassportText + " " + x.Agent.AgentPerson.TaxCode
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentCompaniesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentCompanies,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.FullName + " " + x.OKPOCode + " " + x.Description + " " + x.TaxCode + " " + x.VATCode
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentPersons,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.FullName + " " + x.Description + " " + x.TaxCode + " " + x.BirthDate + " " + x.PassportNumber + " " + x.PassportSerial + " " + x.PassportText
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentBanks,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.FullName + " " + x.Description + " " + x.MFOCode + " " + x.Swift
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryContacts,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.Description + " " + x.Contact + " " + x.ContactType.Code + " " + x.ContactType.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryContactTypesSet.Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryContactType,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Code + " " + x.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentAddresses,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.Description + " " + x.Address + " " + x.PostCode + " " + x.AddressType.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryAddressTypesSet.Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAddressType,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Code + " " + x.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentAccounts,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Agent.Name + " " + x.AgentBank.FullName + " " + x.AccountNumber + " " + x.Name + " " + x.AgentBank.MFOCode
                }).ToList());

                // не должны добавляться в полнотекст т.к. значения не локализованы
                //res.AddRange(dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                //{
                //    DocumentId = 0,
                //    ItemType = EnumObjects.DictionaryDocumentType,
                //    OperationType = EnumOperationType.AddNew,
                //    ClientId = ctx.CurrentClientId,
                //    ObjectId = x.Id,
                //    ObjectText = x.Name
                //}).ToList());

                res.AddRange(dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryDocumentSubjects,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryRegistrationJournals,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Index + " " + x.Name + " " + x.Department.FullName + " " + x.Department.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryDepartments,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.FullName + " " + x.Code + " " + x.Name + " " + x.Company.FullName + " " + x.ChiefPosition.FullName
                }).ToList());

                res.AddRange(dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryPositions,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.FullName + " " + x.Name + " " + x.Department.Name + " " + x.ExecutorAgent.Name + " " + x.MainExecutorAgent.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryStandartSendLists,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Name + " " + x.Position.Department.Name + " " + x.Position.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryStandartSendListContent,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Task + " " + x.Description + " " + x.SendType.Name + x.StandartSendList.Name + " " + x.TargetAgent.Name + " " + x.TargetPosition.Name
                }).ToList());

                res.AddRange(dbContext.DictionaryAgentClientCompaniesSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryAgentClientCompanies,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.FullName
                }).ToList());

                res.AddRange(dbContext.DictionaryPositionExecutorsSet.Where(x => x.Position.Department.Company.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryPositionExecutors,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Description + " " + x.Agent.Name + " " + x.EndDate + " " + x.Position.Name //+ " " + x.PositionExecutorType.Name // не должны добавляться в полнотекст т.к. значения не локализованы
                }).ToList());

                res.AddRange(dbContext.DictionaryPositionExecutorTypesSet.Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.DictionaryPositionExecutorTypes,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Name + " " + x.Code
                }).ToList());

                #endregion Dictionaries

                #region DocumentTemplates

                res.AddRange(dbContext.TemplateDocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.TemplateDocument,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Description + " " + x.Addressee + " " /*+ x.DocumentDirection.Name + " "*/ + x.DocumentSubject.Name /*+ " " + x.DocumentType.Name */+ " " + x.Name + " " + x.RegistrationJournal.Name + " " + x.SenderAgent.Name + " " + x.SenderAgentPerson.FullName
                }).ToList());

                res.AddRange(dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.TemplateDocumentSendList,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Description + " " + x.Document.Name + /*" " + x.SendType.Name +*/ " " + x.SourceAgent.Name + " " + x.TargetAgent.Name + " " + x.TargetPosition.Name
                }).ToList());

                res.AddRange(dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.TemplateDocumentRestrictedSendList,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Document.Name + " " + x.Position.FullName + " " + x.Position.Name
                }).ToList());

                res.AddRange(dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.TemplateDocumentTask,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Document.Name + " " + x.Position.FullName + " " + x.Position.Name + " " + x.Task
                }).ToList());

                res.AddRange(dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Select(x => new FullTextIndexItem
                {
                    DocumentId = 0,
                    ItemType = EnumObjects.TemplateDocumentAttachedFiles,
                    OperationType = EnumOperationType.AddNew,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.Id,
                    ObjectText = x.Document.Name + " " + x.Extention + " " + x.Name
                }).ToList());

                #endregion DocumentTemplates
                transaction.Complete();
            }
            return res;
        }

        public IEnumerable<FullTextIndexItem> FullTextIndexToDeletePrepare(IContext ctx)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;
                //Add deleted item to  process processing full text index
                res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType == (int)EnumOperationType.Delete).Select(x => new FullTextIndexItem
                {
                    Id = x.Id,
                    DocumentId = (x.ObjectType == (int)EnumObjects.Documents) ? x.ObjectId : 0,
                    ItemType = (EnumObjects)x.ObjectType,
                    OperationType = (EnumOperationType)x.OperationType,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.ObjectId,
                    ObjectText = ""
                }).ToList());
                transaction.Complete();
            }
            return res;
        }

        // перепостраивает поисковый индекс для указанного документа
        public IEnumerable<FullTextIndexItem> FullTextIndexOneDocumentReindexDbPrepare(IContext ctx, int selectBis)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;

                res.AddRange(dbContext.FullTextIndexCashSet.Where(x =>
                            x.Id <= selectBis && x.OperationType == (int)EnumOperationType.UpdateDocument &&
                            x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId)
                        .ThenBy(x => x.Id)
                        .Join(dbContext.DocumentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d })
                        .Where(x => x.doc.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.doc.Id,
                            ItemType = EnumObjects.Documents,
                            OperationType = EnumOperationType.UpdateDocument,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.doc.Id,
                            ObjectText = (x.doc.RegistrationNumber != null
                                ? (x.doc.RegistrationNumberPrefix ?? "") + x.doc.RegistrationNumber +
                                  (x.doc.RegistrationNumberSuffix ?? "")
                                : "#" + x.doc.Id) + " " + x.doc.RegistrationJournal.Name + " " +
                                         x.doc.RegistrationJournal.Department.Name + " "
                                         + x.doc.Description + " "
                                         + x.doc.ExecutorPositionExecutorAgent.Name + " "
                                         //+ x.doc.TemplateDocument.DocumentType.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                                         //+ x.doc.TemplateDocument.DocumentDirection.Name + " " 
                                         + x.doc.DocumentSubject.Name + " "
                                         + x.doc.DocumentSubject.Name + " "
                                         + x.doc.SenderAgent.Name + " "
                                         + x.doc.SenderAgentPerson.Agent.Name + " " +
                                         x.doc.SenderNumber + " "
                        }).ToList());

                res.AddRange(dbContext.FullTextIndexCashSet.Where(x =>
                            x.Id <= selectBis && x.OperationType == (int)EnumOperationType.UpdateDocument &&
                            x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentEventsSet, i => i.ObjectId, d => d.DocumentId,
                            (i, d) => new { ind = i, evt = d })
                        .Where(x => x.evt.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.evt.DocumentId,
                            ItemType = EnumObjects.DocumentEvents,
                            OperationType = EnumOperationType.UpdateDocument,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.evt.Id,
                            ObjectText =
                                x.evt.Description + " " + x.evt.AddDescription + " " + x.evt.Task.Task + " " +
                                x.evt.SourcePositionExecutorAgent.Name + " " + x.evt.TargetPositionExecutorAgent.Name +
                                " " + x.evt.SourceAgent.Name + " " + x.evt.TargetAgent.Name + " "
                        }).ToList());

                res.AddRange(dbContext.FullTextIndexCashSet.Where(x =>
                            x.Id <= selectBis && x.OperationType == (int)EnumOperationType.UpdateDocument &&
                            x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentSendListsSet, i => i.ObjectId, d => d.DocumentId,
                            (i, d) => new { ind = i, sl = d })
                        .Where(x => x.sl.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.sl.DocumentId,
                            ItemType = EnumObjects.DocumentSendLists,
                            OperationType = EnumOperationType.UpdateDocument,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.sl.Id,
                            ObjectText =
                                x.sl.Description + " "
                                //+ x.sl.SendType.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                                + x.sl.SourcePosition.Name + " " +
                                x.sl.TargetPosition.Name + " "
                                + x.sl.SourcePositionExecutorAgent.Name + " "
                                + x.sl.TargetPositionExecutorAgent.Name
                        }).ToList());

                res.AddRange(dbContext.FullTextIndexCashSet.Where(x =>
                            x.Id <= selectBis && x.OperationType == (int)EnumOperationType.UpdateDocument &&
                            x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(
                            dbContext.DocumentFilesSet.Where(
                                x => !x.IsDeleted && x.Document.TemplateDocument.ClientId == ctx.CurrentClientId),
                            i => i.ObjectId, d => d.DocumentId, (i, d) => new { ind = i, fl = d })
                        .Where(x => x.fl.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.fl.DocumentId,
                            ItemType = EnumObjects.DocumentFiles,
                            OperationType = EnumOperationType.UpdateDocument,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.fl.Id,
                            ObjectText = x.fl.Name + "." + x.fl.Extension + " "
                        }).ToList());

                res.AddRange(dbContext.FullTextIndexCashSet.Where(x =>
                            x.Id <= selectBis && x.OperationType == (int)EnumOperationType.UpdateDocument &&
                            x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentSubscriptionsSet, i => i.ObjectId, d => d.DocumentId,
                            (i, d) => new { ind = i, ss = d })
                        .Where(x => x.ss.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.ss.DocumentId,
                            ItemType = EnumObjects.DocumentSubscriptions,
                            OperationType = EnumOperationType.UpdateDocument,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.ss.Id,
                            ObjectText =
                                x.ss.Description + " "
                            //+ x.ss.SubscriptionState.Name + " " // не должны добавляться в полнотекст т.к. значения не локализованы
                            + x.ss.DoneEvent.SourcePositionExecutorAgent.Name + " "
                        }).ToList());

                transaction.Complete();
            }

            return res;
        }

        // перепостраивает все индексы, относящиеся к документооборотной части
        public IEnumerable<FullTextIndexItem> FullTextIndexDocumentsReindexDbPrepare(IContext ctx, EnumObjects objType, int rowToSelect, int rowOffset)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;
                if (objType == EnumObjects.Documents)
                {
                    res.AddRange(dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .OrderBy(x => x.Id)
                        .Select(x => new FullTextIndexItem
                        {
                            DocumentId = x.Id,
                            ClientId = ctx.CurrentClientId,
                            ItemType = EnumObjects.Documents,
                            OperationType = EnumOperationType.AddNew,
                            ObjectId = x.Id,
                            ObjectText = (x.RegistrationNumber != null
                                ? (x.RegistrationNumberPrefix ?? "") + x.RegistrationNumber +
                                  (x.RegistrationNumberSuffix ?? "")
                                : "#" + x.Id) + " " + x.RegistrationJournal.Name + " " +
                                         x.RegistrationJournal.Department.Name + " "
                                         + x.Description + " "
                                         + x.ExecutorPositionExecutorAgent.Name + " "
                                         //+ x.TemplateDocument.DocumentType.Name + " " // не должны добавляться в полнотекст т.к. значения не локализованы
                                         //+x.TemplateDocument.DocumentDirection.Name + " " 
                                         + x.DocumentSubject.Name + " "
                                         + x.DocumentSubject.Name + " "
                                         + x.SenderAgent.Name + " " + x.SenderAgentPerson.Agent.Name + " " +
                                         x.SenderNumber + " "
                        }).Skip(() => rowOffset).Take(() => rowToSelect).ToList());
                }
                else if (objType == EnumObjects.DocumentEvents)
                {
                    res.AddRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).OrderBy(x => x.Id).Select(x => new FullTextIndexItem
                    {
                        DocumentId = x.DocumentId,
                        ItemType = EnumObjects.DocumentEvents,
                        OperationType = EnumOperationType.AddNew,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.Id,
                        ObjectText = x.Description + " "
                        + x.AddDescription + " "
                        + x.Task.Task + " "
                        + x.SourcePositionExecutorAgent.Name + " "
                        + x.TargetPositionExecutorAgent.Name + " "
                        + x.SourceAgent.Name + " "
                        + x.TargetAgent.Name + " "
                    }).Skip(() => rowOffset).Take(() => rowToSelect).ToList());
                }
                else if (objType == EnumObjects.DocumentFiles)
                {
                    res.AddRange(dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => !x.IsDeleted).OrderBy(x => x.Id).Select(x => new FullTextIndexItem
                    {
                        DocumentId = x.DocumentId,
                        ItemType = EnumObjects.DocumentFiles,
                        OperationType = EnumOperationType.AddNew,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.Id,
                        ObjectText = x.Name + "." + x.Extension + " "
                    }).Skip(() => rowOffset).Take(() => rowToSelect).ToList());
                }
                else if (objType == EnumObjects.DocumentSendLists)
                {
                    res.AddRange(dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).OrderBy(x => x.Id).Select(x => new FullTextIndexItem
                    {
                        DocumentId = x.DocumentId,
                        ItemType = EnumObjects.DocumentSendLists,
                        OperationType = EnumOperationType.AddNew,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.Id,
                        ObjectText = x.Description + " "
                        //+ x.SendType.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                        + x.SourcePosition.Name + " "
                        + x.TargetPosition.Name + " "
                        + x.SourcePositionExecutorAgent.Name + " "
                        + x.TargetPositionExecutorAgent.Name + " "
                    }).Skip(() => rowOffset).Take(() => rowToSelect).ToList());
                }
                else if (objType == EnumObjects.DocumentSubscriptions)
                {
                    res.AddRange(dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).OrderBy(x => x.Id).Select(x => new FullTextIndexItem
                    {
                        DocumentId = x.DocumentId,
                        ItemType = EnumObjects.DocumentSubscriptions,
                        OperationType = EnumOperationType.AddNew,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.Id,
                        ObjectText = x.Description + " "
                        //+ x.SubscriptionState.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                        + x.DoneEvent.SourcePositionExecutorAgent.Name + " "
                    }).Skip(() => rowOffset).Take(() => rowToSelect).ToList());
                }
                transaction.Complete();
            }
            return res;
        }

        // создание поисковых индексов в рабочем режиме по таблице FullTextIndexCashSet, относящихся к документу
        public IEnumerable<FullTextIndexItem> FullTextIndexDocumentsPrepare(IContext ctx, EnumObjects objType, int rowToSelect, int selectBis)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;

                if (objType == EnumObjects.Documents)
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(
                        x =>
                            x.Id <= selectBis && x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.Documents)
                        .OrderBy(x => x.ObjectId)
                        .ThenBy(x => x.Id)
                        .Join(dbContext.DocumentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d })
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.doc.Id,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.doc.Id,
                            ObjectText = (x.doc.RegistrationNumber != null
                                ? (x.doc.RegistrationNumberPrefix ?? "") + x.doc.RegistrationNumber +
                                  (x.doc.RegistrationNumberSuffix ?? "")
                                : "#" + x.doc.Id) + " " + x.doc.RegistrationJournal.Name + " " +
                                         x.doc.RegistrationJournal.Department.Name + " "
                                         + x.doc.Description + " "
                                         + x.doc.ExecutorPositionExecutorAgent.Name + " "
                                         //+ x.doc.TemplateDocument.DocumentType.Name + " " // не должны добавляться в полнотекст т.к. значения не локализованы
                                         //+x.doc.TemplateDocument.DocumentDirection.Name + " "
                                         + x.doc.DocumentSubject.Name + " "
                                         + x.doc.DocumentSubject.Name + " "
                                         + x.doc.SenderAgent.Name + " " + x.doc.SenderAgentPerson.Agent.Name + " " +
                                         x.doc.SenderNumber + " "
                        }).Take(() => rowToSelect).ToList());
                }

                if (objType == EnumObjects.DocumentEvents)
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.Id <= selectBis && x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DocumentEvents).OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentEventsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, evt = d })
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.evt.DocumentId,
                            ItemType = EnumObjects.DocumentEvents,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.evt.Id,
                            ObjectText = x.evt.Description + " " + x.evt.AddDescription + " " + x.evt.Task.Task + " " + x.evt.SourcePositionExecutorAgent.Name + " " + x.evt.TargetPositionExecutorAgent.Name + " " + x.evt.SourceAgent.Name + " " + x.evt.TargetAgent.Name + " "
                        }).Take(() => rowToSelect).ToList());
                }

                if (objType == EnumObjects.DocumentFiles)
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.Id <= selectBis && x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DocumentFiles).OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentFilesSet.Where(x => !x.IsDeleted), i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, fl = d })
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.fl.DocumentId,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.fl.Id,
                            ObjectText = x.fl.Name + "." + x.fl.Extension + " "
                        }).Take(() => rowToSelect).ToList());
                }

                if (objType == EnumObjects.DocumentSendLists)
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.Id <= selectBis && x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DocumentSendLists).OrderBy(x => x.ObjectId).ThenBy(x => x.Id)
                        .Join(dbContext.DocumentSendListsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, sl = d })
                        .Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = x.sl.DocumentId,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.sl.Id,
                            ObjectText = x.sl.Description + " "
                            // + x.sl.SendType.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                            + x.sl.SourcePosition.Name + " "
                            + x.sl.TargetPosition.Name + " "
                            + x.sl.SourcePositionExecutorAgent.Name + " "
                            + x.sl.TargetPositionExecutorAgent.Name + " "
                        }).Take(() => rowToSelect).ToList());
                }

                if (objType == EnumObjects.DocumentSubscriptions)
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.Id <= selectBis && x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DocumentSubscriptions).OrderBy(x => x.ObjectId).ThenBy(x => x.Id).Join(dbContext.DocumentSubscriptionsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, ss = d }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = x.ss.DocumentId,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.ss.Id,
                        ObjectText = x.ss.Description + " "
                        //+ x.ss.SubscriptionState.Name + " "  // не должны добавляться в полнотекст т.к. значения не локализованы
                        + x.ss.DoneEvent.SourcePositionExecutorAgent.Name + " "
                    }).Take(() => rowToSelect).ToList());
                }
                var iDs = res.Select(x => x.Id);
                var removedItems = dbContext.FullTextIndexCashSet.Where(x => x.ObjectType == (int)objType && !iDs.Contains(x.Id)).ToList();

                if (removedItems.Any())
                {
                    dbContext.FullTextIndexCashSet.RemoveRange(removedItems);
                    dbContext.SaveChanges();
                }
                transaction.Complete();
            }
            return res;
        }

        // создание поисковых индексов в рабочем режиме по таблице FullTextIndexCashSet, НЕ относящихся к документу
        public IEnumerable<FullTextIndexItem> FullTextIndexNonDocumentsPrepare(IContext ctx)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;

                var objectTypesToProcess = dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType != (int)EnumObjects.Documents && x.ObjectType != (int)EnumObjects.DocumentSendLists && x.ObjectType != (int)EnumObjects.DocumentEvents && x.ObjectType != (int)EnumObjects.DocumentFiles && x.ObjectType != (int)EnumObjects.DocumentSubscriptions).Select(x => x.ObjectType).Distinct().ToList().Select(x => (EnumObjects)x);

                #region Dictionaries

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgents))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgents)
                        .Join(dbContext.DictionaryAgentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, agent = d, id = d.Id }).Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = 0,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.id,
                            ObjectText = x.agent.Name.Trim() + " " + x.agent.Description.Trim()
                        }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentEmployees))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentEmployees)
                        .Join(dbContext.DictionaryAgentEmployeesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, employee = d, id = d.Id }).Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = 0,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.id,
                            ObjectText = x.employee.PersonnelNumber + " " + x.employee.Description + " " + x.employee.Agent.Name + " " + x.employee.Agent.AgentPerson.FullName + " " + x.employee.Agent.AgentPerson.BirthDate + " " + x.employee.Agent.AgentPerson.PassportDate + " " + x.employee.Agent.AgentPerson.PassportNumber + " " + x.employee.Agent.AgentPerson.PassportSerial + " " + x.employee.Agent.AgentPerson.PassportText + " " + x.employee.Agent.AgentPerson.TaxCode
                        }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentCompanies))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentCompanies)
                        .Join(dbContext.DictionaryAgentCompaniesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, agent = d, id = d.Id }).Select(x => new FullTextIndexItem
                        {
                            Id = x.ind.Id,
                            DocumentId = 0,
                            ItemType = (EnumObjects)x.ind.ObjectType,
                            OperationType = (EnumOperationType)x.ind.OperationType,
                            ClientId = ctx.CurrentClientId,
                            ObjectId = x.id,
                            ObjectText = x.agent.FullName.Trim() + " " + x.agent.OKPOCode.Trim() + " " + x.agent.Description.Trim() + " "
                                     + x.agent.TaxCode.Trim() + " " + x.agent.VATCode.Trim()
                        }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentPersons))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentPersons).Join(dbContext.DictionaryAgentPersonsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, agent = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.agent.FullName.Trim() + " " + x.agent.Description.Trim() + " " + x.agent.TaxCode.Trim() + " "
                                     + x.agent.BirthDate + " " + x.agent.PassportNumber + " " + x.agent.PassportSerial.Trim() + " " + x.agent.PassportText.Trim()
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentBanks))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentBanks).Join(dbContext.DictionaryAgentBanksSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, agent = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.agent.Agent.Name.Trim() + " " + x.agent.Description.Trim() + " " + x.agent.MFOCode.Trim() + " " + x.agent.Swift.Trim()
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryContacts))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryContacts).Join(dbContext.DictionaryAgentContactsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, contact = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.contact.Agent.Name.Trim() + " " + x.contact.Description.Trim() + " " + x.contact.Contact.Trim() + " " + x.contact.ContactType.Code.Trim() + " "
                        + x.contact.ContactType.Name.Trim()
                    }).ToList());
                }

                // не должны добавляться в полнотекст т.к. значения не локализованы
                //if (objectTypesToProcess.Contains(EnumObjects.DictionaryContactType))
                //{
                //    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryContactType).Join(dbContext.DictionaryContactTypesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, contact = d, id = d.Id }).Select(x => new FullTextIndexItem
                //    {
                //        Id = x.ind.Id,
                //        DocumentId = 0,
                //        ItemType = (EnumObjects)x.ind.ObjectType,
                //        OperationType = (EnumOperationType)x.ind.OperationType,
                //        ClientId = ctx.CurrentClientId,
                //        ObjectId = x.id,
                //        ObjectText = x.contact.Code.Trim() + " " + x.contact.Name.Trim()
                //    }).ToList());
                //}

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentAddresses))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentAddresses).Join(dbContext.DictionaryAgentAddressesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, address = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.address.Agent.Name.Trim() + " " + x.address.Description.Trim() + " " + x.address.Address.Trim() + " " + x.address.PostCode.Trim() + " "
                        + x.address.AddressType.Name.Trim()
                    }).ToList());
                }

                //if (objectTypesToProcess.Contains(EnumObjects.DictionaryAddressType))
                //{
                //    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAddressType).Join(dbContext.DictionaryAddressTypesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, address = d, id = d.Id }).Select(x => new FullTextIndexItem
                //    {
                //        Id = x.ind.Id,
                //        DocumentId = 0,
                //        ItemType = (EnumObjects)x.ind.ObjectType,
                //        OperationType = (EnumOperationType)x.ind.OperationType,
                //        ClientId = ctx.CurrentClientId,
                //        ObjectId = x.id,
                //        ObjectText = x.address.Name.Trim()
                //    }).ToList());
                //}

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentAccounts))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentAccounts).Join(dbContext.DictionaryAgentAccountsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, account = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.account.AccountNumber + " " + x.account.Name + " " + x.account.Agent.Name + " " + x.account.AgentBank.MFOCode + " " + x.account.AgentBank.Agent.Name
                    }).ToList());
                }

                //if (objectTypesToProcess.Contains(EnumObjects.DictionaryDocumentType))
                //{
                //    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryDocumentType).Join(dbContext.DictionaryDocumentTypesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                //    {
                //        Id = x.ind.Id,
                //        DocumentId = 0,
                //        ItemType = (EnumObjects)x.ind.ObjectType,
                //        OperationType = (EnumOperationType)x.ind.OperationType,
                //        ClientId = ctx.CurrentClientId,
                //        ObjectId = x.id,
                //        ObjectText = x.doc.Name.Trim()
                //    }).ToList());
                //}

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryDocumentSubjects))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryDocumentSubjects).Join(dbContext.DictionaryDocumentSubjectsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Name.Trim()
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryRegistrationJournals))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryRegistrationJournals).Join(dbContext.DictionaryRegistrationJournalsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Index.Trim() + " " + x.doc.Name.Trim() + " " + x.doc.Department.Name.Trim() + " " + x.doc.Department.FullName.Trim()
                    }).ToList());
                }


                if (objectTypesToProcess.Contains(EnumObjects.DictionaryDepartments))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryDepartments).Join(dbContext.DictionaryDepartmentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.FullName + " " + x.doc.Code + " " + x.doc.Name + " " + x.doc.Company.FullName + " " + x.doc.ChiefPosition.FullName
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositions))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryPositions).Join(dbContext.DictionaryPositionsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.FullName + " " + x.doc.Name + " " + x.doc.Department.Name + " " + x.doc.ExecutorAgent.Name + " " + x.doc.MainExecutorAgent.Name
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryStandartSendLists))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryStandartSendLists).Join(dbContext.DictionaryStandartSendListsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Name + " " + x.doc.Position.Department.Name + " " + x.doc.Position.Name
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryStandartSendListContent))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryStandartSendListContent).Join(dbContext.DictionaryStandartSendListContentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Task + " " + x.doc.Description + " " + /*x.doc.SendType.Name +*/ x.doc.StandartSendList.Name + " " + x.doc.TargetAgent.Name + " " + x.doc.TargetPosition.Name
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryAgentClientCompanies))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryAgentClientCompanies).Join(dbContext.DictionaryAgentClientCompaniesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.FullName
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositionExecutorTypes))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryPositionExecutorTypes).Join(dbContext.DictionaryPositionExecutorTypesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Name + " " + x.doc.Code
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.DictionaryPositionExecutors))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.DictionaryPositionExecutors).Join(dbContext.DictionaryPositionExecutorsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Description + " " + x.doc.Agent.Name + " " + x.doc.EndDate + " " + x.doc.Position.Name //+ " " + x.doc.PositionExecutorType.Name
                    }).ToList());
                }

                #endregion Dictionaries

                #region TemplateDocuments

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocument))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.TemplateDocument).Join(dbContext.TemplateDocumentsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Description + " " + x.doc.Addressee + " " /*+ x.doc.DocumentDirection.Name + " "*/ + x.doc.DocumentSubject.Name + " " /*+ x.doc.DocumentType.Name + " "*/ + x.doc.Name + " " + x.doc.RegistrationJournal.Name + " " + x.doc.SenderAgent.Name + " " + x.doc.SenderAgentPerson.FullName
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentSendList))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.TemplateDocumentSendList).Join(dbContext.TemplateDocumentSendListsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Description + " " + x.doc.Document.Name + " " + x.doc.SendType.Name + " " + x.doc.SourceAgent.Name + " " + x.doc.TargetAgent.Name + " " + x.doc.TargetPosition.Name
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentRestrictedSendList))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.TemplateDocumentRestrictedSendList).Join(dbContext.TemplateDocumentRestrictedSendListsSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Document.Name + " " + x.doc.Position.FullName + " " + x.doc.Position.Name
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentTask))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.TemplateDocumentTask).Join(dbContext.TemplateDocumentTasksSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Document.Name + " " + x.doc.Position.FullName + " " + x.doc.Position.Name + " " + x.doc.Task
                    }).ToList());
                }

                if (objectTypesToProcess.Contains(EnumObjects.TemplateDocumentAttachedFiles))
                {
                    res.AddRange(dbContext.FullTextIndexCashSet.Where(x => x.OperationType != (int)EnumOperationType.Delete && x.ObjectType == (int)EnumObjects.TemplateDocumentAttachedFiles).Join(dbContext.TemplateDocumentFilesSet, i => i.ObjectId, d => d.Id, (i, d) => new { ind = i, doc = d, id = d.Id }).Select(x => new FullTextIndexItem
                    {
                        Id = x.ind.Id,
                        DocumentId = 0,
                        ItemType = (EnumObjects)x.ind.ObjectType,
                        OperationType = (EnumOperationType)x.ind.OperationType,
                        ClientId = ctx.CurrentClientId,
                        ObjectId = x.id,
                        ObjectText = x.doc.Document.Name + " " + x.doc.Extention + " " + x.doc.Name
                    }).ToList());
                }
                var iDs = res.Select(s => s.Id);
                var removedItems = dbContext.FullTextIndexCashSet.Where(x => !iDs.Contains(x.Id)).ToList();

                if (removedItems.Any())
                {
                    dbContext.FullTextIndexCashSet.RemoveRange(removedItems);
                    dbContext.SaveChanges();
                }

                #endregion TemplateDocuments
                transaction.Complete();
            }
            return res;
        }

        public void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds, bool deleteSimilarObject = false)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.FullTextIndexCashSet.RemoveRange(dbContext.FullTextIndexCashSet.Where(x => processedIds.Contains(x.Id)));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void FullTextIndexDeleteCash(IContext ctx, int deleteBis)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.FullTextIndexCashSet.Where(x => x.Id <= deleteBis).Delete();
                transaction.Complete();
            }
        }

        public void DeleteRelatedToDocumentRecords(IContext ctx, IEnumerable<int> docIds, int? deleteBis = null)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                ////TODO когда происходит реиндексация базы удалить все из кеша, что связано с переиндексированными документами
                //var qry = dbContext.FullTextIndexCashSet.Where(x => docIds.Contains(x.)));

                //dbContext.FullTextIndexCashSet.RemoveRange(

                //dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion

        public int AddSystemDate(IContext ctx, DateTime date)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var item = new SystemDate
                {
                    Date = date,
                };
                dbContext.SystemDateSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Added;

                dbContext.SaveChanges();
                transaction.Complete();
                return item.Id;
            }
        }

        public DateTime GetSystemDate(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemDateSet.ToList();

                var res = DateTime.UtcNow.AddYears(-50);

                if (qry?.Count > 0)
                    res = qry.LastOrDefault().Date;

                transaction.Complete();

                return res;
            }
        }

    }
}