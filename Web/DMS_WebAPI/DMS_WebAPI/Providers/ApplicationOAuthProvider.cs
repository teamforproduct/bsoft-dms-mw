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

namespace DMS_WebAPI.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

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
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
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
                var userId = context.Identity.GetUserId();
                var isSuperAdmin = false;
                var superAdminRole = "SuperAdmin";

                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                var userRoles = userManager.GetRoles(userId);
                if (userRoles.Any(x => x.Equals(superAdminRole, StringComparison.OrdinalIgnoreCase)))
                {
                    isSuperAdmin = true;
                }

                var token = $"{context.Identity.AuthenticationType} {context.AccessToken}";

                int dbId = 0;

                if (!int.TryParse(System.Web.HttpContext.Current.Request.Headers["DatabaseId"], out dbId))
                {
                    //TODO Remove
                    if (System.Web.HttpContext.Current.IsDebuggingEnabled)
                    {
                        System.Web.HttpContext.Current.Request.Headers["DatabaseId"] = "1";
                        dbId = 1;
                    }
                    else if(!isSuperAdmin)
                    {
                        throw new DatabaseIsNotSet();
                    }
                }

                var db = new Servers().GetServer(dbId);
                if (db == null)
                {
                    if (!isSuperAdmin)
                    {
                        throw new System.Exception("Not found Database");
                    }
                }

                var mngContext = DmsResolver.Current.Get<UserContext>();

                var cxt = mngContext.Set(token, db, userId, isSuperAdmin);
            }

            return Task.FromResult<object>(null);
        }
    }
}