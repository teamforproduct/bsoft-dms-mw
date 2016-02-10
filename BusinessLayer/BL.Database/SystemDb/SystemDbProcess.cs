using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.System;
using BL.Model.SystemCore;

namespace BL.Database.SystemDb
{
    public class SystemDbProcess : CoreDb.CoreDb, ISystemDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public SystemDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public int AddLog(IContext ctx, LogInfo log)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var nlog = new SystemLogs
                {
                    ExecutorAgentId = log.AgentId,
                    LogDate = log.Date,
                    LogLevel = (int)log.LogType,
                    LogException = log.LogException,
                    LogTrace = log.LogObjects,
                    Message = log.Message
                };
                dbContext.LogSet.Add(nlog);
                dbContext.SaveChanges();
                return nlog.Id;
            }
        }

        public int AddSetting(IContext ctx, string name, string value, int? agentId = null)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var cset = dbContext.SettingsSet.FirstOrDefault(x => x.Key == name);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                if (agentId.HasValue)
                {
                    return
                        dbContext.SettingsSet.Where(x => x.Key == name && x.ExecutorAgentId == agentId.Value)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                }
                return
                    dbContext.SettingsSet.Where(x => x.Key == name).OrderBy(x => x.ExecutorAgentId)
                        .Select(x => x.Value)
                        .FirstOrDefault();
            }
        }

        public IEnumerable<BaseSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter)
        {

            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.SystemActionsSet.AsQueryable();

                if (filter.Id?.Count > 0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    qry = qry.Where(x => x.Code.Contains(filter.Code));
                }
                if (!string.IsNullOrEmpty(filter.ObjectCode))
                {
                    qry = qry.Where(x => x.Object.Code.Contains(filter.ObjectCode));
                }

                if (filter.IsAvailabel??false)
                {
                    qry = qry.Where(x => !x.IsGrantable || (x.RoleActions.Any(y => y.Role.UserRoles.Any(z => z.UserId == ctx.CurrentAgentId))) );
                }

                return qry.Select(x => new BaseSystemAction
                {
                    Id = x.Id,
                    Code = x.Code,
                    API = x.API,
                    Description = x.Description,
                    IsGrantable = x.IsGrantable,
                    IsGrantableByRecordId = x.IsGrantableByRecordId
                }).ToList();
            }
        }

        public IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter)
        {
            {

                using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
                {
                    var qry = dbContext.SystemUIElementsSet.AsQueryable();

                    if (filter.Id?.Count > 0)
                    {
                        qry = qry.Where(x => filter.Id.Contains(x.Id));
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
                    return qry.Select(x => new BaseSystemUIElement
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
                }
            }
        }
    }
}