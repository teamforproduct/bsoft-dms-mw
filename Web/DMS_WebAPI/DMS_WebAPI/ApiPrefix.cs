using System;
using System.Web;

namespace DMS_WebAPI
{
    public static class ApiPrefix
    {
        public const string V2 = "api/v2OLD/";

        public const string V3 = "api/v3/";

        public static string CurrentModule()
        {
            var uri = HttpContext.Current.Request.Url.AbsolutePath; //actionContext.Request.RequestUri.AbsolutePath;

            if (!uri.StartsWith("/"+ V3)) return null;

            var param = uri.Replace("/" + V3, "").Split('/');

            if (param.Length < 2) return null;

            return param[0];
        }

        public static string CurrentFeature()
        {
            var uri = HttpContext.Current.Request.Url.AbsolutePath; //actionContext.Request.RequestUri.AbsolutePath;

            if (!uri.StartsWith("/" + V3)) return null;

            var param = uri.Replace("/" + V3, "").Split('/');

            if (param.Length < 2) return null;

            int valueParsed;
            if (Int32.TryParse(param[1], out valueParsed) && param.Length >= 3)
            {
                return param[2];
            }
            else
            {
                return param[1];
            }
        }
    }
}