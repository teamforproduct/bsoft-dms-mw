using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
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
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="settingName"></param>
        /// <returns>Typed setting value.</returns>
        public T GetSetting<T>(IContext ctx, string settingName) where T : IConvertible
        {
            if (!_casheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, settingName);
                if (string.IsNullOrEmpty(val))
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration parameter {0} is not specified in configuration file.", settingName));
                }
                _casheSettings.Add(MakeKey(settingName, ctx), val);
            }

            var settingValue = _casheSettings[MakeKey(settingName, ctx)];
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
        public T GetSetting<T>(IContext ctx, string settingName, T defaulValue) where T : IConvertible
        {
            if (!_casheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, settingName);
                if (string.IsNullOrEmpty(val))
                {
                    return defaulValue;
                }
                _casheSettings.Add(MakeKey(settingName, ctx), val);
            }

            try
            {
                var settingValue = _casheSettings[MakeKey(settingName, ctx)];
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
            if (_casheSettings.ContainsKey(MakeKey(key, ctx)))
            {
                db.AddSetting(ctx, key, val.ToString());
                _casheSettings[MakeKey(key, ctx)] = val;
            }
            else
            {
                db.AddSetting(ctx, key, val.ToString());
                _casheSettings.Add(MakeKey(key, ctx), val);
            }

        }
        
        public void ClearCache(IContext ctx)
        {
            var mask = "_" + ctx.CurrentDB.Address + "_" + ctx.CurrentDB.DefaultDatabase + "_" + ctx.CurrentAgentId;
            var keyLst = _casheSettings.Keys.Where(x => x.Contains(mask));
            foreach (var k in keyLst)
            {
                _casheSettings.Remove(k);
            }
        }

        public void TotalClear()
        {
            _casheSettings.Clear();
        }

        private string MakeKey(string key, IContext ctx)
        {
            return key + "_" + ctx.CurrentDB.Address + "_" + ctx.CurrentDB.DefaultDatabase + "_" + ctx.CurrentAgentId;
        }
    }
}