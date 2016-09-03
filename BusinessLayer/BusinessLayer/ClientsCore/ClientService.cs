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

namespace BL.Logic.ClientCore
{
    public class ClientService : IClientService
    {
        private readonly IAdminsDbProcess _AdminDb;
        private readonly IDictionariesDbProcess _DictDb;
        //private readonly ICommandService _commandService;

        public ClientService(IAdminsDbProcess AdminDb, IDictionariesDbProcess DictionaryDb)
        {
            _AdminDb = AdminDb;
            _DictDb = DictionaryDb;
            //_commandService = commandService;
        }

        //public object ExecuteAction(EnumClientActions act, IContext context, object param)
        //{
        //    var cmd = ClientCommandFactory.GetClientCommand(act, context, param);
        //    var res = _commandService.ExecuteCommand(cmd);
        //    return res;
        //}

        private InternalDictionaryContactType GetNewContactType(IContext context, string code, string name, string inputMask = "")
        {
            return new InternalDictionaryContactType()
            {
                Code = code,
                Name = name,
                InputMask = inputMask,
                IsActive = true,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId
            };
        }

        public void AddNewClient(IContext context, AddClientContent client)
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            context.CurrentClientId = client.ClientId;
            #region [+] ContactsTypes ...

            // Pss Локализация для типов контактов
            var mobiContactType = _DictDb.AddContactType(context, 
                new InternalDictionaryContactType()
                { Code = "МТ", Name = "Мобильный телефон", InputMask = "", IsActive = true ,
                LastChangeDate = DateTime.Now, LastChangeUserId = context.CurrentAgentId
                });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "РТ", Name = "Рабочий телефон", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "ДТ", Name = "Домашний телефон", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "ОТ", Name = "Основной телефон", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "РФ", Name = "Рабочий факс", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "ДФ", Name = "Домашний факс", InputMask = "", IsActive = true });
            var emailContactType = _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "ЛМ", Name = "Личный адрес", InputMask = "/@/", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "РМ", Name = "Рабочий адрес", InputMask = "/@/", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "П", Name = "Пейждер", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "Skype", Name = "Skype", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "ICQ", Name = "ICQ", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "Jab", Name = "Jabber", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "Viber", Name = "Viber", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "Tg", Name = "Telegram", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "Др", Name = "Другой", InputMask = "", IsActive = true });
            _DictDb.AddContactType(context, new InternalDictionaryContactType() { Code = "MVPN", Name = "MVPN", InputMask = "", IsActive = true });
            #endregion

            #region [+] AddressTypes ...
            // Pss Локализация для типов адресов
            _DictDb.AddAddressType(context, new InternalDictionaryAddressType() { Code = "ДА", Name = "Домашний", IsActive = true });
            _DictDb.AddAddressType(context, new InternalDictionaryAddressType() { Code = "РА", Name = "Рабочий", IsActive = true });
            #endregion

            #region [+] Agent-Employee ...

            var agentUser = _DictDb.AddAgentEmployee(context, new InternalDictionaryAgentEmployee()
            {
                FirstName = client.Name,
                LastName = client.LastName,
                Login = client.Login,
                PasswordHash = client.PasswordHash,
                IsActive = true,
                LanguageId = client.LanguageId
            });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = mobiContactType, Value = client.PhoneNumber, IsActive = true, IsConfirmed = true });

            _DictDb.AddContact(context, new InternalDictionaryContact() { AgentId = agentUser, ContactTypeId = emailContactType, Value = client.Email, IsActive = true, IsConfirmed = true });

            #endregion

            #region [+] Agent-Company ....
            // Pss Локализация для названия компании
            var companyId = _DictDb.AddAgentClientCompany(context, new InternalDictionaryAgentClientCompany()
            { Name = "Наша компания", FullName = "Наша компания", IsActive = true });

            //_DictDb.AddContact(context, new InternalDictionaryContact()
            //{ AgentId = companyId, ContactTypeId = mobiContactType, Value = client.PhoneNumber, IsActive = true, IsConfirmed = true });

            //_DictDb.AddContact(context, new InternalDictionaryContact()
            //{ AgentId = companyId, ContactTypeId = emailContactType, Value = client.Email, IsActive = true, IsConfirmed = true });

            var departmentId = _DictDb.AddDepartment(context, new InternalDictionaryDepartment()
            { CompanyId = companyId, Code = "01", Name = "Мой отдел", FullName = "Мой отдел", IsActive = true });

            var positionDirector = _DictDb.AddPosition(context, new InternalDictionaryPosition()
            { DepartmentId = departmentId, Name = "Директор", FullName = "Директор", Order = 1, IsActive = true });

            #endregion

            AddClientRoles(context);

            #region [+] DocumentsTypes ...

            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Письмо", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Приказ", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Распоряжение", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Служебная записка", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Поручение", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Протокол", IsActive = true });
            _DictDb.AddDocumentType(context, new InternalDictionaryDocumentType() { Name = "Договор", IsActive = true });

            // добавить шаблоны под каждый тип














            #endregion


        }

        /// <summary>
        /// Добавление дефолтных ролей для нового клиента
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool AddClientRoles(IContext context)
        {
            //pss локализиция наименований ролей
            _AdminDb.AddNamedRole(context, "Admin", "Администратор", GetRoleActionsForAdmin(context));

            _AdminDb.AddNamedRole(context, "DocumentActions", "Корректировка документов", GetRoleActionsForDocumentActions());

            _AdminDb.AddNamedRole(context, "ControlOfExecution", "Контроль", GetRoleActionsForControlOfExecution());

            _AdminDb.AddNamedRole(context, "Signing", "Подпись", GetRoleActionsForSigning());

            _AdminDb.AddNamedRole(context, "ChangeExecutor", "Выбор исполнителя", GetRoleActionsForChangeExecutor());

            _AdminDb.AddNamedRole(context, "DBN Control", "Управление ДБН", GetRoleActionsForDBNControl());

            _AdminDb.AddNamedRole(context, "Director", "Директор", GetRoleActionsForDirector());

            _AdminDb.AddNamedRole(context, "EmployeeDMS", "Сотрудник", GetRoleActionsForEmployeeDMS());

            return true;

        }

        #region [+] DefaultRoles
        private List<InternalAdminRoleAction> GetRoleActionsForAdmin(IContext context)
        {
            return _AdminDb.GetRoleActionsForAdmin(context);
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDocumentActions()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocument });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentFile });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentLink });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentPaper });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddDocumentTask });
            ///...
            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForControlOfExecution()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ControlChange });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForSigning()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AffixVisaing });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AffixSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.SelfAffixSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectSigning });
            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.RejectVisaing });
            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForChangeExecutor()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.ChangeExecutor });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDBNControl()
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

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForDirector()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumAdminActions.AddRole });

            return items;
        }

        private List<InternalAdminRoleAction> GetRoleActionsForEmployeeDMS()
        {
            var items = new List<InternalAdminRoleAction>();

            items.Add(new InternalAdminRoleAction() { ActionId = (int)EnumDocumentActions.AddNote });

            return items;
        }
        #endregion
       


    }
}
