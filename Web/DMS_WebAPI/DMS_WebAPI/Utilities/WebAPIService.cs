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
using BL.Model.DictionaryCore.FrontModel;
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
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal class WebAPIService
    {
        //        private TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        private string FormRoleNameAdmin(string clientCode) => FormRoleName("Admin", clientCode);

        private string FormRoleName(string roleName, string clientCode) => $"{clientCode.Trim()}_{roleName.Trim()}";

        private string FormUserName(string userEmail, string clientCode) => $"{clientCode.Trim()}_{userEmail.Trim()}";

        private string FormUserName(string userEmail, int clientId)
        {
            var dbProc = new WebAPIDbProcess();
            var client = dbProc.GetClient(clientId);
            if (client == null) throw new ClientIsNotFound();
            return FormUserName(userEmail, client.Code);
        }

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
            //var dbWeb = new WebAPIDbProcess();

            //var client = dbWeb.GetClient(clientCode);

            //var db = dbWeb.GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

            //var ctx = new AdminContext(db);

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return GetUserById(userId);
        }

        public ApplicationUser GetUser(string userName)
        {
            return UserManager.FindByName(userName);
        }

        public ApplicationUser GetUser(string userEmail, string clientCode)
        {
            var userName = FormUserName(userEmail, clientCode);
            return GetUser(userName);
        }

        public ApplicationUser GetUser(string userEmail, int clientId)
        {
            var userName = FormUserName(userEmail, clientId);
            return GetUser(userName);
        }

        public async Task<ApplicationUser> GetUser(string userEmail, string userPassword, string clientCode)
        {
            return await UserManager.FindAsync(FormUserName(userEmail, clientCode), userPassword);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await UserManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserAsync(string userEmail, string clientCode)
        {
            var userName = FormUserName(userEmail, clientCode);
            return await GetUserAsync(userName);
        }

        public async Task<ApplicationUser> GetUserAsync(string userEmail, int clientId)
        {
            var userName = FormUserName(userEmail, clientId);
            return await GetUserAsync(userName);
        }

        public async Task<ApplicationUser> GetUserAsync(IContext context, int agentId)
        {
            //var dbWeb = new WebAPIDbProcess();

            //var client = dbWeb.GetClient(clientCode);

            //var db = dbWeb.GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

            //var ctx = new AdminContext(db);

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var userId = tmpService.GetDictionaryAgentUserId(context, agentId);

            return await GetUserByIdAsync(userId);
        }


        public bool ExistsUser(string userName) => GetUser(userName) != null;

        public FrontAgentEmployeeUser GetUserInfo(IContext context)
        {
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var employee = dicProc.GetDictionaryAgentEmployee(context, context.CurrentAgentId);

            var user = GetUser(context, context.CurrentAgentId);

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
            };
        }

        public int AddUserEmployee(IContext context, AddAgentEmployeeUser model)
        {

            string userId = string.Empty;
            string userName = FormUserName(model.Login, context.CurrentClientId);

            // Проверяю не используется ли логин
            if (ExistsUser(userName)) throw new UserNameAlreadyExists(model.Login);

            // пробую создать сотрудника
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, context, model);

            if (tmpItem > 0)
            {
                userId = AddUser(new AddWebUser
                {
                    Email = model.Login,
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
                Id = tmpItem,
                UserId = userId
            });

            return tmpItem;

        }

        public int UpdateUserEmployee(IContext context, ModifyAgentEmployee model)
        {
            var user = GetUser(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();

            if (model.ImageId.HasValue)
            {
                var tmpStore = DmsResolver.Current.Get<ITempStorageService>();
                var avaFile = tmpStore.ExtractStoreObject(model.ImageId.Value);
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


            return model.Id;
        }

        public void DeleteUserEmployee(IContext context, int agentId)
        {
            var user = GetUser(context, agentId);

            if (user == null) throw new UserIsNotDefined(); ;

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, context, agentId);

            DeleteUser(user.Id);

        }

        private string AddUser(string userName, string userPassword, string userEmail, string userPhone = "")
        {
            using (var transaction = Transactions.GetTransaction())
            {
                if (ExistsUser(userName)) throw new UserNameAlreadyExists(userEmail);

                var user = new ApplicationUser()
                {
                    UserName = userName.Trim(),
                    Email = userEmail.Trim(),
                    PhoneNumber = userPhone.Trim(),
                    IsChangePasswordRequired = true,
                    IsEmailConfirmRequired = true,
                    CreateDate = DateTime.UtcNow,
                    LastChangeDate = DateTime.UtcNow,
                };

                IdentityResult result = UserManager.Create(user, userPassword);

                if (!result.Succeeded) throw new UserCouldNotBeAdded(userEmail);

                transaction.Complete();

                return user.Id;
            }
        }

        private string DeleteUser(string userId)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                DeleteUserClients(new FilterAspNetUserClients { UserIds = new List<string> { userId } });
                DeleteUserServers(new FilterAspNetUserServers { UserIds = new List<string> { userId } });

                var user = GetUserById(userId);

                IdentityResult result = UserManager.Delete(user);

                if (!result.Succeeded) throw new UserCouldNotBeDeleted();

                transaction.Complete();

                return user.Id;
            }
        }

        public void DeleteUserClients(FilterAspNetUserClients filter)
        {
            var webDb = new WebAPIDbProcess();

            try
            {
                webDb.DeleteUserClients(filter);
            }
            catch (Exception) { throw; }
        }

        public void DeleteUserServers(FilterAspNetUserServers filter)
        {
            var webDb = new WebAPIDbProcess();
            try
            {
                webDb.DeleteUserServers(filter);
            }
            catch (Exception) { throw; }
        }

        public string AddUser(AddWebUser model)
        {
            var dbWeb = new WebAPIDbProcess();
            var userId = string.Empty;

            using (var transaction = Transactions.GetTransaction())
            {
                userId = AddUser(FormUserName(model.Email, model.ClientId), model.Password, model.Email, "");

                dbWeb.AddUserClient(new ModifyAspNetUserClient { UserId = userId, ClientId = model.ClientId });
                dbWeb.AddUserServer(new ModifyAspNetUserServer { UserId = userId, ClientId = model.ClientId, ServerId = model.ServerId });

                transaction.Complete();
            }

            // Выслать письмо с ссылкой на пароль

            RestorePasswordAgentUserAsync(new RestorePasswordAgentUser
            {
                ClientCode = dbWeb.GetClientCode(model.ClientId),
                Email = model.Email,
                FirstEntry = "true"
            }, new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "restore-password").ToString(), null, "Ostrean. Приглашение", RenderPartialView.RestorePasswordAgentUserVerificationEmail);

            return userId;
        }

        private string AddRole(string roleName)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                var role = roleManager.FindByName(roleName);

                if (role == null || string.IsNullOrEmpty(role.Id)) throw new RoleNameAlreadyExists(roleName);

                IdentityResult result = roleManager.Create(new IdentityRole { Name = roleName });

                if (!result.Succeeded) throw new RoleCouldNotBeAdded(roleName);
                transaction.Complete();
                return role.Id;
            }
        }

        public string AddFirstAdmin(AddFirstAdminClient model)
        {
            var dbWeb = new WebAPIDbProcess();

            #region Verification client code 
            var client = dbWeb.GetClients(new FilterAspNetClients { Code = model.ClientCode, VerificationCode = model.VerificationCode }).FirstOrDefault();

            if (client == null) throw new ClientVerificationCodeIncorrect();

            #endregion Verification client code 

            try
            {
                //using (var dbContext = new ApplicationDbContext())
                using (var transaction = Transactions.GetTransaction())
                {
                    var clientId = dbWeb.GetClientId(model.ClientCode);
                    var serverId = dbWeb.GetServerIdByClientId(clientId);

                    #region Create user   
                    var userId = AddUser(new AddWebUser
                    {
                        Email = model.Admin.Email,
                        Password = model.Admin.Password,
                        ClientId = clientId,
                        ServerId = serverId,
                    });
                    #endregion

                    #region add user to role admin
                    var roleName = FormRoleNameAdmin(model.ClientCode);

                    AddRole(roleName);

                    UserManager.AddToRole(userId, roleName);

                    #endregion

                    transaction.Complete();

                    return userId;
                }
            }
            catch (UserNameAlreadyExists)
            {
                throw new UserNameAlreadyExists(model.Admin.Email);
            }
            catch (ClientNameAlreadyExists)
            {
                throw new ClientNameAlreadyExists();
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public string AddClientSaaS(AddClientSaaS model)
        {
            var dbWeb = new WebAPIDbProcess();

            // Проверка уникальности доменного имени
            if (dbWeb.ExistsClients(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            //TODO Автоматическое определение сервера
            // определяю сервер для клиента пока первый попавшийся
            // сервер может определяться более сложным образом: с учетом нагрузки, количества клиентов
            var server = dbWeb.GetServers(new FilterAdminServers()).FirstOrDefault();
            if (server == null) throw new ServerIsNotFound();

            // Если не указан язык, беру язык по умолчанию 
            if (model.LanguageId == null)
            {
                var languages = DmsResolver.Current.Get<BL.Logic.AdminCore.Interfaces.ILanguages>();
                model.LanguageId = languages.GetLanguageIdByHttpContext();
            }

            if (string.IsNullOrEmpty(model.Password)) model.Password = "admin_" + model.ClientCode;

            using (var transaction = Transactions.GetTransaction())
            {
                // Создаю клиента
                model.ClientId = dbWeb.AddClient(new ModifyAspNetClient
                {
                    Name = model.ClientCode,
                    Code = model.ClientCode,
                });

                // Линкую клиента на сервер
                dbWeb.AddClientServer(new ModifyAspNetClientServer { ClientId = model.ClientId, ServerId = server.Id });

                // Линкую клиента на лицензию

                // Создаю первого пользователя
                var userId = AddFirstAdmin(new BL.Model.WebAPI.IncomingModel.AddFirstAdminClient
                {
                    ClientCode = model.ClientCode,
                    Admin = new ModifyAspNetUser
                    {
                        Email = model.Email,
                        Password = model.Password,
                    }

                });

                transaction.Complete();
            }

            var ctx = new AdminContext(server);

            var clientService = DmsResolver.Current.Get<IClientService>();

            clientService.AddNewClient(ctx, model);


            //UserManager.AddLogin(userId, new UserLoginInfo {    })


            return "token";

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
            var dbWeb = new WebAPIDbProcess();

            //TODO в transaction не может подлючиться к базе
            if (model.Server.Id <= 0)
            {
                dbWeb.InitializerDatabase(model.Server);
            }

            // Проверка уникальности доменного имени
            if (dbWeb.ExistsClients(new FilterAspNetClients { Code = model.Client.Code })) throw new ClientCodeAlreadyExists(model.Client.Code);

            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));



                #region Create client 1 

                // Создаю клиента
                var clientId = dbWeb.AddClient(new ModifyAspNetClient
                {
                    Name = model.Client.Name,
                    Code = model.Client.Code,
                });

                AspNetClientLicences clientLicence;
                if (model.LicenceId > 0)
                {
                    dbWeb.AddClientLicence(clientId, model.LicenceId.GetValueOrDefault());
                }

                #endregion Create client 1

                #region Create DB

                if (model.Server.Id <= 0)
                {
                    model.Server.Id = dbWeb.AddServer(model.Server);
                }

                dbWeb.AddClientServer(new ModifyAspNetClientServer { ClientId = clientId, ServerId = model.Server.Id });

                #endregion Create DB

                #region Create user                        

                #endregion Create user
                var userId = AddFirstAdmin(new BL.Model.WebAPI.IncomingModel.AddFirstAdminClient
                {
                    ClientCode = model.Client.Code,
                    Admin = new ModifyAspNetUser
                    {
                        Email = model.Admin.Email,
                        Password = model.Admin.Password,
                    }
                });

                transaction.Complete();

                return clientId;

            }
        }


        #region UserAgent

        public async void ChangeLoginAgentUser(ChangeLoginAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var userContext = userContexts.Get();

            // VerifyAccessCommand
            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ExecuteAction(EnumAdminActions.ChangeLogin, userContext, model.Id);

            var user = GetUser(userContext, model.Id);

            user.UserName = FormUserName(model.NewEmail, userContext.CurrentClientId);//  model.NewEmail.UserNameFormatByClientId(userContext.CurrentClientId);
            user.Email = model.NewEmail;
            user.IsEmailConfirmRequired = model.IsVerificationRequired;
            user.LastChangeDate = DateTime.UtcNow;

            if (user.IsEmailConfirmRequired)
                user.EmailConfirmed = false;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError();

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

                mailService.SendMessage(userContext, model.NewEmail, languages.GetTranslation("##l@EmailSubject:EmailConfirmation@l##"), htmlContent);
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

            if (!result.Succeeded) throw new DatabaseError();

        }

        public async Task RestorePasswordAgentUserAsync(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject, string renderPartialView)
        {
            if (query == null) query = new NameValueCollection();

            var user = await GetUserAsync(model.Email, model.ClientCode);

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
            string callbackurl = builder.ToString();

            var htmlContent = callbackurl.RenderPartialViewToString(renderPartialView);

            var dbWeb = new WebAPIDbProcess();

            var client = dbWeb.GetClient(model.ClientCode);

            var db = dbWeb.GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

            var ctx = new AdminContext(db);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(ctx, model.Email, emailSubject, htmlContent);
        }

        public void RestorePasswordAgentUser(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject, string renderPartialView)
        {
            if (query == null) query = new NameValueCollection();

            var user = GetUser(model.Email, model.ClientCode);

            if (user == null) throw new UserIsNotDefined();

            if (user.IsLockout) throw new UserIsDeactivated(user.UserName);

            var passwordResetToken = UserManager.GeneratePasswordResetToken(user.Id);

            query.Add("UserId", user.Id);
            query.Add("Code", passwordResetToken);


            var builder = new UriBuilder(baseUrl);
            builder.Query = query.ToString();
            string callbackurl = builder.ToString();

            var htmlContent = callbackurl.RenderPartialViewToString(renderPartialView);

            var settings = DmsResolver.Current.Get<ISettings>();

            var dbWeb = new WebAPIDbProcess();

            var client = dbWeb.GetClient(model.ClientCode);

            var db = dbWeb.GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

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

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(" ", result.Errors));
            }

            user.IsChangePasswordRequired = model.IsChangePasswordRequired;//true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(" ", result.Errors));
            }

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

            if (!result.Succeeded) throw new DatabaseError();

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            if (model.IsKillSessions)
                userContexts.RemoveByAgentId(model.Id);
        }

        public async Task ConfirmEmailAgentUser(string userId, string code)
        {
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(" ", result.Errors));
            }

            ApplicationUser user = await GetUserByIdAsync(userId);

            if (user == null) throw new UserIsNotDefined();

            user.IsEmailConfirmRequired = false;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError();

        }

        public async Task<string> ConfirmRestorePasswordAgentUser(ConfirmRestorePasswordAgentUser model)
        {

            var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.NewPassword);

            if (!result.Succeeded)
                throw new ResetPasswordCodeInvalid();

            ApplicationUser user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null) throw new UserIsNotDefined();


            user.EmailConfirmed = true;
            user.IsEmailConfirmRequired = false;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError();


            var userContexts = DmsResolver.Current.Get<UserContexts>();
            userContexts.RemoveByUserId(model.UserId);

            return user.Email;
        }

        #endregion


        public IEnumerable<FrontAspNetUserFingerprint> GetUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            var dbWeb = new WebAPIDbProcess();
            return dbWeb.GetUserFingerprints(filter);
        }

        public bool ExistsUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            var dbWeb = new WebAPIDbProcess();
            return dbWeb.ExistsUserFingerprints(filter);
        }

        public int MergeUserFingerprint(AddAspNetUserFingerprint model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                var userContext = DmsResolver.Current.Get<UserContexts>().Get();
                var user = GetUser(userContext, userContext.CurrentAgentId);
                model.UserId = user.Id;
            }

            var dbWeb = new WebAPIDbProcess();

            var fp = dbWeb.GetUserFingerprints(new FilterAspNetUserFingerprint
            {
                UserIDs = new List<string> { model.UserId },
                FingerprintExact = model.Fingerprint
            }).FirstOrDefault();

            if (fp == null) return AddUserFingerprint(model);
            else return fp.Id;

        }

        public int AddUserFingerprint(AddAspNetUserFingerprint model)
        {

            if (string.IsNullOrEmpty(model.UserId))
            {
                var userContext = DmsResolver.Current.Get<UserContexts>().Get();
                var user = GetUser(userContext, userContext.CurrentAgentId);
                model.UserId = user.Id;
            }

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

            model.Browser = bc.Browser;
            model.Platform = bc.Platform;

            var dbWeb = new WebAPIDbProcess();
            return dbWeb.AddUserFingerprint(model);
        }

        public void UpdateUserFingerprint(ModifyAspNetUserFingerprint model)
        {
            var dbWeb = new WebAPIDbProcess();
            dbWeb.UpdateUserFingerprint(model);
        }

        public void DeleteUserFingerprint(int id)
        {
            var dbWeb = new WebAPIDbProcess();
            dbWeb.DeleteUserFingerprint(id);
        }


        public IEnumerable<ListItem> GetControlQuestions()
        {
            var dbWeb = new WebAPIDbProcess();
            return dbWeb.GetControlQuestions();
        }

    }
}