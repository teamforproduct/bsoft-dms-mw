using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettings
    {
        TValue GetSetting<TValue>(IContext ctx, string settingName) where TValue : IConvertible;
        TValue GetSetting<TValue>(IContext ctx, string settingName, TValue defaulValue) where TValue : IConvertible;
        void SaveSetting(IContext ctx, string key, object val);
        void ClearCache(IContext ctx);
        void TotalClear();
    }
}