using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.AdminCore.WebUser;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel.Employees;
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

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
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
            // Пользователь может быть еще не залинкован с сотрудником
            var userId = tmpService.GetAgentUserId(context, agentId);

            if (string.IsNullOrEmpty(userId)) return null;

            return GetUserById(userId);
        }

        public AspNetUsers GetUser(string userName)
        {
            AspNetUsers res = null;

            try
            {
                res = UserManager.FindByName(userName);
            }
            catch { }

            return res;
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
            // Пользователь может быть еще не залинкован с сотрудником
            var userId = tmpService.GetAgentUserId(context, agentId);

            if (string.IsNullOrEmpty(userId)) return null;

            return await GetUserByIdAsync(userId);
        }

        public bool ExistsUser(string userName)
        {
            var user = GetUser(userName);
            return user != null;
        }

        public bool ExistsUserInClient(AspNetUsers user, int clientId)
        {
            if (user == null) return false;
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

        public int GetUserLanguageId(string userName)
        {
            var user = GetUser(userName);
            if (user == null) return -1;
            return user.LanguageId;
        }

        // Пользователь запрашивает информацию о себе внутри докума
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
                FullName = employee.FullName,

                // У пользователя есть свои FirstName и LastName
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,

                IsMale = employee.IsMale,

                TaxCode = employee.TaxCode,
                PersonnelNumber = employee.PersonnelNumber,
                BirthDate = employee.BirthDate,
                Description = employee.Description,
                IsActive = employee.IsActive,

                LastSuccessLogin = lastUserLoginInfo.LastSuccessLogin,
                LastErrorLogin = lastUserLoginInfo.LastErrorLogin,
                CountErrorLogin = lastUserLoginInfo.CountErrorLogin,


                Login = user.Email,

                LanguageId = user.LanguageId,
                LanguageCode = user.Language.Code,
                LanguageName = user.Language.Name,

            };
        }

        public async Task RegisterUser(Account model)
        {
            var recapcha = DmsResolver.Current.Get<GoogleRecapcha>();
            await recapcha.ValidateAsync(model.Recaptcha);

            var languages = DmsResolver.Current.Get<ILanguages>();

            var languageId = languages.GetLanguageIdByCode(model.LanguageCode);

            var user = await AddUserAsync(model.FullName, model.Email, model.Password, languageId, model.Email, model.PhoneNumber);

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var addr = tmpService.GetAuthAddress();

            await SendConfirmEmailMail(user, addr);
        }

        private AspNetUsers AddUser(string fullName, string userName, string userPassword, int languageId, string userEmail, string userPhone = "",
            bool emailConfirmed = false, bool isChangePasswordRequired = false)
        {
            var now = DateTime.UtcNow;

            var user = new AspNetUsers
            {
                FirstName = fullName?.Trim(),
                UserName = userName?.Trim(),
                Email = userEmail?.Trim(),
                LanguageId = languageId,
                PhoneNumber = userPhone?.Trim(),
                IsChangePasswordRequired = isChangePasswordRequired,
                EmailConfirmed = emailConfirmed,
                CreateDate = now,
                LastChangeDate = now,
            };

            var result = UserManager.Create(user, userPassword);

            if (!result.Succeeded) throw new UserCouldNotBeAdded(userEmail, result.Errors);

            return user;
        }

        private async Task<AspNetUsers> AddUserAsync(string fullName, string userName, string userPassword, int languageId, string userEmail, string userPhone = "",
            bool emailConfirmed = false, bool isChangePasswordRequired = false)
        {
            var now = DateTime.UtcNow;

            var user = new AspNetUsers
            {
                FirstName = fullName?.Trim(),
                UserName = userName?.Trim(),
                Email = userEmail?.Trim(),
                LanguageId = languageId,
                PhoneNumber = userPhone?.Trim(),
                IsChangePasswordRequired = isChangePasswordRequired,
                EmailConfirmed = emailConfirmed,
                CreateDate = now,
                LastChangeDate = now,
            };

            var result = await UserManager.CreateAsync(user, userPassword);

            if (!result.Succeeded) throw new UserCouldNotBeAdded(userEmail, result.Errors);

            return user;
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
                    user = AddUser(model.FullName, model.Email, model.Password, model.LanguageId, model.Email, model.Phone,
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
            //if (userIDs == null)
            //{
            //    // запоминаю пользователей клиента, которых потенциально нужно удалить
            //    userIDs = _webDb.GetUserClientList(new FilterAspNetUserClient { ClientIDs = new List<int> { clientId } }).Select(x => x.UserId).ToList();
            //}

            //if (userIDs.Count() == 0) return;

            //using (var transaction = Transactions.GetTransaction())
            {
                // Удаляю связи пользователя с клиентом
                _webDb.DeleteUserClient(new FilterAspNetUserClient
                {
                    UserIDs = userIDs,
                    ClientIDs = new List<int> { clientId }
                });

                //// пользователи, которые завязаны на других клиентов удалять нельзя, но они в списке для удаления
                //var safeList = _webDb.GetUserClientList(new FilterAspNetUserClient { UserIDs = userIDs }).Select(x => x.UserId).ToList();

                //if (safeList.Any()) userIDs.RemoveAll(x => safeList.Contains(x));

                //if (userIDs.Any())
                //{
                //    _webDb.DeleteUserContexts(new FilterAspNetUserContext { UserIDs = userIDs });
                //    _webDb.DeleteUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = userIDs });

                //    foreach (var userId in userIDs)
                //    {
                //        var user = GetUserById(userId);

                //        IdentityResult result = UserManager.Delete(user);

                //        if (!result.Succeeded) throw new UserCouldNotBeDeleted(user.Email, result.Errors);
                //    }
                //}

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



        public IEnumerable<ListItem> GetControlQuestions(string language)
        {
            return _webDb.GetControlQuestions(language);
        }





        public FrontAspNetClientLicence GetClientLicenceActive(int clientId)
        {
            return _webDb.GetClientLicenceActive(clientId);
        }

        public async Task ThrowErrorGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context, Exception ex)
        {
            string message = HttpContext.Current.Request.Browser.Info();
            var clientCode = await context.Request.Body.GetClientCodeAsync();

            if (string.IsNullOrEmpty(clientCode) || clientCode == "-") throw ex;

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
            var loginLogId = logger.Error(ctx, message, exceptionText, objectId: (int)EnumObjects.System, actionId: (int)EnumActions.Login, logObject: errorInfo, agentId: agentId);

            // Эти исключения отлавливает Application_Error в Global.asax
            throw ex;
        }

    }
}