using System;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Model.SystemCore;

namespace BL.Logic.Logging
{
    public class Logger :ILogger
    {
        private int loggerlevel;
        public Logger(ISettings settings)
        {
            loggerlevel = 1;//TODO Get it from settings
        } 

        private void AddLogToDb(IContext ctx, LogInfo info)
        {
            if ((int) info.LogType >= loggerlevel)
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                info.Date = DateTime.Now;
                info.AgentId = ctx.CurrentAgentId;
                db.AddLog(ctx, info);
            }
        }

        public void Trace(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Trace,
                Message =  message,
                LogObjects = string.Format("",args)
            });
        }


        public void Information(IContext ctx, string message)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Information,
                Message = message
            });
        }

        public void Warning(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Warning,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Error(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Error,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Error(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Error,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(),exception.Message, exception.Data, exception.StackTrace),
                LogObjects = string.Format("", args)
            });
        }

        public void Fatal(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Fatal,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Fatal(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogType.Fatal,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(), exception.Message, exception.Data, exception.StackTrace),
                LogObjects = string.Format("", args)
            });
        }
    }
}