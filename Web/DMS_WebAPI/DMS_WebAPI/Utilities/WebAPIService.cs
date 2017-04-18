using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
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
using BL.Model.Database;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.AdminCore.IncomingModel;

namespace DMS_WebAPI.Utilities
{
    internal class WebAPIService
    {
        private readonly WebAPIDbProcess _webDb;
        private AddAgentEmployeeUser employee;

        public WebAPIService(WebAPIDbProcess webDb)
        {
            _webDb = webDb;
        }

        private string FormRoleNameAdmin(string clientCode) => FormRoleName("Admin", clientCode);

        private string FormRoleName(string roleName, string clientCode) => $"{clientCode.Trim()}_{roleName.Trim()}";

        public ApplicationUserManager UserManager
        {
            get
            {
                var owinContext = HttpContext.Current.Request.GetOwinContext();
                return owinContext.GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationUser GetUserById(string id)
        {
            return UserManager.FindById(id);
        }

        public ApplicationUser GetUser(IContext context, int agentId)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return GetUserById(userId);
        }

        public ApplicationUser GetUser(string userName)
        {
            return UserManager.FindByName(userName);
        }

        public async Task<ApplicationUser> GetUser(string userName, string userPassword)
        {
            return await UserManager.FindAsync(userName, userPassword);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await UserManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserAsync(IContext context, int agentId)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return await GetUserByIdAsync(userId);
        }

        public bool ExistsUser(string userName, int clientId)
        {
            var user = GetUser(userName);

            if (user == null) return false;

            var ucs = _webDb.GetUserClientServerList(new FilterAspNetUserClientServer
            {
                ClientIDs = new List<int> { clientId },
                UserIDs = new List<string> { user.Id }
            }).FirstOrDefault();

            return (ucs != null);
        }

        public FrontAgentEmployeeUser GetUserInfo(IContext context)
        {
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var employee = dicProc.GetDictionaryAgentEmployee(context, context.CurrentAgentId);

            var user = GetUser(context, context.CurrentAgentId);

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

        public int AddUserEmployee(IContext context, AddAgentEmployeeUser model)
        {

            string userId = string.Empty;
            model.UserName = model.Login;

            var employeeId = -1;

            // проверяю нет ли уже сотрудника с указанным имененм у клиента
            if (ExistsUser(model.UserName, context.CurrentClientId)) throw new UserNameAlreadyExists(model.UserName);

            // пробую создать сотрудника
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            try
            {
                employeeId = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, context, model);

                if (employeeId > 0)
                {
                    userId = AddUserToClient(new AddWebUser
                    {
                        Email = model.Login,
                        Phone = model.Phone,
                        // Для нового пользователя высылается письмо с линком на страницу "введите новый пароль"
                        Password = "k~WPop8V%W~11hG~~VGR",

                        // Предполагаю, что человек, который создает пользователей. создает их в тойже базе и в том же клиенте
                        ClientId = context.CurrentClientId,
                        ServerId = context.CurrentDB.Id,
                    });
                }

                // обновляю сотрудника 
                tmpService.SetAgentUserUserId(context, new InternalDictionaryAgentUser
                {
                    Id = employeeId,
                    UserId = userId
                });

                return employeeId;
            }
            catch (Exception e)
            {
                if (employeeId > 0) tmpService.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, context, employeeId);

                if (!string.IsNullOrEmpty(userId)) DeleteUsersInClient(context.CurrentClientId, new List<string> { userId });

                throw e;
            }



        }

        public int AddUserEmployeeInOrg(IContext context, AddEmployeeInOrg model)
        {
            var dicService = DmsResolver.Current.Get<IDictionaryService>();
            var admService = DmsResolver.Current.Get<IAdminService>();
            var employee = new AddAgentEmployeeUser();
            int assignmentId = -1;
            int empoyeeId = -1;
            int orgId = -1;
            int depId = -1;
            int posId = -1;

            if (model.OrgId == null && string.IsNullOrEmpty(model.OrgName)) throw new OrgRequired();

            if (model.DepartmentId == null && string.IsNullOrEmpty(model.DepartmentName)) throw new DepartmentRequired();

            if (model.PositionId == null && string.IsNullOrEmpty(model.PositionName)) throw new PositionRequired();

            employee.Login = model.Login;
            employee.Phone = model.Phone;
            employee.FirstName = model.FirstName;
            employee.MiddleName = model.MiddleName;
            employee.LastName = model.LastName;
            employee.IsActive = true;
            employee.LanguageId = model.LanguageId;

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

                    if (model.PositionId == null && !string.IsNullOrEmpty(model.PositionName))
                    {
                        var pos = new AddPosition();
                        pos.DepartmentId = depId;
                        pos.FullName = model.PositionName;
                        pos.Name = model.PositionName;
                        pos.IsActive = true;
                        pos.Role = model.Role;

                        // Создается должность. + доступы к журналам, рассылка и роль
                        posId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddPosition, context, pos);
                    }

                    transaction.Complete();
                }

                // Создаю сотрудника-пользователя
                empoyeeId = AddUserEmployee(context, employee);

                // назначаю сотрудника на должность
                var ass = new AddPositionExecutor();
                ass.AccessLevelId = model.AccessLevel;
                ass.AgentId = empoyeeId;
                ass.IsActive = true;
                ass.PositionId = posId;
                ass.StartDate = DateTime.UtcNow;
                ass.PositionExecutorTypeId = model.ExecutorType;

                assignmentId = (int)dicService.ExecuteAction(EnumDictionaryActions.AddExecutor, context, ass);

                return empoyeeId;
            }
            catch (Exception e)
            {
                if (assignmentId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteExecutor, context, assignmentId);

                if (empoyeeId > 0) DeleteUserEmployee(context, empoyeeId);

                if (posId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeletePosition, context, posId);

                if (depId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteDepartment, context, depId);

                if (orgId > 0) dicService.ExecuteAction(EnumDictionaryActions.DeleteOrg, context, orgId);

                throw e;
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
                if (avaFile is string)
                {
                    model.PostedFileData = avaFile as string;
                }
            }

            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, context, model);

            // При деактивации сотрудника деактивирую пользователя
            if (!model.IsActive)
            {
                ChangeLockoutAgentUserAsync(context, new ChangeLockoutAgentUser { IsLockout = model.IsActive, Id = model.Id, IsKillSessions = true });
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

            if (user == null) throw new UserIsNotDefined(); ;

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, context, agentId);

            DeleteUsersInClient(context.CurrentClientId, new List<string> { user.Id });
        }

        private string AddUser(string userName, string userPassword, string userEmail, string userPhone = "")
        {
            var now = DateTime.UtcNow;

            var user = new ApplicationUser()
            {
                UserName = userName?.Trim(),
                Email = userEmail?.Trim(),
                PhoneNumber = userPhone?.Trim(),
                IsChangePasswordRequired = true,
                IsEmailConfirmRequired = true,
                CreateDate = now,
                LastChangeDate = now,
            };

            using (var transaction = Transactions.GetTransaction())
            {
                IdentityResult result = UserManager.Create(user, userPassword);

                if (!result.Succeeded) throw new UserCouldNotBeAdded(userEmail, result.Errors);

                transaction.Complete();

                return user.Id;
            }
        }



        public string AddUserToClient(AddWebUser model)
        {
            var userId = string.Empty;

            // Решено для разных клиентов не создавать дубликаты юзеров
            // Один пользователь может работать на разных клиентов
            // Один клиент имеет выделенное место в базе согластно алгоритму
            var user = GetUser(model.Email);

            using (var transaction = Transactions.GetTransaction())
            {
                if (user != null)
                {
                    userId = user.Id;
                    // Если пользователь существует, то проверяю не пытаются ли его залинковать с тем же клиентом
                    var uc = _webDb.GetUserClientServerList(new FilterAspNetUserClientServer
                    {
                        ClientIDs = new List<int> { model.ClientId },
                        UserIDs = new List<string> { userId }
                    }).FirstOrDefault();

                    if (uc != null) throw new UserNameAlreadyExists(user.UserName);
                }
                else userId = AddUser(model.Email, model.Password, model.Email, model.Phone);

                // линкую пользователя с клиентом
                _webDb.AddUserClientServer(new SetUserClientServer { UserId = userId, ClientId = model.ClientId, ServerId = model.ServerId });

                transaction.Complete();
            }


            // Тут, конечно, рановато высылать письмо, пользователя еще не назначили



            // Если пользователь уже был в базе, то ему нужно выслать только ссылку на нового клиента, а если нет то ссылку на смену пароля
            if (user == null)
            {
                var tmp = new RestorePasswordAgentUser
                {
                    ClientCode = _webDb.GetClientCode(model.ClientId),
                    Email = model.Email,
                    FirstEntry = "true"
                };

                // http://docum.ostrean.com/restore-password
                var uri = new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "restore-password").ToString();

                RestorePasswordAgentUserAsync(tmp, uri, null, "Ostrean. Приглашение");
            }

            else
            {
                var clientCode = _webDb.GetClientCode(model.ClientId);
                var we = new WelcomeEmailModel()
                {
                    UserName = user.UserName,
                    UserEmail = user.Email,
                    ClientUrl = clientCode + ".ostrean.com",
                    CabinetUrl = clientCode + ".ostrean.com/cabinet/",
                    OstreanEmail = "info@ostrean.com",
                    SpamUrl = "noreplay@ostrean.com",
                };

                var htmlContent = we.RenderPartialViewToString(RenderPartialView.WelcomeEmail);
                var db = GetClientServer(model.ClientId);
                var ctx = new AdminContext(db);
                var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
                mailService.SendMessage(ctx, model.Email, "Ostrean. Приглашение", htmlContent);
            }
            return userId;
        }

        private void DeleteUsersInClient(int clientId, List<string> userIDs)
        {
            if (userIDs == null)
            {
                // запоминаю пользователей клиента, которых потенциально нужно удалить
                userIDs = _webDb.GetUserClientServerList(new FilterAspNetUserClientServer { ClientIDs = new List<int> { clientId } }).Select(x => x.UserId).ToList();
            };

            if (userIDs.Count() == 0) return;

            //using (var transaction = Transactions.GetTransaction())
            {
                // Удаляю связи пользователя с клиентом
                _webDb.DeleteUserClientServer(new FilterAspNetUserClientServer
                {
                    UserIDs = userIDs,
                    ClientIDs = new List<int> { clientId }
                });

                // пользователи, которые завязаны на других клиентов удалять нельзя, но они в списке для удаления
                var safeList = _webDb.GetUserClientServerList(new FilterAspNetUserClientServer { UserIDs = userIDs }).Select(x => x.UserId).ToList();

                if (safeList?.Count() > 0) userIDs.RemoveAll(x => safeList.Contains(x));

                if (userIDs?.Count() > 0)
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

        public string AddClientSaaS(AddClientSaaS model)
        {
            // Проверка уникальности доменного имени
            if (_webDb.ExistsClients(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            //TODO Автоматическое определение сервера
            // определяю сервер для клиента пока первый попавшийся
            // сервер может определяться более сложным образом: с учетом нагрузки, количества клиентов
            var server = _webDb.GetServers(new FilterAdminServers()).FirstOrDefault();
            if (server == null) throw new ServerIsNotFound();


            //if (string.IsNullOrEmpty(model.Password)) model.Password = "admin_" + model.ClientCode;

            using (var transaction = Transactions.GetTransaction())
            {
                // Создаю запись о клиенте
                model.ClientId = _webDb.AddClient(new ModifyAspNetClient
                {
                    Name = model.ClientCode,
                    Code = model.ClientCode,
                });


                // Линкую клиента на лицензию




                // Создаю первого пользователя
                //var userId = AddFirstAdmin(new BL.Model.WebAPI.IncomingModel.AddFirstAdminClient
                //{
                //    ClientCode = model.ClientCode,
                //    Admin = new ModifyAspNetUser
                //    {
                //        Email = model.Email,
                //        Password = model.Password,
                //    }

                //});

                transaction.Complete();
            }

            server.ClientId = model.ClientId;

            var ctx = new AdminContext(server);

            var languages = DmsResolver.Current.Get<ILanguages>();

            // Если не указан язык, беру язык по умолчанию 
            if (string.IsNullOrEmpty(model.Language))
            {
                ctx.CurrentEmployee.LanguageId = languages.GetLanguageIdByHttpContext();
            }
            else
            {
                ctx.CurrentEmployee.LanguageId = languages.GetLanguageIdByCode(model.Language);
            }

            var clientService = DmsResolver.Current.Get<IClientService>();


            try
            {
                clientService.AddDictionary(ctx, model);

            }
            catch (Exception e)
            {
                if (model.ClientId > 0) DeleteClient(model.ClientId);
                throw e;
            }

            AddUserEmployeeInOrg(ctx, new AddEmployeeInOrg
            {
                FirstName = model.Name,
                LastName = model.LastName,
                //MiddleName = model.MiddleName,
                OrgName = languages.GetTranslation(ctx.CurrentEmployee.LanguageId, "##l@Clients:" + "MyCompany" + "@l##"),
                DepartmentName = languages.GetTranslation(ctx.CurrentEmployee.LanguageId, "##l@Clients:" + "MyDepartment" + "@l##"),
                PositionName = languages.GetTranslation(ctx.CurrentEmployee.LanguageId, "##l@Clients:" + "MyPosition" + "@l##"),
                ExecutorType = EnumPositionExecutionTypes.Personal,
                AccessLevel = EnumAccessLevels.Personally,
                LanguageId = ctx.CurrentEmployee.LanguageId,
                Phone = model.PhoneNumber,
                Login = model.Email,
                Role = Roles.Admin,
            });


            //UserManager.AddLogin(userId, new UserLoginInfo {    })


            return "token";

        }

        public void DeleteClient(int Id)
        {
            if (!_webDb.ExistsClients(new FilterAspNetClients { ClientIds = new List<int> { Id } })) throw new ClientIsNotFound();

            if (Id == 1) throw new ClientIsNotFound();

            var clients = new List<int> { Id };

            var server = _webDb.GetClientServer(Id);
            var ctx = new AdminContext(server);

            var clientService = DmsResolver.Current.Get<IClientService>();
            clientService.Delete(ctx);

            //using (var transaction = Transactions.GetTransaction())
            {
                _webDb.DeleteClientLicence(new FilterAspNetClientLicences { ClientIds = clients });

                DeleteUsersInClient(Id, null);

                _webDb.DeleteClient(Id);

                //transaction.Complete();
            }



            //_webDb.DeleteUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = users });
            //пользователя пока не удаляю

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

        public async void ChangeLoginAgentUser(ChangeLoginAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var context = userContexts.Get();

            // VerifyAccessCommand
            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ChangeLoginAgentUser(context, model);

            var user = GetUser(context, model.Id);

            user.UserName = model.NewEmail;
            user.Email = model.NewEmail;
            user.IsEmailConfirmRequired = model.IsVerificationRequired;
            user.LastChangeDate = DateTime.UtcNow;

            if (user.IsEmailConfirmRequired)
                user.EmailConfirmed = false;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserLoginCouldNotBeChanged(model.NewEmail, result.Errors);

            // выкидываю пользователя из системы
            userContexts.RemoveByAgentId(model.Id);

            if (model.IsVerificationRequired)
            {
                var emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackurl = new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "email-confirmation").AbsoluteUri;

                callbackurl += String.Format("?userId={0}&code={1}", user.Id, HttpUtility.UrlEncode(emailConfirmationCode));

                var htmlContent = callbackurl.RenderPartialViewToString(RenderPartialView.PartialViewNameChangeLoginAgentUserVerificationEmail);

                var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

                var languages = DmsResolver.Current.Get<ILanguages>();

                mailService.SendMessage(context, model.NewEmail, languages.GetTranslation("##l@EmailSubject:EmailConfirmation@l##"), htmlContent);
            }

        }

        public async void ChangeFingerprintEnabled(bool parm)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var userContext = userContexts.Get();
            var user = GetUser(userContext, userContext.CurrentAgentId);

            user.IsFingerprintEnabled = parm;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

        }

        public void ChangeControlQuestion(ModifyAspNetUserControlQuestion model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var userContext = userContexts.Get();
            var user = GetUser(userContext, userContext.CurrentAgentId);

            user.ControlQuestionId = model.QuestionId;
            user.ControlAnswer = model.Answer;
            user.LastChangeDate = DateTime.UtcNow;

            var result = UserManager.Update(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

        }

        public async Task RestorePasswordAgentUserAsync(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject)
        {
            if (query == null) query = new NameValueCollection();

            var user = await GetUserAsync(model.Email);

            if (user == null) throw new UserIsNotDefined();

            if (user.IsLockout) throw new UserIsDeactivated(user.UserName);

            var passwordResetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var builder = new UriBuilder(baseUrl);
            var newQuery = HttpUtility.ParseQueryString(builder.Query);
            newQuery.Add("UserId", user.Id);
            newQuery.Add("Code", passwordResetToken);

            if (!string.IsNullOrEmpty(model.FirstEntry))
            {
                newQuery.Add("FirstEntry", model.FirstEntry);
            }

            newQuery.Add(query);

            builder.Query = newQuery.ToString();// string.Join("&", nvc.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
            // сылка на восстановление пароля
            string callbackurl = builder.ToString();

            var m = new WelcomeEmailModel()
            {
                CabinetUrl = "ostrean.com/cabinet",
                ClientUrl = callbackurl,
                OstreanEmail = "info@ostrean.com",
                SpamUrl = "noreplay@ostrean.com",
                UserEmail = user.Email,
                UserName = user.UserName,
            };

            // html с подставленной ссылкой
            var htmlContent = m.RenderPartialViewToString(RenderPartialView.WelcomeEmail);


            var db = GetClientServer(model.ClientCode);
            var ctx = new AdminContext(db);
            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(ctx, model.Email, emailSubject, htmlContent);
        }

        public void RestorePasswordAgentUser(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject, string renderPartialView)
        {
            if (query == null) query = new NameValueCollection();

            var user = GetUser(model.Email);

            if (user == null) throw new UserIsNotDefined();

            if (user.IsLockout) throw new UserIsDeactivated(user.UserName);

            var passwordResetToken = UserManager.GeneratePasswordResetToken(user.Id);

            query.Add("UserId", user.Id);
            query.Add("Code", passwordResetToken);


            var builder = new UriBuilder(baseUrl);
            builder.Query = query.ToString();
            string callbackurl = builder.ToString();

            var htmlContent = callbackurl.RenderPartialViewToString(renderPartialView);

            var db = _webDb.GetClientServer(model.ClientCode);

            var ctx = new AdminContext(db);
            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(ctx, model.Email, emailSubject, htmlContent);
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

        public async Task ChangeLockoutAgentUserAsync(IContext context, ChangeLockoutAgentUser model)
        {
            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ExecuteAction(EnumAdminActions.ChangeLockout, context, model.Id);

            var user = await GetUserAsync(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            user.IsLockout = model.IsLockout;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            if (model.IsKillSessions)
                userContexts.RemoveByAgentId(model.Id);
        }

        public async Task ConfirmEmailAgentUser(string userId, string code)
        {
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            ApplicationUser user = await GetUserByIdAsync(userId);

            if (user == null) throw new UserIsNotDefined();

            user.IsEmailConfirmRequired = false;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

        }

        public async Task<string> ConfirmRestorePasswordAgentUser(ConfirmRestorePasswordAgentUser model)
        {

            var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.NewPassword);

            if (!result.Succeeded) throw new ResetPasswordCodeInvalid(result.Errors);

            ApplicationUser user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null) throw new UserIsNotDefined();


            user.EmailConfirmed = true;
            user.IsEmailConfirmRequired = false;
            user.IsChangePasswordRequired = false;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);


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


        public IEnumerable<ListItem> GetControlQuestions()
        {
            return _webDb.GetControlQuestions();
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
                Token = context.CurrentEmployee.Token,
                ClientId = context.CurrentClientId,
                CurrentPositionsIdList = string.Join(",", context.CurrentPositionsIdList),
                DatabaseId = context.CurrentDB.Id,
                IsChangePasswordRequired = context.IsChangePasswordRequired,
                UserId = context.CurrentEmployee.UserId,
                UserName = context.UserName,
                LoginLogId = context.LoginLogId,
                LoginLogInfo = context.LoginLogInfo,
                LastChangeDate = DateTime.UtcNow,
            };


            var uc = _webDb.GetUserContexts(new FilterAspNetUserContext
            {
                TokenExact = model.Token
            }).FirstOrDefault();

            if (uc == null)
            {
                return _webDb.AddUserContext(model);
            }
            else
            {
                model.Id = uc.Id;
                _webDb.UpdateUserContext(model);
                return model.Id;
            }

        }

        public void DeleteUserContext(string token)
        {
            _webDb.DeleteUserContext(token);
        }

        public string GetClientCode(int clientId)
        {
            return _webDb.GetClientCode(clientId);

        }

        public DatabaseModel GetClientServer(int clientId)
        {
            return _webDb.GetClientServer(clientId);
        }

        public DatabaseModel GetClientServer(string clientCode)
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

    }
}