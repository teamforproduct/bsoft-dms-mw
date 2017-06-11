using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace DMS_WebAPI.Providers
{
    public static class StreamBodyExtensions
    {
        //public static async Task<string> GetClientCodeAsync(this Stream Body)
        //{
        //    // ВНИМАНИЕ!!! в SoapUI параметр называется так client_id
        //    return await GetFromBodyAsync(Body, "client_id"); ;
        //}

        public static async Task<string> GetFingerprintAsync(this Stream Body)
        {
            return await GetFromBodyAsync(Body, "fingerprint");
        }

        public static async Task<string> GetScopeAsync(this Stream Body)
        {
            return await GetFromBodyAsync(Body, "scope");
        }
        public static async Task<string> GetClientSecretAsync(this Stream Body)
        {
            return await GetFromBodyAsync(Body, "client_secret");
        }
        public static async Task<string> GetControlAnswerAsync(this Stream Body)
        {
            var res = await GetFromBodyAsync(Body, "answer");
            if (!string.IsNullOrEmpty(res)) res = res.Trim();
            return res;
        }

        public static async Task<bool> GetRememberFingerprintAsync(this Stream Body)
        {
            try { return bool.Parse(await GetFromBodyAsync(Body, "remember_fingerprint")); } catch (Exception) { return false; }
        }

        private static async Task<string> GetFromBodyAsync(Stream Body, string key)
        {
            var value = string.Empty;

            try
            {
                Body.Position = 0;
                var body = new StreamReader(Body);
                //var bodyStr = HttpUtility.UrlDecode(body.ReadToEnd());
                var bodyStr = await body.ReadToEndAsync();

                var dic = HttpUtility.ParseQueryString(bodyStr);

                value = dic[key] ?? string.Empty;

            }
            catch (Exception) { }

            return value;
        }

    }
}