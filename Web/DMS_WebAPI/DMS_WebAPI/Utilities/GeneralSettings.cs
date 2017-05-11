using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace DMS_WebAPI.Utilities
{
    public class GeneralSettings : TypedValues, IGeneralSettings
    {

        private readonly Dictionary<string, object> _cacheSettings = new Dictionary<string, object>();

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <param name="setting"></param>
        /// <returns>Typed setting value.</returns>
        public T GetSetting<T>(EnumGeneralSettings setting) where T : IConvertible
        {
            string settingName = setting.ToString();

            if (!_cacheSettings.ContainsKey(MakeKey(settingName)))
            {
                var db = DmsResolver.Current.Get<WebAPIDbProcess>();
                var val = db.GetSettingValue(settingName);
                if (string.IsNullOrEmpty(val))
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} is not specified in configuration file.", settingName));
                }
                _cacheSettings.Add(MakeKey(settingName), val);
            }

            var settingValue = _cacheSettings[MakeKey(settingName)];
            try
            {
                return (T)((IConvertible)settingValue).ToType(typeof(T), null);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} has incorrect value {1}.", settingName, settingValue), invalidCastException);
            }
        }

        public void SaveSetting(InternalGeneralSetting setting)
        {
            var db = DmsResolver.Current.Get<WebAPIDbProcess>();

            db.MergeSetting(setting);
            MergeCasheSettings(setting);
        }

        private void MergeCasheSettings(InternalGeneralSetting setting)
        {
            if (_cacheSettings.ContainsKey(MakeKey(setting.Key)))
            {
                _cacheSettings[MakeKey(setting.Key)] = GetTypedValue(setting.Value, setting.ValueType);
            }
            else
            {
                _cacheSettings.Add(MakeKey(setting.Key), GetTypedValue(setting.Value, setting.ValueType));
            }
        }

        public void ClearCache()
        {
            TotalClear();
        }

        public void TotalClear()
        {
            _cacheSettings.Clear();
        }

        public void ReadAll()
        {
            ClearCache();
            var db = DmsResolver.Current.Get<WebAPIDbProcess>();
            var list = db.GetSystemSettingsInternal();

            foreach (var item in list)
            {
                MergeCasheSettings(item);
            }
        }

        private string MakeKey(string key) => key;

    }
}