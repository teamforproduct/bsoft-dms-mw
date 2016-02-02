using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DBModel.System;
using BL.Model.SystemCore;

namespace BL.Database.System
{
    public class SystemDbProcess: CoreDb.CoreDb, ISystemDbProcess
    {
        public int AddLog(IContext context, LogInfo log)
        {
            var dbContext = GetUserDmsContext(context);
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

        public int AddSetting(IContext context, string name, string value, int? agentId = null)
        {
            var dbContext = GetUserDmsContext(context);
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

        public string GetSettingValue(IContext context, string name, int? agentId = null)
        {
            var dbContext = GetUserDmsContext(context);
            if (agentId.HasValue)
            {
                return
                    dbContext.SettingsSet.Where(x => x.Key == name && x.ExecutorAgentId == agentId.Value)
                        .Select(x => x.Value)
                        .FirstOrDefault();
            }
            return
                dbContext.SettingsSet.Where(x => x.Key == name).OrderBy(x=>x.ExecutorAgentId)
                    .Select(x => x.Value)
                    .FirstOrDefault();
        }
    }
}