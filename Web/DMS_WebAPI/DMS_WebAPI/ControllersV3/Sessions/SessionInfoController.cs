using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Системные настройки
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Settings)]
    public class SessionInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Сессии пользователей
        /// </summary>
        /// <param name="ftSearch">модель фильтров сессий</param>
        /// <param name="filter">модель фильтров сессий</param>
        /// <param name="paging">параметры пейджинга</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/Main")]
        [ResponseType(typeof(FrontSystemSession))]
        public IHttpActionResult Sessions([FromUri]FullTextSearch ftSearch, [FromUri] FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpService = DmsResolver.Current.Get<ILogger>();
            var tmpItems = tmpService.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }
    }
}