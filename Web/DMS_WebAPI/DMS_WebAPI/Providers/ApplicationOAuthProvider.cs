using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL.CrossCutting.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using BL.Logic.DependencyInjection;
using BL.Model.Exception;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using BL.Logic.AdminCore.Interfaces;
using System.Web.Http;
using System.Net;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;

namespace DMS_WebAPI.Providers
{
    /// <summary>
    /// Этот класс устанавливается в качестве провайдера для овина
    /// Отвечает за авторизацию пользователей посредством логина и пароля
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        /// <summary>
        /// Установка значения _publicClientId в "self" при конфигурации опций овина, который используется в ValidateClientRedirectUri
        /// </summary>
        /// <param name="publicClientId"></param>
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        /// Этап № 1. Валидация контекста при получении токена
        /// См. ApplicationUserManager
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Этап №2. Проверка логина и пароля
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userEmail = context.UserName;

            string clientCode = GetClientCodeFromBody(context.Request.Body);

            var webService = new WebAPIService();
            
            ApplicationUser user = await webService.GetUser(userEmail, context.Password, clientCode);

            // Эти исключения отлавливает Application_Error в Global.asax
            if (user == null) throw new UserNameOrPasswordIsIncorrect();

            if (user.IsLockout) throw new UserIsDeactivated(user.UserName);

            // 
            if (!user.EmailConfirmed && user.IsEmailConfirmRequired) throw new UserMustConfirmEmail();

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);

            //     Add information to the response environment that will cause the appropriate authentication
            //     middleware to grant a claims-based identity to the recipient of the response.
            //     The exact mechanism of this may vary. Examples include setting a cookie, to adding
            //     a fragment on the redirect url, or producing an OAuth2 access code or token response.
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }



        /// <summary>
        /// Этап №3. Добавление параметров в Response
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            // добавляю дополнительные свойства - userName в параметры ответа
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Этап №4. 
        /// На этом этапе уже известен токен пользователя
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            if (context.Identity.IsAuthenticated)
            {
                // Получаю ID WEb-пользователя
                var userId = context.Identity.GetUserId();

                var webProc = new WebAPIDbProcess();

                // Предполагаю, что один пользователь всегда привязан только к одному клиенту 
                var client = webProc.GetClientByUser(userId);
                if (client == null) throw new ClientIsNotFound();

                // Предполагаю, что один пользователь всегда привязан только к одному серверу 
                var server = webProc.GetServerByUser(userId, new BL.Model.WebAPI.IncomingModel.SetUserServer { ClientId = client.Id, ServerId = -1 });
                if (server == null) throw new DatabaseIsNotFound();

                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                ApplicationUser user = userManager.FindById(userId);

                var token = $"{context.Identity.AuthenticationType} {context.AccessToken}";

                //var clientCode = GetClientCodeFromBody(context.Request.Body);

                var userContexts = DmsResolver.Current.Get<UserContexts>();

                // Создаю пользовательский контекст
                var ctx = userContexts.Set(token, userId, client.Code, user.IsChangePasswordRequired);

                // Добавляю в пользовательский контекст сервер
                userContexts.Set(token, server, client.Id);

                // Получаю информацию о браузере
                string message = GetBrowswerInfo();
                
                var logger = DmsResolver.Current.Get<ILogger>();
                var loginLogId = logger.Information(ctx, message, (int)EnumObjects.System, (int)EnumSystemActions.Login);

                // Добавляю в пользовательский сведения о браузере
                userContexts.Set(token, loginLogId, message);

                context.AdditionalResponseParameters.Add("ChangePasswordRequired", user.IsChangePasswordRequired);
            }

            return Task.FromResult<object>(null);
        }

        private static string GetBrowswerInfo()
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            var userAgent = HttpContext.Current.Request.UserAgent;
            var mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;
            var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;
            var message = $"{ip}; {bc.Browser} {bc.Version}; {bc.Platform}; {mobile}";
            //{HttpContext.Current.Request.UserHostAddress}
            //var js = new JavaScriptSerializer();
            //message += $"; {js.Serialize(HttpContext.Current.Request.Headers)}";
            //message += $"; {HttpContext.Current.Request.Headers["X-Real-IP"]}";

            return message;
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Создает дополнительное свойства при формировании токена.
        /// См. GrantResourceOwnerCredentials => CreateProperties
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        private static string GetClientCodeFromBody(Stream Body)
        {
            var clientCode = string.Empty;

            try
            {
                // Из боди POST-запроса извлекаю ClientCode - доменное имя 3 уровня клиента
                Body.Position = 0;
                var body = new StreamReader(Body);
                //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                var bodyStr = body.ReadToEnd();

                var dic = HttpUtility.ParseQueryString(bodyStr);

                // ВНИМАНИЕ!!! в SoapUI параметр называется так client_id
                clientCode = dic["client_id"] ?? string.Empty; //ClientCode

            }
            catch (Exception ex) { }

            return clientCode;
        }


    }
}