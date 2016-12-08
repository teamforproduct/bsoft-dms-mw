using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Действия системы
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "SystemActions")]
    public class SystemActionsController : ApiController
    {
        /// <summary>
        /// Список действий системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        // GET: api/SystemActions
        public IHttpActionResult Get([FromUri] FilterSystemAction filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemActions(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        [HttpPost]
        [Route("RefreshSystemActions")]
        public IHttpActionResult RefreshSystemActions()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            tmpService.RefreshSystemActions(cxt);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        [HttpPost]
        [Route("RefreshSystemObjects")]
        public IHttpActionResult RefreshSystemObjects()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            tmpService.RefreshSystemObjects(cxt);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}