using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.WebAPI
{
    [Authorize]
    public class UserServersController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetUserServers filter)
        {
            var dbProc = new WebAPIDbProcess();
            var items = dbProc.GetUserServers(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = new WebAPIDbProcess();
            var item = dbProc.GetUserServer(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(ModifyAspNetUserServer model)
        {
            var dbProc = new WebAPIDbProcess();
            var itemId = dbProc.AddUserServer(model);
            return Get(itemId);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = new WebAPIDbProcess();
            dbProc.DeleteUserServer(id);
            var item = new FrontAspNetUserServer { Id = id };
            return new JsonResult(item, this);
        }
    }
}
