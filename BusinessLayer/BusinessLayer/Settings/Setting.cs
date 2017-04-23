using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;

namespace BL.Logic.Settings
{
    public class Setting : TypedValues, ISettings
    {

        private readonly Dictionary<string, object> _cacheSettings = new Dictionary<string, object>();

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="setting"></param>
        /// <returns>Typed setting value.</returns>
        public T GetSetting<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            string settingName = setting.ToString();

            if (!_cacheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, new FilterSystemSetting { Key = settingName });
                if (string.IsNullOrEmpty(val))
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} is not specified in configuration file.", settingName));
                }
                _cacheSettings.Add(MakeKey(settingName, ctx), val);
            }

            var settingValue = _cacheSettings[MakeKey(settingName, ctx)];
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
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="settingName">Setting name.</param>
        /// <param name="defaulValue">Expected default value.</param>
        /// <returns>Typed setting value or default value.</returns>
        private T GetSetting<T>(IContext ctx, string settingName, T defaulValue) where T : IConvertible
        {
            if (!_cacheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, new FilterSystemSetting { Key = settingName });
                if (string.IsNullOrEmpty(val))
                {
                    return defaulValue;
                }
                _cacheSettings.Add(MakeKey(settingName, ctx), val);
            }

            try
            {
                var settingValue = _cacheSettings[MakeKey(settingName, ctx)];
                return (T)((IConvertible)settingValue).ToType(typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return defaulValue;
            }
        }

        /// <summary>
        /// Возвращает настройку по ключу. Если настройка не найдена в таблице, записывает настройку по умолчанию и возвращает ее значение.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaulValue">Expected default value.</param>
        /// <returns>Typed setting value or default value.</returns>
        public T GetSettingWithWriteDefaultIfEmpty<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            var settingKey = setting.ToString();
            var defaulValue = SettingFactory.GetDefaultSetting(setting);

            string casheKey = MakeKey(settingKey, ctx);

            // Если нет в _casheSettings...
            if (!_cacheSettings.ContainsKey(casheKey))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                // ... вычитываю из базы
                var val = db.GetSystemSettingsInternal(ctx, new FilterSystemSetting { Key = settingKey });

                // Если нет в базе...
                if (val.Count() == 0 & defaulValue != null)
                {
                    // ... записываю дефолт в базу и в _casheSettings
                    SaveSetting(ctx, defaulValue);
                }
                else
                {
                    // ... записываю val в _casheSettings
                    MergeCasheSettings(ctx, val.FirstOrDefault());
                }

            }

            var settingValue = _cacheSettings[casheKey];
            return (T)((IConvertible)settingValue).ToType(typeof(T), null);
        }

        public T GetSettingOrDefaultIfEmpty<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            var settingKey = setting.ToString();
            var defaulValue = SettingFactory.GetDefaultSetting(setting);

            string casheKey = MakeKey(settingKey, ctx);

            // Если нет в _casheSettings...
            if (!_cacheSettings.ContainsKey(casheKey))
            {
                // ... записываю val в _casheSettings
                MergeCasheSettings(ctx, defaulValue);
            }

            var settingValue = _cacheSettings[casheKey];
            return (T)((IConvertible)settingValue).ToType(typeof(T), null);
        }


        //public void SaveSetting(IContext ctx, string key, object val)
        //{
        //    SaveSetting(ctx, new InternalSystemSetting() { Key = key, Value = val.ToString() });
        //}

        public void SaveSetting(IContext ctx, InternalSystemSetting setting)
        {
            var db = DmsResolver.Current.Get<ISystemDbProcess>();

            db.MergeSetting(ctx, setting);
            MergeCasheSettings(ctx, setting);
        }

        private void MergeCasheSettings(IContext ctx, InternalSystemSetting setting)
        {
            if (_cacheSettings.ContainsKey(MakeKey(setting.Key, ctx)))
            {
                _cacheSettings[MakeKey(setting.Key, ctx)] = GetTypedValue(setting.Value, setting.ValueType);
            }
            else
            {
                _cacheSettings.Add(MakeKey(setting.Key, ctx), GetTypedValue(setting.Value, setting.ValueType));
            }
        }

        public void ClearCache(IContext ctx)
        {
            var mask = "_" + ctx.CurrentDB.Address + "_" + ctx.CurrentDB.DefaultDatabase + "_" + ctx.CurrentAgentId;
            var keyLst = _cacheSettings.Keys.Where(x => x.Contains(mask));
            foreach (var k in keyLst)
            {
                _cacheSettings.Remove(k);
            }
        }

        public void TotalClear()
        {
            _cacheSettings.Clear();
        }

        private string MakeKey(string key, IContext ctx)
        {
            return key + "_" + ctx.CurrentDB.Address + "_" + ctx.CurrentDB.DefaultDatabase + "_" + ctx.CurrentAgentId;
        }

    }
}