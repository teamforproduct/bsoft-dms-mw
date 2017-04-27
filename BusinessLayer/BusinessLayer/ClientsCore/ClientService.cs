using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.Dictionaries;
using BL.Database.Encryption.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.Settings;
using BL.Model.AdminCore.Clients;
using BL.Model.DictionaryCore.IncomingModel;
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
        private readonly IEncryptionDbProcess _EncrDb;
        private readonly ISystemDbProcess _SystemDb;
        private readonly IFullTextDbProcess _FTextDb;
        //private readonly ICommandService _commandService;
        private readonly IAdminService _AdminService;

        public ClientService(AdminsDbProcess AdminDb, DictionariesDbProcess DictionaryDb, IAdminService AdminService, ISystemDbProcess SystemDb, IEncryptionDbProcess EncrypDb, IFullTextDbProcess FTextDb)
        {
            _AdminDb = AdminDb;
            _AdminService = AdminService;
            _DictDb = DictionaryDb;
            _SystemDb = SystemDb;
            _EncrDb = EncrypDb;
            _FTextDb = FTextDb;
            //_commandService = commandService;
        }

        //public object ExecuteAction(EnumClientActions act, IContext context, object param)
        //{
        //    var cmd = ClientCommandFactory.GetClientCommand(act, context, param);
        //    var res = _commandService.ExecuteCommand(cmd);
        //    return res;
        //}

        private static string GetLabel(string module, string item) => "##l@" + module.Trim() + ":" + item.Trim() + "@l##";

        public void AddDictionary(IContext context, AddClientSaaS client)
        {
            // SystemSettings
            var languages = DmsResolver.Current.Get<ILanguages>();

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

            /// Переводы
            #region [+] AddressTypes ...
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            foreach (var item in GetAddressTypes())
            {
                item.Name = languages.GetTranslation(context.Employee.LanguageId, item.Name);
                item.Code = languages.GetTranslation(context.Employee.LanguageId, item.Code);
                tmpService.ExecuteAction(EnumDictionaryActions.AddAddressType, context, item);
            };
            #endregion

            #region [+] DocumentsTypes ...

            foreach (var item in GetDocumentTypes())
            {
                item.Name = languages.GetTranslation(context.Employee.LanguageId, item.Name);
                tmpService.ExecuteAction(EnumDictionaryActions.AddDocumentType, context, item);
            };

            AddClientRoles(context);

            // добавить шаблоны под каждый тип

            #endregion

            // Включить соответствующие воркеры
        }

        public void Delete(IContext context)
        {
            // Остановить соответствующие воркеры
            using (var transaction = Transactions.GetTransaction())
            {
                _DictDb.DeleteRegistrationJournals(context, null);

                _DictDb.DeleteDocumentType(context, null);

                _AdminDb.DeleteDepartmentAdmins(context, null);

                _SystemDb.DeletePropertyLinks(context, null);
                _SystemDb.DeleteProperties(context, null);


                _AdminDb.DeleteUserRoles(context, null);
                _AdminDb.DeletePositionRoles(context, null);
                _AdminDb.DeleteRolePermissions(context, null);
                _AdminDb.DeleteRoles(context, null);

                _AdminDb.DeleteSubordinations(context, null);

                _AdminDb.DeleteRegistrationJournalPositions(context, null);

                _EncrDb.DeleteCertificate(context, null);



                _DictDb.DeleteTags(context, null);

                _DictDb.DeleteStandartSendListContents(context, null);
                _DictDb.DeleteStandartSendList(context, null);

                _DictDb.DeleteCustomDictionaries(context, null);
                _DictDb.DeleteCustomDictionaryType(context, null);

                _DictDb.DeleteStandartSendListContents(context, null);
                _DictDb.DeleteStandartSendList(context, null);

                // Структура организации
                _DictDb.DeleteExecutors(context, null);
                _DictDb.DeletePositions(context, null);
                _DictDb.DeleteDepartments(context, null);
                _DictDb.DeleteAgentOrg(context, null);

                // Перед удалением агентов нужно удалить логи
                _SystemDb.DeleteSystemLogs(context, null);
                _SystemDb.DeleteSystemSearchQueryLogs(context, null);
                _SystemDb.DeleteSystemSettings(context);

                // Агенты
                _DictDb.DeleteAgentFavourite(context, null);

                _DictDb.DeleteAgentBank(context, null);
                _DictDb.DeleteAgentCompanies(context, null);
                _DictDb.DeleteAgentPersons(context, null);
                _DictDb.DeleteAgentEmployees(context, null);
                _DictDb.DeleteAgentUsers(context, null);
                _DictDb.DeleteAgentPeoples(context, null);

                _DictDb.DeleteAgentAccounts(context, null);
                _DictDb.DeleteAgentAddress(context, null);
                _DictDb.DeleteAgentContacts(context, null);
                _DictDb.DeleteAddressTypes(context, null);
                _DictDb.DeleteContactType(context, null);

                _DictDb.DeleteAgents(context, null);

                _FTextDb.Delete(context, context.Client.Id);

                // Удаляю, то что накопилось во время удаленя агентов
                _SystemDb.DeleteSystemLogs(context, null);

                transaction.Complete();
            }
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
            string code = languageService.GetTranslation(context.Employee.LanguageId, GetLabel("ContactTypesCode", ctype.ToString())); ;
            string name = languageService.GetTranslation(context.Employee.LanguageId, GetLabel("ContactTypes", ctype.ToString()));

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

            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_EXECUTION));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.SUBORDINATIONS_SEND_ALL_FOR_INFORMING));

            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN));


            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR));

            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument));

            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED));

            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.RUN_AUTOPLAN_TIMEOUT_MINUTE));
            items.Add(SettingFactory.GetDefaultSetting(EnumSystemSettings.RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE));

            return items;
        }

    }
}
