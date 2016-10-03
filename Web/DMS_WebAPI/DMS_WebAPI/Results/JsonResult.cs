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
        string _spentTime; // время выполнения запроса

        public UIPaging Paging { set { _paging = value; } }

        public JsonResult(object data, ApiController controller)
        {
            _data = data;
            _request = controller.Request;
            _success = true;
            _msg = string.Empty;
            _meta = null;
            _paging = null;
            _spentTime = null;
        }

        public JsonResult(object data, ApiController controller, TimeSpan SpentTime) : this(data, controller)
        {
            SetSpentTime(SpentTime);
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
            var json = JsonConvert.SerializeObject(new { success = _success, data = _data, msg = _msg, meta = _meta, paging = _paging, spentTime = _spentTime }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);

            var languageService = DmsResolver.Current.Get<Languages>();

            json = languageService.ReplaceLanguageLabel(HttpContext.Current, json);

            HttpResponseMessage response = _request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return Task.FromResult(response);
        }

        private void SetSpentTime(TimeSpan SpentTime)
        {
            _spentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        SpentTime.Hours, SpentTime.Minutes, SpentTime.Seconds, SpentTime.Milliseconds / 10);
        }
    }
}
