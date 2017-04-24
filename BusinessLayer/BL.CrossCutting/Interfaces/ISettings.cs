using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettings
    {
        TValue GetSetting<TValue>(IContext ctx, EnumSystemSettings setting) where TValue : IConvertible;
        TValue GetSettingWithWriteDefaultIfEmpty<TValue>(IContext ctx, EnumSystemSettings setting) where TValue : IConvertible;
        void SaveSetting(IContext ctx, InternalSystemSetting val);
        void ClearCache(IContext ctx);
        void TotalClear();
        object GetTypedValue(string Value, EnumValueTypes ValueType);

    }
}