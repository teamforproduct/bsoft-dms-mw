using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Exception;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMS_WebAPI.Utilities
{
    public class GoogleRecapcha
    {

        public async Task ValidateAsync(string item)
        {
            if (string.IsNullOrEmpty(item)) throw new ArgumentException();


            //  https://developers.google.com/recaptcha/docs/verify
            var setVal = DmsResolver.Current.Get<ISettingValues>();
            var url = setVal.GetGoogleReCaptchaURL();

            var values = new Dictionary<string, string>
                {
                   { "secret", setVal.GetGoogleReCaptchaSecret() }, // Required. The shared key between your site and reCAPTCHA.
                   { "response", item }, // Required. The user response token provided by reCAPTCHA, verifying the user on your site.
                   //{ "remoteip",  }, // Optional. The user's IP address.
                };

            var content = new FormUrlEncodedContent(values);

            var httpClient = DmsResolver.Current.Get<HttpClient>();

            var response = await httpClient.PostAsync(url, content);

            /* The response is a JSON object:
            {
              "success": true|false,
              "challenge_ts": timestamp,  // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
              "hostname": string,         // the hostname of the site where the reCAPTCHA was solved
              "error-codes": [...]        // optional
            } */

            //if (response.StatusCode != System.Net.HttpStatusCode.OK) throw new HttpException((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(json);

            if (jObject.Value<bool>("success") == false)
            {
                throw new ClientCreateException(jObject.GetValue("error-codes").Select(x => x.ToString()).ToList());
            }

        }

    }
}