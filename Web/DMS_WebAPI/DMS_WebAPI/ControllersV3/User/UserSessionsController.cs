using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Сессии 
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserSessionsController : WebApiController
    {

        /// <summary>
        /// Возвращает историю подключений
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Sessions)]
        [ResponseType(typeof(FrontSystemSession))]
        public async Task<IHttpActionResult> Get([FromUri]FilterSessionsLog filter, [FromUri]UIPaging paging)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (filter == null) filter = new FilterSessionsLog();
                filter.UserId = context.User.Id;
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var tmpItems = webService.GetSessionLogs(filter, paging);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает текущие подключения
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Sessions + "/Current")]
        [ResponseType(typeof(FrontSystemSession))]
        public async Task<IHttpActionResult> GetCurrent([FromUri]FilterSessionsLog filter, [FromUri]UIPaging paging)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (filter == null) filter = new FilterSessionsLog();
                filter.UserId = context.User.Id;
                filter.IsActive = true;
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var tmpItems = webService.GetSessionLogs(filter, paging);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Выход на всех устройствах
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Sessions + "/KillAll")]
        public async Task<IHttpActionResult> KillAll()
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            ctxs.RemoveByUserId(User.Identity.GetUserId());
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Выход на всех устройствах, кроме текущего
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Sessions + "/KillAllButThis")]
        public async Task<IHttpActionResult> KillAllButThis()
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            ctxs.RemoveByUserId(User.Identity.GetUserId());
            return new JsonResult(null, this);
        }

    }
}
