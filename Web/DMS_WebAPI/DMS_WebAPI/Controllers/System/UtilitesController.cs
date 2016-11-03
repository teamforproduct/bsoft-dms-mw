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

namespace DMS_WebAPI.Controllers
{
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

    }
}
