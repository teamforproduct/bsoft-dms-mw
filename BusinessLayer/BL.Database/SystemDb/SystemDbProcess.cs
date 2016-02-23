using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.System;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.Enums;

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

        public IEnumerable<InternalSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter)
        {
            var res = new List<InternalSystemAction>();
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                foreach (int posId in filter.PositionsIdList)
                {
                    var qry = dbContext.SystemActionsSet.AsQueryable();

                    if (filter.ActionId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.ActionId.Contains(x.Id));
                    }
                    if (filter.DocumentAction.HasValue)
                    {
                        qry = qry.Where(x => x.Id == (int)filter.DocumentAction);
                    }
                    if (filter.Object.HasValue)
                    {
                        qry = qry.Where(x => x.ObjectId == (int)filter.Object);
                    }
                    if (filter.IsAvailable ?? false)
                    {
                        qry = qry.Where(x =>  x.IsVisible &&
                                              (!x.IsGrantable
                                                || x.RoleActions.Any(y => (posId == y.Role.PositionId)
                                                                            &&
                                                                            y.Role.UserRoles.Any(z => z.UserId == ctx.CurrentAgentId))));
                    }
                    res.AddRange(qry.Select(
                              a => new InternalSystemAction
                              {
                                  DocumentAction = (EnumDocumentActions)a.Id,
                                  Object = (EnumObjects)a.ObjectId,
                                  ActionCode = a.Code,
                                  ObjectCode = a.Object.Code,
                                  API = a.API,
                                  Description = a.Description,
                              }).ToList());
                }
                return res;
            }
        }

        public IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter)
        {
            {

                using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
                {
                    var qry = dbContext.SystemUIElementsSet.AsQueryable();

                    if (filter.UIElementId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.UIElementId.Contains(x.Id));
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