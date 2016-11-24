using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.AgentEmployee;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.Settings
{
    public static class SettingsFactory
    {
        public static InternalSystemSetting GetDefaultSetting(EnumSystemSettings key)
        {
            var res = new InternalSystemSetting();

            switch (key)
            {
                case EnumSystemSettings.MAILSERVER_TYPE:
                    res.Value = "2";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_NAME:
                    res.Value = "";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_LOGIN:
                    res.Value = "";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_PASSWORD:
                    res.Value = "";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_PORT:
                    res.Value = "";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_SYSTEMMAIL:
                    res.Value = "";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 4;
                    break;
                case EnumSystemSettings.MAILSERVER_TIMEOUT_MINUTE:
                    res.Value = "1";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 4;
                    break;


                case EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION:
                    res.Value = "true";
                    res.ValueType = EnumValueTypes.Bool;
                    res.SettingTypeId = 2;
                    break;
                case EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING:
                    res.Value = "true";
                    res.ValueType = EnumValueTypes.Bool;
                    res.SettingTypeId = 2;
                    break;


                case EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN:
                    res.Value = "false";
                    res.ValueType = EnumValueTypes.Bool;
                    res.SettingTypeId = 2;
                    break;
                case EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN:
                    res.Value = "false";
                    res.ValueType = EnumValueTypes.Bool;
                    res.SettingTypeId = 2;
                    break;

                case EnumSystemSettings.FULLTEXTSEARCH_DATASTORE_PATH:
                    res.Value = @"D:\FULLTEXT_INDEXES";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 2;
                    break;
                case EnumSystemSettings.FULLTEXTSEARCH_REFRESH_TIMEOUT:
                    res.Value = "1";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 2;
                    break;
                case EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED:
                    res.Value = "false";
                    res.ValueType = EnumValueTypes.Bool;
                    res.SettingTypeId = 2;
                    break;


                case EnumSystemSettings.IRF_DMS_FILESTORE_PATH:
                    res.Value = @"D:\FULLTEXT_INDEXES";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 2;
                    break;


                case EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature:
                    res.Value = @"ReportDocumentForDigitalSignature.rpt";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 6;
                    break;
                case EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments:
                    res.Value = @"ReportRegisterTransmissionDocuments.rpt";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 6;
                    break;
                case EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument:
                    res.Value = @"ReportRegistrationCardIncomingDocument.rpt";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 6;
                    break;
                case EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument:
                    res.Value = @"ReportRegistrationCardInternalDocument.rpt";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 6;
                    break;
                case EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument:
                    res.Value = @"ReportRegistrationCardOutcomingDocument.rpt";
                    res.ValueType = EnumValueTypes.Text;
                    res.SettingTypeId = 6;
                    break;


                case EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE:
                    res.Value = "1";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 2;
                    break;

                case EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE:
                    res.Value = "1";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 2;
                    break;

                case EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR:
                    res.Value = "360";
                    res.ValueType = EnumValueTypes.Number;
                    res.SettingTypeId = 2;
                    break;


                    //default:
                    //throw new CommandNotDefinedError(act.ToString());
            }

            res.Key = key.ToString();
            res.Name = GetLabel(key.GetType().Name.Replace("Enum", ""), key.ToString());
            res.Description = GetLabel(key.GetType().Name.Replace("Enum", ""), key.ToString() + ".Description");
            res.AgentId = null;
            res.AccessType = 0;
            res.Order = 0;
            return res;
        }

        private static string GetLabel(string group, string itemName) => "##l@" + group.Trim() + ":" + itemName.Trim() + "@l##";
    }
}