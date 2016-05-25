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
    public class ClientLicencesController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetClientLicences filter)
        {
            var dbProc = new WebAPIDbProcess();
            var items = dbProc.GetClientLicences(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = new WebAPIDbProcess();
            var item = dbProc.GetClientLicence(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(ModifyAspNetClientLicence model)
        {
            var dbProc = new WebAPIDbProcess();
            var itemId = dbProc.AddClientLicence(model);
            return Get(itemId);
        }
        public IHttpActionResult Put(int id, ModifyAspNetClientLicence model)
        {
            model.Id = id;
            var dbProc = new WebAPIDbProcess();
            dbProc.UpdateClientLicence(model);
            return Get(model.Id);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = new WebAPIDbProcess();
            dbProc.DeleteClientLicence(id);
            var item = new FrontAspNetClientLicence { Id = id };
            return new JsonResult(item, this);
        }
    }
}
