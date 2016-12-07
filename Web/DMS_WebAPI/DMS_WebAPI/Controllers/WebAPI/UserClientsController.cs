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
    public class UserClientsController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetUserClients filter)
        {
            var dbProc = new WebAPIDbProcess();
            var items = dbProc.GetUserClients(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = new WebAPIDbProcess();
            var item = dbProc.GetUserClient(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(ModifyAspNetUserClient model)
        {
            var dbProc = new WebAPIDbProcess();
            var itemId = dbProc.AddUserClient(model);
            return Get(itemId);
        }
        [Route("AddFirstAdmin")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PostAddFirstAdmin(AddFirstAdminClient model)
        {
            var dbProc = new WebAPIService();
            dbProc.AddFirstAdmin(model);
            return new JsonResult(null, this);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = new WebAPIDbProcess();
            dbProc.DeleteUserClient(id);
            var item = new FrontAspNetUserClient { Id = id };
            return new JsonResult(item, this);
        }
    }
}
