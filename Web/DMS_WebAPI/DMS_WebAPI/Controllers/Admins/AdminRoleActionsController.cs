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

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Структура описывает действия, которые разрешены для ролей.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminRoleActions")]
    public class AdminRoleActionsController : ApiController
    {
        /// <summary>
        /// Возвращает действия для ролей
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminRoleAction filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRoleActions(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает действия для ролей по Id записи
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetRoleActions(ctx, new FilterAdminRoleAction() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        [HttpGet]
        [Route("GetRoleActionsDIP")]
        [ResponseType(typeof(List<FrontAdminRoleAction>))]
        public IHttpActionResult Get([FromUri] int roleId, [FromUri] FilterAdminRoleActionDIP filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetRoleActionsDIP(ctx, roleId, filter);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Разрешает действие для роли
        /// </summary>
        /// <param name="model">ModifyAdminRoleAction</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminRoleAction model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddRoleAction, cxt, model);
            return Get((int)tmpItem);
        }

        ///// <summary>
        ///// Изменяет действие для роли
        ///// </summary>
        ///// <param name="id">Record Id</param>
        ///// <param name="model">ModifyAdminRoleAction</param>
        ///// <returns>FrontAdminRoleAction</returns>
        //public IHttpActionResult Put(int id, [FromBody]ModifyAdminRoleAction model)
        //{
        //    model.Id = id;
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    tmpService.ExecuteAction(EnumAdminActions.ModifyRoleAction, cxt, model);
        //    return Get(model.Id);
        //}

        /// <summary>
        /// Запрещает действие для роли
        /// </summary>
        /// <returns>FrontAdminRoleAction</returns> 
        public IHttpActionResult Delete([FromUri] int roleId, [FromUri] int actionId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteRoleAction, cxt, new ModifyAdminRoleAction() { RoleId = roleId , ActionId = actionId });
            FrontAdminRoleAction tmpItem = new FrontAdminRoleAction() { RoleId = roleId, ActionId = actionId };
            return new JsonResult(tmpItem, this);
        }
    }
}