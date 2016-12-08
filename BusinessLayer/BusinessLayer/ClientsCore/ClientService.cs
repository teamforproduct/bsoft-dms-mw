using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.Database;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Database.DBModel.System;
using BL.Model.Constants;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using System.Transactions;
using BL.Model.SystemCore.InternalModel;
using BL.Database.DatabaseContext;
using BL.Logic.Settings;
using BL.Database.SystemDb;

namespace BL.Logic.ClientCore
{
    public class ClientService : IClientService
    {
        private readonly IAdminsDbProcess _AdminDb;
        private readonly IDictionariesDbProcess _DictDb;
        private readonly ISystemDbProcess _SystemDb;
        //private readonly ICommandService _commandService;
        private readonly IAdminService _AdminService;

        public ClientService(IAdminsDbProcess AdminDb, IDictionariesDbProcess DictionaryDb, IAdminService AdminService, ISystemDbProcess SystemDb)
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
                Login = client.Email,
                //PasswordHash = client.PasswordHash,
                IsActive = true,
                LanguageId = client.LanguageId
            });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = mobiContactType, Value = client.PhoneNumber, IsActive = true, IsConfirmed = true });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = emailContactType, Value = client.Email, IsActive = true, IsConfirmed = true });

            #endregion

            #region [+] Agent-Company ....
            // Pss Локализация для названия компании
            var company = new InternalDictionaryAgentClientCompany()
            {
                Name = "Наша компания",
                FullName = "Наша компания"
            };

            CommonDocumentUtilities.SetLastChange(context, company);

            var companyId = _DictDb.AddAgentClientCompany(context, company);



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
            _AdminService.AddNamedRole(context, "Admin", "##l@Roles:Administrator@l##", GetRoleActionsForAdmin(context));

            _AdminService.AddNamedRole(context, "DocumentReview", "##l@Roles:ViewDocuments@l##", GetRoleActionsForDocumentReview());

            _AdminService.AddNamedRole(context, "DocumentActions", "##l@Roles:ExecuteDocumentActions@l##", GetRoleActionsForDocumentReview());

            _AdminService.AddNamedRole(context, "DocumentControl", "##l@Roles:ControlDocumentActions@l##", GetRoleActionsForDocumentControl());

            _AdminService.AddNamedRole(context, "DocumentSigning", "##l@Roles:SigningDocumentActions@l##", GetRoleActionsForSigning());

            _AdminService.AddNamedRole(context, "DocumentPapers", "##l@Roles:PaperActions@l##", GetRoleActionsForPapers());

            _AdminService.AddNamedRole(context, "DocumentAccess", "##l@Roles:AccessDocumentActions@l##", GetRoleActionsForDocumentAccess());

            _AdminService.AddNamedRole(context, "DictionariesDMS", "##l@Roles:DmsDictionaryActions@l##", GetRoleActionsForDictionaryDMS());

            _AdminService.AddNamedRole(context, "DictionaryAgents", "##l@Roles:DictionaryAgentActions@l##", GetRoleActionsForDictionaryAgents());

            _AdminService.AddNamedRole(context, "DictionaryAgentContacts", "##l@Roles:DictionaryAgentContactActions@l##", GetRoleActionsForDictionaryAgentContats());

            _AdminService.AddNamedRole(context, "DictionaryStaffList", "##l@Roles:StaffListActions@l##", GetRoleActionsForDictionaryStaffList());

        }

        #region [+] DefaultRoles
        private List<InternalAdminRoleAction> GetRoleActionsForAdmin(IContext context)
        {
            return _AdminDb.GetRoleActionsForAdmin(context);
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDocumentReview()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ViewDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddSavedFilter });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifySavedFilter });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteSavedFilter });
            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDocumentActions()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddLinkedDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.CopyDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.LaunchPlan });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentSendListItem });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.StopPlan });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ChangeExecutor });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RegisterDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.MarkDocumentEventAsRead });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForInformation });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForConsideration });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForInformationExternal });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForVisaing });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForАgreement });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForАpproval });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.WithdrawVisaing });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.WithdrawАgreement });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.WithdrawАpproval });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.WithdrawSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendMessage });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddNote });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ReportRegistrationCardDocument });
            //items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ПосмотретьPDFДокументаПередПодписанием });
            //items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ПолучитьПодписаныйPDFДокумента });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddFavourite });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteFavourite });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.FinishWork });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.StartWork });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.CopyDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentSendListStage });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentSendListStage });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.LaunchDocumentSendListItem });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentFileUseMainNameFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AcceptDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RenameDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentFileVersion });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentFileVersionRecord });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AcceptMainVersionDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentTags });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddTag });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyTag });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteTag });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDocumentControl()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ControlOn });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ControlChange });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForExecutionChange });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForResponsibleExecutionChange });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ControlTargetChange });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ControlOff });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForControl });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForResponsibleExecution });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendForExecution });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.MarkExecution });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AcceptResult });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.CancelExecution });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectResult });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForSigning()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AffixVisaing });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AffixSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SelfAffixSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectАpproval });
            //items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ПосмотретьPDFДокументаПередПодписанием });
            //items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ПолучитьПодписаныйPDFДокумента });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumEncryptionActions.AddEncryptionCertificate });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumEncryptionActions.ModifyEncryptionCertificate });
            //items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumEncryptionActions.ExportEncryptionCertificate });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumEncryptionActions.DeleteEncryptionCertificate });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumEncryptionActions.VerifyPdf });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForPapers()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.MarkOwnerDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.MarkСorruptionDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.PlanDocumentPaperEvent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.CancelPlanDocumentPaperEvent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SendDocumentPaperEvent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.CancelSendDocumentPaperEvent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RecieveDocumentPaperEvent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentPaperList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyDocumentPaperList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteDocumentPaperList });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDocumentAccess()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ChangePosition });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDictionaryDMS()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddDocumentType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyDocumentType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteDocumentType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAddressType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAddressType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAddressType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddDocumentSubject });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyDocumentSubject });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteDocumentSubject });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddRegistrationJournal });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyRegistrationJournal });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteRegistrationJournal });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddContactType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyContactType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteContactType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddExecutorType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyExecutorType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteExecutorType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddExecutor });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyExecutor });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteExecutor });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyTemplateDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteTemplateDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyTemplateDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteTemplateDocumentSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyTemplateDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteTemplateDocumentTask });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateAttachedFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ModifyTemplateAttachedFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.DeleteTemplateAttachedFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddTemplateDocumentPaper });

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddTag });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyTag });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteTag });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddCustomDictionaryType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyCustomDictionaryType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteCustomDictionaryType });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddCustomDictionary });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyCustomDictionary });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteCustomDictionary });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.AddProperty });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.ModifyProperty });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.DeleteProperty });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.AddPropertyLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.ModifyPropertyLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.DeletePropertyLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumPropertyActions.ModifyPropertyValues });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDictionaryAgents()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentAddress });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentAddress });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentAddress });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentCompany });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentCompany });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentCompany });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentBank });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentBank });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentBank });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentAccount });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentAccount });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentAccount });


            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDictionaryAgentContats()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentContact });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentContact });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentContact });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDictionaryStaffList()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentPerson });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddDepartment });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyDepartment });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteDepartment });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddPosition });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyPosition });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeletePosition });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentEmployee });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteStandartSendListContent });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteStandartSendList });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.AddAgentClientCompany });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.ModifyAgentClientCompany });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDictionaryActions.DeleteAgentClientCompany });

            return items;
        }


        #endregion



    }
}
