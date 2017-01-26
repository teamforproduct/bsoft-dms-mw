using System;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;
using System.Web.Script.Serialization;
using System.Linq;
using BL.CrossCutting.Context;
using BL.Model.DictionaryCore.FrontModel;
using LinqKit;
using BL.Database.Common;

namespace BL.Logic.Logging
{
    public class Logger : ILogger
    {
        private readonly ISystemDbProcess _systemDb;

        private const string _LOG_LEVEL_KEY = "LOG_LEVEL";

        public Logger(ISystemDbProcess dbProcess)
        {
            _systemDb = dbProcess;

        }

        public void UpdateLogDate1(IContext ctx, List<int> ids, DateTime datetime)
        {
            _systemDb.UpdateLogDate1(ctx, ids, datetime);
        }

        public IEnumerable<int> GetOnlineUsers(IContext context, IQueryable<FrontSystemSession> sessions)
        {

            List<int> res = sessions.Where(x => x.AgentId.HasValue && x.LastUsage > DateTime.UtcNow.AddMinutes(-1)).Select(x=>x.AgentId.Value).Distinct().ToList();
            return res;
        }


        public IEnumerable<FrontSystemSession> GetSystemSessions(IContext context, IQueryable<FrontSystemSession> sessions, FilterSystemSession filter, UIPaging paging)
        {
            List<FrontSystemSession> res;
            if (filter?.IsOnlyActive ?? false)
            {
                var qry = sessions;
                if (filter != null)
                {
                    if (filter.ExecutorAgentIDs?.Count > 0)
                    {
                        qry = qry.Where(x => x.AgentId != null && filter.ExecutorAgentIDs.Contains(x.AgentId.Value));
                    }
                    if (filter.CreateDateFrom.HasValue)
                    {
                        qry = qry.Where(x => x.CreateDate >= filter.CreateDateFrom.Value);
                    }
                    if (filter.CreateDateTo.HasValue)
                    {
                        qry = qry.Where(x => x.CreateDate <= filter.CreateDateTo.Value);
                    }
                    if (!String.IsNullOrEmpty(filter.LoginLogInfo))
                    {
                        var filterContains = PredicateBuilder.False<FrontSystemSession>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.LoginLogInfo)
                                    .Aggregate(filterContains, (current, value) => current.Or(e => e.LoginLogInfo.Contains(value)).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (!String.IsNullOrEmpty(filter.ExecutorAgentName))
                    {
                        var filterContains = PredicateBuilder.False<FrontSystemSession>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.ExecutorAgentName)
                                    .Aggregate(filterContains, (current, value) => current.Or(e => e.Name.Contains(value)).Expand());
                        qry = qry.Where(filterContains);
                    }
                    qry = qry.OrderByDescending(x => x.CreateDate);
                }
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontSystemSession>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;
                        qry = qry.Skip(skip).Take(take);
                    }
                }
                res = qry.ToList();
            }
            else
            {
                res = _systemDb.GetSystemLogs(context,
                    new FilterSystemLog
                    {
                        ObjectIDs = new List<int> { (int)EnumObjects.System },
                        ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                        ExecutorAgentIDs = filter?.ExecutorAgentIDs,
                        ExecutorAgentName = filter?.ExecutorAgentName,
                        LogDateFrom = filter?.CreateDateFrom,
                        LogDateTo = filter?.CreateDateTo,
                        Message = filter?.LoginLogInfo,
                    }, paging).Select(x => new FrontSystemSession
                    {
                        CreateDate = x.LogDate,
                        LoginLogId = x.Id,
                        LoginLogInfo = x.Message,
                        AgentId = x.ExecutorAgentId,
                        Name = x.ExecutorAgent,
                        ClientId = x.ClientId ?? 0,
                    }).ToList();
                res.Join(sessions, x => x.LoginLogId, y => y.LoginLogId, (x, y) => new { x, y }).ToList()
                    .ForEach(r =>
                    {
                        r.x.CreateDate = r.y.CreateDate;
                        r.x.Token = r.y.Token;
                        r.x.LastUsage = r.y.LastUsage;
                        r.x.UserId = r.y.UserId;
                        r.x.IsActive = true;
                    });
            }
            return res;
        }

        public IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging)
        {
            var res = _systemDb.GetSystemLogs(context, filter, paging);
            foreach (var item in res.Where(x => !string.IsNullOrEmpty(x.ObjectLog) && !string.IsNullOrEmpty(x.LogTrace)))
            {
                var js = new JavaScriptSerializer();
                Type type = Type.GetType(item.LogTrace + ", BL.Model");//, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null");
                item.LogObject = js.Deserialize(item.ObjectLog, type);
                item.Message = GetObjectChangeDescription(item.LogObject);
                if (((filter?.IDs?.Count) ?? 0) == 0)
                    item.LogObject = null;
                item.ObjectLog = null;
                item.LogTrace = null;
            }
            return res;
        }

        private string GetObjectChangeDescription(object logObject)
        {
            if (logObject is FrontDictionaryAgentClientCompany)
            {
                var model = logObject as FrontDictionaryAgentClientCompany;
                return string.Format("{0}",
                    model.Name);
            }
            if (logObject is FrontDictionaryDepartment)
            {
                var model = logObject as FrontDictionaryDepartment;
                return string.Format("{0}\r\n{1}",
                    model.CompanyName, model.Name);
            }
            if (logObject is FrontDictionaryPosition)
            {
                var model = logObject as FrontDictionaryPosition;
                return string.Format("{0}\r\n{1}\r\n{2}",
                    model.CompanyName, model.DepartmentName, model.Name);
            }
            if (logObject is FrontDictionaryPositionExecutor)
            {
                var model = logObject as FrontDictionaryPositionExecutor;
                return string.Format("{0}\r\n{1}\r\n{2} {3} - {4}",
                    model.DepartmentName, model.PositionName, model.AgentName,
                    model.StartDate.ToString("dd.MM.yyyy"),
                    model.EndDate.HasValue ? model.EndDate.Value.ToString("dd.MM.yyyy") : " ");
            }
            return null;
        }

        private int? AddLogToDb(IContext ctx, LogInfo info)
        {
            int loggerLevel = 0;//TODO Get it from settings
            if ((int)info.LogType >= loggerLevel)
            {
                info.Date = DateTime.UtcNow;
                info.AgentId = ctx.CurrentAgentId;
                var id = _systemDb.AddLog(ctx, info);
                return id;
            }
            return null;
        }

        public int? Trace(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Trace,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Information(IContext ctx, string message, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null)
        {
            var js = new JavaScriptSerializer();
            var frontObjJson = logObject != null ? js.Serialize(logObject) : null;
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Information,
                Message = message,
                ObjectId = objectId,
                ActionId = actionId,
                RecordId = recordId,
                LogObject = frontObjJson,
                LogTrace = (logObject != null ? logObject.GetType().ToString() : null),
            });
        }

        public int? Warning(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Warning,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Error(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Error(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogException = $"{message ?? ""} // {exception.GetType()} // {exception.Message} // {exception.Data} // {exception.StackTrace}",
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Fatal(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Fatal(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            return AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogException =$"{message ?? ""} // {exception.GetType()} // {exception.Message} // {exception.Data} // {exception.StackTrace}",
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

    }
}