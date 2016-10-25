using System;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.SystemDb;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;
using System.Web.Script.Serialization;
using System.Linq;
using BL.Model.DictionaryCore.FrontModel;

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

        public IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging)
        {
            var res = _systemDb.GetSystemLogs(context, filter, paging);
            foreach (var item in res.Where(x => !string.IsNullOrEmpty(x.ObjectLog) && !string.IsNullOrEmpty(x.LogTrace)))
            {
                var js = new JavaScriptSerializer();
                Type type = Type.GetType(item.LogTrace + ", BL.Model");//, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null");
                item.LogObject = js.Deserialize(item.ObjectLog, type);
                item.ObjectLog = null;
                item.LogTrace = null;
            }
            return res;
        }

        private void AddLogToDb(IContext ctx, LogInfo info)
        {
            int loggerLevel = 0;//TODO Get it from settings
            if ((int)info.LogType >= loggerLevel)
            {
                info.Date = DateTime.Now;
                info.AgentId = ctx.CurrentAgentId;
                _systemDb.AddLog(ctx, info);
            }
        }

        public void Trace(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Trace,
                Message = message,
                LogTrace = string.Join(" / ", args)
            });
        }


        public void Information(IContext ctx, string message, int? objectId = null, int? actionId = null, object logObject = null)
        {
            var js = new JavaScriptSerializer();
            var frontObjJson = logObject != null ? js.Serialize(logObject) : null;
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Information,
                Message = message,
                ObjectId = objectId,
                ActionId = actionId,
                LogObject = frontObjJson,
                LogTrace = (logObject != null? logObject.GetType().ToString():null),
            });
        }

        public void Warning(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Warning,
                Message = message,
                LogTrace = string.Join(" / ", args)
            });
        }

        public void Error(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogTrace = string.Join(" / ", args)
            });
        }

        public void Error(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(), exception.Message, exception.Data, exception.StackTrace),
                LogTrace = string.Join(" / ", args)
            });
        }

        public void Fatal(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogTrace = string.Join(" / ", args)
            });
        }

        public void Fatal(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(), exception.Message, exception.Data, exception.StackTrace),
                LogTrace = string.Join(" / ", args)
            });
        }

    }
}