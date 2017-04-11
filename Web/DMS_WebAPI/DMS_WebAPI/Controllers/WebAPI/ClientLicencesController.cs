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
    [RoutePrefix(ApiPrefix.V2 + "ClientLicences")]
    public class ClientLicencesController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetClientLicences filter)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var items = dbProc.GetClientLicences(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var item = dbProc.GetClientLicence(id);
            return new JsonResult(item, this);
        }

        [Route("GetRegCode")]
        [HttpGet]
        public IHttpActionResult GetRegCode(int clientLicenceId)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var si = new SystemInfo();

            var lic = dbProc.GetClientLicence(clientLicenceId);

            var regCode = si.GetRegCode(lic);

            return new JsonResult(regCode, this);
        }

        /// <summary>
        /// Добавить лицензию клиенту чтобы получить регистрационный код
        /// </summary>
        /// <param name="id">ИД лицензии</param>
        /// <returns></returns>
        public IHttpActionResult Post(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var itemId = dbProc.AddClientLicence(ctx, id);
            return Get(itemId);
        }

        [Route("SetLicenceKey")]
        [HttpPost]
        public IHttpActionResult PostSetLicenceKey(SetClientLicenceKey model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var itemId = dbProc.SetClientLicenceKey(ctx, model);
            return Get(itemId);
        }

        public IHttpActionResult Put(int id, ModifyAspNetClientLicence model)
        {
            model.Id = id;
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            dbProc.UpdateClientLicence(model);
            return Get(model.Id);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            //dbProc.DeleteClientLicence(id);
            var item = new FrontAspNetClientLicence { Id = id };
            return new JsonResult(item, this);
        }
    }
}
