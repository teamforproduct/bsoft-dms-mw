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
    public class AdminSubordinationsController : ApiController
    {
        /// <summary>
        /// GetAdminSubordinations
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        public IHttpActionResult Get([FromUri] FilterAdminSubordination filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminSubordinations(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// GetAdminSubordinations by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminSubordination</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminSubordinations(ctx, new FilterAdminSubordination() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Add in GetAdminSubordinations
        /// </summary>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminSubordination model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddSubordination, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Chg in GetAdminSubordinations
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminSubordination</param>
        /// <returns>FrontAdminSubordination</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAdminSubordination model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.ExecuteAction(EnumAdminActions.ModifySubordination, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Del in GetAdminSubordinations
        /// </summary>
        /// <returns>FrontAdminSubordination</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteSubordination, cxt, id);
            FrontAdminSubordination tmpItem = new FrontAdminSubordination() { Id = id };
            return new JsonResult(tmpItem, this);
        }
    }
}