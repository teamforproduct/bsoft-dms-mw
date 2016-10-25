using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettings
    {
        TValue GetSetting<TValue>(IContext ctx, string settingName) where TValue : IConvertible;
        TValue GetSetting<TValue>(IContext ctx, string settingName, TValue defaulValue) where TValue : IConvertible;
        TValue GetSetting<TValue>(IContext ctx, string settingKey, InternalSystemSetting defaulValue) where TValue : IConvertible;
        void SaveSetting(IContext ctx, InternalSystemSetting val);
        void ClearCache(IContext ctx);
        void TotalClear();
        object GetTypedValue(string Value, EnumValueTypes ValueType);



        bool GetSubordinationsSendAllForExecution(IContext ctx);
        bool GetSubordinationsSendAllForInforming(IContext ctx);

    }
}