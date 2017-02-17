
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

    }
}