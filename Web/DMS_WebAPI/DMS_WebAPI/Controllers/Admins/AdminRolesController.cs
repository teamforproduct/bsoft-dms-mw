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
    /// Соответствие ролей и должностей
    /// </summary>
    [Authorize]
    public class AdminRolesController : ApiController
    {
        /// <summary>
        /// GetAdminRoles
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminRole filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminRoles(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// GetAdminRoles by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminRole</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminRoles(ctx, new FilterAdminRole() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Add in GetAdminRoles
        /// </summary>
        /// <param name="model">ModifyAdminRole</param>
        /// <returns>FrontAdminRole</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminRole model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddRole, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Chg in GetAdminRoles
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminRole</param>
        /// <returns>FrontAdminRole</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAdminRole model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.ExecuteAction(EnumAdminActions.ModifyRole, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Del in GetAdminRoles
        /// </summary>
        /// <returns>FrontAdminRole</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteRole, cxt, id);
            FrontAdminRole tmpItem = new FrontAdminRole() { Id = id };
            return new JsonResult(tmpItem, this);
        }
    }
}