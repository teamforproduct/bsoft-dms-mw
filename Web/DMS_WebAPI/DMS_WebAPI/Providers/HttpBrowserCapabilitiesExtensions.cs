using System.Web;

namespace DMS_WebAPI.Providers
{
    public static class HttpBrowserCapabilitiesExtensions
    {
        public static string Info(this HttpBrowserCapabilities browser)
        {
            var userAgent = HttpContext.Current.Request.UserAgent;
            var mobile = string.Empty;

            if (userAgent != null) mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;

            var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;

            var message = $"{ip}; {browser.Browser} {browser.Version}; {browser.Platform}; {mobile}";

            //{HttpContext.Current.Request.UserHostAddress}
            //var js = new JavaScriptSerializer();
            //message += $"; {js.Serialize(HttpContext.Current.Request.Headers)}";
            //message += $"; {HttpContext.Current.Request.Headers["X-Real-IP"]}";

            return message;
        }

        public static string IP(this HttpBrowserCapabilities browser)
        {
            var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.UserHostAddress;

            return ip;
        }

        public static string Identifier(this HttpBrowserCapabilities browser)
        {
            return HttpContext.Current.Request.Headers["identity_token"];
        }

        public static string Name(this HttpBrowserCapabilities browser)
        {
            return browser.Browser + " v" + browser.Version;
        }

    }
}