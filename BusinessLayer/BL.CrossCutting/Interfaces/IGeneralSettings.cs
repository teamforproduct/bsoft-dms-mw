using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface IGeneralSettings
    {
        TValue GetSetting<TValue>(EnumGeneralSettings setting) where TValue : IConvertible;
        void SaveSetting(InternalGeneralSetting val);
        void ClearCache();
        void TotalClear();
        object GetTypedValue(string Value, EnumValueTypes ValueType);

    }
}