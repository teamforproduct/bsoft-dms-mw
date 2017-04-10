using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// История поисковых запросов. Доступ НЕ ограничен
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class SystemSearchQueriesController : WebApiController
    {

        /// <summary>
        /// Возвращает историю поисковых запросов 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SearchQueries)]
        [ResponseType(typeof(List<FrontSearchQueryLog>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemSearchQueryLog filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ILogger>();
                var tmpItems = tmpService.GetSystemSearchQueryLogs(context, filter, new UIPaging { PageSize = 6, CurrentPage = 1, IsOnlyCounter = false });
                return new JsonResult(tmpItems, this);
            });
        }

        /// <summary>
        /// Удаляет историю поисковых запросов 
        /// </summary>
        /// <param name="filter">фильтр для удаления</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.SearchQueries + "/DeleteForCurrentUser")]
        public async Task<IHttpActionResult> DeleteSearchQueries([FromBody] FilterSystemSearchQueryLog filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ILogger>();
                tmpService.DeleteSystemSearchQueryLogsForCurrentUser(context, filter);
                return new JsonResult(null, this);
            });
        }

    }
}