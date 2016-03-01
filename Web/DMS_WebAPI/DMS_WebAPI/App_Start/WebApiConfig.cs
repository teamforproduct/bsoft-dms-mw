using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using DMS_WebAPI.Areas.HelpPage;
using System.Web;
using DMS_WebAPI.Infrastructure;

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

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v2/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var jsonConfig = config.Formatters.JsonFormatter;
            jsonConfig.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/bin"), new string[] { "BL.Model.XML", "DMS_WebAPI.XML" }));
            //config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/bin/DMS_WebAPI.XML")));
        }
    }
}
