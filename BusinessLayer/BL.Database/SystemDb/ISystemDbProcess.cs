using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
        int AddLog(IContext ctx, LogInfo log);
        int AddSetting(IContext ctx, string name, string value, int? agentId = null);
        string GetSettingValue(IContext ctx, string name, int? agentId = null);
        IEnumerable<BaseSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter);
    }
}