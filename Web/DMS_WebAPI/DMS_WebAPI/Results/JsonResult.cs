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
using System.Diagnostics;

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
        Stopwatch _stopwatch; // время выполнения запроса

        public UIPaging Paging { set { _paging = value; } }
        public Stopwatch SpentTime { set { _stopwatch = value; } }
        public JsonResult(object data, ApiController controller)
        {
            _data = data;
            _request = controller.Request;
            _success = true;
            _msg = string.Empty;
            _meta = null;
            _paging = null;
            _stopwatch = null;
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

            string spentTimeStr = null;
            if (_stopwatch != null)
            {
                if (_stopwatch.IsRunning) _stopwatch.Stop();

                var spentTime = _stopwatch.Elapsed;

                spentTimeStr = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                spentTime.Hours, spentTime.Minutes, spentTime.Seconds, spentTime.Milliseconds / 10);
            }

            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            //settings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            // По наблюдениям: Если задать  DateTimeZoneHandling.Utc то локальная дата будет переведена в utc и будет отображена c буквой Z:
            //settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            // Если форматом отрезать милисекунды, то Z перестает отображаться
            var json = JsonConvert.SerializeObject(new { success = _success, data = _data, msg = _msg, meta = _meta, paging = _paging, spentTime = spentTimeStr }, settings);

            // ВНИМАНИЕ!!! Здесь достаточно опасно модифицировать готовый json. 
            // Возникает задача экранирования символов {}":[]
            var languageService = DmsResolver.Current.Get<ILanguages>();
            json = languageService.GetTranslation(json);

            HttpResponseMessage response = _request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return Task.FromResult(response);
        }

    }
}
