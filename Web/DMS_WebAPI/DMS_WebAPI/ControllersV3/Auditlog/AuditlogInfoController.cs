using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Auditlog
{
    /// <summary>
    /// Аудит подключений
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Auditlog)]
    public class AuditlogInfoController : ApiController
    {
        //TODO ASYNC
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Сессии пользователей
        /// </summary>
        /// <param name="ftSearch">модель фильтров сессий</param>
        /// <param name="filter">модель фильтров сессий</param>
        /// <param name="paging">параметры пейджинга</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(FrontSystemSession))]
        public IHttpActionResult Sessions([FromUri]FullTextSearch ftSearch, [FromUri] FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();

            if (filter == null) filter = new FilterSystemSession();
            filter.FullTextSearchString = ftSearch?.FullTextSearchString;

            var tmpItems = tmpService.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает историю подключений сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Employee/{Id:int}")]
        [ResponseType(typeof(FrontSystemSession))]
        public IHttpActionResult Get([FromUri]int Id, [FromUri]FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();
            if (filter == null) filter = new FilterSystemSession();
            filter.ExecutorAgentIDs = new List<int> { Id };
            var tmpItems = tmpService.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает текущие подключения сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Employee/{Id:int}/Current")]
        [ResponseType(typeof(FrontSystemSession))]
        public IHttpActionResult GetCurrent([FromUri]int Id, [FromUri]FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();
            if (filter == null) filter = new FilterSystemSession();
            filter.ExecutorAgentIDs = new List<int> { Id };
            filter.IsOnlyActive = true;
            var tmpItems = tmpService.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }
    }
}