using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DMS_WebAPI.Models;
using DMS_WebAPI.Providers;
using DMS_WebAPI.Results;
using BL.Logic.DependencyInjection;
using DMS_WebAPI.Utilities;
using System.Reflection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.SystemCore.IncomingModel;
using BL.Logic.SystemCore.Interfaces;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Utilities")]
    public class UtilitiesController : ApiController
    {
        /// <summary>
        /// Возвращает версию сборки
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route("GetVersion")]
        public IHttpActionResult GetVersion()
        {
            var tmpItems = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает объект Request. Для определения параметров, которые браузер отправляет в запрос
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRequest")]
        public IHttpActionResult GetRequest()
        {
            return new JsonResult(HttpContext.Current.Request, this);
        }

        /// <summary>
        /// Возвращает дату в том виде, в котором она пришла в контроллер
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDate")]
        public IHttpActionResult GetDate([FromUri]ModifyDate item)
        {
            return SetDate(item);
        }

        [HttpPost]
        [Route("SetDate")]
        public IHttpActionResult SetDate([FromBody]ModifyDate item)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();

            tmpService.AddSystemDate(ctx, item.Date);

            var dateFromBase = tmpService.GetSystemDate(ctx);

            return new JsonResult(new
            {
                Item = item,
                Date = item.Date,
                DateFromBase = dateFromBase,
                DateToString = item.Date.ToString(),
                DateToLocalTime = item.Date.ToLocalTime(),
                DateNow = DateTime.Now,
                DateNowUTC = DateTime.UtcNow
            }, this);
        }

        [HttpGet]
        [Route("GetUserContextsCount")]
        public IHttpActionResult GetUserContextsCount()
        {
            var count = DmsResolver.Current.Get<UserContexts>().Count;
            return new JsonResult(count, this);
        }

        [HttpPost]
        [Route("ClearUserContexts")]
        public IHttpActionResult ClearUserContexts()
        {
            DmsResolver.Current.Get<UserContexts>().ClearCache();
            return new JsonResult(null, this);
        }

    }
}
