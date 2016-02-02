using System;
using System.Collections.Generic;
using System.Configuration;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;


namespace BL.Logic.Settings
{
    public class Setting:ISettings
    {
        private readonly Dictionary<string, object> _casheSettings = new Dictionary<string, object>();

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <returns>Typed setting value.</returns>
        public T GetSetting<T>(IContext ctx, string settingName) where T : IConvertible
        {
            if (!_casheSettings.ContainsKey(settingName))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, settingName);
                if (string.IsNullOrEmpty(val))
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} is not specified in configuration file.", settingName));
                }
                _casheSettings.Add(settingName, val);
            }

            var settingValue = _casheSettings[settingName];
            try
            {
                return (T)((IConvertible)settingValue).ToType(typeof(T), null);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} has incorrect value {1}.", settingName, settingValue), invalidCastException);
            }
        }

        /// <summary>
        /// Gets setting value by its name and returns default value if setting not exists in configuration file.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param name="settingName">Setting name.</param>
        /// <param name="defaulValue">Expected default value.</param>
        /// <returns>Typed setting value or default value.</returns>
        public T GetSetting<T>(IContext ctx, string settingName, T defaulValue) where T : IConvertible
        {
            if (!_casheSettings.ContainsKey(settingName))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, settingName);
                if (string.IsNullOrEmpty(val))
                {
                    return defaulValue;
                }
                _casheSettings.Add(settingName, val);
            }

            try
            {
                var settingValue = _casheSettings[settingName];
                return (T)((IConvertible)settingValue).ToType(typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return defaulValue;
            }
        }

        public void SaveSetting(IContext ctx, string key, object val)
        {
            var db = DmsResolver.Current.Get<ISystemDbProcess>();
            db.AddSetting(ctx, key, val.ToString());
            _casheSettings.Add(key, val);
        }
        
        public void ClearCache()
        {
            _casheSettings.Clear();
        }
    }
}