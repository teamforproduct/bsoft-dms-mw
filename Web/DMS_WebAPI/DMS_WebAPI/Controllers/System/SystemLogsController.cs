using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore;
using System.Web;
using BL.Model.FullTextSearch;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Логи
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "SystemLogs")]
    public class SystemLogsController : ApiController
    {
        /// <summary>
        /// Список логов
        /// </summary>
        /// <param name="filter">модель фильтров лога</param>
        /// <param name="paging">параметры пейджинга</param>
        /// <returns></returns>
        // GET: api/SystemFormats
        public IHttpActionResult Get([FromUri] FilterSystemLog filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ILogger>();
            var tmpDicts = tmpSysProc.GetSystemLogs(ctx, filter, paging);
            var res = new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Сессии пользователей
        /// </summary>
        /// <param name="filter">модель фильтров сессий</param>
        /// <param name="paging">параметры пейджинга</param>
        /// <returns></returns>
        [Route("Sessions")]
        [HttpGet]
        public IHttpActionResult Sessions([FromUri]FullTextSearch ftSearch, [FromUri] FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var ctx = ctxs.Get();
            var sesions = ctxs.GetContextListQuery();
            var tmpSysProc = DmsResolver.Current.Get<ILogger>();
            var tmpDicts = tmpSysProc.GetSystemSessions(ctx, sesions, filter, paging);
            var res = new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

    }
}