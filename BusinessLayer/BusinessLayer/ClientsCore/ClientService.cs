using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.DatabaseContext;
using BL.Database.Dictionaries;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.Settings;
using BL.Model.AdminCore.Clients;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;

namespace BL.Logic.ClientCore
{
    public class ClientService : IClientService
    {
        private readonly AdminsDbProcess _AdminDb;
        private readonly DictionariesDbProcess _DictDb;
        private readonly ISystemDbProcess _SystemDb;
        //private readonly ICommandService _commandService;
        private readonly IAdminService _AdminService;

        public ClientService(AdminsDbProcess AdminDb, DictionariesDbProcess DictionaryDb, IAdminService AdminService, ISystemDbProcess SystemDb)
        {
            _AdminDb = AdminDb;
            _AdminService = AdminService;
            _DictDb = DictionaryDb;
            _SystemDb = SystemDb;
            //_commandService = commandService;
        }

        //public object ExecuteAction(EnumClientActions act, IContext context, object param)
        //{
        //    var cmd = ClientCommandFactory.GetClientCommand(act, context, param);
        //    var res = _commandService.ExecuteCommand(cmd);
        //    return res;
        //}

        private static string GetLabel(string module, string item) => "##l@" + module.Trim() + ":" + item.Trim() + "@l##";

        public void AddNewClient(IContext context, AddClientSaaS client)
        {
            // SystemSettings

            foreach (var item in GetSystemSettings())
            {
                _SystemDb.MergeSetting(context, item);
            };


            #region [+] ContactsTypes ...
            // EnumDictionaryContactsTypes!!!!!!!!!!!!!!!!!!!!!!
            // Контакты при отображении сортируются по Id ContactType. т.е. в порядке добавления типов
            var mobiContactType = AddContactType(context, EnumContactTypes.MainPhone);
            AddContactType(context, EnumContactTypes.MobilePhone);
            AddContactType(context, EnumContactTypes.WorkPhone);
            AddContactType(context, EnumContactTypes.HomePhone);
            AddContactType(context, EnumContactTypes.SIP);

            AddContactType(context, EnumContactTypes.WorkFax);
            AddContactType(context, EnumContactTypes.HomeFax);

            var emailContactType = AddContactType(context, EnumContactTypes.MainEmail, "/@/");
            AddContactType(context, EnumContactTypes.WorkEmail, "/@/");
            AddContactType(context, EnumContactTypes.PersonalEmail, "/@/");

            AddContactType(context, EnumContactTypes.Skype);
            AddContactType(context, EnumContactTypes.Viber);
            AddContactType(context, EnumContactTypes.ICQ);
            AddContactType(context, EnumContactTypes.Jabber);
            AddContactType(context, EnumContactTypes.Telegram);
            AddContactType(context, EnumContactTypes.Pager);
            AddContactType(context, EnumContactTypes.Another);
            #endregion

            #region [+] AddressTypes ...
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            foreach (var item in GetAddressTypes())
            {
                tmpService.ExecuteAction(EnumDictionaryActions.AddAddressType, context, item);
            };
            #endregion

            #region [+] DocumentsTypes ...

            foreach (var item in GetDocumentTypes())
            {
                tmpService.ExecuteAction(EnumDictionaryActions.AddDocumentType, context, item);
            };

            AddClientRoles(context);

            // добавить шаблоны под каждый тип

            #endregion

            #region [+] Agent-Company ....
            // Pss Локализация для названия компании
            var company = new InternalDictionaryAgentOrg()
            {
                Name = "Наша компания",
                FullName = "Наша компания"
            };

            CommonDocumentUtilities.SetLastChange(context, company);

            var companyId = _DictDb.AddAgentOrg(context, company);



            //_DictDb.AddContact(context, new InternalDictionaryContact()
            //{ AgentId = companyId, ContactTypeId = mobiContactType, Value = client.PhoneNumber, IsActive = true, IsConfirmed = true });

            //_DictDb.AddContact(context, new InternalDictionaryContact()
            //{ AgentId = companyId, ContactTypeId = emailContactType, Value = client.Email, IsActive = true, IsConfirmed = true });
            var department = new InternalDictionaryDepartment()
            { CompanyId = companyId, Index = "01", Code = "01", Name = "Мой отдел", FullName = "Мой отдел", IsActive = true };

            CommonDocumentUtilities.SetLastChange(context, department);

            var departmentId = _DictDb.AddDepartment(context, department);

            var position = new InternalDictionaryPosition()
            { DepartmentId = departmentId, Name = "Директор", FullName = "Директор", Order = 1, IsActive = true };

            CommonDocumentUtilities.SetLastChange(context, position);

            var positionDirector = _DictDb.AddPosition(context, position);

            #endregion

            

            


        }

        /// <summary>
        /// Добавление дефолтных ролей для нового клиента
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void AddClientRoles(IContext context)
        {

            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                _AdminService.AddNamedRole(context, (Roles)role);
            }

        }


        private int AddContactType(IContext context, EnumContactTypes ctype, string inputMask = "")
        {
            var languageService = DmsResolver.Current.Get<ILanguages>();

            string specCode = ctype.ToString();
            string code = languageService.GetTranslation(context.CurrentEmployee.LanguageId, GetLabel("ContactTypesCode", ctype.ToString())); ;
            string name = languageService.GetTranslation(context.CurrentEmployee.LanguageId, GetLabel("ContactTypes", ctype.ToString()));

            var model = new AddContactType()
            {
                SpecCode = specCode,
                Code = code,
                Name = name,
                InputMask = inputMask,
                IsActive = true,
            };

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            return (int)tmpService.ExecuteAction(EnumDictionaryActions.AddContactType, context, model);

        }

        private static List<AddAddressType> GetAddressTypes()
        {
            var items = new List<AddAddressType>();

            items.Add(GetAddressType(EnumAddressTypes.Actual));
            items.Add(GetAddressType(EnumAddressTypes.Current));
            items.Add(GetAddressType(EnumAddressTypes.Legal));
            items.Add(GetAddressType(EnumAddressTypes.Working));
            return items;
        }

        private static AddAddressType GetAddressType(EnumAddressTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            string code = GetLabel("AddressTypesCode", id.ToString());
            return new AddAddressType()
            {
                Name = name,
                Code = code,
                SpecCode = id.ToString(),
                IsActive = true,
            };
        }

        private static List<AddDocumentType> GetDocumentTypes()
        {
            var items = new List<AddDocumentType>();

            items.Add(GetDocumentTypes(EnumDocumentTypes.Agreement));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Commission));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Decree));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Letter));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Memo));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Order));
            items.Add(GetDocumentTypes(EnumDocumentTypes.Protocol));


            return items;
        }

        private static AddDocumentType GetDocumentTypes(EnumDocumentTypes id)
        {
            string name = GetLabel(id.GetType().Name.Replace("Enum", ""), id.ToString());
            return new AddDocumentType()
            {
                Name = name,
                IsActive = true,
            };
        }


        public static List<InternalSystemSetting> GetSystemSettings()
        {
            var items = new List<InternalSystemSetting>();

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_TIMEOUT_MINUTE));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_TYPE));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_SYSTEMMAIL));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_NAME));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_LOGIN));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_PASSWORD));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.MAILSERVER_PORT));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.IRF_DMS_FILESTORE_PATH));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_DATASTORE_PATH));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_REFRESH_TIMEOUT));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_ROWLIMIT));

            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE));
            items.Add(SettingsFactory.GetDefaultSetting(EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE));

            return items;
        }

    }
}
