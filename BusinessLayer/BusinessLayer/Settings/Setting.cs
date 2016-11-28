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
using BL.Model.Constants;

namespace BL.Logic.Settings
{
    public class Setting : ISettings
    {

        #region [+] Управление настройками ...
        private readonly Dictionary<string, object> _casheSettings = new Dictionary<string, object>();

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="setting"></param>
        /// <returns>Typed setting value.</returns>
        private T GetSetting<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            string settingName = setting.ToString();

            if (!_casheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, new FilterSystemSetting { Key = settingName });
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
        private T GetSetting<T>(IContext ctx, string settingName, T defaulValue) where T : IConvertible
        {
            if (!_casheSettings.ContainsKey(MakeKey(settingName, ctx)))
            {
                var db = DmsResolver.Current.Get<ISystemDbProcess>();
                var val = db.GetSettingValue(ctx, new FilterSystemSetting { Key = settingName });
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

        /// <summary>
        /// Возвращает настройку по ключу. Если настройка не найдена в таблице, записывает настройку по умолчанию и возвращает ее значение.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param name="ctx">Current context (contains user, database)</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaulValue">Expected default value.</param>
        /// <returns>Typed setting value or default value.</returns>
        private T GetSettingWithWriteDefaultIfEmpty<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            var settingKey = setting.ToString();
            var defaulValue = SettingsFactory.GetDefaultSetting(setting);

            string casheKey = MakeKey(settingKey, ctx);

            // Если нет в _casheSettings...
            if (!_casheSettings.ContainsKey(casheKey))
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

            var settingValue = _casheSettings[casheKey];
            return (T)((IConvertible)settingValue).ToType(typeof(T), null);
        }

        private T GetSettingOrDefaultIfEmpty<T>(IContext ctx, EnumSystemSettings setting) where T : IConvertible
        {
            var settingKey = setting.ToString();
            var defaulValue = SettingsFactory.GetDefaultSetting(setting);

            string casheKey = MakeKey(settingKey, ctx);

            // Если нет в _casheSettings...
            if (!_casheSettings.ContainsKey(casheKey))
            {
                // ... записываю val в _casheSettings
                MergeCasheSettings(ctx, defaulValue);
            }

            var settingValue = _casheSettings[casheKey];
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
            if (_casheSettings.ContainsKey(MakeKey(setting.Key, ctx)))
            {
                _casheSettings[MakeKey(setting.Key, ctx)] = GetTypedValue(setting.Value, setting.ValueType);
            }
            else
            {
                _casheSettings.Add(MakeKey(setting.Key, ctx), GetTypedValue(setting.Value, setting.ValueType));
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

        public object GetTypedValue(string Value, EnumValueTypes ValueType)
        {
            object res;

            switch (ValueType)
            {
                case EnumValueTypes.Text:
                case EnumValueTypes.Api:
                case EnumValueTypes.Password:
                    res = Value;
                    break;
                case EnumValueTypes.Number:
                    res = Int32.Parse(Value);
                    break;
                case EnumValueTypes.Date:
                    res = DateTime.Parse(Value);
                    break;
                case EnumValueTypes.Bool:
                    res = Boolean.Parse(Value);
                    break;
                default:
                    res = Value;
                    break;
            }

            return res;
        }

        #endregion

        #region [+] Частные настройки ...
        public bool GetSubordinationsSendAllForExecution(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION);

        public bool GetSubordinationsSendAllForInforming(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING);


        public int GetMailTimeoutMin(IContext ctx) =>
             GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_TIMEOUT_MINUTE);

        public MailServerType GetMailInfoServerType(IContext ctx) =>
             (MailServerType)GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_TYPE);

        public string GetMailInfoSystemMail(IContext ctx) =>
             GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_SYSTEMMAIL);

        public string GetMailInfoName(IContext ctx) =>
             GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_NAME);

        public string GetMailInfoLogin(IContext ctx) =>
             GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_LOGIN);

        public string GetMailInfoPassword(IContext ctx) =>
             GetSetting<string>(ctx, EnumSystemSettings.MAILSERVER_PASSWORD);

        public int GetMailInfoPort(IContext ctx) =>
             GetSetting<int>(ctx, EnumSystemSettings.MAILSERVER_PORT);

        public bool GetDigitalSignatureIsUseCertificateSign(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN);

        public bool GetDigitalSignatureIsUseInternalSign(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN);

        public string GetFulltextDatastorePath(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FULLTEXTSEARCH_DATASTORE_PATH);

        public int GetFulltextRefreshTimeout(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.FULLTEXTSEARCH_REFRESH_TIMEOUT);

        public bool GetFulltextWasInitialized(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<bool>(ctx, EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED);

        public string GetFileStorePath(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.IRF_DMS_FILESTORE_PATH);

        public string GetReportDocumentForDigitalSignature(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature);

        public string GetReportRegisterTransmissionDocuments(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments);

        public string GetReportRegistrationCardIncomingDocument(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument);

        public string GetReportRegistrationCardInternalDocument(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument);

        public string GetReportRegistrationCardOutcomingDocument(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<string>(ctx, EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument);


        public int GetAutoplanTimeoutMinute(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE);

        public int GetClearTrashDocumentsTimeoutMinute(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE);

        public int GetClearTrashDocumentsTimeoutMinuteForClear(IContext ctx) =>
             GetSettingWithWriteDefaultIfEmpty<int>(ctx, EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR);

        #endregion


    }
}