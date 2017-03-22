using BL.CrossCutting.Extensions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.System;
using BL.Database.Helper;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.InternalModel;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BL.CrossCutting.Helpers.CashService;
using BL.Model.Constants;

namespace BL.Database.SystemDb
{
    public class SystemDbProcess : CoreDb.CoreDb, ISystemDbProcess
    {
        private readonly ICacheService _casheService;

        public SystemDbProcess(ICacheService casheService)
        {
            _casheService = casheService;
        }

        public void InitializerDatabase(IContext ctx)
        {

        }

        #region Common

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

        public void RefreshModuleFeature(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminRolePermissionsSet.Delete();

                dbContext.SystemPermissionsSet.Delete();

                dbContext.SystemFeaturesSet.Delete();

                dbContext.SystemModulesSet.Delete();

                dbContext.SystemAccessTypesSet.Delete();

                DmsDbImportData.InitPermissions();

                foreach (var item in DmsDbImportData.GetSystemAccessTypes())
                {
                    dbContext.SystemAccessTypesSet.Attach(item);
                    dbContext.Entry(item).State = EntityState.Added;
                    dbContext.SaveChanges();
                }

                foreach (var item in DmsDbImportData.GetSystemModules())
                {
                    dbContext.SystemModulesSet.Attach(item);
                    dbContext.Entry(item).State = EntityState.Added;
                    dbContext.SaveChanges();
                }

                foreach (var item in DmsDbImportData.GetSystemFeatures())
                {
                    dbContext.SystemFeaturesSet.Attach(item);
                    dbContext.Entry(item).State = EntityState.Added;
                    dbContext.SaveChanges();
                }

                foreach (var item in DmsDbImportData.GetSystemPermissions())
                {

                    dbContext.SystemPermissionsSet.Attach(item);
                    dbContext.Entry(item).State = EntityState.Added;
                    dbContext.SaveChanges();
                }
                transaction.Complete();
                _casheService.RefreshKey(context, SettingConstants.PERMISSION_CASHE_KEY);
                _casheService.RefreshKey(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
            }

        }

        #endregion

        #region [+] Logs ...

        public IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemLogsQuery(context, dbContext, filter);

                qry = qry.OrderByDescending(x => x.LogDate);

                Paging.Set(ref qry, paging);

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
                    LogDate1 = x.LogDate1,
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

        public IEnumerable<FrontSearchQueryLog> GetSystemSearchQueryLogs(IContext context, FilterSystemSearchQueryLog filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemSearchQueryLogsQuery(context, dbContext, filter);

                var qryT = qry.GroupBy(x => x.SearchQueryText).Select(x => new FrontSearchQueryLog { SearchQueryText = x.Key, IsOwn = x.Any(y => y.LastChangeUserId == context.CurrentAgentId) });

                qryT = qryT.OrderByDescending(x => x.IsOwn).ThenBy(x => x.SearchQueryText.Length).ThenBy(x => x.SearchQueryText);

                Paging.Set(ref qryT, paging);

                var res = qryT.ToList();
                for (int i = 0; i < res.Count(); i++) res[i].Index = i;
                transaction.Complete();
                return res;
            }
        }

        public void DeleteSystemSearchQueryLogsForCurrentUser(IContext context, FilterSystemSearchQueryLog filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemSearchQueryLogsQuery(context, dbContext, filter).Where(x => x.LastChangeUserId == context.CurrentAgentId);
                dbContext.SystemSearchQueryLogsSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public FrontAgentEmployeeUser GetLastSuccessLoginInfo(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemLogsQuery(context, dbContext, new FilterSystemLog
                {
                    NotContainsIDs = new List<int> { context.LoginLogId.HasValue ? context.LoginLogId.Value : 0 },
                    ObjectIDs = new List<int> { (int)EnumObjects.System },
                    ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                    ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
                    LogLevels = new List<int> { (int)EnumLogTypes.Information },
                });
                qry = qry.OrderByDescending(x => x.LogDate);
                var res = qry.Select(x => new FrontAgentEmployeeUser { LastSuccessLogin = x.LogDate }).FirstOrDefault();
                return res;
            }
        }

        public FrontAgentEmployeeUser GetLastErrorLoginInfo(IContext context, DateTime? dateFrom)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSystemLogsQuery(context, dbContext, new FilterSystemLog
                {
                    ObjectIDs = new List<int> { (int)EnumObjects.System },
                    ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                    ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
                    LogLevels = new List<int> { (int)EnumLogTypes.Error },
                });
                if (dateFrom != null)
                {
                    qry = qry.Where(x => x.LogDate > dateFrom);
                }
                qry = qry.OrderByDescending(x => x.LogDate);
                var res = qry.Select(x => new FrontAgentEmployeeUser { LastErrorLogin = x.LogDate }).FirstOrDefault();
                if (res != null)
                {
                    res.CountErrorLogin = qry.Where(x => x.LogException.Equals("DmsExceptions:UserNameOrPasswordIsIncorrect")
                                                     || x.LogException.Equals("DmsExceptions:UserIsDeactivated")
                                                     || x.LogException.Equals("DmsExceptions:UserAnswerIsIncorrect")
                                                     || x.LogException.Equals("DmsExceptions:FingerprintRequired")).Count();
                }
                return res;
            }
        }

        private IQueryable<SystemSearchQueryLogs> GetSystemSearchQueryLogsQuery(IContext context, DmsContext dbContext, FilterSystemSearchQueryLog filter)
        {
            var qry = dbContext.SystemSearchQueryLogsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.Module?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(false);
                    var moduleId = filter.Module.Select(x => Modules.GetId(x)).ToList();
                    filterContains = moduleId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ModuleId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.Feature?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(false);
                    var featureId = filter.Feature.Select(x => Features.GetId(x)).ToList();
                    filterContains = featureId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FeatureId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.AllSearchQueryTextParts))
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.AllSearchQueryTextParts).Aggregate(filterContains,
                        (current, value) => current.And(e => e.SearchQueryText.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.OneSearchQueryTextParts))
                {
                    var filterContains = PredicateBuilder.New<SystemSearchQueryLogs>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.OneSearchQueryTextParts)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.SearchQueryText.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.SearchQueryTextExact))
                {
                    qry = qry.Where(x => x.SearchQueryText == filter.SearchQueryTextExact);
                }
            }
            return qry;
        }

        private IQueryable<SystemLogs> GetSystemLogsQuery(IContext context, DmsContext dbContext, FilterSystemLog filter)
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
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<SystemLogs>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());
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

        public int AddSearchQueryLog(IContext ctx, InternalSearchQueryLog model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var nlog = new SystemSearchQueryLogs
                {
                    ClientId = model.ClientId,
                    LastChangeUserId = model.LastChangeUserId,
                    LastChangeDate = model.LastChangeDate,
                    FeatureId = model.FeatureId,
                    ModuleId = model.ModuleId,
                    SearchQueryText = model.SearchQueryText
                };
                dbContext.SystemSearchQueryLogsSet.Add(nlog);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {

                }
                transaction.Complete();
                return nlog.Id;
            }
        }

        public int AddLog(IContext ctx, InternalLog log)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var nlog = new SystemLogs
                {
                    ClientId = log.ClientId,
                    ExecutorAgentId = log.AgentId,
                    LogDate = log.Date,
                    LogDate1 = log.Date1,
                    LogLevel = (int)log.LogType,
                    LogException = log.LogException.Truncate(2000),
                    LogTrace = log.LogTrace.Truncate(2000),
                    ObjectLog = log.LogObject,
                    Message = log.Message.Truncate(2000),
                    ObjectId = log.ObjectId,
                    ActionId = log.ActionId,
                    RecordId = log.RecordId,
                };
                dbContext.LogSet.Add(nlog);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {

                }
                transaction.Complete();
                return nlog.Id;
            }
        }

        public void UpdateLogDate1(IContext context, int id, DateTime datetime)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.LogSet.Where(x => x.Id == id).Update(x => new SystemLogs { LogDate1 = datetime });
                transaction.Complete();
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

                qry = qry.OrderBy(x => x.Id);

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

                qry = qry.OrderBy(x => x.Id);

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

        public IQueryable<SystemSettings> GetSettingsQuery(IContext context, DmsContext dbContext, FilterSystemSetting filter)
        {
            var qry = dbContext.SettingsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

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

        #endregion

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
                var qry = GetSystemActionsQuery(context, dbContext, filter);

                var res = qry.Select(x => x.ObjectId).ToList();
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
                qry.Delete();

                transaction.Complete();

            }

        }

        public void AddSystemAction(IContext context, SystemActions item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = dbContext.Database.BeginTransaction())
            {
                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemActions] ON");

                dbContext.Database.ExecuteSqlCommand(
                String.Format(@"INSERT INTO[DMS].[SystemActions]
                (Id, ObjectId, Code, API, [Description], IsGrantable, IsGrantableByRecordId, IsVisible, IsVisibleInMenu,  GrantId, Category, PermissionId) 
                VALUES
                ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})",
                item.Id, item.ObjectId, "'" + item.Code + "'", "'" + item.API + "'", "'" + item.Description + "'", item.IsGrantable ? 1 : 0, item.IsGrantableByRecordId ? 1 : 0, item.IsVisible ? 1 : 0, item.IsVisibleInMenu ? 1 : 0, item.GrantId.ToString() == string.Empty ? "null" : item.GrantId.ToString(), item.Category ?? "null", item.PermissionId.ToString() == string.Empty ? "null" : item.PermissionId.ToString())
                );

                //dbContext.SystemActionsSet.Add(item);

                //dbContext.SaveChanges();

                dbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [DMS].[SystemActions] OFF");

                transaction.Commit();
            }
        }

        public void UpdateSystemAction(IContext context, SystemActions item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.SystemActionsSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
                _casheService.RefreshKey(context, SettingConstants.ACTION_CASHE_KEY);
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

            if (filter != null)
            {

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
            }

            return qry;
        }

        private IQueryable<SystemActions> GetWhereSystemActions(IQueryable<SystemActions> qry, FilterSystemAction filter)
        {


            return qry;
        }

        #endregion

        #region [+] Permissions
        private IQueryable<SystemPermissions> GetPermissionsQuery(IContext context, DmsContext dbContext, FilterSystemPermissions filter)
        {
            var qry = dbContext.SystemPermissionsSet.AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemPermissions>();

                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<SystemPermissions>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ModuleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemPermissions>();

                    filterContains = filter.ModuleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ModuleId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.FeatureIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemPermissions>();

                    filterContains = filter.FeatureIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FeatureId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AccessTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<SystemPermissions>();

                    filterContains = filter.AccessTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (!string.IsNullOrEmpty(filter.Module))
                {
                    qry = qry.Where(x => x.Module.Code == filter.Module);
                }

                if (!string.IsNullOrEmpty(filter.Feature))
                {
                    qry = qry.Where(x => x.Feature.Code == filter.Feature);
                }

                if (!string.IsNullOrEmpty(filter.AccessType))
                {
                    qry = qry.Where(x => x.AccessType.Code == filter.AccessType);
                }


            }

            return qry;
        }

        public int GetPermissionId(IContext context, string module, string feture, string accessType)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPermissionsQuery(context, dbContext, new FilterSystemPermissions { Module = module, Feature = feture, AccessType = accessType });

                var item = qry.FirstOrDefault();

                transaction.Complete();

                if (item != null) return item.Id;
                else return -1;
            }
        }

        public IEnumerable<InternalPermissions> GetInternalPermissions(IContext ctx, FilterSystemPermissions filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPermissionsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalPermissions
                {
                    Id = x.Id,

                    AccessTypeId = (EnumAccessTypes)x.AccessTypeId,
                    AccessTypeCode = x.AccessType.Code,
                    AccessTypeName = x.AccessType.Name,
                    AccessTypeOrder = x.AccessType.Order,

                    ModuleId = x.ModuleId,
                    ModuleCode = x.Module.Code,
                    ModuleName = x.Module.Name,
                    ModuleOrder = x.Module.Order,

                    FeatureId = x.FeatureId,
                    FeatureCode = x.Feature.Code,
                    FeatureName = x.Feature.Name,
                    FeatureOrder = x.Feature.Order,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }
        #endregion

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
                var res = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)
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

                var closedSendLists = dbContext.DocumentSendListsSet.Where(x => x.IsInitial && !x.CloseEventId.HasValue);

                if (documentId.HasValue)
                {
                    closedSendLists = closedSendLists.Where(x => x.DocumentId == documentId);
                }

                var sendLists = dbContext.DocumentSendListsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                                    .Where(x => x.Document.IsLaunchPlan && !x.StartEventId.HasValue);

                if (documentId.HasValue)
                {
                    sendLists = sendLists.Where(x => x.DocumentId == documentId);
                }

                var qry2 = sendLists.Where(x => !closedSendLists.Any(y => y.DocumentId == x.DocumentId && x.Stage > y.Stage))
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
                    qry2 = qry2.Take(50);
                }

                var res = qry2.ToList();

                //var qry3 = sendListsSet
                //    .Where(x => !x.IsInitial && !x.StartEventId.HasValue
                //                && !qry.Select(s => s.DocId).Contains(x.DocumentId))
                //    .OrderBy(x => x.DocumentId)
                //    .ThenBy(
                //        x =>
                //            new
                //            {
                //                x.Stage,
                //                SendTypeId = x.SendTypeId == (int)EnumSendTypes.SendForControl ? 0 : x.SendTypeId
                //            })
                //    .Select(x => x.Id);

                //if (!documentId.HasValue)
                //{
                //    qry3 = qry3.Take(50);
                //}

                //res.AddRange(qry3.ToList());

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<int> GetDocumentIdsForClearTrashDocuments(IContext ctx, int timeMinForClearTrashDocuments)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var date = DateTime.UtcNow.AddMinutes(-timeMinForClearTrashDocuments);
                var qry = dbContext.DocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId)
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


    }
}