using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DMS_WebAPI.Providers
{
    public static class StreamBodyExtensions
    {
        public static string GetClientCode(this Stream Body)
        {
            // ВНИМАНИЕ!!! в SoapUI параметр называется так client_id
            return GetFromBody(Body, "client_id"); ;
        }

        public static string GetFingerprint(this Stream Body)
        {
            return GetFromBody(Body, "fingerprint");
        }

        public static string GetScope(this Stream Body)
        {
            return GetFromBody(Body, "scope");
        }
        public static string GetClientSecret(this Stream Body)
        {
            return GetFromBody(Body, "client_secret");
        }
        public static string GetControlAnswer(this Stream Body)
        {
            var res = GetFromBody(Body, "answer");
            if (!string.IsNullOrEmpty(res)) res = res.Trim();
            return res;
        }

        public static bool GetRememberFingerprint(this Stream Body)
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


        public static string GetString(this Stream Body)
        {
            var value = string.Empty;

            try
            {
                Body.Position = 0;
                var body = new StreamReader(Body);
                //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                value = body.ReadToEnd();

            }
            catch (Exception ex) { }

            return value;
        }

    }
}