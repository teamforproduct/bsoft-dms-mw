using System;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.Logging
{
    public class Logger :ILogger
    {
        private readonly ISystemDbProcess _dbProcess;

        private const string _LOG_LEVEL_KEY = "LOG_LEVEL";

        public Logger(ISystemDbProcess dbProcess)
        {
            _dbProcess = dbProcess;
            
        }

        private void AddLogToDb(IContext ctx, LogInfo info)
        {
            int  loggerLevel = 1;//TODO Get it from settings
            if ((int) info.LogType >= loggerLevel)
            {
                info.Date = DateTime.Now;
                info.AgentId = ctx.CurrentAgentId;
                _dbProcess.AddLog(ctx, info);
            }
        }

        public void Trace(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Trace,
                Message =  message,
                LogObjects = string.Format("",args)
            });
        }


        public void Information(IContext ctx, string message)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Information,
                Message = message
            });
        }

        public void Warning(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Warning,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Error(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Error(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Error,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(),exception.Message, exception.Data, exception.StackTrace),
                LogObjects = string.Format("", args)
            });
        }

        public void Fatal(IContext ctx, string message, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogObjects = string.Format("", args)
            });
        }

        public void Fatal(IContext ctx, Exception exception, string message = null, params object[] args)
        {
            AddLogToDb(ctx, new LogInfo
            {
                LogType = EnumLogTypes.Fatal,
                Message = message,
                LogException = string.Format("{0} // {1} // {2} // {3}", exception.GetType(), exception.Message, exception.Data, exception.StackTrace),
                LogObjects = string.Format("", args)
            });
        }
    }
}