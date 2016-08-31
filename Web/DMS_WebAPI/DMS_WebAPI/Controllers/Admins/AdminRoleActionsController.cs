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

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Создание ролей
    /// </summary>
    [Authorize]
    public class AdminRoleActionsController : ApiController
    {
        /// <summary>
        /// GetAdminRoleActions
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminRoleAction filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminRoleActions(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// GetAdminRoleActions by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminRoleActions(ctx, new FilterAdminRoleAction() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Add in GetAdminRoleActions
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

        /// <summary>
        /// Chg in GetAdminRoleActions
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminRoleAction</param>
        /// <returns>FrontAdminRoleAction</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAdminRoleAction model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.ExecuteAction(EnumAdminActions.ModifyRoleAction, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Del in GetAdminRoleActions
        /// </summary>
        /// <returns>FrontAdminRoleAction</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteRoleAction, cxt, id);
            FrontAdminRoleAction tmpItem = new FrontAdminRoleAction() { Id = id };
            return new JsonResult(tmpItem, this);
        }
    }
}