﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Extensions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.AdminCore.WebUser;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using DMS_WebAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {
        public EmployeeCreationResult AddUserEmployee(IContext context, AddAgentEmployeeUser model)
        {

            UserCreationResult user = null;
            model.UserName = model.Login;

            var employeeId = -1;

            // проверяю нет ли уже сотрудника с указанным имененм у клиента
            if (ExistsUserInClient(model.UserName, context.Client.Id)) throw new UserNameAlreadyExists(model.UserName);

            // пробую создать сотрудника
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            try
            {
                employeeId = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, context, model);

                if (employeeId > 0)
                {
                    user = AddUserToClient(new AddWebUser
                    {
                        FullName = model.LastName + " " + model.FirstName,

                        LanguageId = model.LanguageId,

                        Email = model.Login,
                        Phone = model.Phone,
                        // Для нового пользователя высылается письмо с линком на страницу "введите новый пароль"
                        Password = string.IsNullOrEmpty(model.Password) ? "k~WPop8V%W~11hG~~VGR" : model.Password,

                        EmailConfirmed = model.EmailConfirmed,
                        IsChangePasswordRequired = model.IsChangePasswordRequired,


                        // Предполагаю, что человек, который создает пользователей. создает их в тойже базе и в том же клиенте
                        // Первый пользователь создается под админ-контекстом
                        ClientId = context.Client.Id,
                    });

                    // обновляю сотрудника 
                    tmpService.SetAgentUserUserId(context, employeeId, user.Id);

                }

                var res = new EmployeeCreationResult(user);
                res.EmployeeId = employeeId;

                return res;
            }
            catch (Exception e)
            {
                if (employeeId > 0) tmpService.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, context, employeeId);

                if (user != null) DeleteUsersInClient(context.Client.Id, new List<string> { user.Id });

                throw e;
            }
        }

        public int AddUserEmployeeInOrg(IContext context, AddEmployeeInOrg model, AddJournalsInOrg jmodel = null, bool sendEmail = true)
        {
            var dicService = DmsResolver.Current.Get<IDictionaryService>();
            var employee = new AddAgentEmployeeUser();
            int assignmentId = -1;
            int orgId = -1;
            int depId = -1;
            int posId = -1;
            EmployeeCreationResult res = null;

            // проверяю нет ли уже сотрудника с указанным именем у клиента
            if (ExistsUserInClient(model.Login, context.Client.Id)) throw new UserNameAlreadyExists(model.Login);

            if (model.OrgId == null && string.IsNullOrEmpty(model.OrgName)) throw new OrgRequired();

            if (model.DepartmentId == null && string.IsNullOrEmpty(model.DepartmentName)) throw new DepartmentRequired();

            if (model.PositionId == null && string.IsNullOrEmpty(model.PositionName)) throw new PositionRequired();

            employee.Login = model.Login;
            employee.Password = model.Password;
            employee.Phone = model.Phone;
            employee.FirstName = model.FirstName;
            employee.MiddleName = model.MiddleName;
            employee.LastName = model.LastName;
            employee.IsActive = true;
            employee.LanguageId = model.LanguageId;
            employee.EmailConfirmed = model.EmailConfirmed;
            employee.IsChangePasswordRequired = model.IsChangePasswordRequired;

            try
            {  // тут возникает проблама транзакционности: если возникнет проблема при назначении сотрудника, то 
                using (var transaction = Transactions.GetTransaction())
                {
                    if (model.OrgId == null && !string.IsNullOrEmpty(model.OrgName))
                    {
                        var org = new AddOrg
                        {
                            FullName = model.OrgName,
                            Name = model.OrgName,
                            IsActive = true
                        };

                        orgId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddOrg, context, org);
                    }
                    else
                    {
                        orgId = model.OrgId.Value;
                    }


                    if (model.DepartmentId == null && !string.IsNullOrEmpty(model.DepartmentName))
                    {
                        var dep = new AddDepartment
                        {
                            CompanyId = orgId,
                            FullName = model.DepartmentName,
                            Name = model.DepartmentName,
                            IsActive = true,
                            Index = model.DepartmentIndex
                        };

                        depId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddDepartment, context, dep);
                    }
                    else
                    {
                        depId = model.DepartmentId.Value;
                    }

                    // не ожидал такой поворот событий. Добавляю тут журналы чтобы при создании должности на них проставились доступы по умолчанию
                    // нужно дробить логику создания должности на блоки и выносить в сервис из команды

                    if (jmodel != null)
                    {
                        var jrn = new AddRegistrationJournal
                        {
                            DepartmentId = depId,
                            IsActive = true,
                            IsIncoming = true,
                            Index = jmodel.IncomingJournalIndex,
                            Name = jmodel.IncomingJournalName,
                        };

                        dicService.ExecuteAction(EnumDictionaryActions.AddRegistrationJournal, context, jrn);

                        jrn = new AddRegistrationJournal
                        {
                            DepartmentId = depId,
                            IsActive = true,
                            IsOutcoming = true,
                            Index = jmodel.OutcomingJournalIndex,
                            Name = jmodel.OutcomingJournalName,
                        };

                        dicService.ExecuteAction(EnumDictionaryActions.AddRegistrationJournal, context, jrn);

                        jrn = new AddRegistrationJournal
                        {
                            DepartmentId = depId,
                            IsActive = true,
                            IsInternal = true,
                            Index = jmodel.InternalJournalIndex,
                            Name = jmodel.InternalJournalName,
                        };

                        dicService.ExecuteAction(EnumDictionaryActions.AddRegistrationJournal, context, jrn);
                    }

                    if (model.PositionId == null && !string.IsNullOrEmpty(model.PositionName))
                    {
                        var pos = new AddPosition
                        {
                            DepartmentId = depId,
                            FullName = model.PositionName,
                            Name = model.PositionName,
                            IsActive = true,
                            Role = model.Role
                        };

                        // Создается должность. + доступы к журналам, рассылка и роль
                        posId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddPosition, context, pos);
                    }
                    else
                    {
                        posId = model.PositionId.Value;
                    }

                    transaction.Complete();
                }

                // Создаю сотрудника-пользователя
                res = AddUserEmployee(context, employee);


                // назначаю сотрудника на должность
                var ass = new AddPositionExecutor
                {
                    AccessLevelId = model.AccessLevel,
                    AgentId = res.EmployeeId,
                    IsActive = true,
                    PositionId = posId,
                    StartDate = DateTime.UtcNow.StartOfDay(),
                    PositionExecutorTypeId = model.ExecutorType
                };
                // AAV попросил делать назначение на начало дня.

                assignmentId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddExecutor, context, ass);


                var languages = DmsResolver.Current.Get<ILanguages>();

                // Отправка приглашения
                // Если пользователь уже был в базе, то ему нужно выслать только ссылку на нового клиента, а если нет то ссылку на смену пароля
                if (sendEmail)
                {
                    var clickURL = string.Empty;
                    var clientCode = _webDb.GetClientCode(context.Client.Id);
                    var settVal = DmsResolver.Current.Get<ISettingValues>();

                    if (res.IsNew)
                    {
                        var baseUri = new Uri(settVal.GetClientAddress(clientCode));

                        string url = new Uri(baseUri, "finish-registration").ToString();

                        var passwordResetToken = UserManager.GeneratePasswordResetToken(res.Id);

                        var builder = new UriBuilder(url);
                        var newQuery = HttpUtility.ParseQueryString(builder.Query);
                        newQuery.Add("UserId", res.Id);
                        newQuery.Add("Code", passwordResetToken);

                        builder.Query = newQuery.ToString();

                        clickURL = builder.ToString();
                    }
                    else
                    {
                        clickURL = settVal.GetClientAddress(clientCode);
                    }

                    var we = new WelcomeEmailModel()
                    {
                        Url = clickURL,
                        FirstName = model.FirstName,
                        ClientName = settVal.GetClientAddress(clientCode),
                        InvitingName = context.Employee.Name,
                    };

                    var htmlContent = we.RenderPartialViewToString(RenderPartialView.WelcomeEmail);
                    var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

                    mailService.SendMessage(null, MailServers.Noreply, res.Email, languages.GetTranslation(model.LanguageId, "##l@Mail.Welcome.Subject@l##"), htmlContent);
                }

                return res.EmployeeId;
            }
            catch (Exception)
            {
                if (assignmentId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteExecutor, context, assignmentId);

                // Если создавали новую должность
                if (!model.PositionId.HasValue && posId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeletePosition, context, posId);

                // Если создавали новый отдел
                if (!model.DepartmentId.HasValue && depId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteDepartment, context, depId);

                // Если создавали новую организацию
                if (!model.OrgId.HasValue && orgId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteOrg, context, orgId);

                if (res != null) DeleteUserEmployee(context, res.EmployeeId);

                throw;
            }
        }

        public int UpdateUserEmployee(IContext context, ModifyAgentEmployee model)
        {
            var user = GetUser(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            var tmpStore = DmsResolver.Current.Get<ITempStorageService>();

            if (model.ImageId.HasValue)
            {
                var avaFile = tmpStore.GetStoreObject(model.ImageId.Value);
                if (avaFile is BaseFile)
                {
                    model.PostedFileData = Convert.ToBase64String((avaFile as BaseFile).FileContent);
                }
            }

            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, context, model);

            // При деактивации сотрудника 
            //if (!model.IsActive)
            //{
            //    var userContexts = DmsResolver.Current.Get<UserContexts>();
            //    userContexts.RemoveByClientId(model.Id);
            //}

            if (model.ImageId.HasValue)
            {
                tmpStore.RemoveStoreObject(model.ImageId.Value);
            }

            return model.Id;
        }

        public void DeleteUserEmployee(IContext context, int agentId)
        {
            var user = GetUser(context, agentId);

            if (user == null) throw new UserIsNotDefined();

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, context, agentId);

            DeleteUsersInClient(context.Client.Id, new List<string> { user.Id });
        }

    }
}