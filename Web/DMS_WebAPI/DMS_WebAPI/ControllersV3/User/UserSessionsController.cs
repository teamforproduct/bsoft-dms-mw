using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore.Filters;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сессии 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Employee)]
    public class UserSessionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает историю подключений
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Sessions")]
        [ResponseType(typeof(FrontSystemSession))]
        public IHttpActionResult Get(FilterSystemSession filter, UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();
            if (filter == null) filter = new FilterSystemSession();
            filter.ExecutorAgentIDs = new List<int> { ctx.CurrentAgentId };
            var tmpItems = tmpService.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}
