using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Infrastructure;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

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

            var webService = DmsResolver.Current.Get<WebAPIService>();

            ApplicationUser user = await webService.GetUser(userEmail, context.Password, clientCode);

            //context.SetError("invalid_grant", new UserNameOrPasswordIsIncorrect().Message); return;
            // Эта штука возвращает в респонсе {"error":"invalid_grant","error_description":"Привет!!"} - на фронте всплывает красный тостер с error_description
            // Эта штука доступна только в OAuthGrantResourceOwnerCredentialsContext в OAuthTokenEndpointResponseContext я ее уже не обнаружил
            // Эта штука НЕ отлавливается нашим обработчиком ошибок и не фиксируется в файл лог

            // Эти исключения отлавливает Application_Error в Global.asax
            if (user == null) ThrowErrorGrantResourceOwnerCredentials(context, new UserNameOrPasswordIsIncorrect());

            if (user.IsLockout) ThrowErrorGrantResourceOwnerCredentials(context, new UserIsDeactivated(user.UserName));

            // Проверка подтверждения адреса
            if (!user.EmailConfirmed && user.IsEmailConfirmRequired) ThrowErrorGrantResourceOwnerCredentials(context, new UserMustConfirmEmail());

            // Проверка Fingerprint: Если пользователь включил Fingerprint
            if (user.IsFingerprintEnabled)
            {
                var answer = GetControlAnswerFromBody(context.Request.Body);
                var rememberFingerprint = GetRememberFingerprintFromBody(context.Request.Body);
                var fingerprint = GetFingerprintFromBody(context.Request.Body);

                if (!string.IsNullOrEmpty(answer))  // переданы расширенные параметры получения токена с ответом на секретный вопрос
                {
                    // Проверка ответа на секретный вопрос
                    if (!(user.ControlAnswer == answer))
                        ThrowErrorGrantResourceOwnerCredentials (context, new UserAnswerIsIncorrect());

                    // Добавление текущего отпечатка в доверенные
                    if (rememberFingerprint)
                    {
                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

                        webService.AddUserFingerprint(new AddAspNetUserFingerprint
                        {
                            UserId = user.Id,
                            Fingerprint = fingerprint,
                            Name = bc.Browser + " " + bc.Platform + " " + DateTime.UtcNow.ToString("HHmmss"),
                            IsActive = true
                        });
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(fingerprint)) ThrowErrorGrantResourceOwnerCredentials(context, new FingerprintRequired());

                    if (!webService.ExistsUserFingerprints(new FilterAspNetUserFingerprint
                    {
                        UserIDs = new List<string> { user.Id },
                        FingerprintExact = fingerprint,
                        IsActive = true,
                    })) ThrowErrorGrantResourceOwnerCredentials(context, new UserFingerprintIsIncorrect());
                }
            }

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

        private void ThrowErrorGrantResourceOwnerCredentials (OAuthGrantResourceOwnerCredentialsContext context, Exception ex)
        {
            string message = GetBrowswerInfo();
            var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();
            var server = dbWeb.GetServerByUser(null, new SetUserServer { ClientId = -1, ServerId = -1, ClientCode = GetClientCodeFromBody(context.Request.Body) });
            var ctx = new AdminContext(server);
            var logger = DmsResolver.Current.Get<ILogger>();
            var errorInfo = new AuthError
            {
                ClientCode = GetClientCodeFromBody(context.Request.Body),
                EMail = context.UserName,
                FingerPrint = GetFingerprintFromBody(context.Request.Body)
            };
            ctx.CurrentClientId = dbWeb.GetClientId(errorInfo.ClientCode);
            int? agentId = null;
            var dbService = DmsResolver.Current.Get<WebAPIService>();
            var user = dbService.GetUser(errorInfo.EMail, errorInfo.ClientCode);
            if (user != null)
            {
                var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(ctx, user.Id);
                agentId = agentUser?.AgentId;
            }
            var logExpression = string.Empty;
            var exceptionText = ExceptionHandling.GetExceptionText(ex, out logExpression);

            var loginLogId = logger.Error(ctx, message, exceptionText, objectId: (int)EnumObjects.System, actionId : (int)EnumSystemActions.Login, logObject: errorInfo, agentId: agentId );

            throw ex;
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

                var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();


                // Предполагаю, что один пользователь всегда привязан только к одному клиенту 
                var client = dbWeb.GetClientByUser(userId);
                if (client == null) throw new ClientIsNotFound();

                // Предполагаю, что один пользователь всегда привязан только к одному серверу 
                var server = dbWeb.GetServerByUser(userId, new SetUserServer { ClientId = client.Id, ServerId = -1 });
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
                string message = GetBrowswerInfo(context);

                var logger = DmsResolver.Current.Get<ILogger>();
                var loginLogId = logger.Information(ctx, message, (int)EnumObjects.System, (int)EnumSystemActions.Login, isCopyDate1: true);

                // Добавляю в пользовательский контекст сведения о браузере
                userContexts.Set(token, loginLogId, message);

                context.AdditionalResponseParameters.Add("ChangePasswordRequired", user.IsChangePasswordRequired);

            }

            return Task.FromResult<object>(null);
        }

        private static string GetBrowswerInfo(OAuthTokenEndpointResponseContext context = null, bool isIncludeFingerPrintInfo = true)
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            var userAgent = HttpContext.Current.Request.UserAgent;
            var mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;
            var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;
            var message = $"{ip}; {bc.Browser} {bc.Version}; {bc.Platform}; {mobile}";
            if (isIncludeFingerPrintInfo && context !=null)
            {
                var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();
                var fingerprint = GetFingerprintFromBody(context.Request.Body);
                var fps = dbWeb.GetUserFingerprints(new FilterAspNetUserFingerprint { FingerprintExact = fingerprint });
                if (fps.Any())
                {
                    var fp = fps.First();
                    message = $"{message};{fp.Fingerprint};{fp.Name}";
                }
            }

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
            // ВНИМАНИЕ!!! в SoapUI параметр называется так client_id
            return GetFromBody(Body, "client_id"); ;
        }

        private static string GetFingerprintFromBody(Stream Body)
        {
            return GetFromBody(Body, "fingerprint");
        }

        private static string GetControlAnswerFromBody(Stream Body)
        {
            var res = GetFromBody(Body, "answer");
            if (!string.IsNullOrEmpty(res)) res = res.Trim();
            return res;
        }

        private static bool GetRememberFingerprintFromBody(Stream Body)
        {
            try { return bool.Parse(GetFromBody(Body, "remember_fingerprint")); } catch (Exception ex) { return false; }
        }

        private static string GetFromBody(Stream Body, string key)
        {
            var value = string.Empty;

            try
            {
                Body.Position = 0;
                var body = new StreamReader(Body);
                //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                var bodyStr = body.ReadToEnd();

                var dic = HttpUtility.ParseQueryString(bodyStr);

                value = dic[key] ?? string.Empty;

            }
            catch (Exception ex) { }

            return value;
        }


    }
}