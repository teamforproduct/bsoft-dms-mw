using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
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
            var clientCode = context.Request.Body.GetClientCode();
            var fingerprint = context.Request.Body.GetFingerprint();

            // код клиента - обязательный параметр
            if (string.IsNullOrEmpty(clientCode?.Trim())) throw new ClientCodeRequired();

            // отпечаток - обязательный параметр (решили сделать обязательным, хотя дальше по логике он может не понадобиться)
            // не можем передавать из соапа
            //if (string.IsNullOrEmpty(fingerprint?.Trim())) throw new FingerprintRequired();

            // Если передали несуществующие код клиента. дальше не пускаю
            var webService = DmsResolver.Current.Get<WebAPIService>();
            if (!webService.ExistsClients(new FilterAspNetClients { Code = clientCode })) throw new ClientIsNotFound(); // TODO может тут нужен ThrowErrorGrantResourceOwnerCredentials - не знаю - и зачем не понимаю

            // проверить принадлежность пользователя к клиенту
            if (!webService.ExistsUser(context.UserName, clientCode)) throw new UserIsNotDefined();

            // Нахожу пользователя по логину и паролю
            AspNetUsers user = await webService.GetUser(context.UserName, context.Password);

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
                var answer = context.Request.Body.GetControlAnswer();
                var rememberFingerprint = context.Request.Body.GetRememberFingerprint();
                

                if (string.IsNullOrEmpty(fingerprint?.Trim())) ThrowErrorGrantResourceOwnerCredentials(context, new FingerprintRequired());

                if (!string.IsNullOrEmpty(answer))  // переданы расширенные параметры получения токена с ответом на секретный вопрос
                {
                    // Проверка ответа на секретный вопрос
                    if (!(user.ControlAnswer == answer))
                        ThrowErrorGrantResourceOwnerCredentials(context, new UserAnswerIsIncorrect());

                    // Добавление текущего отпечатка в доверенные
                    if (rememberFingerprint)
                    {
                        webService.AddUserFingerprint(new AddAspNetUserFingerprint
                        {
                            UserId = user.Id,
                            Fingerprint = fingerprint,
                        });
                    }
                }
                else
                {
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

                var clientCode = context.Request.Body.GetClientCode();

                var webService = DmsResolver.Current.Get<WebAPIService>();

                var server = webService.GetClientServer(clientCode);
                if (server == null) throw new DatabaseIsNotFound();

                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                AspNetUsers user = userManager.FindById(userId);

                var token = $"{context.Identity.AuthenticationType} {context.AccessToken}";

                //var clientCode = GetClientCodeFromBody(context.Request.Body);

                var userContexts = DmsResolver.Current.Get<UserContexts>();

                // Создаю пользовательский контекст
                var ctx = userContexts.Set(token, userId, user.UserName, user.IsChangePasswordRequired, clientCode);

                // Добавляю в пользовательский контекст сервер
                userContexts.Set(token, server);

                // Получаю информацию о браузере
                var message = HttpContext.Current.Request.Browser.Info();

                var fingerPrint = context.Request.Body.GetFingerprint();

                // Добавляю в пользовательский контекст сведения о браузере
                userContexts.Set(token, message, fingerPrint);

                context.AdditionalResponseParameters.Add("ChangePasswordRequired", user.IsChangePasswordRequired);

            }

            return Task.FromResult<object>(null);
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

        private void ThrowErrorGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context, Exception ex)
        {
            string message = HttpContext.Current.Request.Browser.Info();
            var clientCode = context.Request.Body.GetClientCode();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var server = webService.GetClientServer(clientCode);

            var ctx = new AdminContext(server);
            var logger = DmsResolver.Current.Get<ILogger>();
            var errorInfo = new AuthError
            {
                ClientCode = clientCode,
                EMail = context.UserName,
                FingerPrint = context.Request.Body.GetFingerprint()
            };
            int? agentId = null;
            var dbService = DmsResolver.Current.Get<WebAPIService>();
            var user = dbService.GetUser(errorInfo.EMail);
            if (user != null)
            {
                var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(ctx, user.Id);
                agentId = agentUser?.AgentId;
            }

            var exceptionText = (ex is DmsExceptions) ? "DmsExceptions:" + ex.GetType().Name : ex.Message;
            var loginLogId = logger.Error(ctx, message, exceptionText, objectId: (int)EnumObjects.System, actionId: (int)EnumSystemActions.Login, logObject: errorInfo, agentId: agentId);

            throw ex;
        }


    }
}