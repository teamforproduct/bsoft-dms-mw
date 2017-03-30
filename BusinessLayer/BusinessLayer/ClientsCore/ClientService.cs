using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.DatabaseContext;
using BL.Database.Dictionaries;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.Settings;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.InternalModel;
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

        private InternalDictionaryContactType GetNewContactType(IContext context, string specCode, string code, string name, string inputMask = "")
        {

            var res = new InternalDictionaryContactType()
            {
                SpecCode = specCode,
                Code = code,
                Name = name,
                InputMask = inputMask,
                IsActive = true,
            };

            CommonDocumentUtilities.SetLastChange(context, res);

            return res;

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

        public void AddNewClient(IContext context, AddClientSaaS client)
        {

            //GetSystemSettings

            #region [+] ContactsTypes ...
            // EnumDictionaryContactsTypes!!!!!!!!!!!!!!!!!!!!!!
            // Pss Локализация для типов контактов
            // Контакты при отображении сортируются по Id ContactType. т.е. в порядке добавления типов
            var mobiContactType = _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.MainPhone.ToString(), "т.осн.", "Основной телефон"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.MobilePhone.ToString(), "т.моб.", "Мобильный телефон"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.WorkPhone.ToString(), "т.раб.", "Рабочий телефон"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.HomePhone.ToString(), "т.дом.", "Домашний телефон"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.SIP.ToString(), "sip", "Sip телефон"));

            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.WorkFax.ToString(), "ф.раб.", "Рабочий факс"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.HomeFax.ToString(), "ф.дом.", "Домашний факс"));

            var emailContactType = _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.MainEmail.ToString(), "е.осн.", "Основной адрес", "/@/"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.WorkEmail.ToString(), "е.раб.", "Рабочий адрес", "/@/"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.PersonalEmail.ToString(), "е.личн.", "Личный адрес", "/@/"));

            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Skype.ToString(), "skype", "Skype"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Viber.ToString(), "viber", "Viber"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.ICQ.ToString(), "ICQ", "ICQ"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Jabber.ToString(), "jab", "Jabber"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Telegram.ToString(), "tg", "Telegram"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Pager.ToString(), "п", "Пейждер"));
            _DictDb.AddContactType(context, GetNewContactType(context, EnumContactTypes.Another.ToString(), "др", "Другой"));
            #endregion

            #region [+] AddressTypes ...
            foreach (var item in DmsDbImportData.GetAddressTypes())
            {
                _DictDb.AddAddressType(context, item);
            };
            #endregion

            #region [+] Agent-Employee ...

            var agentUser = _DictDb.AddAgentEmployee(context, new InternalDictionaryAgentEmployee()
            {
                FirstName = client.Name,
                LastName = client.LastName,
                UserEmail = client.Email,
                //PasswordHash = client.PasswordHash,
                IsActive = true,
                //LanguageId = client.LanguageId
            });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = mobiContactType, Value = client.PhoneNumber, IsActive = true, IsConfirmed = true });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = emailContactType, Value = client.Email, IsActive = true, IsConfirmed = true });

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

            AddClientRoles(context);

            #region [+] DocumentsTypes ...

            foreach (var item in DmsDbImportData.GetDocumentTypes())
            {
                _DictDb.AddDocumentType(context, item);
            };

            foreach (var item in GetSystemSettings())
            {
                _SystemDb.MergeSetting(context, item);
            };
            // добавить шаблоны под каждый тип

            #endregion


        }

        /// <summary>
        /// Добавление дефолтных ролей для нового клиента
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void AddClientRoles(IContext context)
        {

            //Администратор
            //Администратор - управление структурой организации
            //Документы - право выполнения действий
            //Документы - право контроля по документам
            //Документы - право подписания
            //Документы - право просмотра документов
            //Документы - право управления бумажными носителями
            //Инструменты - история подключения
            //Инструменты - управление доступом к документам
            //Пользователь
            //Справочники - управление авторизацией пользователей
            //Справочники - управление журналами организации
            //Справочники - управление контактными лицами котрагентов
            //Справочники - управление контрагентами
            //Справочники - управление сотрудниками
            //Справочники - управление справочниками СЭДО

            _AdminService.AddNamedRole(context, "Admin", "##l@Roles:Administrator@l##", GetRoleActionsForAdmin(context));

            //_AdminService.AddNamedRole(context, "DocumentReview", "##l@Roles:ViewDocuments@l##", GetRoleActionsForDocumentReview());

            //_AdminService.AddNamedRole(context, "DocumentActions", "##l@Roles:ExecuteDocumentActions@l##", GetRoleActionsForDocumentReview());

            //_AdminService.AddNamedRole(context, "DocumentControl", "##l@Roles:ControlDocumentActions@l##", GetRoleActionsForDocumentControl());

            //_AdminService.AddNamedRole(context, "DocumentSigning", "##l@Roles:SigningDocumentActions@l##", GetRoleActionsForSigning());

            //_AdminService.AddNamedRole(context, "DocumentPapers", "##l@Roles:PaperActions@l##", GetRoleActionsForPapers());

            //_AdminService.AddNamedRole(context, "DocumentAccess", "##l@Roles:AccessDocumentActions@l##", GetRoleActionsForDocumentAccess());

            //_AdminService.AddNamedRole(context, "DictionariesDMS", "##l@Roles:DmsDictionaryActions@l##", GetRoleActionsForDictionaryDMS());

            //_AdminService.AddNamedRole(context, "DictionaryAgents", "##l@Roles:DictionaryAgentActions@l##", GetRoleActionsForDictionaryAgents());

            //_AdminService.AddNamedRole(context, "DictionaryAgentContacts", "##l@Roles:DictionaryAgentContactActions@l##", GetRoleActionsForDictionaryAgentContats());

            //_AdminService.AddNamedRole(context, "DictionaryStaffList", "##l@Roles:StaffListActions@l##", GetRoleActionsForDictionaryStaffList());

        }

        #region [+] DefaultRoles
        private List<InternalAdminRolePermission> GetRoleActionsForAdmin(IContext context)
        {
            return _AdminDb.GetRolePermissionsForAdmin(context);
        }

        #endregion



    }
}
