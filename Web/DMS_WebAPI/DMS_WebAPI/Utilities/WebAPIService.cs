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
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BL.Logic.SystemServices.TaskManagerService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DMS_WebAPI.Utilities
{
    internal class WebAPIService
    {
        private readonly WebAPIDbProcess _webDb;

        public WebAPIService(WebAPIDbProcess webDb)
        {
            _webDb = webDb;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                var owinContext = HttpContext.Current.Request.GetOwinContext();
                return owinContext.GetUserManager<ApplicationUserManager>();
            }
        }

        public AspNetUsers GetUserById(string id)
        {
            return UserManager.FindById(id);
        }

        public AspNetUsers GetUser(IContext context, int agentId)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return GetUserById(userId);
        }

        public AspNetUsers GetUser(string userName)
        {
            return UserManager.FindByName(userName);
        }

        public async Task<AspNetUsers> GetUser(string userName, string userPassword)
        {
            return await UserManager.FindAsync(userName, userPassword);
        }

        public async Task<AspNetUsers> GetUserByIdAsync(string id)
        {
            return await UserManager.FindByIdAsync(id);
        }

        public async Task<AspNetUsers> GetUserAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        public async Task<AspNetUsers> GetUserAsync(IContext context, int agentId)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return await GetUserByIdAsync(userId);
        }

        public bool ExistsUser(string userName)
        {
            var user = GetUser(userName);
            return user != null;
        }

        public bool ExistsUserInClient(AspNetUsers user, int clientId)
        {
            return (GetUserClient(user.Id, clientId) != null);
        }

        public FrontAspNetUserClient GetUserClient(string userId, int clientId)
        {
            return _webDb.GetUserClientList(new FilterAspNetUserClient
            {
                ClientIDs = new List<int> { clientId },
                UserIDs = new List<string> { userId }
            }).FirstOrDefault();
        }

        public bool ExistsUserInClient(string userName, int clientId)
        {
            var user = GetUser(userName);

            return ExistsUserInClient(user, clientId);
        }


        public FrontAgentEmployeeUser GetUserInfo(IContext context)
        {
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var employee = dicProc.GetDictionaryAgentEmployee(context, context.CurrentAgentId);

            var user = GetUserById(context.User.Id);

            var logger = DmsResolver.Current.Get<ILogger>();
            var lastUserLoginInfo = logger.GetLastUserLoginInfo(context);

            return new FrontAgentEmployeeUser()
            {
                Id = employee.Id,
                Name = employee.Name,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                FullName = employee.FullName,
                IsMale = employee.IsMale,
                LanguageId = employee.LanguageId,
                LanguageCode = employee.LanguageCode,
                LanguageName = employee.LanguageName,

                Login = user.Email,
                TaxCode = employee.TaxCode,
                PersonnelNumber = employee.PersonnelNumber,
                BirthDate = employee.BirthDate,
                Description = employee.Description,
                IsActive = employee.IsActive,

                LastSuccessLogin = lastUserLoginInfo.LastSuccessLogin,
                LastErrorLogin = lastUserLoginInfo.LastErrorLogin,
                CountErrorLogin = lastUserLoginInfo.CountErrorLogin,
            };
        }

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
                        FirstName = model.FirstName,
                        LastName = model.LastName,

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
                    tmpService.SetAgentUserUserId(context, new InternalDictionaryAgentUser
                    {
                        Id = employeeId,
                        UserId = user.Id
                    });

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
                        var org = new AddOrg();
                        org.FullName = model.OrgName;
                        org.Name = model.OrgName;
                        org.IsActive = true;

                        orgId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddOrg, context, org);
                    }
                    else
                    {
                        orgId = model.OrgId.Value;
                    }


                    if (model.DepartmentId == null && !string.IsNullOrEmpty(model.DepartmentName))
                    {
                        var dep = new AddDepartment();
                        dep.CompanyId = orgId;
                        dep.FullName = model.DepartmentName;
                        dep.Name = model.DepartmentName;
                        dep.IsActive = true;
                        dep.Index = model.DepartmentIndex;

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
                var ass = new AddPositionExecutor();
                ass.AccessLevelId = model.AccessLevel;
                ass.AgentId = res.EmployeeId;
                ass.IsActive = true;
                ass.PositionId = posId;
                ass.StartDate = DateTime.UtcNow.StartOfDay(); // AAV попросил делать назначение на начало дня.
                ass.PositionExecutorTypeId = model.ExecutorType;

                assignmentId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddExecutor, context, ass);


                var languages = DmsResolver.Current.Get<ILanguages>();

                // Отправка приглашения
                // Если пользователь уже был в базе, то ему нужно выслать только ссылку на нового клиента, а если нет то ссылку на смену пароля
                if (sendEmail)
                {
                    if (res.IsNew)
                    {
                        var tmp = new RestorePasswordAgentUser
                        {
                            ClientCode = _webDb.GetClientCode(context.Client.Id),
                            Email = res.Email,
                            FirstEntry = "true",
                        };

                        // Это временная залипуха, нужно разбираться почему password-restore
                        //Task.Factory.StartNew(async () => { await RestorePasswordAgentUserAsync(tmp); }).Wait();

                        RestorePassword(tmp);
                    }
                    else
                    {
                        var clientCode = _webDb.GetClientCode(context.Client.Id);
                        var settVal = DmsResolver.Current.Get<ISettingValues>();
                        var we = new WelcomeEmailModel()
                        {
                            UserName = res.UserName,
                            UserEmail = res.Email,
                            ClientUrl = settVal.GetClientAddress(clientCode),
                            CabinetUrl = settVal.GetClientAddress(clientCode) + "/cabinet/",
                            OstreanEmail = settVal.GetMailDocumEmail(),
                            SpamUrl = settVal.GetMailNoreplyEmail(),
                        };

                        var htmlContent = we.RenderPartialViewToString(RenderPartialView.WelcomeEmail);
                        var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

                        mailService.SendMessage(null, MailServers.Noreply, res.Email, languages.GetTranslation(model.LanguageId, "##l@Mail.Welcome.Subject@l##"), htmlContent);
                    }
                }

                return res.EmployeeId;
            }
            catch (Exception)
            {
                if (assignmentId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteExecutor, context, assignmentId);

                if (res != null) DeleteUserEmployee(context, res.EmployeeId);

                // Если создавали новую должность
                if (!model.PositionId.HasValue && posId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeletePosition, context, posId);

                // Если создавали новый отдел
                if (!model.DepartmentId.HasValue && depId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteDepartment, context, depId);

                // Если создавали новую организацию
                if (!model.OrgId.HasValue && orgId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteOrg, context, orgId);

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

            // При деактивации сотрудника деактивирую пользователя
            if (!model.IsActive)
            {
                var userContexts = DmsResolver.Current.Get<UserContexts>();
                userContexts.RemoveByAgentId(model.Id);
            }

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

        private AspNetUsers AddUser(string firstName, string lastName, string userName, string userPassword, int languageId, string userEmail, string userPhone = "",
            bool emailConfirmed = false, bool isChangePasswordRequired = true)
        {
            var now = DateTime.UtcNow;

            var user = new AspNetUsers()
            {
                FirstName = firstName?.Trim(),
                LastName = lastName?.Trim(),
                UserName = userName?.Trim(),
                Email = userEmail?.Trim(),
                LanguageId = languageId,
                PhoneNumber = userPhone?.Trim(),
                IsChangePasswordRequired = isChangePasswordRequired,
                EmailConfirmed = emailConfirmed,
                CreateDate = now,
                LastChangeDate = now,
            };

            using (var transaction = Transactions.GetTransaction())
            {
                IdentityResult result = UserManager.Create(user, userPassword);

                if (!result.Succeeded) throw new UserCouldNotBeAdded(userEmail, result.Errors);

                transaction.Complete();

                return user;
            }
        }


        public UserCreationResult AddUserToClient(AddWebUser model)
        {

            // Решено для разных клиентов не создавать дубликаты юзеров
            // Один пользователь может работать на разных клиентов
            // Один клиент имеет выделенное место в базе согластно алгоритму
            var user = GetUser(model.Email);
            var isNew = user == null;

            using (var transaction = Transactions.GetTransaction())
            {
                if (isNew)
                {
                    user = AddUser(model.FirstName, model.LastName, model.Email, model.Password, model.LanguageId, model.Email, model.Phone,
                        model.EmailConfirmed, model.IsChangePasswordRequired);

                }
                else
                {
                    // Если пользователь существует, то проверяю не пытаются ли его залинковать с тем же клиентом
                    var uc = _webDb.GetUserClientList(new FilterAspNetUserClient
                    {
                        ClientIDs = new List<int> { model.ClientId },
                        UserIDs = new List<string> { user.Id }
                    }).FirstOrDefault();

                    if (uc != null) throw new UserNameAlreadyExists(user.UserName);

                }

                // линкую пользователя с клиентом
                _webDb.AddUserClient(new SetUserClient { UserId = user.Id, ClientId = model.ClientId });

                transaction.Complete();
            }

            return new UserCreationResult(user, isNew);
        }

        private void DeleteUsersInClient(int clientId, List<string> userIDs)
        {
            if (userIDs == null)
            {
                // запоминаю пользователей клиента, которых потенциально нужно удалить
                userIDs = _webDb.GetUserClientList(new FilterAspNetUserClient { ClientIDs = new List<int> { clientId } }).Select(x => x.UserId).ToList();
            }

            if (userIDs.Count() == 0) return;

            //using (var transaction = Transactions.GetTransaction())
            {
                // Удаляю связи пользователя с клиентом
                _webDb.DeleteUserClient(new FilterAspNetUserClient
                {
                    UserIDs = userIDs,
                    ClientIDs = new List<int> { clientId }
                });

                // пользователи, которые завязаны на других клиентов удалять нельзя, но они в списке для удаления
                var safeList = _webDb.GetUserClientList(new FilterAspNetUserClient { UserIDs = userIDs }).Select(x => x.UserId).ToList();

                if (safeList.Any()) userIDs.RemoveAll(x => safeList.Contains(x));

                if (userIDs.Any())
                {
                    _webDb.DeleteUserContexts(new FilterAspNetUserContext { UserIDs = userIDs });
                    _webDb.DeleteUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = userIDs });

                    foreach (var userId in userIDs)
                    {
                        var user = GetUserById(userId);

                        IdentityResult result = UserManager.Delete(user);

                        if (!result.Succeeded) throw new UserCouldNotBeDeleted(user.Email, result.Errors);
                    }
                }

                //transaction.Complete();
            }
        }

        private string AddRole(string roleName)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                var role = roleManager.FindByName(roleName);

                if (role == null || string.IsNullOrEmpty(role.Id)) throw new RoleNameAlreadyExists(roleName);

                IdentityResult result = roleManager.Create(new IdentityRole { Name = roleName });

                if (!result.Succeeded) throw new RoleCouldNotBeAdded(roleName, result.Errors);
                transaction.Complete();
                return role.Id;
            }
        }

        public bool ExistsClient(FilterAspNetClients filter)
        {
            var setVals = DmsResolver.Current.Get<ISettingValues>();
            var banHosts = setVals.GetSystemHosts();

            if (filter == null) throw new FilterRequired();

            if (banHosts.Contains(filter.Code)) return true;

            var f = new FilterAspNetClientRequests { CodeExact = filter.Code };

            // Проверка уникальности доменного имени
            var exists = ExistsClientRequest(f);

            if (exists) return true;

            return _webDb.ExistsClients(filter);
        }

        public bool ExistsClientRequest(FilterAspNetClientRequests filter)
        {
            if (filter == null) throw new FilterRequired();

            return _webDb.ExistsClientRequests(filter);
        }

        public async Task<int> AddClientSaaSRequest(AddClientSaaS model)
        {
            if (!string.IsNullOrEmpty(model.Recaptcha))
            {
                //  https://developers.google.com/recaptcha/docs/verify
                var setVal = DmsResolver.Current.Get<ISettingValues>();
                var url = setVal.GetGoogleReCaptchaURL();

                var values = new Dictionary<string, string>
                {
                   { "secret", setVal.GetGoogleReCaptchaSecret() }, // Required. The shared key between your site and reCAPTCHA.
                   { "response", model.Recaptcha }, // Required. The user response token provided by reCAPTCHA, verifying the user on your site.
                   //{ "remoteip",  }, // Optional. The user's IP address.
                };

                var content = new FormUrlEncodedContent(values);

                var httpClient = DmsResolver.Current.Get<HttpClient>();

                var response = await httpClient.PostAsync(url, content);

                /* The response is a JSON object:
                {
                  "success": true|false,
                  "challenge_ts": timestamp,  // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
                  "hostname": string,         // the hostname of the site where the reCAPTCHA was solved
                  "error-codes": [...]        // optional
                } */

                //if (response.StatusCode != System.Net.HttpStatusCode.OK) throw new HttpException((int)response.StatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(json);

                if (jObject.Value<bool>("success") == false)
                {
                    throw new ClientCreateException(jObject.GetValue("error-codes").Select(x => x.ToString()).ToList());
                }

            }

            if (ExistsClient(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            if (string.IsNullOrEmpty(model.ClientName)) model.ClientName = model.ClientCode;

            int res = 0;

            var languages = DmsResolver.Current.Get<ILanguages>();

            try
            {
                model.HashCode = model.ClientCode.md5();
                model.SMSCode = DateTime.UtcNow.ToString("ssHHmm");

                if (string.IsNullOrEmpty(model.Language))
                {
                    var language = languages.GetDefaultLanguage();
                    if (language != null) model.Language = language.Code;
                }

                res = _webDb.AddClientRequest(model);

                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var addr = tmpService.GetAuthAddress();
                var callbackurl = new Uri(new Uri(addr), "new-client").AbsoluteUri;

                // isNew можно вычислить только на текущий момент времени (пользователь может сделать несколько компаний)
                var isNew = !ExistsUser(model.Email);

                callbackurl += $"?hash={model.HashCode}&login={model.Email}&code={model.ClientCode}&isNew={isNew}&language={model.Language}";

                var m = new WelcomeEmailModel()
                {
                    UserEmail = model.Email,
                    UserName = model.FirstName,
                    ClientUrl = callbackurl,
                };

                var htmlContent = m.RenderPartialViewToString(RenderPartialView.WelcomeEmail);
                var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();


                mailService.SendMessage(null, MailServers.Docum, model.Email, languages.GetTranslation("##l@Mail.Welcome.Subject@l##"), htmlContent);
            }
            catch (Exception)
            {
                if (res > 0) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { IDs = new List<int> { res } });
                throw;
            }

            return res;
        }

        public void DeleteOldClientRequest()
        {
            // Заявки которые старее 24 часов
            _webDb.DeleteClientRequest(new FilterAspNetClientRequests { DateCreateLess = DateTime.UtcNow.AddDays(-1) });
        }

        public void DeleteOldUserContexts()
        {
            // контексты, которые не испольвались 14 дней
            _webDb.DeleteUserContexts(new FilterAspNetUserContext { LastUsegeDateLess = DateTime.UtcNow.AddDays(-14) });
        }

        public async Task<string> AddClientByEmail(AddClientFromHash model)
        {
            var responseString = string.Empty;

            var request = _webDb.GetClientRequests(new FilterAspNetClientRequests { HashCodeExact = model.Hash }).FirstOrDefault();

            if (request == null) throw new ClientRequestIsNotFound();

            request.Password = model.Password;
            request.OrgName = model.OrgName;
            request.DepartmentName = model.DepartmentName;
            request.PositionName = model.PositionName;

            var isDone = await AddClientSaaS(request);

            if (isDone) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { HashCodeExact = model.Hash });

            //---------------------------------------------------

            // есть задача авторизовать по темному нового пользователя

            if (!string.IsNullOrEmpty(model.Password))
            {
                var setVal = DmsResolver.Current.Get<ISettingValues>();
                var localHost = setVal.GetLocalHost();
                var uri = new Uri(new Uri(localHost), ApiPrefix.V3 + "token");

                var values = new Dictionary<string, string>
                {
                   { "username", request.Email },
                   { "password", model.Password },
                   { "client_id", request.ClientCode },
                   { "client_secret", request.ClientCode },
                   { "scope", "" },
                   { "grant_type", "password" },
                   { "fingerprint", "SystemAuthorization" }
                };

                var content = new FormUrlEncodedContent(values);

                var httpContext = HttpContext.Current;
                var httpClient = DmsResolver.Current.Get<HttpClient>();

                httpClient.DefaultRequestHeaders.Clear();
                // Начиняю Headers запроса параметрами из текущего запроса
                foreach (var key in httpContext.Request.Headers.AllKeys)
                {

                    if (key == "Content-Type") continue;
                    if (key == "Content-Length") continue;

                    try
                    {
                        var value = httpContext.Request.Headers.Get(key);
                        //content.Headers.Add(key, value);
                        httpClient.DefaultRequestHeaders.Add(key, value);
                    }
                    catch { }

                }

                var response = await httpClient.PostAsync(uri, content);

                responseString = await response.Content.ReadAsStringAsync();

            }

            return responseString;
        }

        public async Task AddClientBySMS(AddClientFromSMS model)
        {
            var request = _webDb.GetClientRequests(new FilterAspNetClientRequests { SMSCodeExact = model.SMSCode }).FirstOrDefault();

            if (request == null) throw new ClientRequestIsNotFound();

            var isDone = await AddClientSaaS(request);

            if (isDone) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { SMSCodeExact = model.SMSCode });
        }

        public async Task<bool> AddClientSaaS(AddClientSaaS model)
        {
            // Проверка уникальности доменного имени
            if (_webDb.ExistsClients(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            var httpClient = DmsResolver.Current.Get<HttpClient>();

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var mHost = tmpService.GetMainHost();
            var vHost = tmpService.GetVirtualHost();
#if DEBUG
            vHost = "http://10.88.12.21:82";
#endif
            var request = $"{vHost}/newhost.pl?fqdn={model.ClientCode}.{mHost}";

            var responseString = await httpClient.GetStringAsync(request);

            switch (responseString)
            {
                case "Created":
                    break;
                case "BadRequest":
                    throw new ClientCodeRequired(); //- не указан параметр fqdn
                case "PreconditionFailed":
                    throw new ClientCodeInvalid(); //- параметр не соответствует маске[-0 - 9a - z].ostrean.com
                case "Conflict":
                    throw new ClientCodeAlreadyExists(model.ClientCode); //- такой субдомен уже существует
                default:
                    throw new ClientCreateException(new List<string> { responseString });
            }

            // сервер может определяться более сложным образом: с учетом нагрузки, количества клиентов
            var settings = DmsResolver.Current.Get<ISettingValues>();
            var dbName = settings.GetCurrentServerName();

            var db = _webDb.GetServers(new FilterAdminServers { ServerNameExact = dbName }).FirstOrDefault();

            if (db == null)
            {
                // определяю сервер для клиента пока первый попавшийся
                db = _webDb.GetServers(new FilterAdminServers()).FirstOrDefault();
            }

            if (db == null) throw new ServerIsNotFound();


            //if (string.IsNullOrEmpty(model.Password)) model.Password = "admin_" + model.ClientCode;

            using (var transaction = Transactions.GetTransaction())
            {
                // Создаю запись о клиенте
                model.ClientId = _webDb.AddClient(new ModifyAspNetClient
                {
                    Name = model.ClientCode,
                    Code = model.ClientCode,
                });


                // Линкую клиента на сервер
                _webDb.AddClientServer(new SetClientServer
                {
                    ClientId = model.ClientId,
                    ServerId = db.Id,
                });


                transaction.Complete();
            }

            var dbAdmin = new DatabaseModelForAdminContext(db);
            dbAdmin.ClientId = model.ClientId;
            dbAdmin.ClientCode = model.ClientCode;

            var ctx = new AdminContext(dbAdmin);

            var languages = DmsResolver.Current.Get<ILanguages>();

            // Если не указан язык, беру язык по умолчанию 
            ctx.User.LanguageId = string.IsNullOrEmpty(model.Language)
                ? languages.GetLanguageIdByHttpContext()
                : languages.GetLanguageIdByCode(model.Language);

            var clientService = DmsResolver.Current.Get<IClientService>();


            try
            {
                // Предзаполняю клиентскую базу настроками, ролями
                clientService.AddDictionary(ctx, model);

            }
            catch (Exception)
            {
                if (model.ClientId > 0) await DeleteClient(model.ClientId);

                throw;
            }

            AddUserEmployeeInOrg(ctx,
                new AddEmployeeInOrg
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    OrgName = model.OrgName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyCompany" + "@l##"),
                    DepartmentIndex = "01",
                    DepartmentName = model.DepartmentName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyDepartment" + "@l##"),
                    PositionName = model.PositionName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyPosition" + "@l##"),
                    ExecutorType = EnumPositionExecutionTypes.Personal,
                    AccessLevel = EnumAccessLevels.Personally,
                    LanguageId = ctx.User.LanguageId,
                    Phone = model.PhoneNumber,
                    Login = model.Email,
                    Role = Roles.Admin,
                    Password = model.Password,
                    // Создание клиента происходит по факту клика по ссылке в письме, поэтому при создании пользователя подтверждать емаил не нужно
                    EmailConfirmed = true,
                    IsChangePasswordRequired = false,
                    IsEmailConfirmRequired = false,
                },
                new AddJournalsInOrg
                {
                    IncomingJournalIndex = "01",
                    IncomingJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Incoming.ToString())),

                    OutcomingJournalIndex = "02",
                    OutcomingJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Outcoming.ToString())),

                    InternalJournalIndex = "03",
                    InternalJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Internal.ToString())),
                },
                sendEmail: false);

            try
            {
                //add workers for new client. Check if settings exists for that workers. 
                var tskInit = DmsResolver.Current.Get<ICommonTaskInitializer>();
                tskInit.InitializeAutoPlanTask(new List<DatabaseModelForAdminContext> { dbAdmin });
                tskInit.InitializeClearTrashTask(new List<DatabaseModelForAdminContext> { dbAdmin });
                tskInit.InitializeMailWorkerTask(new List<DatabaseModelForAdminContext> { dbAdmin });
            }
            catch (Exception)
            {
                if (model.ClientId > 0) await DeleteClient(model.ClientId);

                throw;
            }

            //UserManager.AddLogin(userId, new UserLoginInfo {    })
            return true;

        }

        public async Task DeleteClient(int Id)
        {
            if (Id == 1) throw new ClientIsNotFound();

            var client = _webDb.GetClients(new FilterAspNetClients { IDs = new List<int> { Id } }).FirstOrDefault();

            if (client == null) throw new ClientIsNotFound();

            var clients = new List<int> { Id };

            var server = _webDb.GetClientServer(Id);
            var ctx = new AdminContext(server);
            var taskManager = DmsResolver.Current.Get<ITaskManager>();
            taskManager.RemoveTaskForClient(Id);
            var clientService = DmsResolver.Current.Get<IClientService>();
            clientService.Delete(ctx);

            //using (var transaction = Transactions.GetTransaction())
            {
                _webDb.DeleteClientLicence(new FilterAspNetClientLicences { ClientIds = clients });

                DeleteUsersInClient(Id, null);

                _webDb.DeleteClientServer(new FilterAspNetClientServer { ClientIDs = clients });

                _webDb.DeleteClient(Id);

                //transaction.Complete();
            }
            var httpClient = DmsResolver.Current.Get<HttpClient>();

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var mHost = tmpService.GetMainHost();
            var vHost = tmpService.GetVirtualHost();
            var request = $"{vHost}/deletehost.pl?fqdn={client.Code}.{mHost}";
            var responseString = await httpClient.GetStringAsync(request);

        }

        //private string LogIn(string userName, string userPassword)
        //{
        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //    {
        //        return BadRequest("Incoming login failure.");
        //    }

        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));
        //}

        public int AddClient(AddAspNetClient model)
        {
            //TODO в transaction не может подлючиться к базе
            if (model.Server.Id <= 0)
            {
                _webDb.InitializerDatabase(model.Server);
            }

            // Проверка уникальности доменного имени
            if (_webDb.ExistsClients(new FilterAspNetClients { Code = model.Client.Code })) throw new ClientCodeAlreadyExists(model.Client.Code);

            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));


                #region Create client 1 

                // Создаю клиента
                var clientId = _webDb.AddClient(new ModifyAspNetClient
                {
                    Name = model.Client.Name,
                    Code = model.Client.Code,
                });

                AspNetClientLicences clientLicence;
                if (model.LicenceId > 0)
                {
                    _webDb.AddClientLicence(clientId, model.LicenceId.GetValueOrDefault());
                }

                #endregion Create client 1

                transaction.Complete();

                return clientId;

            }
        }


        #region UserAgent

        public async Task ChangeLoginAgentUser(ChangeLoginAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var context = userContexts.Get();

            var user = GetUser(context, model.Id);
            if (user == null) throw new UserIsNotDefined();

            if (user.UserName == model.NewEmail) return;

            user.UserName = model.NewEmail;
            user.Email = model.NewEmail;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                //TODO тут нужно пробежаться по всем клиентским базам и заменить логин
                var admService = DmsResolver.Current.Get<IAdminService>();
                admService.ChangeLoginAgentUser(context, model);
            }
            else throw new UserLoginCouldNotBeChanged(model.NewEmail, result.Errors);

            // выкидываю пользователя из системы
            userContexts.RemoveByAgentId(model.Id);

            await ConfirmEmailAgentUser(model.Id);

        }

        public async Task ConfirmEmailAgentUser(int agentId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var user = GetUser(context, agentId);
            if (user == null) throw new UserIsNotDefined();
            user.EmailConfirmed = false;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var addr = tmpService.GetClientAddress(context.Client.Code);
            var callbackurl = new Uri(new Uri(addr), "email-confirmation").AbsoluteUri;

            callbackurl += String.Format("?userId={0}&code={1}", user.Id, HttpUtility.UrlEncode(emailConfirmationCode));

            var htmlContent = callbackurl.RenderPartialViewToString(RenderPartialView.PartialViewNameChangeLoginAgentUserVerificationEmail);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

            var languages = DmsResolver.Current.Get<ILanguages>();

            mailService.SendMessage(context, MailServers.Noreply, user.Email, languages.GetTranslation("##l@EmailSubject:EmailConfirmation@l##"), htmlContent);
        }

        public async Task ChangeFingerprintEnabled(bool parm)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var userContext = userContexts.Get();
            var user = GetUserById(userContext.User.Id);

            user.IsFingerprintEnabled = parm;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

        }

        public void ChangeControlQuestion(ModifyAspNetUserControlQuestion model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var userContext = userContexts.Get();
            var user = GetUserById(userContext.User.Id);

            user.ControlQuestionId = model.QuestionId;
            user.ControlAnswer = model.Answer;
            user.LastChangeDate = DateTime.UtcNow;

            var result = UserManager.Update(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

        }

        public async Task RestorePassword(RestorePasswordAgentUser model)
        {
            var user = await GetUserAsync(model.Email);

            if (user == null) throw new UserIsNotDefined();

            // Для заблокированного пользователя запрещаю смену пароля
            if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user.Id) && await UserManager.IsLockedOutAsync(user.Id)) throw new UserIsLockout();

            var settVal = DmsResolver.Current.Get<ISettingValues>();
            string baseUrl = new Uri(new Uri(settVal.GetClientAddress(model.ClientCode)), "restore-password").ToString();

            var passwordResetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var builder = new UriBuilder(baseUrl);
            var newQuery = HttpUtility.ParseQueryString(builder.Query);
            newQuery.Add("UserId", user.Id);
            newQuery.Add("Code", passwordResetToken);

            if (!string.IsNullOrEmpty(model.FirstEntry)) newQuery.Add("FirstEntry", model.FirstEntry);

            //var query = new NameValueCollection();
            //newQuery.Add(query);

            builder.Query = newQuery.ToString();// string.Join("&", nvc.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));


            var m = new RestorePasswordModel()
            {
                FirstName = user.FirstName,
                // сылка на восстановление пароля
                Url = builder.ToString(),
            };

            // html с подставленной моделью
            var htmlContent = m.RenderPartialViewToString(RenderPartialView.RestorePassword);


            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            var languages = DmsResolver.Current.Get<ILanguages>();
            mailService.SendMessage(null, MailServers.Noreply, model.Email, languages.GetTranslation(user.LanguageId, "##l@Mail.RestorePassword.Subject@l##"), htmlContent);
        }


        public async Task ChangePasswordAgentUserAsync(ChangePasswordAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var ctx = userContexts.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ExecuteAction(EnumAdminActions.ChangePassword, ctx, model.Id);

            var user = await GetUserAsync(ctx, model.Id);

            if (user == null) throw new UserIsNotDefined();

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            user.IsChangePasswordRequired = model.IsChangePasswordRequired;//true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            if (model.IsKillSessions)
                userContexts.RemoveByAgentId(model.Id);

        }

        public async Task ConfirmEmail(string userId, string code)
        {
            // Для заблокированного пользователя запрещаю смену пароля
            if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(userId) && await UserManager.IsLockedOutAsync(userId)) throw new UserIsLockout();

            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (!result.Succeeded) throw new UserEmailCouldNotBeConfirmd(result.Errors);
        }

        public async Task<string> ResetPassword(ConfirmRestorePassword model)
        {
            var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.NewPassword);

            if (!result.Succeeded) throw new ResetPasswordCodeInvalid(result.Errors);

            AspNetUsers user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null) throw new UserIsNotDefined();

            user.IsChangePasswordRequired = false;
            user.EmailConfirmed = true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            userContexts.RemoveByUserId(model.UserId);

            return user.Email;
        }

        #endregion

        #region Fingerprints
        public IEnumerable<FrontAspNetUserFingerprint> GetUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            return _webDb.GetUserFingerprints(filter);
        }

        public FrontAspNetUserFingerprint GetUserFingerprint(int id)
        {
            return _webDb.GetUserFingerprints(new FilterAspNetUserFingerprint { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public bool ExistsUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            return _webDb.ExistsUserFingerprints(filter);
        }

        //public int MergeUserFingerprint(IContext userContext, AddAspNetUserFingerprint model)
        //{
        //    if (string.IsNullOrEmpty(model.UserId))
        //    {
        //        var user = GetUser(userContext, userContext.CurrentAgentId);
        //        model.UserId = user.Id;
        //    }

        //    var fp = _webDb.GetUserFingerprints(new FilterAspNetUserFingerprint
        //    {
        //        UserIDs = new List<string> { model.UserId },
        //        FingerprintExact = model.Fingerprint
        //    }).FirstOrDefault();

        //    return fp?.Id ?? AddUserFingerprint(userContext, model);
        //}

        public int AddUserFingerprint(AddAspNetUserFingerprint model)
        {

            //if (string.IsNullOrEmpty(model.UserId))
            //{
            //    //TODO ASYNC
            //    var user = GetUser(userContext, userContext.CurrentAgentId);
            //    model.UserId = user.Id;
            //}

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

            model.Browser = bc.Browser;
            model.Platform = bc.Platform;

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = bc.Browser + " " + bc.Platform + " " + DateTime.UtcNow.ToString("HHmmss");
            }

            return _webDb.AddUserFingerprint(model);
        }

        public void UpdateUserFingerprint(ModifyAspNetUserFingerprint model)
        {
            _webDb.UpdateUserFingerprint(model);
        }

        public void DeleteUserFingerprint(int id)
        {
            _webDb.DeleteUserFingerprints(new FilterAspNetUserFingerprint { IDs = new List<int> { id } });
        }


        public IEnumerable<ListItem> GetControlQuestions(string language)
        {
            return _webDb.GetControlQuestions(language);
        }

        public async Task SwitchOffFingerprint(IContext context, Item model)
        {
            var user = await GetUserAsync(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            user.IsFingerprintEnabled = false;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);
        }

        #endregion

        public IEnumerable<AspNetUserContexts> GetUserContexts(FilterAspNetUserContext filter)
        {
            return _webDb.GetUserContexts(filter);
        }

        public int SaveUserContexts(IContext context)
        {

            var model = new AspNetUserContexts
            {
                Token = context.Token,
                ClientId = context.Client.Id,
                CurrentPositionsIdList = string.Join(",", context.CurrentPositionsIdList),
                UserId = context.User.Id,
                LastChangeDate = DateTime.UtcNow,
                Fingerprint = context.User.Fingerprint,
            };


            var uc = _webDb.GetUserContexts(new FilterAspNetUserContext
            {
                TokenExact = model.Token
            }).FirstOrDefault();

            if (uc == null)
            {
                return _webDb.AddUserContext(model);
            }

            model.Id = uc.Id;
            _webDb.UpdateUserContext(model);
            return model.Id;
        }

        public void UpdateUserContextLastChangeDate(string token, DateTime date)
        {
            _webDb.UpdateUserContextLastChangeDate(token, date);
        }

        public void DeleteUserContext(string token)
        {
            _webDb.DeleteUserContext(token);
        }

        public int GetClientId(string clientCode)
        {
            return _webDb.GetClientId(clientCode);
        }

        public string GetClientCode(int clientId)
        {
            return _webDb.GetClientCode(clientId);

        }

        public DatabaseModelForAdminContext GetClientServer(int clientId)
        {
            return _webDb.GetClientServer(clientId);
        }

        public DatabaseModelForAdminContext GetClientServer(string clientCode)
        {
            return _webDb.GetClientServer(clientCode);
        }

        public bool ExistsClients(FilterAspNetClients filter)
        {
            return _webDb.ExistsClients(filter);
        }

        public DatabaseModel GetServer(int Id)
        {
            return _webDb.GetServer(Id);
        }

        public FrontAspNetClientLicence GetClientLicenceActive(int clientId)
        {
            return _webDb.GetClientLicenceActive(clientId);
        }

        public async Task ThrowErrorGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context, Exception ex)
        {
            string message = HttpContext.Current.Request.Browser.Info();
            var clientCode = await context.Request.Body.GetClientCodeAsync();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var server = webService.GetClientServer(clientCode);

            var ctx = new AdminContext(server);
            var logger = DmsResolver.Current.Get<ILogger>();
            var errorInfo = new AuthError
            {
                ClientCode = clientCode,
                EMail = context.UserName,
                FingerPrint = await context.Request.Body.GetFingerprintAsync()
            };
            int? agentId = null;
            var dbService = DmsResolver.Current.Get<WebAPIService>();

            var user = await dbService.GetUserAsync(errorInfo.EMail);

            if (user != null)
            {
                var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(ctx, user.Id);
                agentId = agentUser?.Id;
            }

            var exceptionText = ExceptionHandling.GetExceptionText(ex);
            var loginLogId = logger.Error(ctx, message, exceptionText, objectId: (int)EnumObjects.System, actionId: (int)EnumSystemActions.Login, logObject: errorInfo, agentId: agentId);

            // Эти исключения отлавливает Application_Error в Global.asax
            throw ex;
        }

    }
}