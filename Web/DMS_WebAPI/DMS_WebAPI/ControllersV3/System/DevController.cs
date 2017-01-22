using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Common;
using System.Diagnostics;
using BL.Logic.SystemServices.TempStorage;
using System.Web;
using System;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Для разработки
    /// </summary>
    //[Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class DevController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает Hello, world!
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Test")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var res = new JsonResult("Hello, world!", this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}