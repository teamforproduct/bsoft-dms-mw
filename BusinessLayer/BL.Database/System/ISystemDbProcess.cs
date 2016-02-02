using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;

namespace BL.Database.System
{
    public interface ISystemDbProcess
    {
        int AddLog(IContext context, LogInfo log);
        int AddSetting(IContext context, string name, string value, int? agentId = null);
        string GetSettingValue(IContext context, string name, int? agentId = null);
    }
}