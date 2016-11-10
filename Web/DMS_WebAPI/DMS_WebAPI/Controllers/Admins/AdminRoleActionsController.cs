using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Tree;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Структура описывает действия, которые разрешены для ролей.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminRoleActions")]
    public class AdminRoleActionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает действия для ролей
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminRoleAction filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRoleActions(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает действия для ролей по Id записи
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetRoleActions(ctx, new FilterAdminRoleAction() { IDs = new List<int> { id } });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        [HttpGet]
        [Route("GetRoleActionsDIP")]
        [ResponseType(typeof(List<FrontAdminRoleAction>))]
        public IHttpActionResult Get([FromUri] int roleId, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var tmpItem = tmpService.GetSystemActionForDIP(ctx, roleId, filter);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает действие для роли
        /// </summary>
        /// <param name="model">ModifyAdminRoleAction</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminRoleAction model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddRoleAction, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Запрещает действие для роли
        /// </summary>
        /// <returns>FrontAdminRoleAction</returns> 
        public IHttpActionResult Delete([FromUri] int roleId, [FromUri] int actionId)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteRoleAction, cxt, new ModifyAdminRoleAction() { RoleId = roleId , ActionId = actionId });
            FrontAdminRoleAction tmpItem = new FrontAdminRoleAction() { RoleId = roleId, ActionId = actionId };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        [HttpPost]
        [Route("SetByObject")]
        public IHttpActionResult SetByObject([FromBody] ModifyAdminRoleActionByObject model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetRoleActionByObject, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}