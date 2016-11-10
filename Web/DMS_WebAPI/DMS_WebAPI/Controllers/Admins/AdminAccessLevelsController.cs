using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    public class AdminAccessLevelsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Получение словаря уровней доступа
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список уровней доступа</returns>
        public IHttpActionResult Get([FromUri] FilterAdminAccessLevel filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAdminAccessLevels(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Получение словаря уровней доступа по ИД
        /// </summary>
        /// <param name="id">ИД уровня доступа</param>
        /// <returns>Уровень доуступа</returns>
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAdminAccessLevel(ctx, id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}