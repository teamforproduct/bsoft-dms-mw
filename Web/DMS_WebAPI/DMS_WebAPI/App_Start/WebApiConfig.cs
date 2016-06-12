﻿using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using DMS_WebAPI.Areas.HelpPage;
using System.Web;
using DMS_WebAPI.Infrastructure;
using System.Web.Http.Hosting;

namespace DMS_WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Filters.Add(new ExceptionHandlingAttribute());
            config.Filters.Add(new ModelValidationErrorHandlerFilterAttribute());

            config.Services.Replace(typeof(IHostBufferPolicySelector), new NoBufferPolicySelector());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v2/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var jsonConfig = config.Formatters.JsonFormatter;
            jsonConfig.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/bin"), new string[] { "BL.Model.XML", "DMS_WebAPI.XML" }));
            config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/bin/DMS_WebAPI.XML")));
        }
    }
}
