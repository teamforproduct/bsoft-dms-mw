using BL.CrossCutting.DependencyInjection;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.Utilities;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_WebAPI.Providers
{
    public static class HttpBrowserCapabilitiesExtensions
    {
        public static string Info(this HttpBrowserCapabilities browser)
        {
            var userAgent = HttpContext.Current.Request.UserAgent;

            var mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;

            var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;

            var message = $"{ip}; {browser.Browser} {browser.Version}; {browser.Platform}; {mobile}";

            var webService = DmsResolver.Current.Get<WebAPIService>();
            var fingerprint = HttpContext.Current.Request.InputStream.GetFingerprint();
            var fps = webService.GetUserFingerprints(new FilterAspNetUserFingerprint { FingerprintExact = fingerprint });
            if (fps.Any())
            {
                var fp = fps.First();
                message = $"{message};{fp.Fingerprint};{fp.Name}";
            }
            else
                message = $"{message};{fingerprint.Substring(1, 8) + "..."};Not Saved";
            //{HttpContext.Current.Request.UserHostAddress}
            //var js = new JavaScriptSerializer();
            //message += $"; {js.Serialize(HttpContext.Current.Request.Headers)}";
            //message += $"; {HttpContext.Current.Request.Headers["X-Real-IP"]}";

            return message;
        }
    }
}