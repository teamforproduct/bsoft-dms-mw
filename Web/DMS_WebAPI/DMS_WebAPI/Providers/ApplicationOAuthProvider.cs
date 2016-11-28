﻿using System;
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
        /// Установка значения _publicClientId в "self" при конфигурации опций овина
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

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var email = context.UserName;
            try
            {
                context.Request.Body.Position = 0;
                var body = new StreamReader(context.Request.Body);
                //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                var bodyStr = body.ReadToEnd();

                var dic = HttpUtility.ParseQueryString(bodyStr);
                var clientCode = dic["ClientCode"] ?? string.Empty;

                // если фронт передал код (доменное имя) клиента
                if (!string.IsNullOrEmpty(clientCode))
                {
                    var dbProc = new WebAPIDbProcess();
                    var client = dbProc.GetClient(clientCode);
                    if (client != null && client.Id > 0)
                    {
                        email = $"Client_{client.Id}_{email}";
                    }
                }
            }
            catch { }

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(email, context.Password);

            if (user == null)
            {

                var languageService = DmsResolver.Current.Get<ILanguages>();
                var errText = languageService.GetTranslation("##l@DmsExceptions:UserNameOrPasswordIsIncorrect@l##");

                // pss локализация
                context.SetError("invalid_grant", errText);
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
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

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            if (context.Identity.IsAuthenticated)
            {
                var clientCode = string.Empty;
                try
                {
                    context.Request.Body.Position = 0;
                    var body = new StreamReader(context.Request.Body);
                    //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                    var bodyStr = body.ReadToEnd();

                    var dic = HttpUtility.ParseQueryString(bodyStr);
                    clientCode = dic["ClientCode"] ?? string.Empty;
                }
                catch (Exception ex) { }

                // Получаю ID WEb-пользователя
                var userId = context.Identity.GetUserId();

                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                ApplicationUser user = userManager.FindById(userId);
                
                if (user.IsLockout)
                throw new UserIsDeactivated(user.UserName);
                //{
                //    var httpContext = HttpContext.Current;
                //    httpContext.Response.Clear();
                //    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                //    httpContext.Response.ContentType = "application/json";

                //    var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;

                //    var json = JsonConvert.SerializeObject(new { success = false, msg = "User Is Deactivated" }, settings);

                //    httpContext.Response.Write(json);
                //    httpContext.Response.End();
                //    httpContext.Response.Flush();
                //}

                //if (user.IsEmailConfirmRequired && !user.EmailConfirmed)
                //    throw new EmailConfirmRequiredAgentUser();

                var token = $"{context.Identity.AuthenticationType} {context.AccessToken}";

                var mngContext = DmsResolver.Current.Get<UserContexts>();

                // Добавляю пользовательский контекст в коллекцию
                var ctx = mngContext.Set(token, userId, clientCode, user.IsChangePasswordRequired);

                context.AdditionalResponseParameters.Add("ChangePasswordRequired", user.IsChangePasswordRequired);
            }

            return Task.FromResult<object>(null);
        }
    }
}