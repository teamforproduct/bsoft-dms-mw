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
    public class ClientsController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetClients filter)
        {
            var dbProc = new WebAPIDbProcess();
            var items = dbProc.GetClients(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = new WebAPIDbProcess();
            var item = dbProc.GetClient(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(AddAspNetClient model)
        {
            var dbProc = new WebAPIDbProcess();
            var itemId = dbProc.AddClient(model);
            return Get(itemId);
        }
        public IHttpActionResult Put(int id, ModifyAspNetClient model)
        {
            model.Id = id;
            var dbProc = new WebAPIDbProcess();
            dbProc.UpdateClient(model);
            return Get(model.Id);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = new WebAPIDbProcess();
            dbProc.DeleteClient(id);
            var item = new FrontAspNetClient { Id = id };
            return new JsonResult(item, this);
        }
    }
}
