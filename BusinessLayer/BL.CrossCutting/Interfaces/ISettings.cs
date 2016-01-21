using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettings
    {
        TValue GetSetting<TValue>(string settingName) where TValue : IConvertible;
        TValue GetSetting<TValue>(string settingName, TValue defaulValue) where TValue : IConvertible;
        void SaveSetting(string key, object val);
        void ClearCache();
    }
}