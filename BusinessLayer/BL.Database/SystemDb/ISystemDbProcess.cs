using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
        int AddLog(IContext ctx, LogInfo log);
        int AddSetting(IContext ctx, string name, string value, int? agentId = null);
        string GetSettingValue(IContext ctx, string name, int? agentId = null);
        IEnumerable<InternalSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter);
        IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter);
    }
}