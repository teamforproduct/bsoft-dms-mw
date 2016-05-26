using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.SystemCore;
using DMS_WebAPI.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using BL.CrossCutting.DependencyInjection;
using System.Text;

namespace DMS_WebAPI.Results
{
    public class JsonResult : IHttpActionResult
    {
        object _data;
        HttpRequestMessage _request;
        bool _success;
        string _msg;
        object _meta;
        UIPaging _paging;

        public UIPaging Paging { set { _paging = value; } }

        public JsonResult(object data, ApiController controller)
        {
            _data = data;
            _request = controller.Request;
            _success = true;
            _msg = string.Empty;
            _meta = null;
            _paging = null;
        }
        public JsonResult(object data, object meta, ApiController controller) : this(data, controller)
        {
            _meta = meta;
        }
        public JsonResult(object data, bool success, ApiController controller) : this(data, controller)
        {
            _success = success;
        }
        public JsonResult(object data, object meta, bool success, ApiController controller) : this(data, meta, controller)
        {
            _success = success;
        }
        public JsonResult(object data, string msg, ApiController controller) : this(data, controller)
        {
            _msg = msg;
        }
        public JsonResult(object data, object meta, string msg, ApiController controller) : this(data, meta, controller)
        {
            _msg = msg;
        }
        public JsonResult(object data, bool success, string msg, ApiController controller) : this(data, success, controller)
        {
            _msg = msg;
        }
        public JsonResult(object data, object meta, bool success, string msg, ApiController controller) : this(data, success, controller)
        {
            _msg = msg;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(new { success = _success, data = _data, msg = _msg, meta = _meta, paging = _paging }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);

            try
            {
                IContext ctx = null;
                try
                {
                    ctx = DmsResolver.Current.Get<UserContext>().GetByLanguage();
                    if (HttpContext.Current.User.Identity.IsAuthenticated && ctx != null)
                    {
                        var service = DmsResolver.Current.Get<ILanguageService>();
                        json = service.ReplaceLanguageLabel(ctx, json);
                    }
                }
                catch { }
                var languageService = DmsResolver.Current.Get<Languages>();
                json = languageService.ReplaceLanguageLabel(HttpContext.Current.Request.UserLanguages?[0], json);
            }
            catch { }

            HttpResponseMessage response = _request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return Task.FromResult(response);
        }
    }
}
