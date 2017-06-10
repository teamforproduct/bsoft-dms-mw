using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Extensions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.WebUser;
using BL.Model.Common;
using BL.Model.Context;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Infrastructure;
using DMS_WebAPI.Models;
using DMS_WebAPI.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BL.Logic.SystemServices.TaskManagerService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BL.Logic.Common;

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
                employeeId = (int)tmpService.ExecuteAction(EnumActions.AddAgentEmployee, context, model);

                if (employeeId > 0)
                {
                    user = AddUserToClient(new AddWebUser
                    {
                        FullName = model.FirstName + " " + model.LastName,

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
                if (employeeId > 0) tmpService.ExecuteAction(EnumActions.DeleteAgentEmployee, context, employeeId);

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

                        orgId = (int)dicService.ExecuteAction(EnumActions.AddOrg, context, org);
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

                        depId = (int)dicService.ExecuteAction(EnumActions.AddDepartment, context, dep);
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

                        dicService.ExecuteAction(EnumActions.AddRegistrationJournal, context, jrn);

                        jrn = new AddRegistrationJournal
                        {
                            DepartmentId = depId,
                            IsActive = true,
                            IsOutcoming = true,
                            Index = jmodel.OutcomingJournalIndex,
                            Name = jmodel.OutcomingJournalName,
                        };

                        dicService.ExecuteAction(EnumActions.AddRegistrationJournal, context, jrn);

                        jrn = new AddRegistrationJournal
                        {
                            DepartmentId = depId,
                            IsActive = true,
                            IsInternal = true,
                            Index = jmodel.InternalJournalIndex,
                            Name = jmodel.InternalJournalName,
                        };

                        dicService.ExecuteAction(EnumActions.AddRegistrationJournal, context, jrn);
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
                        posId = (int)dicService.ExecuteAction(EnumActions.AddPosition, context, pos);
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

                assignmentId = (int)dicService.ExecuteAction(EnumActions.AddExecutor, context, ass);


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

                    var m = new MailWithCallToActionModel()
                    {
                        Greeting = languages.GetTranslation(model.LanguageId, Labels.Get("Mail", "Greeting"), new List<string> { model.FirstName }),
                        Closing = languages.GetTranslation(model.LanguageId, Labels.Get("Mail", "Closing")),
                        CallToActionUrl = clickURL,
                        CallToActionName = languages.GetTranslation(model.LanguageId, Labels.Get("Mail", "Welcome", "CallToActionName")),
                        CallToActionDescription = languages.GetTranslation(model.LanguageId, Labels.Get("Mail", "Welcome", "CallToActionDescription"), new List<string> { context.Employee.Name, settVal.GetClientAddress(clientCode) }),
                    };

                    var htmlContent = m.RenderPartialViewToString(RenderPartialView.MailWithCallToAction);
                    var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

                    mailService.SendMessage(null, MailServers.Noreply, res.Email, languages.GetTranslation(model.LanguageId, Labels.Get("Mail", "Welcome.Subject")), htmlContent);
                }

                return res.EmployeeId;
            }
            catch (Exception)
            {
                if (assignmentId > 0) dicService.ExecuteAction(EnumActions.DeleteExecutor, context, assignmentId);

                // Если создавали новую должность
                if (!model.PositionId.HasValue && posId > 0) dicService.ExecuteAction(EnumActions.DeletePosition, context, posId);

                // Если создавали новый отдел
                if (!model.DepartmentId.HasValue && depId > 0) dicService.ExecuteAction(EnumActions.DeleteDepartment, context, depId);

                // Если создавали новую организацию
                if (!model.OrgId.HasValue && orgId > 0) dicService.ExecuteAction(EnumActions.DeleteOrg, context, orgId);

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

            tmpItem.ExecuteAction(EnumActions.ModifyAgentEmployee, context, model);

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
            tmpService.ExecuteAction(EnumActions.DeleteAgentEmployee, context, agentId);

            DeleteUsersInClient(context.Client.Id, new List<string> { user.Id });
        }

    }
}