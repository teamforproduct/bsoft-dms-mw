
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using BL.Model.AdminCore.Clients;
using BL.Logic.ClientCore.Interfaces;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    ///
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "ClientActions")]
    public class ClientActionsController : ApiController
    {
       
        /// <summary>
        /// Добавляет нового клиента
        /// </summary>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        public IHttpActionResult Post([FromBody]AddClientSaaS model)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IClientService>();
            tmpService.AddNewClient(cxt, model);
            return new JsonResult(model, this);
        }

        /// <summary>
        /// Добавляет нового клиента
        /// </summary>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        [HttpPost]
        [Route("AddDefaultRoles")]
        public IHttpActionResult AddDefaultRoles()
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IClientService>();
            tmpService.AddClientRoles(cxt);
            return new JsonResult(true, this);
        }

        /// <summary>
        /// Удаляет роль для должности
        /// </summary>
        /// <returns>FrontAdminPositionRole</returns> 
        //public IHttpActionResult Delete([FromUri] int positionId, [FromUri] int roleId)
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    tmpService.ExecuteAction(EnumAdminActions.DeletePositionRole, cxt, new ModifyAdminPositionRole() { PositionId = positionId, RoleId = roleId });
        //    FrontAdminPositionRole tmpItem = new FrontAdminPositionRole() { PositionId = positionId, RoleId = roleId };
        //    return new JsonResult(tmpItem, this);
        //}
    }
}