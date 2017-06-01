using BL.CrossCutting.DependencyInjection;
using BL.Model.Exception;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
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
            //var clientCode = await context.Request.Body.GetClientCodeAsync();
            var clientSecret = await context.Request.Body.GetClientSecretAsync();
            var fingerprint = await context.Request.Body.GetFingerprintAsync();

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            // отпечаток - обязательный параметр (решили сделать обязательным, хотя дальше по логике он может не понадобиться)
            // не можем передавать из соапа
            //if (string.IsNullOrEmpty(fingerprint?.Trim())) throw new FingerprintRequired();


            // Нахожу пользователя по логину
            AspNetUsers user = await userManager.FindByNameAsync(context.UserName);

            if (user == null) throw new UserIsNotDefined();

            // Пользователь может быть заблокирован по двум причинам:
            // - пользователь сам себя заблокировал при очередной попытке ввода неправильного пароля (или взломщик)
            // - клиентский администратор заблокировал вход сотрудника в свою организацию  (проверяется при добавлении контекста)


            var webService = DmsResolver.Current.Get<WebAPIService>();

            // Если для пользователя включена возможность самоблокировки
            if (userManager.SupportsUserLockout && userManager.GetLockoutEnabled(user.Id) && userManager.IsLockedOut(user.Id))
            {
                await webService.ThrowErrorGrantResourceOwnerCredentials(context, new UserIsLockout());
            }

            var passwordIsValid = await userManager.CheckPasswordAsync(user, context.Password);

            // Если для пользователя включена возможность самоблокировки
            if (userManager.SupportsUserLockout && userManager.GetLockoutEnabled(user.Id))
            {
                if (passwordIsValid)
                {
                    // Если пароль введен верно, то сбрасываю попытки
                    if (await userManager.GetAccessFailedCountAsync(user.Id) > 0)
                    {
                        IdentityResult result = await userManager.ResetAccessFailedCountAsync(user.Id);
                        if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);
                    }
                }
                else
                {
                    // Фиксирую еще одну неверную попытку
                    IdentityResult result = await userManager.AccessFailedAsync(user.Id);
                    if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);
                }
            }

            if (!passwordIsValid) await webService.ThrowErrorGrantResourceOwnerCredentials(context, new UserPasswordIsIncorrect());

            /////////////////////////////
            // Лигин и пароль верные!!!
            ////////////////////////////

            // Проверка подтверждения адреса
            if (!user.EmailConfirmed) await webService.ThrowErrorGrantResourceOwnerCredentials(context, new UserMustConfirmEmail());

            // Проверка Fingerprint: Если пользователь включил Fingerprint
            if (user.IsFingerprintEnabled)
            {
                var answer = await context.Request.Body.GetControlAnswerAsync();
                var rememberFingerprint = await context.Request.Body.GetRememberFingerprintAsync();


                if (string.IsNullOrEmpty(fingerprint?.Trim())) throw new FingerprintRequired();

                if (!string.IsNullOrEmpty(answer))  // переданы расширенные параметры получения токена с ответом на секретный вопрос
                {
                    // Проверка ответа на секретный вопрос
                    if (!(user.ControlAnswer == answer))
                        await webService.ThrowErrorGrantResourceOwnerCredentials(context, new UserAnswerIsIncorrect());

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
                    })) await webService.ThrowErrorGrantResourceOwnerCredentials(context, new UserFingerprintIsIncorrect());
                }
            }


            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);

            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

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
        public override async Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            if (context.Identity.IsAuthenticated)
            {
                // Получаю ID WEb-пользователя
                var userId = context.Identity.GetUserId();

                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                AspNetUsers user = await userManager.FindByIdAsync(userId);

                context.AdditionalResponseParameters.Add("ChangePasswordRequired", user.IsChangePasswordRequired);

                //var token = $"{context.Identity.AuthenticationType} {context.AccessToken}";

                

                //!!! WriteLog
            }

            //return Task.FromResult<object>(null);

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




    }
}