using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Roles
{
    /// <summary>
    /// Роли
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Role)]
    public class RoleInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Список ролей
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetMain([FromUri]FullTextSearch ftSearch, [FromUri]FilterAdminRole filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetMainRoles(ctx, ftSearch, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты роли
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAdminRole))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminRole(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет роль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddAdminRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.AddRole, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты роли
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyAdminRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumAdminActions.ModifyRole, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет роль
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumAdminActions.DeleteRole, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
