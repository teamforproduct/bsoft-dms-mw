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
            return new JsonResult(new
            {
                Item = item,
                Date = item.Date,
                DateToString = item.Date.ToString(),
                DateToLocalTime = item.Date.ToLocalTime(),
                DateNow = DateTime.Now,
                DateNowUTC = DateTime.UtcNow
            }, this);
        }

        [HttpPost]
        [Route("SetDate")]
        public IHttpActionResult SetDate([FromBody]ModifyDate item)
        {
            return new JsonResult(new
            {
                Item = item,
                Date = item.Date,
                DateToString = item.Date.ToString(),
                DateToLocalTime = item.Date.ToLocalTime(),
                DateNow = DateTime.Now,
                DateNowUTC = DateTime.UtcNow
            }, this);
        }

        [HttpPost]
        [Route("SetUsers")]
        public IHttpActionResult SetUsers()
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            var tmpItems = tmpDict.GetInternalContacts(ctx,
                new BL.Model.DictionaryCore.FilterModel.FilterDictionaryContact
                {
                    ContactTypeIDs = new List<int> { 28 },
                    NotContainsAgentIDs = new List<int> { 1036, 1040, 1489 }
                });

            var dbProc = new WebAPIDbProcess();
            dbProc.AddUsersTemp( tmpItems);
            return new JsonResult(null, this);
        }

    }
}
