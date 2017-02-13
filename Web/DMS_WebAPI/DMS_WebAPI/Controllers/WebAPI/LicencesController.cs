using BL.CrossCutting.DependencyInjection;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.WebAPI
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "Licences")]
    public class LicencesController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetLicences filter)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var items = dbProc.GetLicences(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var item = dbProc.GetLicence(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(ModifyAspNetLicence model)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var itemId = dbProc.AddLicence(model);
            return Get(itemId);
        }
        public IHttpActionResult Put(int id, ModifyAspNetLicence model)
        {
            model.Id = id;
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            dbProc.UpdateLicence(model);
            return Get(model.Id);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            dbProc.DeleteLicence(id);
            var item = new FrontAspNetLicence { Id = id };
            return new JsonResult(item, this);
        }
    }
}
