using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.WebAPI.FrontModel;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

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

        public void UpdateLogDate1(IContext ctx, int id, DateTime datetime)
        {
            _systemDb.UpdateLogDate1(ctx, id, datetime);
        }

        public IEnumerable<int> GetOnlineUsers(IContext context, IQueryable<FrontSystemSession> sessions)
        {

            List<int> res = sessions.Where(x => x.AgentId.HasValue && x.LastUsage > DateTime.UtcNow.AddMinutes(-1)).Select(x => x.AgentId.Value).Distinct().ToList();
            return res;
        }

        public FrontAgentEmployeeUser GetLastUserLoginInfo(IContext context)
        {
            var lastSuccessLoginInfo = _systemDb.GetLastSuccessLoginInfo(context);
            var lastErrorLoginInfo = _systemDb.GetLastErrorLoginInfo(context, lastSuccessLoginInfo?.LastSuccessLogin);
            var res = new FrontAgentEmployeeUser
            {
                LastSuccessLogin = lastSuccessLoginInfo?.LastSuccessLogin,
                LastErrorLogin = lastErrorLoginInfo?.LastErrorLogin,
                CountErrorLogin = lastErrorLoginInfo?.CountErrorLogin,
            };
            return res;
        }


        public IEnumerable<FrontSystemSession> GetSystemSessions(IContext context, FilterSystemSession filter, UIPaging paging)
        {
            List<FrontSystemSession> res;

            //if (filter?.IsOnlyActive ?? false)
            //{
            //    var qry = sessions;

            //    if (filter != null)
            //    {
            //        if (filter.ExecutorAgentIDs?.Count > 0)
            //        {
            //            qry = qry.Where(x => x.AgentId != null && filter.ExecutorAgentIDs.Contains(x.AgentId.Value));
            //        }
            //        if (filter.CreateDateFrom.HasValue)
            //        {
            //            qry = qry.Where(x => x.CreateDate >= filter.CreateDateFrom.Value);
            //        }
            //        if (filter.CreateDateTo.HasValue)
            //        {
            //            qry = qry.Where(x => x.CreateDate <= filter.CreateDateTo.Value);
            //        }
            //        if (!string.IsNullOrEmpty(filter.LoginLogInfo))
            //        {
            //            var filterContains = PredicateBuilder.New<FrontSystemSession>(false);
            //            filterContains = CommonFilterUtilites.GetWhereExpressions(filter.LoginLogInfo)
            //                        .Aggregate(filterContains, (current, value) => current.Or(e => e.LoginLogInfo.Contains(value)).Expand());
            //            qry = qry.Where(filterContains);
            //        }
            //        if (!string.IsNullOrEmpty(filter.ExecutorAgentName))
            //        {
            //            var filterContains = PredicateBuilder.New<FrontSystemSession>(false);
            //            filterContains = CommonFilterUtilites.GetWhereExpressions(filter.ExecutorAgentName)
            //                        .Aggregate(filterContains, (current, value) => current.Or(e => e.Name.ToLower().Contains(value)).Expand());
            //            qry = qry.Where(filterContains);
            //        }
            //        if (!string.IsNullOrEmpty(filter.FullTextSearchString))
            //        {
            //            var filterContains = PredicateBuilder.New<FrontSystemSession>(true);
            //            filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullTextSearchString)
            //                        .Aggregate(filterContains, (current, value) => current.And(e => (e.LoginLogInfo + " " + e.Name).ToLower().Contains(value)).Expand());
            //            qry = qry.Where(filterContains);
            //        }
            //        qry = qry.OrderByDescending(x => x.CreateDate);
            //    }
            //    if (paging != null)
            //    {
            //        if (paging.IsOnlyCounter ?? true)
            //        {
            //            paging.TotalItemsCount = qry.Count();
            //        }

            //        if (paging.IsOnlyCounter ?? false)
            //        {
            //            return new List<FrontSystemSession>();
            //        }

            //        if (!paging.IsAll)
            //        {
            //            var skip = paging.PageSize * (paging.CurrentPage - 1);
            //            var take = paging.PageSize;
            //            qry = qry.Skip(skip).Take(take);
            //        }
            //    }
            //    res = qry.ToList();
            //}
            //else
            //{
            //    res = _systemDb.GetSystemLogs(context,
            //        new FilterSystemLog
            //        {
            //            ObjectIDs = new List<int> { (int)EnumObjects.System },
            //            ActionIDs = new List<int> { (int)EnumActions.Login },
            //            ExecutorAgentIDs = filter?.ExecutorAgentIDs,
            //            ExecutorAgentName = filter?.ExecutorAgentName,
            //            LogDateFrom = filter?.CreateDateFrom,
            //            LogDateTo = filter?.CreateDateTo,
            //            FullTextSearchString = filter?.FullTextSearchString,
            //        }, paging).Select(x => new FrontSystemSession
            //        {
            //            CreateDate = x.LogDate,
            //            LastUsage = x.LogDate1,
            //            LoginLogId = x.Id,
            //            LoginLogInfo = x.Message,
            //            LogException = x.LogException,
            //            ObjectLog = x.ObjectLog,
            //            AgentId = x.ExecutorAgentId,
            //            Name = x.ExecutorAgent,
            //            ClientId = x.ClientId ?? 0,
            //            IsSuccess = x.LogLevel == 0,
            //            Host = "*.ostrean.com"
            //        }).ToList();
            //    res.Where(x => !string.IsNullOrEmpty(x.LogException) && x.LogException.StartsWith("DmsExceptions:")).ToList()
            //        .ForEach(x => { x.TypeException = x.LogException; x.LogException = "##l@" + x.LogException + "@l##"; });
            //    res.Join(sessions, x => x.LoginLogId, y => y.LoginLogId, (x, y) => new { x, y }).ToList()
            //        .ForEach(r =>
            //        {
            //            r.x.CreateDate = r.y.CreateDate;
            //            r.x.LastUsage = r.y.LastUsage;
            //            r.x.UserId = r.y.UserId;
            //            r.x.IsActive = true;
            //        });
            //}

            var time = DateTime.UtcNow.AddMinutes(-1);

            var filterLogs = new FilterSystemLog
            {
                ObjectIDs = new List<int> { (int)EnumObjects.System },
                ActionIDs = new List<int> { (int)EnumActions.Login },
                ExecutorAgentIDs = filter?.ExecutorAgentIDs,
                ExecutorAgentName = filter?.ExecutorAgentName,
                LogDateFrom = filter?.CreateDateFrom,
                LogDateTo = filter?.CreateDateTo,
                FullTextSearchString = filter?.FullTextSearchString,
            };

            if (filter?.IsOnlyActive ?? false)
            {
                filterLogs.LogDate1From = time;
            }

            res = _systemDb.GetSystemLogs(context, filterLogs, paging)
                .Select(x => new FrontSystemSession
                {
                    CreateDate = x.LogDate,
                    LastUsage = x.LogDate1,
                    LoginLogId = x.Id,
                    LoginLogInfo = x.Message,
                    LogException = x.LogException,
                    ObjectLog = x.ObjectLog,
                    AgentId = x.ExecutorAgentId,
                    Name = x.ExecutorAgent,
                    ClientId = x.ClientId ?? 0,
                    IsSuccess = x.LogLevel == 0,
                    Host = "*.ostrean.com",
                    IsActive = x.LogDate1.HasValue ? x.LogDate1.Value > time : false
                }).ToList();


            res.Where(x => !string.IsNullOrEmpty(x.LogException) && x.LogException.StartsWith("DmsExceptions:")).ToList()
                .ForEach(x => { x.TypeException = x.LogException; x.LogException = Labels.FirstSigns + x.LogException + Labels.LastSigns; });

            return res;
        }

        public void DeleteSystemLogs(IContext context, FilterSystemLog filter)
        {
            _systemDb.DeleteSystemLogs(context, filter);
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

        public IEnumerable<FrontSearchQueryLog> GetSystemSearchQueryLogs(IContext context, FilterSystemSearchQueryLog filter, UIPaging paging)
        {
            var res = _systemDb.GetSystemSearchQueryLogs(context, filter, paging);
            return res;
        }

        public void DeleteSystemSearchQueryLogsForCurrentUser(IContext context, FilterSystemSearchQueryLog filter)
        {
            _systemDb.DeleteSystemSearchQueryLogsForCurrentUser(context, filter);
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


        public int? AddSearchQueryLog(IContext ctx, InternalSearchQueryLog model)
        {
            model.ClientId = ctx.Client.Id;
            CommonDocumentUtilities.SetLastChange(ctx, model);
            var id = _systemDb.AddSearchQueryLog(ctx, model);
            return id;
        }

        public int? AddSearchQueryLog(IContext ctx, string module, string searchText)
        {
            var model = new InternalSearchQueryLog
            {
                ClientId = ctx.Client.Id,
                ModuleId = Modules.GetId(module),
                SearchQueryText = searchText,
            };
            CommonDocumentUtilities.SetLastChange(ctx, model);
            var id = _systemDb.AddSearchQueryLog(ctx, model);
            return id;
        }

        public int? AddSearchQueryLog(IContext ctx, bool existsResults, string module, string searchText)
        {
            if (!existsResults) return null;

            if (string.IsNullOrEmpty(searchText)) return null;

            return AddSearchQueryLog(ctx, module, searchText);
        }

        private int? AddLogToDb(IContext ctx, InternalLog info)
        {
            int loggerLevel = 0;//TODO Get it from settings
            if ((int)info.LogType >= loggerLevel)
            {
                if (!info.Date.HasValue)
                    info.Date = DateTime.UtcNow;
                if (info.IsCopyDate1 && !info.Date1.HasValue)
                {
                    info.Date1 = info.Date;
                }
                info.AgentId = info.AgentId ?? ctx.CurrentAgentId;
                info.ClientId = ctx.Client.Id;
                var id = _systemDb.AddLog(ctx, info);
                return id;
            }
            return null;
        }

        public int? Trace(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Trace,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Information(IContext ctx, string message, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null, DateTime? logDate = null, bool isCopyDate1 = false)
        {
            var js = new JavaScriptSerializer();
            var frontObjJson = logObject != null ? js.Serialize(logObject) : null;
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Information,
                Message = message,
                ObjectId = objectId,
                ActionId = actionId,
                RecordId = recordId,
                LogObject = frontObjJson,
                LogTrace = (logObject != null ? logObject.GetType().ToString() : null),
                Date = logDate,
                IsCopyDate1 = isCopyDate1,
            });
        }

        public int? Warning(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Warning,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Error(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Error(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogException = $"{message ?? ""} // {exception.GetType()} // {exception.Message} // {exception.Data} // {exception.StackTrace}",
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }
        public int? Error(IContext ctx, string message = null, string exception = null, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null, int? agentId = null, params object[] args)
        {
            var js = new JavaScriptSerializer();
            var frontObjJson = logObject != null ? js.Serialize(logObject) : null;
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogException = exception,
                LogTrace = null,
                ObjectId = objectId,
                ActionId = actionId,
                RecordId = recordId,
                LogObject = frontObjJson,
                AgentId = agentId,
            });
        }
        public int? Fatal(IContext ctx, string message, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

        public int? Fatal(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            return AddLogToDb(ctx, new InternalLog
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogException = $"{message ?? ""} // {exception.GetType()} // {exception.Message} // {exception.Data} // {exception.StackTrace}",
                LogTrace = string.Join(" / ", args, Environment.StackTrace)
            });
        }

    }
}