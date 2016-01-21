using System;
using System.Collections.Generic;
using System.Configuration;
using BL.CrossCutting.Interfaces;


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
        public T GetSetting<T>(string settingName) where T : IConvertible
        {
            var settingValue = _casheSettings[settingName];

            if (settingValue == null)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} is not specified in configuration file.", settingName));
            }

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
        public T GetSetting<T>(string settingName, T defaulValue) where T : IConvertible
        {
            var settingValue = _casheSettings[settingName];

            if (settingValue == null)
            {
                return defaulValue;
            }

            try
            {
                return (T)((IConvertible)settingValue).ToType(typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return defaulValue;
            }
        }

        public void SaveSetting(string key, object val)
        {
            _casheSettings.Add(key, val);
        }
        
        public void ClearCache()
        {
            _casheSettings.Clear();
        }
    }
}